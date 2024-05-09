using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.Factory
{
    public class PlayerFactory : Factory
    {
        public override GameObject Create()
        {
            GameObject playerGo = new GameObject();
            playerGo.Transform.Scale = new Vector2(4, 4);
            playerGo.AddComponent<SpriteRenderer>();
            playerGo.AddComponent<Animator>();
            playerGo.AddComponent<Collider>().SetCollisionBox(10, 20, new(0, 20));
            playerGo.AddComponent<Player>();

            return playerGo;
        }
    }
}
