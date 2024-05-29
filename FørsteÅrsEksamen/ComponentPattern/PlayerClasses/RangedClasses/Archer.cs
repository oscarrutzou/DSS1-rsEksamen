﻿using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses.RangedClasses
{
    // Stefan
    public class Archer : Player
    {
        public Archer(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.KnightIdle);
            CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.KnightRun);
            CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.KnightDeath);
        }
    }
}