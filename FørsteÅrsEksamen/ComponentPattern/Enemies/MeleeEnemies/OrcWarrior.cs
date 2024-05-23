using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies
{
    //Asser

    public class OrcWarrior : EnemyMelee
    {
        public OrcWarrior(GameObject gameObject) : base(gameObject)
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