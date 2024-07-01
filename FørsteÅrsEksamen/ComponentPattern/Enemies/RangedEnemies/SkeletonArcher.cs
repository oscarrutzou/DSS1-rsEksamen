//using FørsteÅrsEksamen.ComponentPattern.Characters;
using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.Enemies.RangedEnemies;

//Asser

public class SkeletonArcher : EnemyMelee
{
    public SkeletonArcher(GameObject gameObject) : base(gameObject)
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