#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;
using AllScenes;
using Infrastructure.AllScenes.LoadLevel;


namespace Infrastructure.MainMenu
{
    [RequireComponent(typeof(SceneLoader))]
    public class LoadingLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset _level;

        private void OnValidate() => _levelName = _level != null ? _level.name : string.Empty;
#endif
        [SerializeField, HideInInspector] private string _levelName;
        private LoadLevel _loadLevel;


        [Inject]
        private void Construct(LoadLevel loadLevel)
        {
            _loadLevel = loadLevel;
        }

        
        public void LoadLevel()
        {
            _loadLevel.LoadLevelScene = _levelName;
            GetComponent<SceneLoader>().Load();
        }
    }
}
