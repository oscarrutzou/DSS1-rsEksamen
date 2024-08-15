using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.ComponentPattern.Enemies.RangedEnemies;
using DoctorsDungeon.ComponentPattern.Weapons;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.Factory;

public enum EnemyTypes
{
    OrcWarrior,
    OrcArcher,
    OrcShaman,
    SkeletonWarrior,
    SkeletonArcher,
    SkeletonMage,
    OrcMiniBoss,
}

// Asser
public static class EnemyFactory
{
    private static readonly Random random = new();

    public static GameObject CreateWithRandomType(List<EnemyTypes> spawnableTypes)
    {
        EnemyTypes randomType = spawnableTypes[random.Next(0, spawnableTypes.Count)];

        // Need to put them into classes
        List<WeaponTypes> weaponTypes = WeaponFactory.EnemyHasWeapon[randomType];
        WeaponTypes randomWeapon = weaponTypes[random.Next(0, weaponTypes.Count)];

        return Create(randomType, randomWeapon);
    }

    public static GameObject Create(EnemyTypes enemyType, WeaponTypes weaponType)
    {
        GameObject enemyGo = new();
        enemyGo.Type = GameObjectTypes.Enemy;
        enemyGo.AddComponent<SpriteRenderer>();
        enemyGo.AddComponent<Animator>();
        enemyGo.AddComponent<Collider>();
        enemyGo.AddComponent<Health>();

        enemyGo = AddEnemyComponent(enemyGo, enemyType);

        // Add weapon
        Enemy enemy = enemyGo.GetComponent<Enemy>();

        GameObject weaponGo = WeaponFactory.Create(weaponType);
        Weapon weapon = weaponGo.GetComponent<Weapon>();
        weapon.EnemyUser = enemy;
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

            case EnemyTypes.OrcMiniBoss:
                enemyGo.AddComponent<MiniBossEnemy>();
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
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Animator>();
        return go;
    }
}