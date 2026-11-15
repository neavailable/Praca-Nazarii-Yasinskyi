using System;
using UnityEngine;


namespace MainScene.Enemy
{
    public class UnableEnemyLogic : IDisposable
    {
        private readonly GameObject _gameObject;
        private readonly EnemyAnimationsLogic _enemyAnimationsLogic;


        public UnableEnemyLogic
        (
            GameObject gameObject,
            EnemyAnimationsLogic enemyAnimationsLogic
        )
        {
            _gameObject           = gameObject;
            _enemyAnimationsLogic = enemyAnimationsLogic;

            _enemyAnimationsLogic.DieAnimationFinished += UnableEnemy;
        }

        public void Dispose()
        {
            _enemyAnimationsLogic.DieAnimationFinished -= UnableEnemy;
        }


        private void UnableEnemy() => _gameObject.SetActive(false);
    }
}