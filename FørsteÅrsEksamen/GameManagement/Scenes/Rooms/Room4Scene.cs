using ShamansDungeon.ComponentPattern.Enemies.MeleeEnemies;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.Factory;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace ShamansDungeon.GameManagement.Scenes.Rooms;

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
        GameObject go = EnemyFactory.Create(EnemyTypes.OrcMiniBoss, WeaponTypes.MagicStaff, new Vector2(6, 6));
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

    protected override void SetQuestLogText()
    {
        int haveKilledBoss = 1;
        if (_miniBossEnemy.State != CharacterState.Dead) haveKilledBoss = 0;
        QuestText = $"Kill the Shaman Garok {haveKilledBoss} / 1";
    }
}