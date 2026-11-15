using System.Linq;
using UnityEngine;


namespace MainScene.Ball
{
    public class BounceBall : MonoBehaviour
    {
        public BounceBallLogic BounceBallLogic { get; private set; }
        
        
        private void Awake()
        {
            BounceBallLogic = new BounceBallLogic
            (
                GetComponents<MonoBehaviour>()
                .OfType<IBallMovementContainer>()
                .FirstOrDefault()
            );
        }
    }
}