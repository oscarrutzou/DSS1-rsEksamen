using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern;
using Microsoft.Xna.Framework.Input;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.DB;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.Factory.Gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using FørsteÅrsEksamen.CommandPattern.Commands;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class CharacterSelectorMenu : MenuScene
    {

        private Dictionary<ClassTypes, List<GameObject>> classWeaponButton;
        private Vector2 buttonScale = new Vector2(9f, 15f);
        public override void Initialize()
        {
            classWeaponButton = new();
            BackCommand();

            base.Initialize();
        }

        protected override void InitFirstMenu()
        {
            foreach (ClassTypes type in Enum.GetValues(typeof(ClassTypes)))
            {
                GameObject btn = ButtonFactory.Create($"{type}", true, () => { SelectClass(type); });
                btn.GetComponent<Button>().ChangeScale(buttonScale);
                FirstMenuObjects.Add(btn);
            }

            GuiMethods.PlaceGameObjectsHorizontal(FirstMenuObjects, Vector2.Zero, 30, true);
        }

        private void SelectClass(ClassTypes type)
        {
            Data.SelectedClass = type;
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
                    GameObject btn = ButtonFactory.Create($"{weaponType}", true, () => { SeletectWeapon(weaponType); });
                    btn.GetComponent<Button>().ChangeScale(buttonScale);
                    gameObjects.Add(btn);
                }

                classWeaponButton.Add(classType, gameObjects);
            }

            foreach (List<GameObject> goList in classWeaponButton.Values)
            {
                ShowHideGameObjects(goList, false);
                GuiMethods.PlaceGameObjectsHorizontal(goList, Vector2.Zero, 30, true);
            }
        }

        private void SeletectWeapon(WeaponTypes weapon)
        {
            Data.SelectedWeapon = weapon;
            // Go into the new scene with a new player.
        }

        private void BackCommand()
        {
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(() =>
            {
                if (!FirstMenuObjects[0].IsEnabled)
                {
                    ShowHideClassMenu();
                }
            }));
        }

        private void ShowHideClassMenu()
        {
            ClassTypes classType = Data.SelectedClass;
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
