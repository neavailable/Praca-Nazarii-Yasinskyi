using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Configuration.Data;
using MainScene.Ball.BallMarker;


namespace MainScene.UI.Minimap
{
    [RequireComponent(typeof(RawImage))]
    public class MinimapView : MonoBehaviour
    {
        public const string LayerName = "Minimap";

        private Transform _ball, _mainCamera;
        private MinimapData _minimapData;
        private MinimapCamera _minimapCamera;


        [Inject]
        private void Constructor(BallMarker ballMarker, MinimapData minimapData)
        {
            _ball        = ballMarker.transform;
            _minimapData = minimapData;
        }

        // Minimap icons are drawn only by the minimap camera.
        private void Start()
        {
            Camera mainCamera = Camera.main;
            _mainCamera = mainCamera.transform;
            mainCamera.cullingMask &= ~LayerMask.GetMask(LayerName);

            _minimapCamera = new MinimapCamera(_minimapData);
            GetComponent<RawImage>().texture = _minimapCamera.Texture;
        }

        private void LateUpdate()
        {
            if (!_ball.gameObject.activeInHierarchy) return;

            _minimapCamera.Follow(_ball.position, _mainCamera.eulerAngles.y);
        }

        private void OnDestroy() => _minimapCamera?.Dispose();
    }
}
