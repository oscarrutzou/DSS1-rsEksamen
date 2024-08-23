using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.Factory;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.GameManagement.Scenes.Menus;

// Oscar
public class CharacterSelectorMenu : MenuScene
{
    private Dictionary<ClassTypes, List<GameObject>> _classWeaponButton;
    private Dictionary<ClassTypes, Button> _classButtons;
    private Dictionary<WeaponTypes, Button> _weaponButtons;

    // Lists of what classes and weapons are done and the user can play as.
    private List<ClassTypes> _classTypesThatAreDone = new()
    {
        ClassTypes.Warrior,
        ClassTypes.Rogue,
    };

    private int _spaceBetween = 30;

    public override void Initialize()
    {
        SaveData.SetBaseValues();

        _classWeaponButton = new();
        _classButtons = new();
        _weaponButtons = new();

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(Back));

        base.Initialize();

        InitBackButton();
    }

    private int _costAmount = 50;

    protected override void InitFirstMenu()
    {
        foreach (ClassTypes type in _classTypesThatAreDone)
        {
            GameObject btnGo = ButtonFactory.Create($"{type} 50g", true, () => { SelectClass(type); }, TextureNames.WideBtn);
            FirstMenuObjects.Add(btnGo);

            Button btn = btnGo.GetComponent<Button>();
            _classButtons.Add(type, btn);

            // Checks if we have the type of weapon
            if (!SaveData.UnlockedClasses.Contains(type)) continue;

            btn.Text = $"{type}";
        }
    }

    private void SelectClass(ClassTypes type)
    {
        // The player dosent own the class
        if (!SaveData.UnlockedClasses.Contains(type))
        {
            // The player cant buy the class
            if (!DB.Instance.RemoveCurrency(_costAmount)) return;

            DB.Instance.UnlockClass(type);

            _classButtons[type].Text = $"{type}";
        }

        SaveData.SelectedClass = type;
        ShowHideClassMenu();
    }

    // Base dict or osmething to hold data for each class type to show?
    // Or maybe show the weapons / with pictures or text.
    protected override void InitSecondMenu()
    {
        foreach (ClassTypes classType in WeaponFactory.ClassHasWeapons.Keys)
        {
            List<GameObject> weaponButtonGameObjects = new();

            foreach (WeaponTypes weaponType in WeaponFactory.ClassHasWeapons[classType])
            {
                if (!WeaponFactory.WeaponTypesThatAreDone.Contains(weaponType)) continue; // If we havent made that weapon yet

                GameObject btnGo = ButtonFactory.Create($"{weaponType} 50g", true, () => { SeletectWeapon(weaponType); }, TextureNames.WideBtn);

                weaponButtonGameObjects.Add(btnGo);

                // Adds the btn to the weapon buttons to change the text if the user have bought the weapon
                Button btn = btnGo.GetComponent<Button>();
                _weaponButtons.Add(weaponType, btn);

                // Checks if we have the type of weapon
                if (!SaveData.UnlockedWeapons.Contains(weaponType)) continue;

                btn.Text = $"{weaponType}";
            }

            _classWeaponButton.Add(classType, weaponButtonGameObjects);
        }

        foreach (List<GameObject> goList in _classWeaponButton.Values)
        {
            ShowHideGameObjects(goList, false);

            foreach (GameObject gameObject in goList)
            {
                GameWorld.Instance.Instantiate(gameObject);
            }
        }
    }

    public override void AfterFirstCleanUp()
    {
        GuiMethods.PlaceGameObjectsHorizontal(FirstMenuObjects, Vector2.Zero, _spaceBetween, true);

        foreach (List<GameObject> goList in _classWeaponButton.Values)
        {
            if (goList.Count == 0) continue;

            GuiMethods.PlaceGameObjectsHorizontal(goList, Vector2.Zero, _spaceBetween, true);
        }
    }

    private void InitBackButton()
    {
        GameObject backBtn = ButtonFactory.Create("Back", true, Back);

        backBtn.Transform.Position = new Vector2(0, 190 + FirstMenuObjects[0].Transform.Position.Y);
        GameWorld.Instance.Instantiate(backBtn);
    }

    private void SeletectWeapon(WeaponTypes weapon)
    {
        // The player dosent own the weapon
        if (!SaveData.UnlockedWeapons.Contains(weapon))
        {
            // The player cant buy the weapon, return
            if (!DB.Instance.RemoveCurrency(_costAmount)) return;

            // Unlocked the weapon
            DB.Instance.UnlockWeapon(weapon);
        }

        SaveData.SelectedWeapon = weapon;

        NextScene();
    }

    private void NextScene()
    {
        DB.Instance.SaveGame(SaveData.CurrentSaveID);

        // Go into the new scene with a new player.
        GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, 1);
    }

    private void Back()
    {
        if (!FirstMenuObjects[0].IsEnabled)
            ShowHideClassMenu();
        else
        {
            GameWorld.Instance.ChangeScene(SceneNames.SaveFileMenu);
        }
    }

    private void ShowHideClassMenu()
    {
        ClassTypes classType = SaveData.SelectedClass;
        ShowSecondMenu = !ShowSecondMenu;

        if (ShowSecondMenu)
        {
            ShowHideGameObjects(FirstMenuObjects, false);
            ShowHideGameObjects(_classWeaponButton[classType], true);
        }
        else
        {
            ShowHideGameObjects(FirstMenuObjects, true);
            ShowHideGameObjects(_classWeaponButton[classType], false);
        }
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        string currentText = $"Currency: {SaveData.Currency}g";
        Vector2 size = GlobalTextures.DefaultFont.MeasureString(currentText);
        Vector2 pos;

        if (!ShowSecondMenu)
            pos = FirstMenuObjects.Last().Transform.Position + new Vector2(size.X / 2 - 30, -size.Y - 60);
        else
            pos = _classWeaponButton[SaveData.SelectedClass].Last().Transform.Position + new Vector2(size.X / 2 - 30, -size.Y - 60);
        
        DrawString(spriteBatch, currentText, pos, new Color(250, 249, 246));
    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, color, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
    }
}