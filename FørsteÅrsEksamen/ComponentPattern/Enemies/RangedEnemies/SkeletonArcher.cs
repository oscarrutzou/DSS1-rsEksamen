//using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.RangedEnemies
{
    //Asser

    public class SkeletonArcher : EnemyMelee
    {
        public SkeletonArcher(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.OrcIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.OrcRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.OrcDeath);
        }

    }
}