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
using FørsteÅrsEksamen.ComponentPattern.WorldObjects;

namespace FørsteÅrsEksamen.Other
{
    // Erik
    public class Spawner: Component
    {
        public Spawner(GameObject gameObject) : base(gameObject)
        {
               
        }

        public List<Enemy> SpawnEnemies(List<Point> spawnLocations, GameObject playerGo)
        {
            List<Enemy> enemies = new();
            for (int i = 0; i < spawnLocations.Count; i++)
            {
                Point spawnPoint = spawnLocations[i];
                GameObject enemyGo = EnemyFactory.Create(EnemyTypes.OrcWarrior);
                //GameObject enemyGo = EnemyFactory.CreateWithRandomType();
                Enemy enemy = enemyGo.GetComponent<Enemy>();
                enemy.SetStartPosition(playerGo, spawnPoint);
                
                enemies.Add(enemy);
                GameWorld.Instance.Instantiate(enemyGo);
            }

            return enemies;
        }

        public void SpawnPotions(List<Point> spawnLocations, GameObject playerGo)
        {
            for (int i = 0; i < spawnLocations.Count; i++)
            {
                GameObject potionGo = ItemFactory.Create(playerGo);
                potionGo.Transform.Position = GridManager.Instance.CurrentGrid.PosFromGridPos(spawnLocations[i]);
                GameWorld.Instance.Instantiate(potionGo);
            }

        }
    }
}
