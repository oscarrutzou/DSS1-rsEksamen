using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{
    public class Health : Component
    {
        private float damageTimerTotal = 0.2f;
        private double damageTimer;
        public Color DamageTakenColor { get; private set; } = Color.Red;
        private SpriteRenderer spriteRenderer;

        public int MaxHealth { get; private set; } = -1;

        /// <summary>
        /// Gets set in Start of Health component
        /// </summary>
        public int CurrentHealth { get; private set; } = -1;
        public Action OnDamageTaken { get; set; }
        public Action OnZeroHealth { get; set; }
        public Action OnResetColor { get; set; }
        public Action<int> AmountDamageTaken;
        public Health(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        }

        public override void Start()
        {
            if (CurrentHealth != -1) return; // Current health has already been set e.g in the DB, where we load the Player
            CurrentHealth = MaxHealth;
        }

        public override void Update()
        {
            HandleOnDamage();
        }

        public void SetHealth(int maxhealth, int currenthealth = -1)
        {
            if (MaxHealth != -1) return; // Already have set maxHealth like in cheats

            MaxHealth = maxhealth;
            
            if (currenthealth != -1)
            {
                CurrentHealth = currenthealth;
            }
            else
            {
                CurrentHealth = MaxHealth;
            }

        }

        public bool AddHealth(int addHealth)
        {
            if (CurrentHealth == MaxHealth) return false;

            CurrentHealth += addHealth;

            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }

            return true;
        }


        public void TakeDamage(int damage)
        {
            if (CurrentHealth <= 0) return; // Already dead

            AmountDamageTaken?.Invoke(damage);

            int newHealth = CurrentHealth - damage;

            if (newHealth < 0) CurrentHealth = 0;
            else CurrentHealth = newHealth;

            if (CurrentHealth > 0)
            {
                DamageTaken();
                return;
            }

            ResetColor();
            OnZeroHealth?.Invoke();
        }

        private void DamageTaken()
        {
            damageTimer = damageTimerTotal;
            spriteRenderer.Color = DamageTakenColor;
            OnDamageTaken?.Invoke(); // For specific behavor when Damage taken
        }

        private void ResetColor()
        {
            spriteRenderer.Color = Color.White;
            OnResetColor?.Invoke();
        }

        private void HandleOnDamage()
        {
            if (damageTimer <= 0) return;

            damageTimer -= GameWorld.DeltaTime;

            // Count down
            if (damageTimer <= 0)
            {
                ResetColor();
            }
        }
    }
}
