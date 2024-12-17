using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiceGameScripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        
        [SerializeField] private GameObject numberSelectionPanel;
        [SerializeField] private GameObject diceScreen;
        
        [SerializeField] private TMP_Text throwCountText;
        [SerializeField] private TMP_Text totalText;
        [SerializeField] private TMP_Text diceTotalText;
        [SerializeField] private TMP_Text remainingThrowCountText;
        [SerializeField] private TMP_Text selectedNumbersText;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public void ShowDiceScreen(string numbers)
        {
            numberSelectionPanel.SetActive(false);
            diceScreen.SetActive(true);
            
            selectedNumbersText.text = "Selected Numbers:\n" + numbers;
        }

        public void SetDiceThrowCount(int throwCount, int remainingThrowCount)
        {
            throwCountText.text = "Throw Count:\n" + throwCount;
            remainingThrowCountText.text = remainingThrowCount.ToString();
        }

        public void UpdateTotalText(int total)
        {
            totalText.text = "TOTAL\n" + total;
        }

        public void UpdateDiceTotalText(int total)
        {
            diceTotalText.text = "Dice total:\n" + total;
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
