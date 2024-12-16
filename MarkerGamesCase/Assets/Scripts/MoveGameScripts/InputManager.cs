using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoveGameScripts
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        public event Action OnHitSpace;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnHitSpace?.Invoke();
            }
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}