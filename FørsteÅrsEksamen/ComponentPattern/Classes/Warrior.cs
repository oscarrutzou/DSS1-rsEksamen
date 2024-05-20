using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.ComponentPattern
{
    public class Warrior : Player
    {
        public Warrior(GameObject gameObject) : base(gameObject)
        {
        }

        public Warrior(GameObject gameObject, GameObject handsGo, GameObject movementColliderGo) : base(gameObject, handsGo, movementColliderGo)
        {
        }

        public override void Awake()
        {
            base.Awake();

            characterStateAnimations.Add(CharacterState.Idle, AnimNames.KnightIdle);
            characterStateAnimations.Add(CharacterState.Moving, AnimNames.KnightRun);
            characterStateAnimations.Add(CharacterState.Dead, AnimNames.KnightDeath);
        }
    }
}