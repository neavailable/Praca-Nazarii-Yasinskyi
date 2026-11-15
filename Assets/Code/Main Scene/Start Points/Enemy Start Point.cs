using UnityEngine;


namespace MainScene.StartPoints
{
    public class EnemyStartPoint : MonoBehaviour
    {
        public EnemyTypes EnemyType => _enemyType;
        
        [SerializeField] private EnemyTypes _enemyType;
        
        
        public enum EnemyTypes { Cactus, Mushroom } 
    }
}