using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room1Scene : RoomBase
{
    public override void Initialize()
    {
        GridName = "Level1";
        GridWidth = 40;
        GridHeight = 28;

        SaveData.Level_Reached = 1;

        BackGroundTexture = TextureNames.Level1BG;
        ForeGroundTexture = TextureNames.Level1FG;

        base.Initialize();

        Point pos = new(10, 7);
        GameObject go = EnemyFactory.Create(EnemyTypes.OrcMiniBoss, WeaponTypes.Dagger);
        miniBossEnemy = go.GetComponent<MiniBossEnemy>();
        go.Transform.GridPosition = pos;
        go.Transform.Position = GridManager.Instance.CurrentGrid.Cells[pos].Transform.Position;
        miniBossEnemy.SetStartPosition(PlayerGo, pos, false);
        GameWorld.Instance.Instantiate(go);
    }

    private MiniBossEnemy miniBossEnemy;

    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(10, 3);
        EndPointSpawnPos = new Point(33, 2);

        //EnemySpawnPoints = new() {
        //new Point(10, 21),
        //new Point(25, 21),
        //new Point(37, 12),};

        PotionSpawnPoints = new() {
        new Point(7, 4),
        new Point(29, 9),};

        MiscGameObjectsInRoom = new()
        {
            { new Point(13, 5), TraningDummyFactory.Create() }
        };

    }
}