using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Enemies.Skeleton;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies
{
    //Asser

    internal class SkeletonWarrior : Melee
    {
        internal Collider playerCollider;
        private float range = 1.5f;
        private Vector2 position;
        private bool inRange = false;

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