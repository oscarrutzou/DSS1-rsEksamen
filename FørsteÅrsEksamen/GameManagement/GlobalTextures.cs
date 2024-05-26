﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement
{
    public enum TextureNames
    {
        Cell,
        Pixel,
        SpaceBG1,
        SpaceBG2,

        Level1BG,
        Level1FG,

        TestLevelBG,
        TestLevelFG,
        WoodSword,
        BoneSword,
        WoodArrow,
        
        HealthPotionFull,
        DoorClosed,
        // UI
        SmallBtn,
        LargeBtn,
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

                {TextureNames.SpaceBG1, content.Load<Texture2D>("World\\Backgrounds\\Space Background1") },
                {TextureNames.SpaceBG2, content.Load<Texture2D>("World\\Backgrounds\\Space Background2") },

                {TextureNames.Level1BG, content.Load<Texture2D>("World\\Levels\\Level1Under") },
                {TextureNames.Level1FG, content.Load<Texture2D>("World\\Levels\\Level1Over") },
                {TextureNames.TestLevelBG, content.Load<Texture2D>("World\\Levels\\Test1UnderPlayer") },
                {TextureNames.TestLevelFG, content.Load<Texture2D>("World\\Levels\\Test1OverPlayer") },

                {TextureNames.WoodSword, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\WoodSword") },
                {TextureNames.BoneSword, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\BoneSword") },
                {TextureNames.WoodArrow, content.Load<Texture2D>("Test\\WoodArrow") },
                {TextureNames.HealthPotionFull, content.Load<Texture2D>("World\\Objects\\Potions\\RedPotion1") },
                {TextureNames.DoorClosed, content.Load<Texture2D>("World\\Objects\\DoorClosed") },

                {TextureNames.SmallBtn, content.Load<Texture2D>("UI\\SmallButtonIndividuel (1)") },
                {TextureNames.LargeBtn, content.Load<Texture2D>("UI\\BigButtonIndividuel (1)") },
            };

            // Load all fonts
            DefaultFont = content.Load<SpriteFont>("Fonts\\SmallFont");
            BigFont = content.Load<SpriteFont>("Fonts\\BigFont");
        }
    }
}