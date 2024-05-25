using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObjectPoolPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies;
using FørsteÅrsEksamen.Factory;
using Microsoft.Xna.Framework;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.ComponentPattern.Enemies.RangedEnemies;

namespace FørsteÅrsEksamen.Other
{
    // Erik
    public class Spawner: Component
    {
        public Spawner(GameObject gameObject) : base(gameObject)
        {
               
        }

        public void SpawnEnemies(List<Point> spawnLocations, GameObject playerGo)
        {
            for (int i = 0; i < spawnLocations.Count; i++)
            {
                Point spawnPoint = spawnLocations[i];
                GameObject enemyGo = EnemyFactory.CreateWithRandomType();
                                
                GameWorld.Instance.Instantiate(enemyGo);
                if (GridManager.Instance.CurrentGrid != null)
                {
                    SkeletonWarrior enemy = enemyGo.GetComponent<SkeletonWarrior>();
                    enemy.SetStartPosition(playerGo, spawnPoint);
                }
            }
        }
    }
}
