using UnityEngine;


namespace MainScene.ClassesParameters
{
    public abstract class AbstractGameObjectParametr
    {
        public GameObject Object { get; private set; }


        protected AbstractGameObjectParametr(GameObject @object) => Object = @object;
    }
}