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
        public static GameObject CreateHealthBar(GameObject characterGo, bool playerHealth)
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Gui;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.UI);
            
            go.AddComponent<Collider>();
            ScalableBar bar = go.AddComponent<ScalableBar>(characterGo, playerHealth);

            return go;
        }
    }
}
