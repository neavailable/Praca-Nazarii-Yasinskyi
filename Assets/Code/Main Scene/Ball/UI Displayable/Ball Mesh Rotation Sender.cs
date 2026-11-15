using UnityEngine;
using R3;


namespace MainScene.Ball.UIDisplayable
{
    public class BallMeshRotationSender : MonoBehaviour
    {
        public ReactiveProperty<float> MeshRotation;


        private void Awake() => MeshRotation = new ();

        private void Update() => MeshRotation.Value = transform.eulerAngles.y;
    }
}