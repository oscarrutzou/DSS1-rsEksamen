using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies;
using FørsteÅrsEksamen.ComponentPattern.Path;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.Factory
{
    //Asser
    internal static class EnemyFactory
    {
        public static GameObject Create()
        {
            GameObject enemyGo = new GameObject();
            enemyGo.Transform.Scale = new Vector2(4, 4);
            enemyGo.AddComponent<SpriteRenderer>();
            enemyGo.AddComponent<Animator>();
            enemyGo.AddComponent<Collider>();
            //enemyGo.AddComponent<Astar>();
            enemyGo.AddComponent<SkeletonWarrior>();
            return enemyGo;
        }
    }
}