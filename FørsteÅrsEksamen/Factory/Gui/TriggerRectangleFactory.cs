using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.Factory.Gui
{
    public static class TriggerRectangleFactory
    {
        public static GameObject Create(Point leftCorner, Point rightCorner, Action onEnter)
        {
            GameObject go = new();
            go.Transform.Scale = new Vector2(1, 1);
            go.AddComponent<SpriteRenderer>();
            Collider col = go.AddComponent<Collider>();
            col.CenterCollisionBox = false;
            go.AddComponent<TriggerRectanglePlayer>(leftCorner, rightCorner, onEnter);

            return go;
        }
    }
}
