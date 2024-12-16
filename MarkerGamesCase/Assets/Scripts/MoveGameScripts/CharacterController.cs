using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MoveGameScripts
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float targetCloseOffset;
        [SerializeField] private float defaultRotationDuration;

        [SerializeField] private Color interactionColor;
        [SerializeField] private float tableInteractionDuration;
        [SerializeField] private float rotationSpeedFactor;

        [ShowInInspector] private bool _isMoving;
        [ShowInInspector] private float _speed;
        [ShowInInspector] private float _currentRotationSpeed;

        public PlayerState playerState;
        private event Action<CharacterController> OnArrived;
        private event Action<CharacterController> OnInteracted;
        private Vector3 _targetPos;
        private Vector3 _directionToTarget;

        private readonly Vector3 _defaultRotation = new(0f, 90f, 0f);

        public void Initialize(float speed)
        {
            _speed = speed;
        }

        public void MoveToPosition(Vector3 pos)
        {
            _isMoving = true;
            _targetPos = pos;
            _currentRotationSpeed = rotateSpeed;
            transform.DOKill();
        }

        public void SetRotationAfterInteraction()
        {
            transform.rotation = Random.Range(0, 2) % 2 == 0
                ? Quaternion.identity
                : Quaternion.Euler(0, 180f, 0);
        }

        private void Update()
        {
            if (!_isMoving)
                return;

            RotateTowardsTarget();
            transform.position += transform.forward * (_speed * Time.deltaTime);
            CheckIfArrived();
        }

        private void CheckIfArrived()
        {
            if (_directionToTarget.magnitude > targetCloseOffset)
                return;
            _isMoving = false;
            transform.DORotate(_defaultRotation, defaultRotationDuration);
            OnArrived?.Invoke(this);
        }

        private void RotateTowardsTarget()
        {
            _directionToTarget = _targetPos - transform.position;
            _directionToTarget.y = 0;

            var targetRotation = Quaternion.LookRotation(_directionToTarget);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, targetRotation, _currentRotationSpeed * Time.deltaTime);
            _currentRotationSpeed += _speed / _directionToTarget.magnitude * rotationSpeedFactor;
        }

        public async UniTask DoTableInteractionAsync()
        {
            await meshRenderer.material.DOColor(interactionColor, tableInteractionDuration).AsyncWaitForCompletion();
            OnInteracted?.Invoke(this);
        }

        public void SetPlayerState(PlayerState newPlayerState)
        {
            playerState = newPlayerState;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public void SubscribeToOnArrived(Action<CharacterController> action)
        {
            OnArrived += action;
        }

        public void SubscribeToOnInteracted(Action<CharacterController> action)
        {
            OnInteracted += action;
        }

        public void UnsubscribeFromOnArrived(Action<CharacterController> action)
        {
            OnArrived -= action;
        }

        public void UnsubscribeFromOnInteracted(Action<CharacterController> action)
        {
            OnInteracted -= action;
        }
    }
}