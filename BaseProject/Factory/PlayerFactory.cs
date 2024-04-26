using BaseProject.CompositPattern;
using BaseProject.CompositPattern.Characters;
using BaseProject.GameManagement;
using Microsoft.Xna.Framework;

namespace BaseProject.Factory
{
    public class PlayerFactory : Factory
    {
        public override GameObject Create()
        {
            GameObject playerGo = new GameObject();
            playerGo.Transform.Scale = new Vector2(4, 4);
            playerGo.AddComponent<SpriteRenderer>();
            playerGo.AddComponent<Animator>();
            playerGo.AddComponent<Collider>();
            playerGo.AddComponent<Player>();

            return playerGo;
        }
    }
}
