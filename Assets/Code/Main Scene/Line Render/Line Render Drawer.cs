using UnityEngine;
using Zenject;
using MainScene.RayPoints.GroundPoint;
using MainScene.Ball.BallMarker;
using MainScene.Ground;


namespace MainScene.LineRender
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRenderDrawer : MonoBehaviour
    {
        private Transform _ballTransform;
        private LineRenderDrawerLogic _lineRenderDrawerLogic;
        private LineRenderEnabler _lineRenderEnabler;
        private float _groundY;
        

        [Inject]
        private void Constructor
        (
            LineRenderEnabler lineRenderEnabler, 
            BallMarker ballMarker,
            GroundMarker groundMarker
        )
        {
            _ballTransform     = ballMarker.transform;
            _lineRenderEnabler = lineRenderEnabler;
            _groundY           = groundMarker.gameObject.transform.position.y;
        }
        
        private void Start()
        {
            _lineRenderEnabler.LineRenderEnabled += SetStartPosition;
            
            _lineRenderDrawerLogic = new
            (
                _ballTransform,
                GetComponent<LineRenderer>(),
                new GroundPoint(), 
                _groundY
            );
        }

        private void SetStartPosition() => _lineRenderDrawerLogic.Draw();
        
        private void FixedUpdate() => _lineRenderDrawerLogic.Draw();

        private void OnDestroy()
        {
            _lineRenderEnabler.LineRenderEnabled -= SetStartPosition;
        }
    }
}