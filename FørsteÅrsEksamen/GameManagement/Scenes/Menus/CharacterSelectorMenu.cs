using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern;
using Microsoft.Xna.Framework.Input;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.LiteDB;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.Factory.Gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class CharacterSelectorMenu : MenuScene
    {

        private Dictionary<ClassTypes, List<GameObject>> classWeaponButton;
        private Vector2 buttonScale = new(6, 6);
        private int spaceBetween = 30;

        public override void Initialize()
        {
            classWeaponButton = new();
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(Back));

            base.Initialize();

            InitBackButton();
        }

        protected override void InitFirstMenu()
        {
            foreach (ClassTypes type in Enum.GetValues(typeof(ClassTypes)))
            {
                GameObject btn = ButtonFactory.Create($"{type}", true, () => { SelectClass(type); }, TextureNames.LargeBtn);
                //btn.GetComponent<Button>().ChangeScale(buttonScale);
                FirstMenuObjects.Add(btn);
            }
        }

        private void SelectClass(ClassTypes type)
        {
            SaveData.SelectedClass = type;
            ShowHideClassMenu();
        }

        // Base dict or osmething to hold data for each class type to show?
        // Or maybe show the weapons / with pictures or text.
        protected override void InitSecondMenu()
        {
            foreach (ClassTypes classType in WeaponFactory.ClassHasWeapons.Keys)
            {
                List<GameObject> gameObjects = new();

                foreach (WeaponTypes weaponType in WeaponFactory.ClassHasWeapons[classType])
                {
                    GameObject btn = ButtonFactory.Create($"{weaponType}", true, () => { SeletectWeapon(weaponType); }, TextureNames.LargeBtn);
                    //btn.GetComponent<Button>().ChangeScale(buttonScale);
                    gameObjects.Add(btn);
                }

                classWeaponButton.Add(classType, gameObjects);
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

    }
}
