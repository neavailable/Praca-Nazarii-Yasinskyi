using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;
using Configuration.Data;
using Infrastructure.AllScenes.LoadLevel;


namespace Loading
{
    public class Loading : MonoBehaviour
    {
        private string _levelScene;
        private LoadingBar _loadingBar;
        private DelayGenerator _delayGenerator;


        [Inject]
        private void Constructor
        (
            LoadLevel loadLevel, 
            LoadingData loadingData, 
            Slider slider
        )
        {
            _delayGenerator = new (loadingData.MinLoadingTime, loadingData.MaxLoadingTime);

            _loadingBar     = new (_delayGenerator, slider);
            
            _levelScene     = loadLevel.LoadLevelScene;
        }

        private void Start()
        {
            _loadingBar.LoadingBarFilled += LoadSceneAfterDelay;

            _delayGenerator.GenerateDelay();
        }

        private void LoadSceneAfterDelay()
        {
            SceneManager.LoadScene(_levelScene);
        }

        private void OnDestroy()
        {
            _loadingBar.Dispose();
            _loadingBar.LoadingBarFilled -= LoadSceneAfterDelay;
        }
    }
}
