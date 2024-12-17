using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DiceGameScripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        [SerializeField] private List<DiceComponent> diceComponents;
        [SerializeField] private DiceResultCalculator diceResultCalculator;
        [SerializeField] private Button throwButton;
        
        private List<int> _selectedNumbers = new();
        private int _thrownCount = 0;
        private int _diceThrowCount = 0;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            _diceThrowCount = diceResultCalculator.diceThrowCount;
            throwButton.onClick.AddListener(ThrowDice);
            SetDiceThrowCount();
        }
        
        public void SetSelectedNumbers(List<int> numbers)
        {
            _selectedNumbers = numbers;
            diceResultCalculator.SetSelectedNumbers(numbers);
        }

        private void SetDiceThrowCount()
        {
            UIManager.instance.SetDiceThrowCount(_thrownCount, _diceThrowCount - _thrownCount);
        }

        private void ThrowDice()
        {
            if (_thrownCount == _diceThrowCount)
                return;
            
            _thrownCount++;
            SetDiceThrowCount();
            var results = diceResultCalculator.GetDiceResults(_thrownCount);
            UIManager.instance.UpdateDiceTotalText(results.Sum());
            UIManager.instance.UpdateTotalText(diceResultCalculator.GetCurrentAllTotal());
            
            for (var i = 0; i < diceComponents.Count; i++)
            {
                diceComponents[i].SetNumberText(results[i]);
            }
        }
    }
}
