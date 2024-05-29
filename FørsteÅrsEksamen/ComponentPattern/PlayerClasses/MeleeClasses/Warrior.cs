using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses.MeleeClasses
{
    // Stefan
    public class Warrior : Player
    {
        public Warrior(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.KnightIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.KnightRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.KnightDeath);
        }
    }
}