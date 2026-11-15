using Configuration.Data;
using UnityEngine;
using Zenject;


namespace MainScene.Enemy
{
    public class MushroomRotation : MonoBehaviour
    {
        private float _yAngle;
        
        
        [Inject]
        private void Constructor(MushroomRotationData mushroomRotationData)
        {
            _yAngle = Random.Range
            (
                mushroomRotationData.MinAngle, 
                mushroomRotationData.MaxAngle
            );
        }

        private void Start() => transform.Rotate(0, _yAngle, 0);
    }
}