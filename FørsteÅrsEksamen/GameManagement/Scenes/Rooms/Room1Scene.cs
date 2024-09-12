using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.ComponentPattern.Particles.Origins;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.Factory;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.LiteDB;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ShamansDungeon.CommandPattern.Commands;
using System.Timers;
using System;
using ShamansDungeon.ComponentPattern.PlayerClasses;

namespace ShamansDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room1Scene : RoomBase
{
    private static float _enemyWeakness = 0.3f;

    public override void Initialize()
    {
        GridName = "Level1";
        GridWidth = 40;
        GridHeight = 28;
        
        SaveData.Level_Reached = 1;
        EnemyWeakness = _enemyWeakness;
        BackGroundTexture = TextureNames.Level1BG;
        ForeGroundTexture = TextureNames.Level1FG;

        base.Initialize();

        // If we want some more help in the first level to show the start potions.
        //RoomSpawner.SpawnPotions(n)
    }

    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(10, 3);
        EndPointSpawnPos = new Point(33, 2);

        EnemySpawnPoints = new() {
        new Point(10, 21),
        new Point(25, 21),
        new Point(37, 12),};

        PotionSpawnPoints = new() {
        new Point(7, 4),
        new Point(29, 9),};

        //MiscGameObjectsInRoom = new()
        //{
        //    { new Point(13, 5), TraningDummyFactory.Create()}
        //};
    }

}