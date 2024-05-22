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

namespace FørsteÅrsEksamen.Other
{
    internal class Spawner
    {
        private int totalEnemyAmount;
        private GameObject PlayerGo;


        // spawnwave metode fra tideliger 
        // se oscar testscen for spawn metode
        // sæt grid position. (klar til at få sat 4 ekstra kordinater ind)
        // 

        private void MakeEnemy()
        {
            GameObject enemGo = EnemyFactory.Create();
            GameWorld.Instance.Instantiate(enemGo);

            if (GridManager.Instance.CurrentGrid != null)
            {
                SkeletonWarrior enemy = enemGo.GetComponent<SkeletonWarrior>();
                enemy.SetStartPosition(PlayerGo, new Point(7, 13));
                enemy.SetStartPosition(PlayerGo, new Point());
                enemy.SetStartPosition(PlayerGo, new Point());
                enemy.SetStartPosition(PlayerGo, new Point());
                enemy.SetStartPosition(PlayerGo, new Point());

            }
        }

        public void SpawnEnemy()
        {
            int spawnLocation = 0;
            for (int i = 0; i < totalEnemyAmount; i++)
            {
                if (spawnLocation == spawnLocation.Count) spawnLocation = 0;

                GameObject go = EnemyFactory.Create();
                if (go != null) GameWorld.Instance.Instantiate(go);

                spawnLocation++;
            }
        }
    }
}
