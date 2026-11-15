#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;


namespace AllScenes
{
    public class SceneLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset _scene;

        private void OnValidate() => _sceneName = _scene != null ? _scene.name : string.Empty;
#endif
        [SerializeField, HideInInspector] private string _sceneName;

        
        public void Load() => SceneManager.LoadScene(_sceneName);
    }
}
