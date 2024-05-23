using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.ComponentPattern.Classes.MeleeClasses;
using FørsteÅrsEksamen.ComponentPattern.Classes.RangedClasses;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies;
using FørsteÅrsEksamen.ComponentPattern.Enemies.RangedEnemies;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern.Weapons;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;

namespace FørsteÅrsEksamen.Factory
{
    public enum EnemyTypes
    {
        OrcWarrior,
        OrcArcher,
        SkeletonWarrior, 
        SkeletonArcher,
    }

    //Asser
    public static class EnemyFactory
    {
        private static Random random = new();
        private static int EnemyDmgDivide = 2;
        public static GameObject CreateWithRandomType()
        {
            Array enemyValue = Enum.GetValues(typeof(EnemyTypes));

            int randomIndex = random.Next(enemyValue.Length);

            EnemyTypes randomType = (EnemyTypes)enemyValue.GetValue(randomIndex);

            return Create(randomType);
        }

        public static GameObject Create(EnemyTypes enemyType)
        {
            GameObject enemyGo = new GameObject();
            enemyGo.Type = GameObjectTypes.Enemy;
            enemyGo.Transform.Scale = new Vector2(4, 4);
            enemyGo.AddComponent<SpriteRenderer>();
            enemyGo.AddComponent<Animator>();
            enemyGo.AddComponent<Collider>();

            enemyGo = AddEnemyComponent(enemyGo, enemyType);

            // Add weapon
            Enemy enemy = enemyGo.GetComponent<Enemy>();

            GameObject weaponGo = WeaponFactory.Create(WeaponTypes.Sword, true);
            Weapon weapon = weaponGo.GetComponent<Weapon>();
            weapon.WeaponUser = enemy;
            weapon.Damage /= EnemyDmgDivide; // Make enemies do less damage
            GameWorld.Instance.Instantiate(weaponGo);

            enemy.WeaponGo = weaponGo;

            // Also add hands

            return enemyGo;
        }

        private static GameObject AddEnemyComponent(GameObject enemyGo, EnemyTypes enemytype)
        {
            switch (enemytype)
            {
                case EnemyTypes.OrcWarrior:
                    enemyGo.AddComponent<OrcWarrior>();
                    break;
                case EnemyTypes.OrcArcher:
                    enemyGo.AddComponent<OrcArcher>();
                    break;
                case EnemyTypes.SkeletonWarrior:
                    enemyGo.AddComponent<SkeletonWarrior>();
                    break;
                case EnemyTypes.SkeletonArcher:
                    enemyGo.AddComponent<SkeletonArcher>();
                    break;
            }

            return enemyGo;
        }

        private static GameObject CreateHands()
        {
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Animator>();
            return go;
        }

    }
}