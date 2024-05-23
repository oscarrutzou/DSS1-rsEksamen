//using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies
{
    //Asser

    public class SkeletonWarrior : Melee
    {
        protected Collider playerCollider;
        private float range = 1.5f;
        private Vector2 position;
        private bool inRange = false;

        public SkeletonWarrior(GameObject gameObject) : base(gameObject)
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