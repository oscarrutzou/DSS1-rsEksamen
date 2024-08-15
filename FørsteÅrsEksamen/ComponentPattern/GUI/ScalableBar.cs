using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.GUI
{
    public class ScalableBar : Component
    {
        private GameObject _characterHealthBar;
        private bool _playerHealth;
        private Collider _collider;
        private SpriteRenderer _spriteRenderer;
        private Vector2 Position
        {
            get
            {
                return GameObject.Transform.Position;
            }
            set
            {
                GameObject.Transform.Position = value;
            }
        }
        private float size = 1.0f;

        private readonly Vector2 _playerOffset = new Vector2(80, 40);
        private Color _playerColor = Color.DarkRed;
        private int _playerHealthBarWidth = 80;
        private int _playerHealthBarHeight = 10;
        public ScalableBar(GameObject gameObject) : base(gameObject)
        {
        }

        public ScalableBar(GameObject gameObject, GameObject characterHealthBar, bool playerHealth) : base(gameObject)
        {
            _characterHealthBar = characterHealthBar;
            _playerHealth = playerHealth;
        }


        public override void Awake()
        {
            _collider = GameObject.GetComponent<Collider>();
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            
            if (_playerHealth)
            {
                Position = GameWorld.Instance.UiCam.TopLeft + _playerOffset;
                _spriteRenderer.IsCentered = false;
                _spriteRenderer.SetSprite(TextureNames.PlayerHealthOver);

                Vector2 spriteToColliderDifference = new Vector2(_spriteRenderer.Sprite.Width - _playerHealthBarWidth, _spriteRenderer.Sprite.Height - _playerHealthBarHeight);
                
                if (spriteToColliderDifference != Vector2.Zero)
                    _spriteRenderer.DrawPosOffSet = -spriteToColliderDifference / 2 * GameObject.Transform.Scale;
            }
            else
            {
                Position = GameWorld.Instance.UiCam.BottomCenter;
            }


            _collider.CenterCollisionBox = false;
            _collider.SetCollisionBox(_playerHealthBarWidth, _playerHealthBarHeight);
        }

        public override void Update()
        {
            size -= 0.01f;
        }

        private void PlayerDraw(SpriteBatch spriteBatch)
        {
            // Need to be little under the layerdepth to be under the texture

            Rectangle collisionBox = _collider.CollisionBox;
            float fillAmount = size * collisionBox.Width;
            Rectangle filledRectangle = new Rectangle(collisionBox.X, collisionBox.Y, (int)fillAmount, collisionBox.Height);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel],
                Position,
                filledRectangle,
                _playerColor,
                GameObject.Transform.Rotation,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                _spriteRenderer.LayerDepth - 0.0001f);
        }

        private void EnemyBossDraw(SpriteBatch spriteBatch)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_playerHealth) PlayerDraw(spriteBatch);
            else EnemyBossDraw(spriteBatch);
        }


    }
}
