using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoveGameScripts
{
    public class CharactersManager : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform charactersParent;
        [SerializeField] private PositionProvider positionProvider;

        [FoldoutGroup("Settings"), SerializeField]
        private int characterCount;

        [FoldoutGroup("Settings"), SerializeField]
        private float characterRotationOffset;

        [FoldoutGroup("Settings"), SerializeField]
        private float characterMinSpeed;

        [FoldoutGroup("Settings"), SerializeField]
        private float characterMaxSpeed;

        private List<CharacterController> _charactersList = new();
        private List<CharacterController> _movingCharactersList = new();
        private List<CharacterController> _lineCharactersList = new();
        private HashSet<float> _characterSpeeds = new();

        private void Start()
        {
            InputManager.Instance.OnHitSpace += MoveRandomCharacter;
            GenerateRandomUniqueSpeedValues();
            GenerateCharacters();
        }

        private void GenerateRandomUniqueSpeedValues()
        {
            for (var i = 0; i < characterCount; i++)
            {
                float randomSpeed;
                do
                {
                    randomSpeed = Random.Range(characterMinSpeed, characterMaxSpeed);
                } while (_characterSpeeds.Contains(randomSpeed));

                _characterSpeeds.Add(randomSpeed);
            }
        }

        private void GenerateCharacters()
        {
            for (var i = 0; i < characterCount; i++)
            {
                var createPosition = positionProvider.GetRandomStartPosition();
                var randomOffset = Random.Range(-characterRotationOffset, characterRotationOffset);
                var randomRotation = Quaternion.Euler(0, randomOffset, 0);
                var character = Instantiate(characterController, createPosition, randomRotation, charactersParent);
                character.Initialize(_characterSpeeds.ElementAt(i));
                character.SubscribeToOnArrived(OnPlayerArrived);
                character.SubscribeToOnInteracted(OnPlayerInteracted);
                _charactersList.Add(character);
            }
        }

        private void OnPlayerArrived(CharacterController character)
        {
            if (_lineCharactersList.Contains(character) || character.playerState == PlayerState.Finish)
            {
                if (_lineCharactersList.Count > 0 && _lineCharactersList[0] == character)
                    character.DoTableInteractionAsync().Forget();
                return;
            }

            _movingCharactersList.Remove(character);
            _lineCharactersList.Add(character);
            _movingCharactersList.ForEach(MoveCharacter);

            if (_lineCharactersList.Count == 1)
                character.DoTableInteractionAsync().Forget();
        }

        private void OnPlayerInteracted(CharacterController character)
        {
            var finishPosition = positionProvider.GetFinishPosition();
            character.SetRotationAfterInteraction();
            character.MoveToPosition(finishPosition);
            _lineCharactersList.Remove(character);
            character.SetPlayerState(PlayerState.Finish);

            for (var i = 0; i < _lineCharactersList.Count; i++)
            {
                var linePosition = positionProvider.GetLinePosition(i + 1);
                _lineCharactersList[i].MoveToPosition(linePosition);
            }
        }

        private void MoveRandomCharacter()
        {
            var startStateCharacters = _charactersList.Where(x => x.playerState == PlayerState.Start).ToList();
            if (startStateCharacters.Count == 0)
                return;
            var randomIndex = Random.Range(0, startStateCharacters.Count);

            var character = startStateCharacters.ElementAt(randomIndex);
            character.SetPlayerState(PlayerState.Line);
            MoveCharacter(character);
            _movingCharactersList.Add(character);
        }

        private void MoveCharacter(CharacterController character)
        {
            var linePos = positionProvider.GetLinePosition(_lineCharactersList.Count + 1);
            character.MoveToPosition(linePos);
        }

        private void OnDestroy()
        {
            InputManager.Instance.OnHitSpace -= MoveRandomCharacter;
            _charactersList.ForEach(x => x.UnsubscribeFromOnArrived(OnPlayerArrived));
            _charactersList.ForEach(x => x.UnsubscribeFromOnInteracted(OnPlayerInteracted));
        }
    }
}