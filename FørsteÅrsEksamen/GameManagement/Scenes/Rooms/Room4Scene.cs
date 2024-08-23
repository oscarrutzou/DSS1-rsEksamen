using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.Factory;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room4Scene : RoomBase
{
    private MiniBossEnemy _miniBossEnemy;

    public override void Initialize()
    {
        GridName = "Level4";
        GridWidth = 45;
        GridHeight = 15;

        SaveData.Level_Reached = 4;

        BackGroundTexture = TextureNames.Level4BG;
        ForeGroundTexture = TextureNames.Level4FG;

        base.Initialize();

        SpawnMiniBoss();
    }

    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(3, 6);
        EndPointSpawnPos = new Point(39, 3);
        EnemySpawnPoints = new();
        PotionSpawnPoints = new() { new Point(19, 4), new Point(34, 13)};
    }

    private void SpawnMiniBoss()
    {
        Point pos = new(25, 8);
        GameObject go = EnemyFactory.Create(EnemyTypes.OrcMiniBoss, WeaponTypes.Dagger);
        go.Transform.GridPosition = pos;
        go.Transform.Position = GridManager.Instance.CurrentGrid.Cells[pos].Transform.Position;

        _miniBossEnemy = go.GetComponent<MiniBossEnemy>();
        _miniBossEnemy.SetStartPosition(PlayerGo, pos, false);
        _miniBossEnemy.SetRoom(this);
        EnemiesInRoom.Add(_miniBossEnemy);

        GameWorld.Instance.Instantiate(go);

        GameObject barGo = ScalableBarFactory.CreateHealthBar(go, false);
        // Should be able to show a name or something, and change the user og the bar
        GameWorld.Instance.Instantiate(barGo);
    }
}