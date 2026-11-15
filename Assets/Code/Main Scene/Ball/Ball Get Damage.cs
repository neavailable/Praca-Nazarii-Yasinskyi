using UnityEngine;
using MainScene.Objects;
using R3;


namespace MainScene.Ball
{
    public class BallGetDamage : MonoBehaviour, IGetDamage
    {
        public Subject<int> SendDamage;


        private void Awake() => SendDamage = new ();


        public void GetDamage(int damage) => SendDamage?.OnNext(damage);
    }
}