using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceGameScripts
{
    public class DiceResultCalculator : MonoBehaviour
    {
        [SerializeField] private int diceFaceCount;
        [SerializeField] private List<int> minIndexesForTotal = new() { 1, 5, 10 };
        [SerializeField] private List<int> maxIndexesForTotal = new() { 10, 15, 20 };

        [SerializeField, BoxGroup] private int diceCount = 3;
        [SerializeField, BoxGroup] private int allTotal = 200;
        [SerializeField, BoxGroup] public int diceThrowCount = 20;

        private List<int> _selectedNumbers;
        private List<bool> _selectedNumbersFound = new();

        private int _currentAllTotal;
        private int _throwCount;

        public void SetSelectedNumbers(List<int> selectedNumbers)
        {
            _selectedNumbers = selectedNumbers;
            for (var i = 0; i < _selectedNumbers.Count; i++)
                _selectedNumbersFound.Add(false);
        }

        public int[] GetDiceResults(int throwCount)
        {
            _throwCount = throwCount;
            var numbers = new int[diceCount];
            var total = 0;
            var minTotal = 0;
            var maxTotal = 0;

            (minTotal, maxTotal) = CalculateMinMaxTotal();

            do
            {
                total = 0;
                for (var i = 0; i < numbers.Length; i++)
                {
                    numbers[i] = Random.Range(1, diceFaceCount + 1);
                    total += numbers[i];
                }

                if (total < minTotal)
                {
                    int deficit = minTotal - total;
                    numbers = AdjustNumbersUpward(numbers, deficit);
                    total = numbers.Sum();
                }
                else if (total > maxTotal)
                {
                    int excess = total - maxTotal;
                    numbers = AdjustNumbersDownward(numbers, excess);
                    total = numbers.Sum();
                }
            } while (total < minTotal || total > maxTotal);

            for (var i = 0; i < _selectedNumbersFound.Count; i++)
            {
                if (_selectedNumbersFound[i])
                    continue;
                if (_selectedNumbers[i] != total)
                    continue;
                if (_throwCount < minIndexesForTotal[i])
                    continue;
                if (_throwCount > maxIndexesForTotal[i])
                    continue;

                Debug.LogWarning("FOUND: " + total);
                _selectedNumbersFound[i] = true;
            }

            _currentAllTotal += total;
            return numbers;
        }

        private int[] AdjustNumbersUpward(int[] numbers, int deficit)
        {
            for (var i = 0; i < 3 && deficit > 0; i++)
            {
                var increase = Math.Min(deficit, diceFaceCount - numbers[i]);
                numbers[i] += increase;
                deficit -= increase;
            }

            return numbers;
        }

        private int[] AdjustNumbersDownward(int[] numbers, int excess)
        {
            for (var i = 0; i < 3 && excess > 0; i++)
            {
                var decrease = Math.Min(excess, numbers[i] - 1);
                numbers[i] -= decrease;
                excess -= decrease;
            }

            return numbers;
        }

        private (int minTotal, int maxTotal) CalculateMinMaxTotal()
        {
            var result = CheckSelectedNumbers();
            if (result.forceTotal)
                return (result.total, result.total);

            var notFoundSelectedNumberCount = _selectedNumbersFound.Count(x => !x);
            var remainingThrowCountToCalculate = diceThrowCount - (_throwCount - 1) - notFoundSelectedNumberCount;

            var neededTotal = allTotal - _currentAllTotal;
            for (var i = 0; i < _selectedNumbersFound.Count; i++)
            {
                if (_selectedNumbersFound[i]) continue;
                
                neededTotal -= _selectedNumbers[i];
            }
            
            var minTotal = CalculateMinTotal(neededTotal, remainingThrowCountToCalculate);
            var maxTotal = CalculateMaxTotal(neededTotal, remainingThrowCountToCalculate);
            return (minTotal, maxTotal);
        }

        private int CalculateMaxTotal(int neededTotal, int remainingThrowCountToCalculate)
        {
            if (remainingThrowCountToCalculate == 1)
                return neededTotal;
            var possibleMin = (remainingThrowCountToCalculate - 1) * diceCount * 1;
            if (possibleMin < neededTotal - diceCount * diceFaceCount)
                return diceCount * diceFaceCount;
            
            return neededTotal - possibleMin;
        }

        private int CalculateMinTotal(int neededTotal, int remainingThrowCountToCalculate)
        {
            if (remainingThrowCountToCalculate == 1)
                return neededTotal;
            var possibleMax = (remainingThrowCountToCalculate - 1) * diceCount * diceFaceCount;
            if (possibleMax >= neededTotal - diceCount)
                return diceCount;

            return neededTotal - possibleMax;
        }

        private (bool forceTotal, int total) CheckSelectedNumbers()
        {
            for (var i = 0; i < maxIndexesForTotal.Count; i++)
            {
                if (_throwCount == maxIndexesForTotal[i] && !_selectedNumbersFound[i])
                    return (true, _selectedNumbers[i]);
            }

            return (false, 0);
        }

        public int GetCurrentAllTotal()
        {
            return _currentAllTotal;
        }
    }
}