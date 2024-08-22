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
        private SpriteRenderer _spriteRenderer;
        private Health _health;
        private Animator _animator;
        private Collider _collider;
        private ParticleEmitter _damageTakenEmitter;

        private int _totalDmg_Taken;

        private double _elapsedTime = 0f;
        public int DamageAccumulated = 0;
        private Queue<double> _damageHistoryTime = new Queue<double>(); // To store damage history
        private Queue<int> _damageHistoryAmount = new Queue<int>(); // To store damage history

        public double trackingTime = 5f; // Time window for tracking (5 seconds)
        
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
            totalDmgTaken += damage;
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
            // Update the timer
            _elapsedTime += GameWorld.DeltaTime;

            // Remove old damage values from the history
            while (_damageHistoryTime.Count > 0 && _elapsedTime - _damageHistoryTime.Peek() > trackingTime)
            {
                _damageHistoryTime.Dequeue();
                DamageAccumulated -= _damageHistoryAmount.Dequeue();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GameObject.Transform.Position + new Vector2(-45, 25);
            int dps = DamageAccumulated / (int)trackingTime;
            string text = $"DPS: {dps}";
            spriteBatch.DrawString(GlobalTextures.DefaultFont, text, pos, BaseMath.TransitionColor(Color.Beige), 0f, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
    }
}
