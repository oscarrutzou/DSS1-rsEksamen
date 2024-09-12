using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace ShamansDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room2Scene : RoomBase
{
    private static float _enemyWeakness = 0.5f;
    public override void Initialize()
    {
        GridName = "Level2";
        GridWidth = 21;
        GridHeight = 27;

        SaveData.Level_Reached = 2;
        EnemyWeakness = _enemyWeakness;
        BackGroundTexture = TextureNames.Level2BG;
        ForeGroundTexture = TextureNames.Level2FG;

        base.Initialize();
    }

    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(3, 3);
        EndPointSpawnPos = new Point(5, 10);
        EnemySpawnPoints = new()
        {
            new Point(17,8),
            new Point(4,14),
            new Point(7, 24),
        };
        PotionSpawnPoints = new()
        {
            new Point(13,14),
            new Point(18,25),
        };
    }
}