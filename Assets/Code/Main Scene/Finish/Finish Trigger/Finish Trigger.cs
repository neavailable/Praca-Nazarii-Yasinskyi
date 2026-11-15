using UnityEngine;
using R3;
using MainScene.Ball.BallMarker;


namespace MainScene.Finish.FinishTrigger
{
    public class FinishTrigger : MonoBehaviour
    {
        public readonly Subject<Unit> BallCollidedFinished = new (); 
        

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out BallMarker _))
            {
                BallCollidedFinished.OnNext(Unit.Default);
            }
        }
    }
}