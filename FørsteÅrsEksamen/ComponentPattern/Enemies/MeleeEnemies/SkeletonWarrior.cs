using FørsteÅrsEksamen.ComponentPattern.Enemies.Skeleton;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies
{
    //Asser

    internal class SkeletonWarrior : Melee
    {
        public SkeletonWarrior(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            characterStateAnimations.Add(CharacterState.Idle, AnimNames.OrcIdle);
            characterStateAnimations.Add(CharacterState.Moving, AnimNames.OrcRun);
            characterStateAnimations.Add(CharacterState.Dead, AnimNames.OrcDeath);
        }

        internal override void AttackAction()
        {

        }

    }
}