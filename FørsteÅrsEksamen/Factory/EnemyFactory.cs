using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.ComponentPattern.Enemies.Skeleton;
using FørsteÅrsEksamen.ComponentPattern.Path;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.Factory
{
    //Asser
    internal class EnemyFactory : Factory
    {
        public override GameObject Create()
        {
            GameObject enemyGo = new GameObject();
            enemyGo.Transform.Scale = new Vector2(4, 4);
            enemyGo.AddComponent<SpriteRenderer>().SetLayerDepth(LAYERDEPTH.Enemies);
            enemyGo.AddComponent<Animator>();
            enemyGo.AddComponent<Collider>();
            enemyGo.AddComponent<Astar>();
            enemyGo.AddComponent<SkeletonWarrior>();
            return enemyGo;
        }
    }
}
