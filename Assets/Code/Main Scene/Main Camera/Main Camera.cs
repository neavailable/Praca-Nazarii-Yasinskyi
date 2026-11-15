using UnityEngine;
using Cinemachine;
using Zenject;
using MainScene.Ball.BallMarker;


namespace MainScene.MainCamera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class MainCamera : MonoBehaviour
    {
        private Transform _followTransform;
        
        
        [Inject]
        private void Constructor(BallMarker ballMarker)
        {
            _followTransform = ballMarker.transform;
        }

        private void Start()
        {
            GetComponent<CinemachineVirtualCamera>().Follow = _followTransform;    
        }
    }
}