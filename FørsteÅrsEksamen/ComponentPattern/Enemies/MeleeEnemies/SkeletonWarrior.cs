//using FørsteÅrsEksamen.ComponentPattern.Characters;
using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies
{
    //Asser

    public class SkeletonWarrior : EnemyMelee
    {
        public SkeletonWarrior(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.OrcBaseIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.OrcBaseRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.OrcBaseDeath);
        }
    }
}