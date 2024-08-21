using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses;

// Stefan
public class Warrior : Player
{
    public Warrior(GameObject gameObject) : base(gameObject)
    {
        Speed = 300;
    }

    public override void Awake()
    {
        base.Awake();

        Health.SetHealth(100);

        CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.KnightIdle);
        CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.KnightRun);
        CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.KnightDeath);
    }
}