using UnityEngine;


namespace MainScene.ClassesParameters
{
    public abstract class EnemyPrefab : AbstractGameObjectParametr
    {
        protected EnemyPrefab(GameObject enemyPrefab) : base(enemyPrefab) { }
    }
}