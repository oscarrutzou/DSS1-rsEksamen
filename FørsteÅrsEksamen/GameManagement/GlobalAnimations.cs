using FørsteÅrsEksamen.ComponentPattern;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace FørsteÅrsEksamen.GameManagement
{
    public enum AnimNames
    {
        TestWizardRightSheet,
        TestWizardRightIndividualFrames,

        KnightDeath,
        KnightIdle,
        KnightRun,

        OrcDeath,
        OrcIdle,
        OrcRun,
    }

    // Oscar


    // Brug en enum til enemy typer, tag ToString() og det er hvad animationernes base animation (pakke) skal være. 
    // En enemy animations pakke er der for at holde de forskellige animationer som bliver brugt til hver.
    // Kan være en dict


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

            //Can upload sprite sheets
            LoadSpriteSheet(AnimNames.TestWizardRightSheet, "Test\\AnimationSheet\\wizardRight", 5, 32);

            LoadSpriteSheet(AnimNames.KnightIdle, "World\\Classes\\Knight-Idle", 4, 32);
            LoadSpriteSheet(AnimNames.OrcIdle, "World\\Enemies\\Orc-Idle", 4, 32);

            #region How to Upload Individual Frame Animation

            // Here can you upload animation that is are individual frames.
            // Before you upload the textures, make sure you select them all, and rename them to e.g. walkRight.
            // Windows will then change the names to end with " (1)". And count it up.
            // You just need to place the path, dosent matter what number it is in the animation, and it will get loaded

            #endregion How to Upload Individual Frame Animation

            //LoadIndividualFramesAnimation(AnimNames.TestWizardRightIndividualFrames, "Test\\MultipleFilesAnim\\wizardRight", 5, 8);
            LoadIndividualFramesAnimation(AnimNames.TestWizardRightIndividualFrames, "Test\\MultipleFilesAnim\\wizardRight (1)", 5, 8);
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
    }
}