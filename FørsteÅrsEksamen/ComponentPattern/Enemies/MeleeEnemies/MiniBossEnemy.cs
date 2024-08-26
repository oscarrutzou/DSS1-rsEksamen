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

        private List<EnemyTypes> _spawnAbleTypes = new List<EnemyTypes>()
        {
            EnemyTypes.OrcArcher,
            EnemyTypes.OrcWarrior,
        };

        private double _showHideTransitionCooldown = 2;
        private double _showHideTransitionTimer;
        private bool _showHideTransition;

        // Could have it when its health is under a certain amount it begins to flee or spawn faster.
        // Maybe make a easy way to add it, Like a normalized health * 100. So u can write ActionOnCertainHealth 25 for 25% and then the action?

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

            SetLowHealthStates();
        }

        private void SetLowHealthStates()
        {
            Health.On75Hp += () => { GameWorld.Instance.SingleColorEffect = true; }; // Together with a sound effect

            Health.On50Hp += () => { 
                Speed = 700;
                MinimumRandomMoveDistance = 5;
                SearchDistancePx = 2000;
            };

            Health.On25Hp += () => {
                _showHideTransition = true;
                SpriteRenderer.ShouldDrawSprite = false;
                if (Weapon != null)
                    Weapon.SpriteRenderer.ShouldDrawSprite = false;
            };

            Health.OnZeroHealth += () => {
                _showHideTransition = false;
                SpriteRenderer.ShouldDrawSprite = true;
                if (Weapon != null)
                    Weapon.SpriteRenderer.ShouldDrawSprite = true;
            };
            Health.OnZeroHealth += () => { GameWorld.Instance.SingleColorEffect = false; }; // Together with a sound effect
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

            if (!HasBeenAwoken || Health.IsDead) return;


            if (_spawnTimer < _spawnCooldown)
            {
                _spawnTimer += GameWorld.DeltaTime;
            }

            SpawnEnemy();

            TransitionShowHide();
        }

        private void TransitionShowHide()
        {
            if (!_showHideTransition) return;

            if (_showHideTransitionTimer < _showHideTransitionCooldown)
                _showHideTransitionTimer += GameWorld.DeltaTime;

            if (_showHideTransitionTimer >= _showHideTransitionCooldown)
            {
                // Reset timer
                _showHideTransitionTimer = 0;

                SpriteRenderer.ShouldDrawSprite = !SpriteRenderer.ShouldDrawSprite;
                if (Weapon != null)
                    Weapon.SpriteRenderer.ShouldDrawSprite = !Weapon.SpriteRenderer.ShouldDrawSprite;
            }
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

            // Select a random point to spawn the enemy, within a certain radius.
            // Also where it should spawn a visual effect for the spawner, before it spawns.

            // Amount to spawn
            _points.Add(GameObject.Transform.GridPosition);

            List<Enemy> newEnemies = _enemySpawner.SpawnEnemies(_points, Player.GameObject, _spawnAbleTypes);

            _roomBase.EnemiesInRoom.AddRange(newEnemies);

            _spawnedEnemies.AddRange(newEnemies);

            // All spawned enemies know where the player is
            foreach (Enemy enemy in _spawnedEnemies)
            {
                enemy.HasBeenAwoken = true;
            }

            // Need to update them all
            foreach (Enemy enemy in _roomBase.EnemiesInRoom)
            {
                enemy.SetStartEnemyRefs(_roomBase.EnemiesInRoom);
            }

        }

    }
}
