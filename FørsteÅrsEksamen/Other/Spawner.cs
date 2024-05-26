using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ObjectPoolPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.Enemies.RangedEnemies;
using DoctorsDungeon.ComponentPattern.WorldObjects;

namespace DoctorsDungeon.Other
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
