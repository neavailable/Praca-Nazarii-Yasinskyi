using System.Collections.Generic;
using Zenject;
using MainScene.ClassesParameters;
using MainScene.StartPoints;


namespace Infrastructure.MainScene.Factories.Enemies
{
    public class EnemiesFactory
    {
        private readonly Stack<EnemyObject> _cactusTransforms, _mushroomTransforms;

        
        public EnemiesFactory
        (
            DiContainer diContainer, 
            AbstractGameObjectParametr cactusPrefab,
            AbstractGameObjectParametr mushroomPrefab,
            Stack<EnemyObject> enemyTransforms
        )
        {
            _cactusTransforms   = new ();
            _mushroomTransforms = new ();

            FillTransforms(enemyTransforms);

            new CactusesFactory(diContainer, cactusPrefab, _cactusTransforms);
            new MushroomsFactory(diContainer, mushroomPrefab, _mushroomTransforms);
        }


        private void FillTransforms(Stack<EnemyObject> enemyTransforms)
        {
            foreach (EnemyObject enemyTransform in enemyTransforms)
            {
                EnemyStartPoint enemyStartPoint = enemyTransform.Object
                    .GetComponent<EnemyStartPoint>();

                if (enemyStartPoint.EnemyType == EnemyStartPoint.EnemyTypes.Cactus)
                {
                    _cactusTransforms.Push(enemyTransform);
                }
                
                else _mushroomTransforms.Push(enemyTransform);
            }
        }
    }
}