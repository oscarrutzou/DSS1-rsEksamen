using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.WorldObjects;

public class MouseComponent : Component
{
    public MouseComponent(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Update()
    {
        GameObject.Transform.Position = InputHandler.Instance.MouseInWorld;
        IndependentBackground.BackgroundEmitter.FollowPoint = GameObject.Transform.Position;
    }
}
