using DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoctorsDungeon.ComponentPattern.GUI
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

            DrawBarColor = Color.WhiteSmoke;
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

            _playerWeapon = (MeleeWeapon)SaveData.Player.Weapon;
            
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
