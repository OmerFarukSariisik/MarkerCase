using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceGameScripts
{
    public class NumberButtonComponent : MonoBehaviour
    {
        [SerializeField] private Button numberButton;
        [SerializeField] private TMP_Text numberText;
        
        public event Action<int> OnNumberButtonClicked;
        
        private int _number;

        private void Start()
        {
            numberButton.onClick.AddListener(OnNumberClicked);
        }

        private void OnNumberClicked()
        {
            OnNumberButtonClicked?.Invoke(_number);
        }

        public void SetNumber(int number)
        {
            numberText.text = number.ToString();
            _number = number;
        }

        private void OnDestroy()
        {
            numberButton.onClick.RemoveAllListeners();
        }
    }
}
