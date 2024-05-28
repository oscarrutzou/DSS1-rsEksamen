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

namespace DoctorsDungeon.GameManagement.Scenes.Menus
{
    public class CharacterSelectorMenu : MenuScene
    {
        private Dictionary<ClassTypes, List<GameObject>> classWeaponButton;
        private Dictionary<ClassTypes, Button> classButtons;
        private Dictionary<WeaponTypes, Button> weaponButtons;

        // Lists of what classes and weapons are done and the user can play as.
        private List<ClassTypes> classTypesThatAreDone = new()
        {
            ClassTypes.Warrior,
        };

        private List<WeaponTypes> weaponTypesThatAreDone = new()
        {
            WeaponTypes.Sword,
        };

        private int spaceBetween = 30;

        public override void Initialize()
        {
            classWeaponButton = new();
            classButtons = new();
            weaponButtons = new();

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(Back));

            // Load Saved classes and
            DBMethods.LoadClassAndWeapons();

            base.Initialize();

            InitBackButton();
        }

        private Color notOwnedColor = Color.Gray;
        private Color ownedColor = Color.White;
        private int costAmount = 50;

        protected override void InitFirstMenu()
        {
            //foreach (ClassTypes type in Enum.GetValues(typeof(ClassTypes)))
            foreach (ClassTypes type in classTypesThatAreDone)
            {
                GameObject btnGo = ButtonFactory.Create($"{type} 50g", true, () => { SelectClass(type); }, TextureNames.LargeBtn);
                FirstMenuObjects.Add(btnGo);

                Button btn = btnGo.GetComponent<Button>();
                classButtons.Add(type, btn);

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
                if (!DBMethods.RemoveCurrency(costAmount)) return;

                // Unlocked the class
                DBMethods.UnlockClass(type);

                classButtons[type].Text = $"{type}";
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
                    if (!weaponTypesThatAreDone.Contains(weaponType)) continue; // If we havent made that weapon yet

                    GameObject btnGo = ButtonFactory.Create($"{weaponType} 50g", true, () => { SeletectWeapon(weaponType); }, TextureNames.LargeBtn);

                    weaponButtonGameObjects.Add(btnGo);

                    // Adds the btn to the weapon buttons to change the text if the user have bought the weapon
                    Button btn = btnGo.GetComponent<Button>();
                    weaponButtons.Add(weaponType, btn);

                    // Checks if we have the type of weapon
                    if (!SaveData.UnlockedWeapons.Contains(weaponType)) continue;

                    btn.Text = $"{weaponType}";
                }

                classWeaponButton.Add(classType, weaponButtonGameObjects);
            }

            foreach (List<GameObject> goList in classWeaponButton.Values)
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
            GuiMethods.PlaceGameObjectsHorizontal(FirstMenuObjects, Vector2.Zero, spaceBetween, true);

            foreach (List<GameObject> goList in classWeaponButton.Values)
            {
                if (goList.Count == 0) continue;

                GuiMethods.PlaceGameObjectsHorizontal(goList, Vector2.Zero, spaceBetween, true);
            }
        }

        private void InitBackButton()
        {
            GameObject backBtn = ButtonFactory.Create("Back", true, Back);

            backBtn.Transform.Position += new Vector2(0, 200 + FirstMenuObjects[0].Transform.Position.Y);
            GameWorld.Instance.Instantiate(backBtn);
        }

        private void SeletectWeapon(WeaponTypes weapon)
        {
            // The player dosent own the weapon
            if (!SaveData.UnlockedWeapons.Contains(weapon))
            {
                // The player cant buy the weapon, return
                if (!DBMethods.RemoveCurrency(costAmount)) return;

                // Unlocked the weapon
                DBMethods.UnlockWeapon(weapon);

                weaponButtons[weapon].Text = $"{weapon}";
            }

            SaveData.SelectedWeapon = weapon;

            NextScene();
        }

        private void NextScene()
        {
            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(SaveData.CurrentSaveID, true);

            DBSaveFile.LoadSaveWeaponType(saveFileData, true);
            DBSaveFile.LoadSaveClassType(saveFileData, true);

            DBRunData.SaveLoadRunData(saveFileData); // Save Run File

            // Go into the new scene with a new player.
            GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, 1);
        }

        private void Back()
        {
            if (!FirstMenuObjects[0].IsEnabled)
            {
                ShowHideClassMenu();
            }
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
                ShowHideGameObjects(classWeaponButton[classType], true);
            }
            else
            {
                ShowHideGameObjects(FirstMenuObjects, true);
                ShowHideGameObjects(classWeaponButton[classType], false);
            }
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            string currentText = $"Currency: {SaveData.Currency}g";
            Vector2 size = GlobalTextures.DefaultFont.MeasureString(currentText);
            Vector2 pos = FirstMenuObjects.Last().Transform.Position + new Vector2(size.X / 2 - 30, -size.Y - 60);
            DrawString(spriteBatch, currentText, pos, Color.DarkRed);
        }

        protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, color, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
        }
    }
}