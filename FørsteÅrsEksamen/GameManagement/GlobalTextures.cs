using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ShamansDungeon.GameManagement;

public enum TextureNames
{
    Cell,
    Pixel,
    PixelTransparent,
    Pixel4x4,
    SpaceBG1,
    SpaceBG2,

    Level1BG,
    Level1FG,
    Level2BG,
    Level2FG,
    Level3BG,
    Level3FG,
    Level4BG,
    Level4FG,


    TestLevelBG,
    TestLevelFG,

    WoodSword,
    BoneSword,
    WoodDagger,
    BoneDagger,
    WoodAxe,
    BoneAxe,
    BoneMagicStaff,

    WoodArrow,

    RedBigPotionHalfFull,
    RedSmallHalfPotion,
    BlueSmallHalfPotion,
    GreenSmallHalfPotion,
    DoorClosed,

    // UI
    ShortBtn,
    LongButton,
    WideBtn,
    DeleteSaveBtn,
    QuestUnder,
    BaseTutorial,
    PlayerStatsKnight,
    PlayerStatsAssassin,
    PlayerHealthOver,
    BossHealthOver,
    BackpackIcon,
    MouseCooldownBar,
    MouseCursorDefault,
    MouseItemInfo,
    MosueBackpackText,
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

    public static Effect SingleColorEffect, GaussianBlurEffect, HighlightsEffect, VignetteEffect, ChromaticAberrationEffect, BlurEffect;
    public static Effect TeleportEffect;
    public static void LoadContent()
    {
        ContentManager content = GameWorld.Instance.Content;

        TeleportEffect = content.Load<Effect>("Shaders\\Teleport");
        SingleColorEffect = content.Load<Effect>("Shaders\\SingleColor");
        GaussianBlurEffect = content.Load<Effect>("Shaders\\GaussianBlur");
        BlurEffect = content.Load<Effect>("Shaders\\Blur");
        HighlightsEffect = content.Load<Effect>("Shaders\\Highlights");
        VignetteEffect = content.Load<Effect>("Shaders\\Vignette");
        ChromaticAberrationEffect = content.Load<Effect>("Shaders\\ChromaticAberration");

        // Load all textures
        Textures = new Dictionary<TextureNames, Texture2D>
        {
            {TextureNames.Cell, content.Load<Texture2D>("World\\16x16White") },
            {TextureNames.Pixel, content.Load<Texture2D>("World\\Pixel") },
            {TextureNames.PixelTransparent, content.Load<Texture2D>("World\\PixelTransparent") },
            {TextureNames.Pixel4x4, content.Load<Texture2D>("World\\4x4Pixel") },

            {TextureNames.SpaceBG1, content.Load<Texture2D>("World\\Backgrounds\\Space Background1") },
            {TextureNames.SpaceBG2, content.Load<Texture2D>("World\\Backgrounds\\Space Background2") },

            {TextureNames.Level1BG, content.Load<Texture2D>("World\\Levels\\Level1Under") },
            {TextureNames.Level1FG, content.Load<Texture2D>("World\\Levels\\Level1Over") },

            {TextureNames.Level2BG, content.Load<Texture2D>("World\\Levels\\Level2Under") },
            {TextureNames.Level2FG, content.Load<Texture2D>("World\\Levels\\Level2Over") },

            {TextureNames.Level3BG, content.Load<Texture2D>("World\\Levels\\Level3Under") },
            {TextureNames.Level3FG, content.Load<Texture2D>("World\\Levels\\Level3Over") },

            {TextureNames.Level4BG, content.Load<Texture2D>("World\\Levels\\Room4\\Level4Under") },
            {TextureNames.Level4FG, content.Load<Texture2D>("World\\Levels\\Room4\\Level4Over") },

            {TextureNames.TestLevelBG, content.Load<Texture2D>("World\\Levels\\Test1UnderPlayer") },
            {TextureNames.TestLevelFG, content.Load<Texture2D>("World\\Levels\\Test1OverPlayer") },

            {TextureNames.WoodSword, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\WoodSword") },
            {TextureNames.BoneSword, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\BoneSword") },
            {TextureNames.WoodDagger, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\WoodDagger") },
            {TextureNames.BoneDagger, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\BoneDagger") },
            {TextureNames.BoneAxe, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\BoneAxe") },
            {TextureNames.WoodAxe, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\WoodAxe") },
            {TextureNames.BoneMagicStaff, content.Load<Texture2D>("World\\Weapons\\Melee Weapons\\BoneMagicStaff") },

            {TextureNames.WoodArrow, content.Load<Texture2D>("Test\\WoodArrow") },

            {TextureNames.RedBigPotionHalfFull, content.Load<Texture2D>("World\\Objects\\Potions\\RedPotion2") },
            {TextureNames.RedSmallHalfPotion, content.Load<Texture2D>("World\\Objects\\Potions\\LittleRedPotion1") },
            {TextureNames.BlueSmallHalfPotion, content.Load<Texture2D>("World\\Objects\\Potions\\LittleBluePotion1") },
            {TextureNames.GreenSmallHalfPotion, content.Load<Texture2D>("World\\Objects\\Potions\\LittleGreenPotion1") },

            {TextureNames.DoorClosed, content.Load<Texture2D>("World\\Objects\\DoorClosed") },

            {TextureNames.ShortBtn, content.Load<Texture2D>("UI\\ShortButton") },
            {TextureNames.LongButton, content.Load<Texture2D>("UI\\LongButton") },
            {TextureNames.WideBtn, content.Load<Texture2D>("UI\\WideButton") },
            {TextureNames.DeleteSaveBtn, content.Load<Texture2D>("UI\\DeleteSaveButton") },
            {TextureNames.QuestUnder, content.Load<Texture2D>("UI\\Quest") },
            {TextureNames.PlayerHealthOver, content.Load<Texture2D>("UI\\PlayerHealthOver") },
            {TextureNames.BackpackIcon, content.Load<Texture2D>("UI\\Icons\\BackpackIcon") },
            {TextureNames.MouseCursorDefault, content.Load<Texture2D>("UI\\Icons\\MouseCursor") },
            {TextureNames.BossHealthOver, content.Load<Texture2D>("UI\\BossHealthBarOver") },
            {TextureNames.MouseCooldownBar, content.Load<Texture2D>("UI\\MouseCooldownBar") },
            {TextureNames.MouseItemInfo, content.Load<Texture2D>("UI\\ItemText") },
            {TextureNames.MosueBackpackText, content.Load<Texture2D>("UI\\BackpackText") },
            {TextureNames.BaseTutorial, content.Load<Texture2D>("UI\\Tutorial") },
            {TextureNames.PlayerStatsKnight, content.Load<Texture2D>("UI\\PlayerStatsKnight") },
            {TextureNames.PlayerStatsAssassin, content.Load<Texture2D>("UI\\PlayerStatsAssassin") },

        };

        // Load all fonts
        DefaultFont = content.Load<SpriteFont>("Fonts\\SmallFont");
        BigFont = content.Load<SpriteFont>("Fonts\\BigFont");
    }
}