using UnityEngine;

namespace Spellbound.Enemies
{
    public enum EnemySize { Small, Medium, Large }

    /// <summary>
    /// Data-only enemy definition. Create one asset per enemy type via
    /// Assets > Create > Spellbound > Enemy.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Spellbound/Enemy")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName = "Goblin";
        public EnemySize size = EnemySize.Small;
        public float maxHP = 1f;
        public float moveSpeed = 2f;
        public float damageToTower = 1f;
        public int scoreValue = 100;
        [Tooltip("Prefab instantiated when this enemy spawns. Must have an Enemy component.")]
        public GameObject prefab;
    }
}
