using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Infrastructure.MainScene.Factories.Coins;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.ClassesParameters;
using MainScene.Finish;


namespace Infrastructure.MainScene.Installers
{
    public class FinishInstaller : MonoInstaller
    {   
        [SerializeField] private FinishMarker _finish;
        [SerializeField] private CoinsFactoryCreator _coinsFactoryCreator;
        private readonly List<Binder> _binders = new ();


        public override void InstallBindings()
        {
            CreateBinders();
            Bind();
        }

        
        private void CreateBinders()
        {
            _binders.Add
            (
                new BindCoinsFactory(Container, _coinsFactoryCreator)
            );
            _binders.Add
            (
                new BindFinish(Container, new Finish(_finish.gameObject))
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}