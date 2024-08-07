using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using DoctorsDungeon.ComponentPattern.GUI;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{

    public class TrainingDummy : Component
    {
        private SpriteRenderer spriteRenderer;
        private Health health;
        private Animator animator;
        private Collider collider;
        private ParticleEmitter damageTakenEmitter;

        private int totalDmgTaken;

        private double elapsedTime = 0f;
        public int DamageAccumulated = 0;
        private Queue<double> damageHistoryTime = new Queue<double>(); // To store damage history
        private Queue<int> damageHistoryAmount = new Queue<int>(); // To store damage history

        public double trackingTime = 5f; // Time window for tracking (5 seconds)
        
        public TrainingDummy(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            health = GameObject.GetComponent<Health>();
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();

            collider.SetCollisionBox(15, 27, new Vector2(0, 19));
            health.SetHealth(100_000_000);

            MakeEmitters();
        
            health.AmountDamageTaken += AmountDamageTaken;
            health.AmountDamageTaken += OnDamageTakenText;
        }

        public override void Start()
        {
            spriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder);
            animator.PlayAnimation(AnimNames.SkeletonMageIdle);
            spriteRenderer.DrawPosOffSet = Character.SmallSpriteOffset;

        }

        private void MakeEmitters()
        {
            GameObject textDamageEmitterGo = EmitterFactory.TextDamageEmitter(new Color[] { Color.OrangeRed, Color.DarkRed, Color.Transparent }, GameObject, new Vector2(-20, -95), new RectangleOrigin(50, 5));
            damageTakenEmitter = textDamageEmitterGo.GetComponent<ParticleEmitter>();

            GameWorld.Instance.Instantiate(textDamageEmitterGo);
        }

        private void AmountDamageTaken(int damage)
        {
            totalDmgTaken += damage;
            // Apply the damage
            DamageAccumulated += damage;
            damageHistoryAmount.Enqueue(damage); // Add timestamp to history
            damageHistoryTime.Enqueue(elapsedTime); // Add timestamp to history

            // Need to show Damage Accumulated somewhere, maybe at the feet
        }

        private void OnDamageTakenText(int damage)
        {
            damageTakenEmitter.LayerName = LayerDepth.PlayerWeapon;
            damageTakenEmitter.SetParticleText(new TextOnSprite() { Text = damage.ToString() });
            damageTakenEmitter.EmitParticles();
        }

        public override void Update() 
        {
            // Update the timer
            elapsedTime += GameWorld.DeltaTime;

            // Remove old damage values from the history
            while (damageHistoryTime.Count > 0 && elapsedTime - damageHistoryTime.Peek() > trackingTime)
            {
                damageHistoryTime.Dequeue();
                DamageAccumulated -= damageHistoryAmount.Dequeue();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GameObject.Transform.Position + new Vector2(-45, 25);
            int dps = DamageAccumulated / (int)trackingTime;
            string text = $"DPS: {dps}";
            spriteBatch.DrawString(GlobalTextures.DefaultFont, text, pos, Color.Beige, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
    }
}
