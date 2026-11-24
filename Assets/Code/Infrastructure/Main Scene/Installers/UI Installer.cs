using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Configuration.Configurations;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.UI.FinishWindow.FinishWindowMarker;


namespace Infrastructure.MainScene.Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private FinishWindowMarker _finishWindowMarker;
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
                new BindToCoinsUI
                (
                    Container,
                    new UICoinConfiguration().Load()
                )
            );
            _binders.Add
            (
                new BindUI (Container, _finishWindowMarker.gameObject)
            );
            _binders.Add
            (
                new BindToLevelTimer(Container, gameObject.scene.name)
            );
            _binders.Add
            (
                new BindToMinimap(Container, new MinimapConfiguration().Load())
            );
            _binders.Add
            (
                new BindToPush(Container, new PushConfiguration().Load())
            );
            _binders.Add
            (
                new BindToFeedback(Container)
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}