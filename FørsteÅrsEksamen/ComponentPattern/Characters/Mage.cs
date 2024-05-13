using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.ComponentPattern
{
    public class Mage : Player
    {
        public Mage(GameObject gameObject) : base(gameObject)
        {
        }

        public Mage(GameObject gameObject, GameObject handsGo, GameObject movementColliderGo) : base(gameObject, handsGo, movementColliderGo)
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