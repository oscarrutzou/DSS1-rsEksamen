using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.ComponentPattern.GUI;

// Oscar
public static class GuiMethods
{
    public static void PlaceGameObjectsVertical(List<GameObject> list, Vector2 startPos, int spaceBetween, bool center = false)
    {
        Collider colReference = list[0].GetComponent<Collider>();
        Button btnScaleReference = list[0].GetComponent<Button>();
        int buttonHeight = colReference.StartCollisionHeight * (int)btnScaleReference.MaxScale.Y;

        if (center)
        {
            // Adjust the starting position to account for the total width of all buttons and spaces
            //startPos -= new Vector2(
            //    0,
            //    (buttonHeight / 2 * (list.Count - 1)) +
            //    (spaceBetween * (list.Count - 2)));
            startPos -= new Vector2(
                0,
                (buttonHeight / 2 * (list.Count - 1)) + (spaceBetween * (list.Count - 1)) / 2);
        }

        for (int i = 0; i < list.Count; i++)
        {
            GameObject btn = list[i];
            int newPosY = (int)startPos.Y + i * (buttonHeight + spaceBetween);
            btn.Transform.Position = new Vector2(startPos.X, newPosY);
        }
    }

    public static void PlaceGameObjectsHorizontal(List<GameObject> list, Vector2 startPos, int spaceBetween, bool center = true)
    {
        // Calculate the width of a button
        Collider colReference = list[0].GetComponent<Collider>();
        Button btnScaleReference = list[0].GetComponent<Button>();
        int buttonWidth = colReference.StartCollisionWidth * (int)btnScaleReference.MaxScale.X;

        if (center)
        {
            // Adjust the starting position to account for the total width of all buttons and spaces
            startPos -= new Vector2(
                    (buttonWidth / 2 * (list.Count - 1)) + (spaceBetween * (list.Count - 1)) / 2
                    , 0);
        }

        // Position each GameObject and instantiate it in the game world
        for (int i = 0; i < list.Count; i++)
        {
            GameObject btn = list[i];
            int newPosX = (int)startPos.X + i * (buttonWidth + spaceBetween);
            btn.Transform.Position = new Vector2(newPosX, startPos.Y);
        }
    }

    /// <summary>
    /// <para>Can take and divide the text and center each part of the text.</para>
    /// </summary>
    public static void DrawTextCentered(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, string text, Color textColor)
    {
        if (string.IsNullOrEmpty(text)) return;

        // Split the text into lines based on the newline character '\n'
        string[] lines = text.Split('\n');

        if (lines.Length == 1) // Only one line so can make a easier and fast way to draw it, if we have another method for 1 line
        {
            DrawSingleLineCentered(spriteBatch, font, position, text, textColor);
            return;
        }

        // Create an array to hold the size of each line
        Vector2[] lineSizes = new Vector2[lines.Length];

        // Measure the size of each line and store it in the array
        for (int i = 0; i < lines.Length; i++)
        {
            lineSizes[i] = font.MeasureString(lines[i]);
        }

        // Find the size of the longest line by comparing the width (X value) of each line. Vector2.Zero is the seed in the Aggregate method
        Vector2 maxSize = lineSizes.Aggregate(Vector2.Zero, (max, current) => (current.X > max.X) ? current : max);

        // Calculate the total height of the text block
        float totalHeight = lines.Length * font.LineSpacing;

        // Calculate the position to center the text based on the size of the longest line and total height
        Vector2 textPosition = position - new Vector2(maxSize.X / 2, totalHeight / 2);

        // Draw each line of the text
        for (int i = 0; i < lines.Length; i++)
        {
            // Calculate the position of the line, centering it horizontally and adjusting vertically based on the line number
            Vector2 linePosition = textPosition + new Vector2((maxSize.X - lineSizes[i].X) / 2, i * font.LineSpacing);

            // Draw the line of text
            spriteBatch.DrawString(font,
                                   lines[i],
                                   linePosition,
                                   textColor,
                                   0,
                                   Vector2.Zero,
                                   1f,
                                   SpriteEffects.None,
                                   1);
        }
    }

    private static void DrawSingleLineCentered(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, string text, Color textColor)
    {
        Vector2 lineSize = font.MeasureString(text);

        // Calculate the position to center the text based on the size of the longest line and total height
        Vector2 textPosition = position - new Vector2(lineSize.X / 2, lineSize.Y / 2);

        spriteBatch.DrawString(font,
                   text,
                   textPosition,
                   textColor,
                   0,
                   Vector2.Zero,
                   1f,
                   SpriteEffects.None,
                   1);
    }

    public static bool IsMouseOverUI()
    {
        Vector2 mousePosUI = InputHandler.Instance.MouseOnUI;

        foreach (GameObject gui in SceneData.GameObjectLists[GameObjectTypes.Gui])
        {
            if (!gui.IsEnabled) continue; // So we only check GameObjects that are enabled

            Collider collider = gui.GetComponent<Collider>();

            if (collider == null) continue;

            if (collider.CollisionBox.Contains(mousePosUI))
            {
                return true;
            }
        }

        return false;
    }
}