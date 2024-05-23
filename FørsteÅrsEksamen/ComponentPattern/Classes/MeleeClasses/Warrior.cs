using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.ComponentPattern.Classes.MeleeClasses
{
    public class Warrior : Player
    {
        public Warrior(GameObject gameObject) : base(gameObject)
        {
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