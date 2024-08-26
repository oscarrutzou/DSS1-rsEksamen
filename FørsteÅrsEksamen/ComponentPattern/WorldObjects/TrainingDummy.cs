using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using DoctorsDungeon.ComponentPattern.GUI;
using System;
using System.Diagnostics;
using DoctorsDungeon.Other;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{

    public class TrainingDummy : Component
    {
        public Vector2 TextPosition;
        public string Text;
        private static double _trackingTime = 5f; // Time window for tracking (5 seconds)

        private SpriteRenderer _spriteRenderer;
        private Health _health;
        private Animator _animator;
        private Collider _collider;
        private ParticleEmitter _damageTakenEmitter;

        private int _totalDmgTaken;

        private double _elapsedTime = 0f;
        public int DamageAccumulated = 0;
        private Queue<double> _damageHistoryTime = new Queue<double>(); // To store damage history
        private Queue<int> _damageHistoryAmount = new Queue<int>(); // To store damage history

        
        public TrainingDummy(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            _health = GameObject.GetComponent<Health>();
            _animator = GameObject.GetComponent<Animator>();
            _collider = GameObject.GetComponent<Collider>();


            _collider.SetCollisionBox(15, 27, new Vector2(0, 19));
            _health.SetHealth(100_000_000);

            MakeEmitters();
        
            _health.AmountDamageTaken += AmountDamageTaken;
            _health.AmountDamageTaken += OnDamageTakenText;
        }

        public override void Start()
        {
            _spriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder);
            _animator.PlayAnimation(AnimNames.SkeletonMageIdle);
            _spriteRenderer.DrawPosOffSet = Character.SmallSpriteOffset;

        }

        private void MakeEmitters()
        {
            GameObject textDamageEmitterGo = EmitterFactory.TextDamageEmitter(new Color[] { Color.OrangeRed, Color.DarkRed, Color.Transparent }, GameObject, new Vector2(-20, -95), new RectangleOrigin(50, 5));
            _damageTakenEmitter = textDamageEmitterGo.GetComponent<ParticleEmitter>();

            GameWorld.Instance.Instantiate(textDamageEmitterGo);
        }

        private void AmountDamageTaken(int damage)
        {
            _totalDmgTaken += damage;
            // Apply the damage
            DamageAccumulated += damage;
            _damageHistoryAmount.Enqueue(damage); // Add timestamp to history
            _damageHistoryTime.Enqueue(_elapsedTime); // Add timestamp to history
        }


        private void OnDamageTakenText(int damage)
        {
            _damageTakenEmitter.LayerName = LayerDepth.DamageParticles;
            _damageTakenEmitter.SetParticleText(new TextOnSprite() { Text = damage.ToString() });
            _damageTakenEmitter.EmitParticles();
        }

        public override void Update() 
        {
            TextPosition = GameObject.Transform.Position + new Vector2(0, 25);
            float dps = DamageAccumulated / (int)_trackingTime;
            Text = $"DPS: {dps}";

            // Update the timer
            _elapsedTime += GameWorld.DeltaTime;

            // Remove old damage values from the history
            while (_damageHistoryTime.Count > 0 && _elapsedTime - _damageHistoryTime.Peek() > _trackingTime)
            {
                _damageHistoryTime.Dequeue();
                DamageAccumulated -= _damageHistoryAmount.Dequeue();
            }
        }
    }
}
