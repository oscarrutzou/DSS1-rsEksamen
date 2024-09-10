using ShamansDungeon.ComponentPattern.Enemies;
using ShamansDungeon.ComponentPattern.Enemies.MeleeEnemies;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.GameManagement;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ComponentPattern.GUI
{
    public class HealthBar : ScalableBar
    {
        private GameObject _characterGo;
        private Enemy _bossEnemy;
        private Health _characterHealth;
        private bool _playerHealth;

        private readonly Vector2 _playerBarOffset = new Vector2(80, 40);
        private readonly Vector2 _bossBarOffset = new Vector2(0, -40);
        private Color _color = Color.DarkRed;
        private int _playerHealthBarWidth = 80;
        private int _playerHealthBarHeight = 10;
        private int _bossHealthBarWidth = 120;
        private int _bossHealthBarHeight = 10;

        private float _timer;
        private float _timeBeforeStopDrawing = 3f; // This is for when the health is 0, and we can stop drawing the bosses healthbar

        public HealthBar(GameObject gameObject) : base(gameObject)
        {
        }

        public HealthBar(GameObject gameObject, GameObject characterGo, bool playerHealth) : base(gameObject)
        {
            _characterGo = characterGo;
            _playerHealth = playerHealth;
        }

        public override void Awake()
        {
            base.Awake();

            _characterHealth = _characterGo.GetComponent<Health>();
            DrawBarColor = _color;


            if (_playerHealth)
            {
                SpriteRenderer.IsCentered = false;
                Collider.CenterCollisionBox = false;
                Position = GameWorld.Instance.UiCam.TopLeft + _playerBarOffset;
                SpriteRenderer.SetSprite(TextureNames.PlayerHealthOver);
                SetDrawPosOffset(_playerHealthBarWidth, _playerHealthBarHeight);
                // Tag point, rotate around draw, then use it as offset
                Collider.SetCollisionBox(_playerHealthBarWidth, _playerHealthBarHeight);
            }
            else
            {
                Position = GameWorld.Instance.UiCam.BottomCenter + _bossBarOffset;
                SpriteRenderer.SetSprite(TextureNames.BossHealthOver);
                Collider.SetCollisionBox(_bossHealthBarWidth, _bossHealthBarHeight);

                _bossEnemy = _characterGo.GetComponent<Enemy>();
            }
        }

        public override void Update()
        {
            HideBarIfBossNotAwake();

            sizeOfDrawnBar = (float)_characterHealth.CurrentHealth / _characterHealth.MaxHealth;

            if (!_characterHealth.IsDead) return;
            _timer += (float)GameWorld.DeltaTime;

            if (_timer < _timeBeforeStopDrawing) return;

            GameObject.IsEnabled = false;
            _timer = 0;
        }

        private void HideBarIfBossNotAwake()
        {
            if (_bossEnemy == null) return;

            SpriteRenderer.ShouldDrawSprite = _bossEnemy.HasBeenAwoken;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!SpriteRenderer.ShouldDrawSprite) return;

            base.Draw(spriteBatch);

            Vector2 pos = Collider.CollisionBox.Center.ToVector2();
            string text = $"{_characterHealth.CurrentHealth} / {_characterHealth.MaxHealth}";
            GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, pos, text, BaseMath.TransitionColor(GameWorld.TextColor));
        }
    }
}
