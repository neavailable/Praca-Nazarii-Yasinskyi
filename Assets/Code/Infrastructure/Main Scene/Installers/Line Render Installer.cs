using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Infrastructure.MainScene.Installers.Binders;
using MainScene.LineRender;


namespace Infrastructure.MainScene.Installers
{
    public class LineRenderInstaller : MonoInstaller
    {
        [SerializeField] private LineRenderDrawer _lineRenderDrawer;
        [SerializeField] private LineRenderEnabler _lineRenderEnabler;
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
                new BindToLineRenderDrawer(Container, _lineRenderEnabler)
            );
            _binders.Add
            (
                new BindLineRenderDrawer(Container, _lineRenderDrawer)
            );
        }

        private void Bind()
        {
            foreach (Binder binder in _binders) binder.Bind();
        }
    }
}