using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Configuration.Data;
using MainScene.Coin;
using MainScene.Objects;


namespace MainScene.Heal
{
    // A ball with full health rolls through the pickup and leaves it for later.
    // A taken pickup shrinks away exactly like a collected coin.
    [RequireComponent(typeof(Collider))]
    public class HealPickup : MonoBehaviour
    {
        private int _healAmount;
        private ShrinkAnimation _shrinkAnimation;


        [Inject]
        private void Constructor(HealData healData, CoinDestroyerData coinDestroyerData)
        {
            _healAmount      = healData.HealAmount;
            _shrinkAnimation = new ShrinkAnimation
            (
                transform,
                coinDestroyerData.ShrinkingSpeed,
                coinDestroyerData.MinScale
            );
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.TryGetComponent(out IGetHeal getHeal) || !getHeal.TryHeal(_healAmount)) return;

            GetComponent<Collider>().enabled = false;
            Disappear().Forget();
        }


        private async UniTaskVoid Disappear()
        {
            if (!await _shrinkAnimation.Play()) return;
            gameObject.SetActive(false);
        }
    }
}
