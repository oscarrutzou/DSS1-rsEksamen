using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement
{
    public enum TextureNames
    {
        Cell,
        Pixel,
        TestLevelBG,
        TestLevelFG,
        WoodSword,
        WoodArrow,
        HealthPotion,
    }

    // Oscar

    /// <summary>
    /// Contains all the textures we need to use, so we know they are in our project from the start.
    /// </summary>
    public static class GlobalTextures
    {
        public static Dictionary<TextureNames, Texture2D> Textures { get; private set; }
        public static SpriteFont DefaultFont { get; private set; }
        public static SpriteFont BigFont { get; private set; }

        public static void LoadContent()
        {
            ContentManager content = GameWorld.Instance.Content;
            // Load all textures
            Textures = new Dictionary<TextureNames, Texture2D>
            {
                {TextureNames.Cell, content.Load<Texture2D>("World\\16x16White") },
                {TextureNames.Pixel, content.Load<Texture2D>("World\\Pixel") },
                {TextureNames.TestLevelBG, content.Load<Texture2D>("World\\Levels\\Test1UnderPlayer") },
                {TextureNames.TestLevelFG, content.Load<Texture2D>("World\\Levels\\Test1OverPlayer") },
                {TextureNames.WoodSword, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\WoodSword") },
                {TextureNames.WoodArrow, content.Load<Texture2D>("Test\\WoodArrow") },
                {TextureNames.HealthPotion, content.Load<Texture2D>("World\\MediumPotion") }
            };

            // Load all fonts
            DefaultFont = content.Load<SpriteFont>("Fonts\\SmallFont");
            BigFont = content.Load<SpriteFont>("Fonts\\BigFont");
        }
    }
}