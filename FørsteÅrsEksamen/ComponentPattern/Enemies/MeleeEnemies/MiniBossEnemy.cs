using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.GameManagement.Scenes.Rooms;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies
{
    /*
     * Tribal Conjurer Lokar: 
     * Ancestral Caller Brug:
     * Spiritforge Garok:
     */
    public class MiniBossEnemy : EnemyMelee
    {
        private double _spawnTimer;
        private double _spawnCooldown = 5.0;

        private Spawner _enemySpawner;
        private List<Enemy> _spawnedEnemies = new();
        private List<Health> _spawnedEnemiesHealth = new();

        private List<Point> _points = new();
        private int _maxAmountOfSpawns = 3;

        private RoomBase _roomBase;

        public MiniBossEnemy(GameObject gameObject) : base(gameObject)
        {
            CanAttack = false;
        }  

        public void SetRoom(RoomBase roomBase) => this._roomBase = roomBase;

        public override void Awake()
        {
            base.Awake();

            Health.SetHealth(400);

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.OrcShamanIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.OrcShamanRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.OrcShamanDeath);

            MakeSpawner();
        }

        private void MakeSpawner()
        {
            GameObject spawnerGo = new();
            _enemySpawner = spawnerGo.AddComponent<Spawner>();
        }

        public override void Attack()
        {
            /*
             * Boss: If hit, hit back if possible.
             * Boss: Not getting hit and spawn timer is down, spawn enemy.
             */
            //SpawnEnemy();

            // Attack with normal
        }

        public override void Update()
        {
            base.Update();

            if (_spawnTimer < _spawnCooldown)
            {
                _spawnTimer += GameWorld.DeltaTime;
            }

            if (Health.IsDead) return;
            SpawnEnemy();
        }
        private void SpawnEnemy()
        {
            if (_spawnTimer < _spawnCooldown) return;

            // Make this cheaper to run, not as often.
            _spawnedEnemiesHealth.Clear();

            foreach (Enemy enemy in _spawnedEnemies)
            {
                Health health = enemy.GameObject.GetComponent<Health>();
                _spawnedEnemiesHealth.Add(health);
            }
            // Finds all enemies that arent dead
            _spawnedEnemiesHealth = _spawnedEnemiesHealth.FindAll(x => !x.IsDead);

            if (_spawnedEnemiesHealth.Count >= _maxAmountOfSpawns) return;

            _spawnTimer = 0;

            // Spawn enemy at location.
            _points.Clear();

            // Select a random point

            // Amount to spawn
            _points.Add(GameObject.Transform.GridPosition);

            List<Enemy> newEnemies = _enemySpawner.SpawnEnemies(_points, Player.GameObject, spawnAbleTypes);

            _roomBase.EnemiesInRoom.AddRange(newEnemies);

            _spawnedEnemies.AddRange(newEnemies);

            foreach (Enemy enemy in _spawnedEnemies)
            {
                enemy.Astar.SetEnemyListReferences(_spawnedEnemies);
                enemy.HasBeenAwoken = true;
            }
        }

        private List<EnemyTypes> spawnAbleTypes = new List<EnemyTypes>()
        {
            EnemyTypes.OrcArcher,
            EnemyTypes.OrcWarrior,
        };

    }
}
