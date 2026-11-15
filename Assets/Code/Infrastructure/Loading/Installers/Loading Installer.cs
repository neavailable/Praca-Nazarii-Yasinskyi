using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Configuration.Data;
using Configuration.Configurations;


namespace Infrastructure.Loading.Installers
{
    public class LoadingInstaller : MonoInstaller
    {
        [SerializeField] private Slider _slider;


        public override void InstallBindings()
        { 
            BindLoadingData();
            BindSlider();
        }
        

        private void BindLoadingData()
        {
            Container.Bind<LoadingData>()
                .FromInstance(new LoadingConfiguration().Load()).AsSingle();
        }

        private void BindSlider()
        {
            Container.Bind<Slider>().FromInstance(_slider).AsSingle();
        }
    }
}