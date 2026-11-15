using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.Coin
{
    public class CoinRotation : MonoBehaviour
    {
        private float _yRotation;


        [Inject]
        private void Constructor(CoinRotationData coinRotationData)
        {
            _yRotation = coinRotationData.YRotation;
        }
        
        private void FixedUpdate() => transform.Rotate(xAngle: 0f, yAngle: _yRotation, zAngle: 0f);
    }
}