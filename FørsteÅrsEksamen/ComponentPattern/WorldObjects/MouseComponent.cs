using DoctorsDungeon.CommandPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{
    public class MouseComponent : Component
    {
        public MouseComponent(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Update()
        {
            GameObject.Transform.Position = InputHandler.Instance.MouseInWorld;
            GameWorld.Instance.BackgroundEmitter.FollowPoint = GameObject.Transform.Position;
        }
    }
}
