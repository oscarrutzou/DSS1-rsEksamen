using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.Factory.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FørsteÅrsEksamen.DB;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class SaveFileMenu : MenuScene
    {
        private Dictionary<int, Button> saveFileButtons;

        public override void Initialize()
        {
            base.Initialize();

            // Add command to delete save files? Right click
            
        }
        private string newSaveFile = "New Save File";

        protected override void InitStartMenu()
        {
            // Should check first for any save files. Then take their id, and the currency and add it to the button text.
            // If there is none it will just be "New Save File"

            saveFileButtons = new Dictionary<int, Button>()
            {
                { 1, ButtonFactory.Create(newSaveFile, true, () => { MakeNewSaveFile(1); }).GetComponent<Button>() },
                { 2, ButtonFactory.Create(newSaveFile, true, () => { MakeNewSaveFile(2); }).GetComponent<Button>()  },
                { 3, ButtonFactory.Create(newSaveFile, true, () => { MakeNewSaveFile(3); }).GetComponent<Button>()  }
            };

            foreach (Button button in saveFileButtons.Values)
            {
                button.ChangeScale(new Vector2(14, 5));
                StartMenuObjects.Add(button.GameObject);
            }

            GameObject backBtn = ButtonFactory.Create("Back", true, () =>
            {
                GameWorld.Instance.ChangeScene(SceneNames.MainMenu);
            });
            StartMenuObjects.Add(backBtn);

            ChangeButtonText();
            GuiMethods.PlaceGameObjectsVertical(StartMenuObjects, TextPos + new Vector2(0, 75), 25);
        }

        private void MakeNewSaveFile(int id)
        {
            SaveData.CurrentSaveID = id;

            // Dont override save files
            List<SaveFileData> saveFiles = DBSaveFile.LoadSaveFiles();

            foreach (SaveFileData saveFile in saveFiles)
            {
                if (saveFile.Save_ID == id)
                {
                    SaveData.Currency = saveFile.Currency;
                    // maybe take player and current weapon and other stuff.
                    break;
                }
            }

            DBMethods.SaveGame();

            DBMethods.AddCurrency(100);

            ChangeButtonText(); // Dont need to be here if we go out of this scene.

            //Either load the run or go into the character creator menu.
        }

        /// <summary>
        /// Updates the Button text
        /// </summary>
        private void ChangeButtonText()
        {
            List<SaveFileData> saveFiles = DBSaveFile.LoadSaveFiles();

            if (saveFiles.Count == 0) return; // There is no files yet, so we dont change the text.

            foreach (SaveFileData saveFile in saveFiles)
            {
                if (saveFileButtons.ContainsKey(saveFile.Save_ID))
                {
                    saveFileButtons[saveFile.Save_ID].Text =
                        $"Save {saveFile.Save_ID}" +
                        $"\nCurrency {saveFile.Currency}" +
                        $"\n Last Login {saveFile.Last_Login:yyyy-MM-dd}"; // Removes .ToString
                }
            }
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

        }
    }
}
