using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.LiteDB;
using FørsteÅrsEksamen.Factory;
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
            SaveData.Level_Reached = 2;
            base.Initialize();

            GameObject enemyGo = EnemyFactory.Create(EnemyTypes.OrcWarrior);
            enemyGo.GetComponent<Enemy>().SetStartPosition(PlayerGo, new Point(5, 5));
            
            GameWorld.Instance.Instantiate(enemyGo);
        }
    }
}