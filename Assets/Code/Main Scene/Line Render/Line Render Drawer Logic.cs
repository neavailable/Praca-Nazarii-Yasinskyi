using UnityEngine;
using MainScene.RayPoints;


namespace MainScene.LineRender
{
    public class LineRenderDrawerLogic
    {
        private const float _yOffset = 0.2f;
        
        private readonly Transform _ballTransform;
        private readonly LineRenderer _lineRenderer;
        private readonly IGetRayPoint _groundPoint;
        private readonly float _groundY;
        

        public LineRenderDrawerLogic
        (
            Transform ballTransform,
            LineRenderer lineRenderer,
            IGetRayPoint groundPoint,
            float groundY
        )
        {
            _ballTransform = ballTransform;
            _lineRenderer  = lineRenderer;
            _groundPoint   = groundPoint;
            _groundY       = groundY;
        }
        
        public void Draw()
        {
            Vector3 currentWorldPos = _groundPoint.GetPoint();
            Vector3 ballPosition = _ballTransform.position;

            currentWorldPos.y = _groundY + _yOffset;
            ballPosition.y    = _groundY + _yOffset;
            
            _lineRenderer.SetPosition(index: 0, position: ballPosition);
            _lineRenderer.SetPosition(index: 1, position: currentWorldPos);
        }
    }
}