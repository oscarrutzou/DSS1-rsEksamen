using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement.Scenes.Menus;

// Erik
public class SaveFileMenu : MenuScene
{
    private Dictionary<int, Button> saveFileButtons;
    private readonly string newSaveFile = "New Save";

    protected override void InitFirstMenu()
    {
        saveFileButtons = new Dictionary<int, Button>()
        {
            { 1, ButtonFactory.Create(newSaveFile, true, () => { MakeNewSaveFile(1); }, TextureNames.LargeBtn).GetComponent<Button>() },
            { 2, ButtonFactory.Create(newSaveFile, true, () => { MakeNewSaveFile(2); }, TextureNames.LargeBtn).GetComponent<Button>()  },
            { 3, ButtonFactory.Create(newSaveFile, true, () => { MakeNewSaveFile(3); }, TextureNames.LargeBtn).GetComponent<Button>()  }
        };

        foreach (Button button in saveFileButtons.Values)
        {
            FirstMenuObjects.Add(button.GameObject);
        }

        GameObject backBtn = ButtonFactory.Create("Back", true, () =>
        {
            GameWorld.Instance.ChangeScene(SceneNames.MainMenu);
        });

        FirstMenuObjects.Add(backBtn);
    }

    public override void AfterFirstCleanUp()
    {
        GuiMethods.PlaceGameObjectsVertical(FirstMenuObjects, TextPos + new Vector2(0, 75), 25);
        ChangeButtonText();
    }

    private void MakeNewSaveFile(int id)
    {
        SaveData.SetBaseValues();

        SaveData.CurrentSaveID = id;

        SaveFileData saveFile = DB.Instance.LoadGame();

        saveFile ??= DB.Instance.SaveGame(id); // Makes the save id

        if (saveFile.RunData == null)
            // Make a new run after the characterSelector
            GameWorld.Instance.ChangeScene(SceneNames.CharacterSelectorMenu);
        else
        {
            SaveData.Time_Left = saveFile.RunData.Time_Left;
            // Loads run
            GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, saveFile.RunData.Room_Reached);
        }
    }

    /// <summary>
    /// Updates the Button text
    /// </summary>
    private void ChangeButtonText()
    {
        List<SaveFileData> saveFiles = DB.Instance.LoadAllSaveFiles();

        if (saveFiles.Count == 0) return; // There is no files yet, so we dont change the text.

        foreach (SaveFileData saveFile in saveFiles)
        {
            if (!saveFileButtons.ContainsKey(saveFile.Save_ID)) continue;

            Button saveFileBtn = saveFileButtons[saveFile.Save_ID];

            saveFileBtn.Text =
                    $"Save {saveFile.Save_ID}" +
                    $"\nCurrency {saveFile.Currency}"; // Removes .ToString+$"\n Last Login {saveFile.Last_Login:MM-dd}"

            // Add a delete button next to it.
            GameObject deleteBtn = ButtonFactory.Create("X", true, () => { DeleteSave(saveFile.Save_ID); }, TextureNames.DeleteSaveBtn);
            Button delete = deleteBtn.GetComponent<Button>();

            deleteBtn.Transform.Position = saveFileBtn.GameObject.Transform.Position + new Vector2(180, 0);
            Collider deleteBtnCol = deleteBtn.GetComponent<Collider>();
            deleteBtnCol.SetCollisionBox(10, 10);

            GameWorld.Instance.Instantiate(deleteBtn);
        }
    }

    private void DeleteSave(int saveID)
    {
        DB.Instance.DeleteSave(saveID);

        GameWorld.Instance.ChangeScene(SceneNames.SaveFileMenu);
    }
}