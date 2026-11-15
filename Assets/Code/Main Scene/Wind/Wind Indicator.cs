using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Wind
{
    // The arrow points where the wind blows as seen from the camera, a bigger arrow means a stronger wind.
    public class WindIndicator : MonoBehaviour
    {
        private const float _minArrowScale = 0.6f;

        [SerializeField] private RectTransform _arrow;

        private LevelWind _levelWind;
        private Transform _camera;


        [Inject]
        private void Constructor(LevelWind levelWind, WindData windData)
        {
            _levelWind = levelWind;

            float range = windData.MaxStrength - windData.MinStrength;
            float part  = range > 0.0f ? (levelWind.Strength - windData.MinStrength) / range : 1.0f;

            _arrow.localScale = Vector3.one * Mathf.Lerp(_minArrowScale, 1.0f, part);
        }

        private void Start() => _camera = Camera.main.transform;

        private void LateUpdate()
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(_camera.forward, Vector3.up);
            float angle = Vector3.SignedAngle(cameraForward, _levelWind.Direction, Vector3.up);

            _arrow.localRotation = Quaternion.Euler(0.0f, 0.0f, -angle);
        }
    }
}
