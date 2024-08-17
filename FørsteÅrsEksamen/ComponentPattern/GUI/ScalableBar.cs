using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.WorldObjects;
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
        private GameObject _characterGo;
        private Health _characterHealth;
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
        private int _bossHealthBarWidth = 120;
        private int _bossHealthBarHeight = 10;

        public ScalableBar(GameObject gameObject) : base(gameObject)
        {
        }

        public ScalableBar(GameObject gameObject, GameObject characterGo, bool playerHealth) : base(gameObject)
        {
            _characterGo = characterGo;
            _playerHealth = playerHealth;
        }

        public override void Awake()
        {
            GameObject.Transform.Rotation = -MathHelper.PiOver4;

            _collider = GameObject.GetComponent<Collider>();
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            _characterHealth = _characterGo.GetComponent<Health>();

            _spriteRenderer.IsCentered = false;
            _collider.CenterCollisionBox = false;

            if (_playerHealth)
            {
                Position = GameWorld.Instance.UiCam.Center + _playerOffset;
                Position += new Vector2(200, 200);
                _spriteRenderer.SetSprite(TextureNames.PlayerHealthOver);
                SetDrawPosOffset(_playerHealthBarWidth, _playerHealthBarHeight);
                // Tag point, rotate around draw, then use it as offset
                _collider.SetCollisionBox(_playerHealthBarWidth, _playerHealthBarHeight);
            }
            else
            {
                Position = GameWorld.Instance.UiCam.BottomCenter;
                Vector2 scale = GameObject.Transform.Scale;
                Position -= new Vector2(_bossHealthBarWidth / 2 * scale.X, _bossHealthBarHeight / 2 * scale.Y) + new Vector2(0, 40);

                _collider.SetCollisionBox(_bossHealthBarWidth, _bossHealthBarHeight);
            }
        }

        private void SetDrawPosOffset(int barWidth, int barHeight)
        {
            Vector2 spriteToColliderDifference = new Vector2(_spriteRenderer.Sprite.Width - _playerHealthBarWidth, _spriteRenderer.Sprite.Height - _playerHealthBarHeight);

            //spriteToColliderDifference = new Vector2(20, 18); // width - barwidth = 20, height - barheight = 2

            if (spriteToColliderDifference != Vector2.Zero)
                _spriteRenderer.DrawPosOffSet = -spriteToColliderDifference / 2 * GameObject.Transform.Scale;
        }

        bool contains;
        public override void Update()
        {
            size = (float)_characterHealth.CurrentHealth / _characterHealth.MaxHealth;
            GameObject.Transform.Rotation += 0.01f;

            contains = _collider.Contains(InputHandler.Instance.MouseOnUI);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            // The draw method for ONLY the drawing part of the scalable bar. 
            // The logic of the contains and collision box is a part of the Collider

            Rectangle collisionBox = _collider.CollisionBox;
            Vector2 pos = new Vector2(_collider.CollisionBox.X, _collider.CollisionBox.Y);

            float fillAmount = size * collisionBox.Width;
            // The final rectangle that will be drawn
            Rectangle filledRectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)fillAmount, collisionBox.Height);

            Color color = contains ? Color.DarkGray : Color.Red;

            // The origin 
            Vector2 origin = Vector2.Zero;
            if (_collider.CenterCollisionBox)
                origin = new Vector2(_collider.CollisionBox.Width / 2, _collider.CollisionBox.Height / 2);
            
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel],
                Position,
                filledRectangle,
                color,
                GameObject.Transform.Rotation,
                origin,
                1f,                                    // Already have scaled from the CollisionBox
                SpriteEffects.None,
                _spriteRenderer.LayerDepth - 0.0001f); // Need to be little under the layerdepth to be under the texture
        }


    }
}
