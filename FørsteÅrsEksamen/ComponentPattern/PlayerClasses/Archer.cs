using DoctorsDungeon.GameManagement;

namespace DoctorsDungeon.ComponentPattern.PlayerClasses;

// Stefan
public class Archer : Player
{
    public Archer(GameObject gameObject) : base(gameObject)
    {
        Speed = 325;
    }

    public override void Awake()
    {
        base.Awake();
        Health.SetHealth(80);

        CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.ArcherIdle);
        CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.ArcherRun);
        CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.ArcherDeath);
    }
}