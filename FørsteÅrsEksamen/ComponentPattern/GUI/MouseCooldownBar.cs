using ShamansDungeon.ComponentPattern.Weapons.MeleeWeapons;
using ShamansDungeon.GameManagement;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.ComponentPattern.GUI
{
    public class MouseCooldownBar : ScalableBar
    {
        private MeleeWeapon _playerWeapon;
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

            if (SaveData.Player.Weapon is MeleeWeapon meleeWeapon)
                _playerWeapon = meleeWeapon;
            else return;

            if (_playerWeapon == null || !_playerWeapon.Attacking) return;
            
            sizeOfDrawnBar = _playerWeapon.NormalizedFullAttack;
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
