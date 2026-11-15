using UnityEngine;
using AllScenes;


namespace Bootstrap
{
    [RequireComponent(typeof(SceneLoader))]
    public class MainMenuLoader : MonoBehaviour
    {
        private void Start() => GetComponent<SceneLoader>().Load();
    }
}