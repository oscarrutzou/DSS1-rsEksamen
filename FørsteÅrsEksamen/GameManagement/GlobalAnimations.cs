using DoctorsDungeon.ComponentPattern;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace DoctorsDungeon.GameManagement;

public enum AnimNames
{
    // GUI
    HourGlass,
    HourGlassReset,

    TestWizardRightIndividualFrames,

    // Player Classes
    KnightDeath,

    KnightIdle,
    KnightRun,

    ArcherDeath,
    ArcherIdle,
    ArcherRun,

    MageDeath,
    MageIdle,
    MageRun,

    // Enemy Classes
    OrcBaseDeath,

    OrcBaseIdle,
    OrcBaseRun,

    OrcWarriorDeath,
    OrcWarriorIdle,
    OrcWarriorRun,

    OrcArcherDeath,
    OrcArcherIdle,
    OrcArcherRun,

    OrcShamanDeath,
    OrcShamanIdle,
    OrcShamanRun,

    SkeletonBaseDeath,
    SkeletonBaseIdle,
    SkeletonBaseRun,

    SkeletonWarriorDeath,
    SkeletonWarriorIdle,
    SkeletonWarriorRun,

    SkeletonArcherDeath,
    SkeletonArcherIdle,
    SkeletonArcherRun,

    SkeletonMageDeath,
    SkeletonMageIdle,
    SkeletonMageRun,
}

// Oscar

/// <summary>
/// Contains all the Animation used in the project.
/// </summary>
public static class GlobalAnimations
{
    // Dictionary of all animations
    public static Dictionary<AnimNames, Animation> Animations { get; private set; }

    public static void LoadContent()
    {
        Animations = new Dictionary<AnimNames, Animation>();

        //Sprite sheets


        // Load animations for Archer
        LoadIndividualSprites(AnimNames.ArcherIdle, "World\\Classes\\Rogue\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.ArcherRun, "World\\Classes\\Rogue\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.ArcherDeath, "World\\Classes\\Rogue\\Death", 6, 6);

        // Load animations for Knight
        LoadIndividualSprites(AnimNames.KnightIdle, "World\\Classes\\Knight\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.KnightRun, "World\\Classes\\Knight\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.KnightDeath, "World\\Classes\\Knight\\Death", 6, 6);

        // Load animations for Mage
        LoadIndividualSprites(AnimNames.MageIdle, "World\\Classes\\Wizard\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.MageRun, "World\\Classes\\Wizard\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.MageDeath, "World\\Classes\\Wizard\\Death", 6, 6);


        // Load animations for Orc Base
        LoadIndividualSprites(AnimNames.OrcBaseIdle, "World\\Enemies\\Orc\\Orc - Base\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.OrcBaseRun, "World\\Enemies\\Orc\\Orc - Base\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.OrcBaseDeath, "World\\Enemies\\Orc\\Orc - Base\\Death", 6, 6);

        // Load animations for Orc Warrior
        LoadIndividualSprites(AnimNames.OrcWarriorIdle, "World\\Enemies\\Orc\\Orc - Warrior\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.OrcWarriorRun, "World\\Enemies\\Orc\\Orc - Warrior\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.OrcWarriorDeath, "World\\Enemies\\Orc\\Orc - Warrior\\Death", 6, 6);

        // Load animations for Orc Archer
        LoadIndividualSprites(AnimNames.OrcArcherIdle, "World\\Enemies\\Orc\\Orc - Rogue\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.OrcArcherRun, "World\\Enemies\\Orc\\Orc - Rogue\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.OrcArcherDeath, "World\\Enemies\\Orc\\Orc - Rogue\\Death", 6, 6);

        // Load animations for Orc Shaman
        LoadIndividualSprites(AnimNames.OrcShamanIdle, "World\\Enemies\\Orc\\Orc - Shaman\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.OrcShamanRun, "World\\Enemies\\Orc\\Orc - Shaman\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.OrcShamanDeath, "World\\Enemies\\Orc\\Orc - Shaman\\Death", 6, 7);

        //// Load animations for Skeleton Base
        LoadIndividualSprites(AnimNames.SkeletonBaseIdle, "World\\Enemies\\Skeleton\\Skeleton - Base\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.SkeletonBaseRun, "World\\Enemies\\Skeleton\\Skeleton - Base\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.SkeletonBaseDeath, "World\\Enemies\\Skeleton\\Skeleton - Base\\Death", 6, 8);

        //// Load animations for Skeleton Warrior
        LoadIndividualSprites(AnimNames.SkeletonWarriorIdle, "World\\Enemies\\Skeleton\\Skeleton - Warrior\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.SkeletonWarriorRun, "World\\Enemies\\Skeleton\\Skeleton - Warrior\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.SkeletonWarriorDeath, "World\\Enemies\\Skeleton\\Skeleton - Warrior\\Death", 6, 6);

        //// Load animations for Skeleton Archer
        LoadIndividualSprites(AnimNames.SkeletonArcherIdle, "World\\Enemies\\Skeleton\\Skeleton - Rogue\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.SkeletonArcherRun, "World\\Enemies\\Skeleton\\Skeleton - Rogue\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.SkeletonArcherDeath, "World\\Enemies\\Skeleton\\Skeleton - Rogue\\Death", 6, 6);

        //// Load animations for Skeleton Shaman
        LoadIndividualSprites(AnimNames.SkeletonMageIdle, "World\\Enemies\\Skeleton\\Skeleton - Mage\\Idle", 5, 4);
        LoadIndividualSprites(AnimNames.SkeletonMageRun, "World\\Enemies\\Skeleton\\Skeleton - Mage\\Run", 10, 6);
        LoadIndividualSprites(AnimNames.SkeletonMageDeath, "World\\Enemies\\Skeleton\\Skeleton - Mage\\Death", 6, 6);

        #region How to Upload Individual Frame Animation

        // Here can you upload animation that is are individual frames.
        // Before you upload the textures, make sure you select them all, and rename them to e.g. walkRight.
        // Windows will then change the names to end with " (1)". And count it up.
        // You just need to place the path, dosent matter what number it is in the animation, and it will get loaded

        #endregion How to Upload Individual Frame Animation

        LoadIndividualFramesAnimation(AnimNames.HourGlass, "UI\\Icons\\HourGlass\\HourGlass (1)", 3, 10);
        LoadIndividualFramesAnimation(AnimNames.HourGlassReset, "UI\\Icons\\HourGlass\\HourGlassReset (1)", 2, 2);
    }

    /// <summary>
    /// Load the spriteSheet into the Animation
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="path">Dont use the absolute path</param>
    /// <param name="fps"></param>
    /// <param name="dem">Demension of the sprite width/height. Need to be same on both sides</param>
    private static void LoadSpriteSheet(AnimNames animName, string path, int fps, int dem)
    {
        Texture2D[] sprite = new Texture2D[]{
            GameWorld.Instance.Content.Load<Texture2D>(path)
        };
        Animations.Add(animName, new Animation(animName, sprite, fps, dem));
    }

    /// <summary>
    /// Loads individual frames into a Animation
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="path">Dont use the absolut path</param>
    /// <param name="fps"></param>
    /// <param name="framesInAnim"></param>
    /// <exception cref="System.Exception"></exception>
    private static void LoadIndividualFramesAnimation(AnimNames animationName, string path, int fps, int framesInAnim)
    {
        #region How we use regular expressions

        // Remove the "(number)" from the path using regular expressions, so it dosent matter what the animation number that has been chosen
        // We use a @ to make the string into a verbatim string literal, so the string is not processed
        // For example, we have 2 strings. One is "Hallo\n World" that will out put "Hallo" and on the next line "World".
        // If we use @ like @"Hallo\n World" it wil just write "Hallo\n World".

        // Next we have the space " " and the the "\(" and "\)" match the literal characters "(" and ")" respectively.
        // The "\d" makes it so we can match digits from [0-9] and the "+" makes it work even with large digits like "1234".
        // For example, in the string "1234", \d+ would match the entire string, because it’s one or more digits. But in the string "12 34", \d+ would match "12" and "34" separately, because the space breaks up the sequence of digits.

        // After we have found it, we the replace it with nothing "", so we have the clean path.

        #endregion How we use regular expressions

        path = System.Text.RegularExpressions.Regex.Replace(path, @" \(\d+\)", "");

        // Gets the aboslutePath to check what kind of animation it is.
        string contentRoot = Path.Combine(Directory.GetCurrentDirectory(), GameWorld.Instance.Content.RootDirectory);

        // You can use the absolutePath for loading, but we have chosen not to do that since it would be very messy when calling the Load methods
        // We make a path with the max frames, to ensure there are the correct amount of frames and the path is correct.
        string absolutePath = Path.Combine(contentRoot, path + $" ({framesInAnim})" + ".xnb");

        if (File.Exists(absolutePath))
        {
            Texture2D[] sprites = new Texture2D[framesInAnim];
            // Here we load our sprites into our arrays
            for (int i = 0; i < framesInAnim; i++)
            {
                sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(path + $" ({i + 1})");
            }
            // We add it to the Animation, for other scripts to use.
            Animations.Add(animationName, new Animation(animationName, sprites, fps));
        }
        else
        {
            throw new System.Exception($"Cant find path in directory {absolutePath}");
        }
    }

    /// <summary>
    /// It should be called something tile tile000, tile001
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="path"></param>
    /// <param name="fps"></param>
    /// <param name="framesInAnim"></param>
    private static void LoadIndividualSprites(AnimNames animationName, string path, int fps, int framesInAnim)
    {
        Texture2D[] sprites = new Texture2D[framesInAnim];
        // Here we load our sprites into our arrays
        for (int i = 0; i < framesInAnim; i++)
        {
            // World\\Enemies\\Skeleton\\Skeleton - Mage\\Run\\tile001
            string formattedNumber = i.ToString("D3");
            sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(path + "\\tile" + formattedNumber);
        }
        // We add it to the Animation, for other scripts to use.
        Animations.Add(animationName, new Animation(animationName, sprites, fps));
    }
}