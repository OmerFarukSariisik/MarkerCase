using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DiceGameScripts
{
    public class NumberSelector : MonoBehaviour
    {
        [SerializeField] private List<NumberButtonComponent> numberButtons;
        [SerializeField] private TMP_Text selectedNumbersText;
        
        private readonly List<int> _selectedNumbers = new();

        private void Start()
        {
            SetNumberButtons();
        }

        private void SetNumberButtons()
        {
            for (var i = 0; i < numberButtons.Count; i++)
            {
                numberButtons[i].SetNumber(i + 3);
                numberButtons[i].OnNumberButtonClicked += OnNumberButtonClicked;
            }
        }

        private void OnNumberButtonClicked(int number)
        {
            if (_selectedNumbers.Count == 3)
                return;
            
            _selectedNumbers.Add(number);
            selectedNumbersText.text = "";
            
            for (var i = 0; i < 3; i++)
            {
                selectedNumbersText.text += _selectedNumbers.Count > i ? _selectedNumbers[i].ToString() : "?";
                if (i < 2)
                    selectedNumbersText.text += "   -   ";
            }

            if (_selectedNumbers.Count != 3) return;
            GameManager.instance.SetSelectedNumbers(_selectedNumbers);
            UIManager.instance.ShowDiceScreen(selectedNumbersText.text);
        }
    }
}
