using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;
using ShamansDungeon.Factory;

namespace ShamansDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room3Scene : RoomBase
{
    private static float _enemyWeakness = 0.5f;

    public override void Initialize()
    {
        GridName = "Level3";
        GridWidth = 24;
        GridHeight = 24;

        SaveData.Level_Reached = 3;
        EnemyWeakness = _enemyWeakness;

        BackGroundTexture = TextureNames.Level3BG;
        ForeGroundTexture = TextureNames.Level3FG;

        base.Initialize();
    }

    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(12, 3);
        EndPointSpawnPos = new Point(11, 14);
        EnemySpawnPoints = new();

        EnemySpawnPoints = new() {
        new Point(17, 21),
        new Point(6, 21),};

        PotionSpawnPoints = new() {
        //new Point(11, 11),
        new Point(12, 11),};

        MiscGameObjectsInRoom = new()
        {
            { new Point(9, 4), BreakableItemFactory.Create(BreakableItemType.Crate)},
            { new Point(1, 17), BreakableItemFactory.Create(BreakableItemType.FatVaseBlue)},
            { new Point(16, 17), BreakableItemFactory.Create(BreakableItemType.Barrel)},
            { new Point(22, 18), BreakableItemFactory.Create(BreakableItemType.Crate)},
        };
    }
}