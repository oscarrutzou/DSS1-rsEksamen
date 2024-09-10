using ShamansDungeon.ComponentPattern.Weapons.MeleeWeapons;
using ShamansDungeon.GameManagement;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.ComponentPattern.GUI
{
    public class MouseCooldownBar : ScalableBar
    {
        public MouseCooldownBar(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();

            DrawBarColor = GameWorld.TextColor;
            SpriteRenderer.SetSprite(TextureNames.MouseCooldownBar); 
            Collider.SetCollisionBox(5, 20);
            sizeOfDrawnBar = 1f;
            FillWidth = false;
        }

        public override void Update()
        {
            sizeOfDrawnBar = 0f;
            SpriteRenderer.ShouldDrawSprite = false;

            if (SaveData.Player == null) return;

            double normalizedDash = SaveData.Player.DashCooldownTimer / SaveData.Player.DashCooldown;
            if (normalizedDash >= 1) return;

            sizeOfDrawnBar = (float)normalizedDash;

            SpriteRenderer.ShouldDrawSprite = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.IsInMenu)
            {
                SpriteRenderer.ShouldDrawSprite = false;
                return;
            }

            base.Draw(spriteBatch);
        }
    }
}
