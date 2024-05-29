using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms
{
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
        }

        protected override void SetSpawnPotions()
        {
            PlayerSpawnPos = new Point(10, 3);
            EndPointSpawnPos = new Point(33, 2);

            enemySpawnPoints = new() {
            new Point(10, 21),
            new Point(25, 21),
            new Point(37, 12),};

            potionSpawnPoints = new() {
            new Point(7, 4),
            new Point(29, 9),};
        }
    }
}