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
    public static class ScalableBarFactory
    {

        public static GameObject CreateCooldownBar()
        {
            GameObject go = new()
            {
                Type = GameObjectTypes.Gui
            };
            go.Transform.Rotation = MathHelper.Pi;

            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.SetLayerDepth(LayerDepth.Cursor);

            Collider collider = go.AddComponent<Collider>();

            go.AddComponent<MouseCooldownBar>();

            return go;
        }

        public static GameObject CreateHealthBar(GameObject characterGo, bool playerHealth)
        {
            GameObject go = new()
            {
                Type = GameObjectTypes.Gui
            };
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.UI);
            
            go.AddComponent<Collider>();
            HealthBar bar = go.AddComponent<HealthBar>(characterGo, playerHealth);

            return go;
        }
    }
}
