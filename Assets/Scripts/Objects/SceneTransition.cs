using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<CharacterController>())
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
