using FørsteÅrsEksamen.DB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Rooms
{
    public class Room2Scene : RoomBase
    {
        public Room2Scene()
        {
            PlayerSpawnPos = new Point(3, 3);
            GridName = "Test2";
            GridHeight = 10;
            GridWidth = 10;
        }

        public override void Initialize()
        {
            Data.Room_Reached = 2;
            //DBGrid.DeleteGrid(GridName);
            base.Initialize();
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);
        }
    }
}