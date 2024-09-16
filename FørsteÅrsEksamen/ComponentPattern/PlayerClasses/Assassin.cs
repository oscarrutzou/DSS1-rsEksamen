using ShamansDungeon.GameManagement;

namespace ShamansDungeon.ComponentPattern.PlayerClasses;

// Stefan
public class Assassin : Player
{
    public Assassin(GameObject gameObject) : base(gameObject)
    {
        Speed = 425;
        Name = "Whisper";
        DashCooldown = 1.5f;
    }

    public override void Awake()
    {
        base.Awake();

        StatsScreen = GlobalTextures.Textures[TextureNames.PlayerStatsAssassin];

        Health.SetHealth(150);

        CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.ArcherIdle);
        CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.ArcherRun);
        CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.ArcherDeath);
    }
}