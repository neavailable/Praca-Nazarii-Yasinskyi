using MainScene.ClassesParameters;
using UnityEngine;
using Zenject;


namespace Infrastructure.MainScene.Factories.DustEffects
{
    public class DustEffectsFactoryCreator : MonoBehaviour
    {
        private DustEffectsFactory _dustEffectsFactory;


        [Inject]
        private void Constructor(DiContainer diContainer, DustEffectPrefab dustEffectPrefab, Walls walls)
        {
            _dustEffectsFactory = new DustEffectsFactory(diContainer, dustEffectPrefab, walls);
        }

        private void OnDisable() => _dustEffectsFactory?.Dispose();
    }
}