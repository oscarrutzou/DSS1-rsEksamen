using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.GUI
{
    public class HealthBar : ScalableBar
    {
        private GameObject _characterGo;
        private Health _characterHealth;
        private bool _playerHealth;

        private readonly Vector2 _playerBarOffset = new Vector2(80, 40);
        private readonly Vector2 _bossBarOffset = new Vector2(0, -40);
        private Color _color = Color.DarkRed;
        private int _playerHealthBarWidth = 80;
        private int _playerHealthBarHeight = 10;
        private int _bossHealthBarWidth = 120;
        private int _bossHealthBarHeight = 10;

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
                Collider.SetCollisionBox(_bossHealthBarWidth, _bossHealthBarHeight);
            }
        }


        public override void Update()
        {
            sizeOfDrawnBar = (float)_characterHealth.CurrentHealth / _characterHealth.MaxHealth;
        }
    }
}
