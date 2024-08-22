using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.GUI
{
    public class MouseIcon : Component
    {
        private GameObject _mouseCooldownBarGo;
        private readonly Vector2 _cooldownBarOffset = new(60, 0);
        public MouseIcon(GameObject gameObject) : base(gameObject)
        {
        }

        public void SetMouseCooldownBar(GameObject gameObject)
        {
            _mouseCooldownBarGo = gameObject;
        }

        public override void Awake()
        {
            _mouseCooldownBarGo?.Awake();
        }

        public override void Start()
        {
            _mouseCooldownBarGo?.Start();
        }

        public override void Update()
        {
            _mouseCooldownBarGo.Transform.Position = GameObject.Transform.Position + _cooldownBarOffset;
            GameObject.Transform.Position = InputHandler.Instance.MouseOnUI;
            
            if (IndependentBackground.BackgroundEmitter != null ) 
                IndependentBackground.BackgroundEmitter.FollowPoint = InputHandler.Instance.MouseInWorld;

            _mouseCooldownBarGo.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _mouseCooldownBarGo?.Draw(spriteBatch);
        }
    }
}
