using UnityEngine;
using Zenject;
using Infrastructure.MainScene.Factories.Coins;


namespace MainScene.Finish
{
    public class FinishEnabler : MonoBehaviour
    {
        private CoinsFactoryCreator _coinsFactoryCreator;
        private ClassesParameters.Finish _finish;
        private FinishEnablerLogic _finishEnablerLogic;

        
        [Inject]
        private void Constructor
        (
            CoinsFactoryCreator coinsFactoryCreator,
            ClassesParameters.Finish finish 
        )
        {
            _coinsFactoryCreator = coinsFactoryCreator;
            _finish              = finish;
        }

        private void Start()
        {
            _finishEnablerLogic = new (_finish.Object, _coinsFactoryCreator.CoinsFactory);
        }
        
        private void OnDestroy() => _finishEnablerLogic.Dispose();
    }
}