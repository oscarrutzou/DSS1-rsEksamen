using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.ComponentPattern;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace DoctorsDungeon.GameManagement.Scenes.Menus
{
    public static class IndependentBackground
    {
        public static ParticleEmitter BackgroundEmitter { get; set; }
        public static Color[] menuColors = new Color[] { Color.DarkCyan, Color.DarkGray, Color.Gray, Color.Transparent };
        public static Color[] RoomColors = new Color[] { Color.DarkRed, Color.DarkGray, Color.Gray, Color.Transparent };
        // We dont need a factory to do this, since its only this place we are going to use this background.
        private static ColorInterval menuColorInterval;
        private static ColorInterval roomColorInterval;

        public static void SpawnBG()
        {
            if (!GameWorld.Instance.ShowBG) return;
            GameObject go = EmitterFactory.CreateParticleEmitter("Space Dust", new Vector2(0, 0), new Interval(50, 100), new Interval(-MathHelper.Pi, MathHelper.Pi), 70, new Interval(1500, 2500), 400, -1, new Interval(-MathHelper.Pi, MathHelper.Pi));

            BackgroundEmitter = go.GetComponent<ParticleEmitter>();
            BackgroundEmitter.LayerName = LayerDepth.WorldBackground;

            BackgroundEmitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));

            BackgroundEmitter.AddModifier(new ColorRangeModifier(menuColors));
            BackgroundEmitter.AddModifier(new ScaleModifier(0.5f, 2));
            BackgroundEmitter.AddModifier(new InwardModifier(10));

            int buffer = 300; // A buffer around the center, so when the player runs, there are already some particles
            BackgroundEmitter.Origin = new RectangleOrigin(GameWorld.Instance.DisplayWidth + buffer, GameWorld.Instance.DisplayHeight + buffer);

            BackgroundEmitter.CustomDrawingBehavior = true;

            go.Awake();
            go.Start();

            menuColorInterval = new ColorInterval(menuColors);
            roomColorInterval = new ColorInterval(RoomColors);

            MakeMouseGo();

            BackgroundEmitter.StartEmitter();
        }

        private static void MakeMouseGo()
        {
            InputHandler.Instance.MouseGo = new();
            InputHandler.Instance.MouseGo.AddComponent<MouseComponent>();

            InputHandler.Instance.MouseGo.Awake();
            InputHandler.Instance.MouseGo.Start();

        }

        public static void DrawBG(SpriteBatch spriteBatch, bool isInMenu)
        {
            if (!GameWorld.Instance.ShowBG)
            {
                BackgroundEmitter?.StopEmitter();
                return;
            }

            if (BackgroundEmitter == null) SpawnBG();

            BackgroundEmitter.StartEmitter();

            ColorRangeModifier colorMod = BackgroundEmitter.GetModifier<ColorRangeModifier>();
            if (isInMenu)
                colorMod.ColorInterval = menuColorInterval;
            else
                colorMod.ColorInterval = roomColorInterval;

            BackgroundEmitter.Update();
            BackgroundEmitter.Draw(spriteBatch);

            // Should draw each in the pool.
            foreach (GameObject go in BackgroundEmitter.ParticlePool.Active)
            {
                go.Draw(spriteBatch);
            }
        }
    }
}
