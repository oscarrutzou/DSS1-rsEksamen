using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.LiteDB;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Rooms
{
    public class Room1Scene : RoomBase
    {
        public Room1Scene()
        {
            PlayerSpawnPos = new Point(5, 5);
            GridName = "Test1";
            GridWidth = 24;
            GridHeight = 18;
        }

        public override void Initialize()
        {
            SaveData.Level_Reached = 1;

            base.Initialize();

            SpawnPotions();
        }

        private void SpawnPotions()
        {
            GameObject go = ItemFactory.Create(PlayerGo);
            go.Transform.Position = GridManager.Instance.GetCornerPositionOfCell(new Point(2, 5));
            GameWorld.Instance.Instantiate(go);
        }
    }
}