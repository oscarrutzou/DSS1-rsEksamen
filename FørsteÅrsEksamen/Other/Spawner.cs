using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;
using ShamansDungeon.ComponentPattern.Enemies;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.Factory;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ShamansDungeon.ComponentPattern.Weapons;

namespace ShamansDungeon.Other;

// Erik
public class Spawner : Component
{
    public Spawner(GameObject gameObject) : base(gameObject)
    {
    }

    public List<Enemy> SpawnEnemies(List<Point> spawnLocations, GameObject playerGo, List<EnemyTypes> randomTypes = null, float weakness = 0)
    {
        List<Enemy> enemies = new();
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            Point spawnPoint = spawnLocations[i];

            GameObject enemyGo;

            if (randomTypes == null)
            {
                if (i % 2 == 0) enemyGo = EnemyFactory.Create(EnemyTypes.OrcWarrior, WeaponTypes.Sword);
                else enemyGo = EnemyFactory.Create(EnemyTypes.OrcArcher, WeaponTypes.Dagger);
            }
            else
            {
                enemyGo = EnemyFactory.CreateWithRandomType(randomTypes);
            }

            Enemy enemy = enemyGo.GetComponent<Enemy>();
            enemy.SetStartPosition(playerGo, spawnPoint);

            Weapon weapon = enemy.WeaponGo.GetComponent<Weapon>();
            weapon.EnemyWeakness = weakness;

            enemies.Add(enemy);
            GameWorld.Instance.Instantiate(enemyGo);
        }

        // The enemies need to have references to each other. Need to be changed if there are many enemies
        foreach (Enemy enemy in enemies)
        {
            enemy.SetStartEnemyRefs(enemies);
        }

        return enemies;
    }

    public void SpawnPotions(List<Point> spawnLocations, GameObject playerGo, List<PotionTypes> randomTypes = null)
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            GameObject potionGo;
            if (randomTypes == null)
            {
                potionGo = ItemFactory.CreatePotion(playerGo, PotionTypes.BigHealth);
            }
            else
            {
                potionGo = ItemFactory.CreatePotionWithRandomType(playerGo, randomTypes);
            }

            potionGo.Transform.Position = GridManager.Instance.CurrentGrid.PosFromGridPos(spawnLocations[i]);
            GameWorld.Instance.Instantiate(potionGo);
        }
    }
}