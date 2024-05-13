using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.Skeleton
{
    //Asser

    internal class SkeletonWarrior : Skeleton
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

    }
}