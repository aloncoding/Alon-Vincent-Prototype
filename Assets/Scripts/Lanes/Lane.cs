using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Spellbound.Enemies;

namespace Spellbound.Lanes
{
    /// <summary>
    /// One of the three horizontal lanes. Tracks which enemies are currently in it and
    /// exposes the spawn/tower endpoints enemies walk between.
    /// </summary>
    public class Lane : MonoBehaviour
    {
        public int laneIndex;
        [Tooltip("World point enemies walk toward (the wizard's tower).")]
        public Transform towerPoint;
        [Tooltip("World point enemies spawn at (right edge of the screen).")]
        public Transform spawnPoint;

        private readonly List<Enemy> _enemies = new List<Enemy>();

        public void Register(Enemy enemy)
        {
            if (!_enemies.Contains(enemy)) _enemies.Add(enemy);
        }

        public void Unregister(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        public Enemy GetFrontmostEnemy()
        {
            return _enemies.OrderBy(e => e.DistanceToTower).FirstOrDefault();
        }

        public List<Enemy> GetEnemiesOrderedByProgress(int max)
        {
            return _enemies.OrderBy(e => e.DistanceToTower).Take(max).ToList();
        }

        public List<Enemy> GetAllEnemies()
        {
            return new List<Enemy>(_enemies);
        }
    }
}
