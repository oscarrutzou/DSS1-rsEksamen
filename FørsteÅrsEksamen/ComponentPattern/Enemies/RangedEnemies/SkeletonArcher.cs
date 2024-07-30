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

        Health.SetHealth(100);

        CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.SkeletonArcherIdle);
        CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.SkeletonArcherRun);
        CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.SkeletonArcherDeath);
    }
}