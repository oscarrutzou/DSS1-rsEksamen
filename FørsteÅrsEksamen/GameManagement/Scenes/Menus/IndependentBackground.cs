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
using DoctorsDungeon.Factory.Gui;

namespace DoctorsDungeon.GameManagement.Scenes.Menus
{
    public static class IndependentBackground
    {
        public static ParticleEmitter BackgroundEmitter { get; set; }
        public static Color[] menuColors = new Color[] { Color.DarkCyan, Color.DarkGray, Color.Gray, Color.Transparent };
        public static Color[] RoomColors = new Color[] { Color.DarkRed, Color.DarkGray, Color.Gray, Color.Transparent };
        // We dont need a factory to do this, since its only this place we are going to use this background.
        private static ColorInterval _menuColorInterval;
        private static ColorInterval _roomColorInterval;

        public static void SpawnBG()
        {
            MakeMouseGo();

            if (!GameWorld.Instance.ShowBG) return;
            GameObject go = EmitterFactory.CreateParticleEmitter("Space Dust", new Vector2(0, 0), new Interval(50, 100), new Interval(-MathHelper.Pi, MathHelper.Pi), 50, new Interval(1500, 2500), 100, -1, new Interval(-MathHelper.Pi, MathHelper.Pi));

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

            _menuColorInterval = new ColorInterval(menuColors);
            _roomColorInterval = new ColorInterval(RoomColors);

            BackgroundEmitter.StartEmitter();
        }

        private static GameObject mouseGo;
        private static void MakeMouseGo()
        {
            InputHandler.Instance.MouseGo = IconFactory.CreateCursorIcon();
            mouseGo = InputHandler.Instance.MouseGo;

            mouseGo.Awake();
            mouseGo.Start();
        }

        public static void Update()
        {
            mouseGo.Update();

            if (!GameWorld.Instance.ShowBG) return;

            // Only stop the emitter if its already running. Otherwise it will keep being in STOPPING state:d
            if (!GameWorld.Instance.ShowBG &&
                BackgroundEmitter != null &&
                BackgroundEmitter.State == Emitter.EmitterState.RUNNING)
            {
                BackgroundEmitter?.StopEmitter();
                return;
            }

            if (BackgroundEmitter == null) SpawnBG();

            ColorRangeModifier colorMod = BackgroundEmitter?.GetModifier<ColorRangeModifier>();
            if (GameWorld.Instance.IsInMenu)
                colorMod.ColorInterval = _menuColorInterval;
            else
                colorMod.ColorInterval = _roomColorInterval;
            
            BackgroundEmitter.Update();
        }

        public static void DrawBG(SpriteBatch spriteBatch)
        {
            if (!GameWorld.Instance.ShowBG) return;

            // Should draw each in the pool.
            foreach (GameObject go in BackgroundEmitter.ParticlePool.Active)
            {
                go.Draw(spriteBatch);
            }
        }
    }
}
