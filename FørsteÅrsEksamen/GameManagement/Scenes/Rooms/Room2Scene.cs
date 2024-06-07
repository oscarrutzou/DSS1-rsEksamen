using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms
{
    // Oscar
    public class Room2Scene : RoomBase
    {
        public override void Initialize()
        {
            //throw new System.Exception("This next room is not made yet");
            GridName = "Level2";
            GridWidth = 21;
            GridHeight = 27;

            SaveData.Level_Reached = 2;

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
                new Point(11,18),
            };
            PotionSpawnPoints = new()
            {
                new Point(18,25),
            };
        }
    }
}