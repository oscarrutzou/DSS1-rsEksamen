using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObjectPoolPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies;
using FørsteÅrsEksamen.Factory;
using Microsoft.Xna.Framework;
using FørsteÅrsEksamen.ComponentPattern.Enemies;

namespace FørsteÅrsEksamen.Other
{
    // Erik
    internal class Spawner: Component
    {
        private int totalEnemyAmount = 5;
        private GameObject playerGo;
        private Grid grid;
        
        private Player player;

        private List<Point> spawnLocationEnem = new List<Point>()
        {
            new Point(5,3),
            new Point(6,3), 
            new Point(7,12), 
            new Point(7,11), 
            new Point(7,13), 
        };

        public Spawner(GameObject gameObject) : base(gameObject)
        {
               
        }

        public void InitializeSpawner(GameObject player, Grid grid) 
        {
            
            playerGo = player;
            this.grid = grid;
            SpawnEnemy();
        }

        public override void Start()
        {
            //SpawnEnemy();
        }

        //public void MakeEnemy()
        //{
        //    GameObject enemGo = EnemyFactory.Create(grid, spawnLocationEnem[0]);
        //    GameWorld.Instance.Instantiate(enemGo);

        //    if (GridManager.Instance.CurrentGrid != null)
        //    {
        //        SkeletonWarrior enemy = enemGo.GetComponent<SkeletonWarrior>();
        //        enemy.SetStartPosition(playerGo, new Point(7, 13));
        //        enemy.SetStartPosition(playerGo, new Point(5, 5));
        //        enemy.SetStartPosition(playerGo, new Point());
        //        enemy.SetStartPosition(playerGo, new Point());
        //        enemy.SetStartPosition(playerGo, new Point());

        //    }
        //}

        public void SpawnEnemy()
        {
            
            for (int i = 0; i < totalEnemyAmount; i++)
            {
                
                // skal laves om :  i klassen sættes i start metoden loop igennem liste lav enemy på position

                
                Point spawnPoint = spawnLocationEnem[i];
                GameObject enemyGo = EnemyFactory.Create(); 
                                
                //SkeletonWarrior enemy = enemyGo.GetComponent<SkeletonWarrior>();                          
                GameWorld.Instance.Instantiate(enemyGo);
                if (GridManager.Instance.CurrentGrid != null)
                {
                    SkeletonWarrior enemy = enemyGo.GetComponent<SkeletonWarrior>();
                    enemy.SetStartPosition(playerGo, spawnPoint);
                }


                //spawnLocation++;
            }
        }
    }
}
