using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms
{
    // Oscar
    public class Room3Scene : RoomBase
    {
        public override void Initialize()
        {
            //throw new System.Exception("This next room is not made yet");
            GridName = "Level3";
            GridWidth = 24;
            GridHeight = 24;

            SaveData.Level_Reached = 3;

            BackGroundTexture = TextureNames.Level3BG;
            ForeGroundTexture = TextureNames.Level3FG;

            base.Initialize();
        }

        protected override void SetSpawnPotions()
        {
            PlayerSpawnPos = new Point(12, 3);
            EndPointSpawnPos = new Point(11, 14);
            enemySpawnPoints = new();
            potionSpawnPoints = new();
            enemySpawnPoints = new() {
            new Point(17, 21),
            new Point(6, 21),};

            potionSpawnPoints = new() {
            //new Point(11, 11),
            new Point(12, 11),};
        }
    }
}