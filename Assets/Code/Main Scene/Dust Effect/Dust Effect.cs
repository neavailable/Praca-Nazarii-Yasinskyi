using System;
using UnityEngine;
using Zenject;
using Configuration.Data;


namespace MainScene.DustEffect
{
    [RequireComponent(typeof(ParticleSystem))]
    public class DustEffect : MonoBehaviour
    {
        public Action<DustEffect> DustEffectStopped;
        
        private float _yOffset;


        [Inject]
        private void Constructor(DustEffectData dustEffectData) => _yOffset = dustEffectData.YOffset;
        
        private void OnEnable() => transform.position += Vector3.up * _yOffset;

        private void OnParticleSystemStopped()
        {
            DustEffectStopped?.Invoke(this);
            
            gameObject.SetActive(false);
        }
    }
}