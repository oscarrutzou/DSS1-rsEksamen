using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses
{
    // Stefan
    public class Mage : Player
    {
        public Mage(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.MageIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.MageRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.MageDeath);
        }
    }
}