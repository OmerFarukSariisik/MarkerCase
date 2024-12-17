using TMPro;
using UnityEngine;

namespace DiceGameScripts
{
    public class DiceComponent : MonoBehaviour
    {
        [SerializeField] private TMP_Text numberText;

        public void SetNumberText(int number)
        {
            numberText.text = number.ToString();
        }
    }
}
