using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{
    /// <summary>
    /// Particle: Component
    ///     Lifetime
    ///     Direction (random)
    ///     
    /// Emitter
    ///     Hold mængde af particles
    ///     ObjectPool
    ///     Ændre sprites på particles
    /// </summary>


    public class TrainingDummy : Component
    {
        private SpriteRenderer spriteRenderer;
        private Health health;
        private Animator animator;

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
            health.MaxHealth = 10_000_000;
            health.AmountDamageTaken += AmountDamageTaken;
        }

        public override void Start()
        {
            spriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder);
            animator.PlayAnimation(AnimNames.SkeletonMageIdle);
            spriteRenderer.OriginOffSet = Character.SmallSpriteOffset;
        }

        private void AmountDamageTaken(int damage)
        {
            totalDmgTaken += damage;
            // Apply the damage
            DamageAccumulated += damage;
            damageHistoryAmount.Enqueue(damage); // Add timestamp to history
            damageHistoryTime.Enqueue(elapsedTime); // Add timestamp to history
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
    }
}
