using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses
{
    // Stefan
    public class Archer : Player
    {
        public Archer(GameObject gameObject) : base(gameObject)
        {
            Speed = 175;
            MaxHealth = 80;
            CurrentHealth = MaxHealth;
        }

        public override void Awake()
        {
            base.Awake();

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.ArcherIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.ArcherRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.ArcherDeath);
        }
    }
}