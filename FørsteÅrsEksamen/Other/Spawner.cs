using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.Other;

// Erik
public class Spawner : Component
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

            GameObject enemyGo;
            if (i % 2 == 0) enemyGo = EnemyFactory.Create(EnemyTypes.OrcWarrior, WeaponTypes.Sword);
            else enemyGo = EnemyFactory.Create(EnemyTypes.OrcArcher, WeaponTypes.Dagger);

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