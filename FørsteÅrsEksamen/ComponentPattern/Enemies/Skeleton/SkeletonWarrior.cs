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

            enemyStateAnimations.Add(EnemyState.Idle, AnimNames.OrcIdle);
            enemyStateAnimations.Add(EnemyState.Moving, AnimNames.OrcRun);
            enemyStateAnimations.Add(EnemyState.Dead, AnimNames.OrcDeath);
        }
    }
}