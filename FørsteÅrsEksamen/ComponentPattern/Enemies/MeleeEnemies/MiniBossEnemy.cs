using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using DoctorsDungeon.GameManagement.Scenes.Rooms;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using DoctorsDungeon.ComponentPattern.Effects;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies
{
    /*
     * Tribal Conjurer Lokar: 
     * Ancestral Caller Brug:
     * Spiritforge Garok:
     */
    public class MiniBossEnemy : EnemyMelee
    {
        #region Properties

        private double _spawnTimer;
        private double _spawnCooldown = 5.0;

        private Spawner _enemySpawner;
        private List<Enemy> _spawnedEnemies = new();
        private List<Health> _spawnedEnemiesHealth = new();

        private List<Point> _points = new();
        private int _maxAmountOfSpawns = 3;

        private RoomBase _roomBase;

        private static List<EnemyTypes> _spawnAbleTypes = new List<EnemyTypes>()
        {
            EnemyTypes.OrcArcher,
            EnemyTypes.OrcWarrior,
        };

        private static SoundNames[] _orcBossDeath = new SoundNames[]
        {
            SoundNames.OrcBossDeath1,
            SoundNames.OrcDeath10,
        };

        private static SoundNames[] _orcBossStartTalk = new SoundNames[]
        {
            SoundNames.OrcBossComeForthMyMinions,
        };

        private SoundEffectInstance _mockingSound;
        private static SoundNames[] _orcBossMocking = new SoundNames[]
        {
            SoundNames.OrcBossMocking1,
            SoundNames.OrcBossMocking2,
            SoundNames.OrcBossMocking3,
            SoundNames.OrcBossMocking4,
            SoundNames.OrcBossMocking5,
        };

        private SoundEffectInstance _onelinerBoss;
        private bool _hasPlayedOneLiner = false;
        private double _lastHitTimer;
        private double _mockingCooldown;
        private Interval _mockingCooldownInterval = new Interval(4, 6);

        #region Transition Hide Show Boss
        private double _showHideTransitionCooldown = 2;
        private double _showHideTransitionTimer;
        private bool _showHideTransition;
        #endregion

        #region Spawn Emitter
        private ParticleEmitter _spawnEmitter;
        private static readonly Vector2 _spawnEmitterPosOffset = new Vector2(0, 15);

        private Point _newSpawnPoint;
        private bool _hasSetNewSpawnPoint;
        private float _emitterTimer;
        private float _emitterCooldown;
        private float _startCooldown = 0.5f;
        private float _startSpawnEmitterAmount = 0.5f;
        #endregion
        #endregion
        // Could have it when its health is under a certain amount it begins to flee or spawn faster.
        // Maybe make a easy way to add it, Like a normalized health * 100. So u can write ActionOnCertainHealth 25 for 25% and then the action?

        public MiniBossEnemy(GameObject gameObject) : base(gameObject)
        {
            //CanAttack = false;
        }  

        public void SetRoom(RoomBase roomBase) => this._roomBase = roomBase;

        public override void Awake()
        {
            SpriteOffset = new(0, -70);

            base.Awake();

            Collider.SetCollisionBox(15, 23, new Vector2(0, 40));

            Weapon.StartPosOffset = new(40, 40);
            Weapon.StartRelativeOffsetPos = new(0, -40);
            Weapon.StartRelativePos = new(0, 80);

            Health.SetHealth(400);
            Health.CanTakeDamage = false;
            Speed = 400; // 400

            CharacterDeath = _orcBossDeath;
            Health.OnDamageTaken += () => { _lastHitTimer = 0; };
            _spawnTimer = _spawnCooldown / 2.5f;

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.OrcShamanIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.OrcShamanRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.OrcShamanDeath);

            MakeSpawner();
            MakeSpawnEmitter();

            SetLowHealthStates();
            SetCooldowns();
        }

        private void SetLowHealthStates()
        {
            Health.On75Hp += () => {     
                Speed = 700;
                SetNewRandomPath();
                RandomMoveCoolDown = 0.75f;
            };

            Health.On50Hp += () => { 
                GameWorld.Instance.SingleColorEffect = true;  // Together with a sound effect
                SetNewRandomPath();
                RandomMoveCoolDown = 0.5f;
            };

            Health.On25Hp += () => {
                SetNewRandomPath();
                Speed = 900;
                
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
            Health.OnZeroHealth += KillAllSpawnedEnemies; // So the player get an incentive to kill the boss fast
        }
         
        private void KillAllSpawnedEnemies()
        {
            foreach (Enemy enemy in _spawnedEnemies)
            {
                Health health = enemy.GameObject.GetComponent<Health>();
                health.TakeDamage(9999, PlayerGo.Transform.Position);
            }
        }

        private void MakeSpawner()
        {
            GameObject spawnerGo = new();
            _enemySpawner = spawnerGo.AddComponent<Spawner>();
        }

        private void MakeSpawnEmitter()
        {
            GameObject go = EmitterFactory.CreateParticleEmitter("Boss Spawn", new Vector2(0, 0), new Interval(100, 100), new Interval(-MathHelper.Pi, MathHelper.Pi), 500, new Interval(1500, 1500), 300, 0.05f, new Interval(-MathHelper.Pi, MathHelper.Pi), new Interval(-0.01, 0.01));

            _spawnEmitter = go.GetComponent<ParticleEmitter>();
            _spawnEmitter.LayerName = LayerDepth.BackgroundDecoration;
            _spawnEmitter.AbrubtStop = true;

            _spawnEmitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
            _spawnEmitter.AddBirthModifier(new ScaleBirthModifier(new Interval(0.5, 2)));
            _spawnEmitter.AddBirthModifier(new OutwardBirthModifier());
            _spawnEmitter.AddModifier(new ColorRangeModifier(IndependentBackground.RoomColors));

            _spawnEmitter.AddModifier(new DragModifier(0.2f, 0.5f, 40, 0.8f));
            _spawnEmitter.Origin = new CircleOrigin(20, true);

            GameWorld.Instance.Instantiate(go);
        }


        private bool _isSpawning;
        private Vector2 _newSpawnPos;
        private float _weaponAngle;

        protected override float GetWeaponAngle()
        {
            // !CanAttack return 0f
            //if (!CanAttack) return 0;
            
            // Gets the previous angle from the direction check
            if (!_isSpawning) return _weaponAngle;

            // Get the angle if its spawning
            Vector2 relativePos = _newSpawnPos - GameObject.Transform.Position;
            return (float)Math.Atan2(relativePos.Y, relativePos.X);
        }

        protected override void UpdateDirection()
        {
            if (Direction.X >= 0)
            {
                DirectionState = AnimationDirectionState.Right;
                SpriteRenderer.SpriteEffects = SpriteEffects.None;
                _weaponAngle = 0;
            }
            else if (Direction.X < 0)
            {
                DirectionState = AnimationDirectionState.Left;
                SpriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
                _weaponAngle = MathHelper.Pi;
            }
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

            //CanMove = false;

            if (!HasBeenAwoken || Health.IsDead) return;

            TransitionShowHide();
            CheckForPlayMockingSound();
            UpdateSoundDistance();

            if (!CheckEnemiesAlive()) return; // If there is the max amount of enemy 

            if (_spawnTimer < _spawnCooldown)
            {
                _spawnTimer += GameWorld.DeltaTime;

                SpawnerEmitter();
            }

            PlayStartLine();

            SpawnEnemy();
        }



        private void PlayStartLine()
        {
            if (_hasPlayedOneLiner)
            {
                if (_onelinerBoss != null && _onelinerBoss.State == SoundState.Stopped)
                {
                    Health.CanTakeDamage = true;
                }
                return;
            }

            double normalizedTime = _spawnTimer / _spawnCooldown;
            
            if (normalizedTime < _startSpawnEmitterAmount) return; // Only before the spawning starts
            _hasPlayedOneLiner = true;

            _onelinerBoss = GlobalSounds.PlayRandomizedSound(_orcBossStartTalk, 1);
        }

        private void SetCooldowns()
        {
            _mockingCooldown = _mockingCooldownInterval.GetValue();
        }

        private void UpdateSoundDistance()
        {
            GlobalSounds.ChangeSoundVolumeDistance(GameObject.Transform.Position, 400, 1000, 1, _mockingSound);
        }

        private void CheckForPlayMockingSound()
        {
            _lastHitTimer += GameWorld.DeltaTime;

            if (_lastHitTimer < _mockingCooldown) return;
            
            // Maybe use a bool so its not every singe 5 sec
            _lastHitTimer = 0;
            SetCooldowns();

            _mockingSound = GlobalSounds.PlayRandomizedSound(_orcBossMocking, 1, 1, true);
        }

        private void SpawnerEmitter()
        {
            if (!_hasSetNewSpawnPoint)
            {
                _newSpawnPoint = PickRandomPointInRoom();
                _newSpawnPos = GridManager.Instance.CurrentGrid.GetCellGameObjectFromPoint(_newSpawnPoint).Transform.Position;
                _spawnEmitter.Position = _newSpawnPos + _spawnEmitterPosOffset;

                _emitterCooldown = _startCooldown;
                _hasSetNewSpawnPoint = true;
            }

            // When spawner is at 50% it will begin spawning the emitter.
            // Already have made a spawn area
            double normalizedTime = _spawnTimer / _spawnCooldown;
            if (normalizedTime > _startSpawnEmitterAmount)
            {
                _emitterTimer += (float)GameWorld.DeltaTime;

                // Play a loop until the spawn is complete
                // Make sure to stop the loop if the player dies
                _emitterCooldown = MathHelper.Lerp(_startCooldown * (1 + _startSpawnEmitterAmount), _startCooldown / 4, (float)normalizedTime);
                
                if (_emitterTimer > _emitterCooldown)
                {
                    _emitterTimer = 0;
                    _spawnEmitter.PlayEmitter();
                    _isSpawning = true;
                }
            }
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

        private bool CheckEnemiesAlive()
        {
            // Make this cheaper to run, not as often.
            _spawnedEnemiesHealth.Clear();

            foreach (Enemy enemy in _spawnedEnemies)
            {
                Health health = enemy.GameObject.GetComponent<Health>();
                _spawnedEnemiesHealth.Add(health);
            }

            // Finds all enemies that arent dead
            _spawnedEnemiesHealth = _spawnedEnemiesHealth.FindAll(x => !x.IsDead);

            if (_spawnedEnemiesHealth.Count >= _maxAmountOfSpawns) return false;

            return true;
        }

        private void SpawnEnemy()
        {
            if (_spawnTimer < _spawnCooldown) return;

            _spawnTimer = 0;
            
            _spawnEmitter.StopEmitter();

            // Spawn enemy at location.
            _points.Clear();

            // Amount to spawn
            _points.Add(_newSpawnPoint);

            List<Enemy> newEnemies = _enemySpawner.SpawnEnemies(_points, Player.GameObject, _spawnAbleTypes);

            _roomBase.EnemiesInRoom.AddRange(newEnemies);

            _spawnedEnemies.AddRange(newEnemies);

            foreach (Enemy enemy in newEnemies)
            {
                enemy.HasBeenAwoken = true;
                enemy.CanMove = false;
                enemy.WeaponGo.IsEnabled = false;

                TeleportEffect enemyEffect = enemy.GameObject.GetComponent<TeleportEffect>();
                enemyEffect.PlayEffect = true;
                enemyEffect.OnStopTeleport += EnemyCanMove;
            }

            // Need to update them all
            foreach (Enemy enemy in _roomBase.EnemiesInRoom)
            {
                enemy.SetStartEnemyRefs(_roomBase.EnemiesInRoom);
            }

            _hasSetNewSpawnPoint = false;
            _isSpawning = false;
        }

        private void EnemyCanMove()
        {
            // Need to only do it to the specific enemy. Maybe just put this on the enemy
            foreach (Enemy enemy in _spawnedEnemies)
            {
                // Can move
                enemy.WeaponGo.IsEnabled = true;
                enemy.CanMove = true;

                enemy.SetPathAfterPlayer();
            }
        }
    }
}
