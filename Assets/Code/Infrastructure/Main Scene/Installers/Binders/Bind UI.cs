using UnityEngine;
using Zenject;
using MainScene.ClassesParameters;


namespace Infrastructure.MainScene.Installers.Binders
{
    public class BindUI : Binder
    {
        private readonly LooseWindow _looseWindow;
        
        
        public BindUI
        (
            DiContainer container,
            GameObject looseWindowShower
        )
        : base(container)
        {
            _looseWindow = new LooseWindow(looseWindowShower);
        }

        public override void Bind()
        { 
            Container.Bind<LooseWindow>().FromInstance(_looseWindow);
        }
    }
}