using ShamansDungeon.GameManagement;

namespace ShamansDungeon.ComponentPattern.PlayerClasses;

// Stefan
public class Warrior : Player
{
    public Warrior(GameObject gameObject) : base(gameObject)
    {
        Speed = 350;
        Name = "Ironclad";
    }

    public override void Awake()
    {
        base.Awake();
        StatsScreen = GlobalTextures.Textures[TextureNames.PlayerStatsKnight];

        Health.SetHealth(200);

        CharacterStateAnimations.Add(CharacterState.Idle, AnimNames.KnightIdle);
        CharacterStateAnimations.Add(CharacterState.Moving, AnimNames.KnightRun);
        CharacterStateAnimations.Add(CharacterState.Dead, AnimNames.KnightDeath);
    }
}