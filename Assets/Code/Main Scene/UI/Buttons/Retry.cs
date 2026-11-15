using UnityEngine;
using UnityEngine.SceneManagement;


namespace MainScene.UI.Buttons
{
    public class Retry : MonoBehaviour
    {
        public void RetryButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}