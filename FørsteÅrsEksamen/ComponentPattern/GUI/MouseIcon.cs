using ShamansDungeon.CommandPattern;
using ShamansDungeon.GameManagement.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.ComponentPattern.GUI
{
    public class MouseIcon : Component
    {
        private GameObject _mouseCooldownBarGo, _mouseItemTextGo;
        private readonly Vector2 _cooldownBarOffset = new(60, 0);
        public MouseIcon(GameObject gameObject) : base(gameObject)
        {
        }

        public void SetMouseCooldownBar(GameObject gameObject)
        {
            _mouseCooldownBarGo = gameObject;
        }
        public void SetMouseItemText(GameObject gameObject)
        {
            _mouseItemTextGo = gameObject;
        }

        public override void Awake()
        {
            _mouseCooldownBarGo?.Awake();
            _mouseItemTextGo?.Awake();
        }

        public override void Start()
        {
            _mouseCooldownBarGo?.Start();
            _mouseItemTextGo?.Start();
        }

        public override void Update()
        {
            _mouseItemTextGo.Transform.Position = GameObject.Transform.Position;
            
            _mouseCooldownBarGo.Transform.Position = GameObject.Transform.Position + _cooldownBarOffset;
            
            GameObject.Transform.Position = InputHandler.Instance.MouseOnUI;
            
            if (IndependentBackground.BackgroundEmitter != null)
                IndependentBackground.BackgroundEmitter.FollowPoint = InputHandler.Instance.MouseInWorld;

            _mouseItemTextGo?.Update();
            if (GameWorld.IsPaused) return;
            _mouseCooldownBarGo?.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _mouseItemTextGo?.Draw(spriteBatch);
            if (GameWorld.IsPaused) return;
            _mouseCooldownBarGo?.Draw(spriteBatch);
        }
    }
}
