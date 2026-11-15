using System;
using UnityEngine;
using Configuration.Data;
using Object = UnityEngine.Object;


namespace MainScene.UI.Minimap
{
    public class MinimapCamera : IDisposable
    {
        public readonly RenderTexture Texture;

        private const string _uiLayerName = "UI";
        private readonly Color _backgroundColor = new (0.1f, 0.12f, 0.15f);
        private readonly Camera _camera;
        private readonly float _height;


        public MinimapCamera(MinimapData minimapData)
        {
            _height = minimapData.Height;
            Texture = new RenderTexture(minimapData.TextureSize, minimapData.TextureSize, 16);

            _camera = new GameObject("Minimap Camera").AddComponent<Camera>();
            _camera.orthographic     = true;
            _camera.orthographicSize = minimapData.OrthographicSize;
            _camera.clearFlags       = CameraClearFlags.SolidColor;
            _camera.backgroundColor  = _backgroundColor;
            _camera.cullingMask      = ~LayerMask.GetMask(_uiLayerName);
            _camera.farClipPlane     = _height * 2.0f;
            _camera.targetTexture    = Texture;
        }

        public void Follow(Vector3 target, float yaw)
        {
            _camera.transform.SetPositionAndRotation
            (
                target + Vector3.up * _height,
                Quaternion.Euler(90.0f, yaw, 0.0f)
            );
        }

        public void Dispose()
        {
            if (_camera != null) Object.Destroy(_camera.gameObject);

            Texture.Release();
            Object.Destroy(Texture);
        }
    }
}
