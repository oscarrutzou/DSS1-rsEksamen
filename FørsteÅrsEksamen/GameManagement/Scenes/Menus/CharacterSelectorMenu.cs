using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.DB;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.Factory.Gui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class CharacterSelectorMenu : MenuScene
    {

        private Dictionary<ClassTypes, GameObject> classes;
        public override void Initialize()
        {
            classes = new();
            base.Initialize();
        }

        protected override void InitFirstMenu()
        {
            foreach (ClassTypes type in Enum.GetValues(typeof(ClassTypes)))
            {
                GameObject btn = ButtonFactory.Create($"{type}", true, () => { SelectClass(type); });
                btn.GetComponent<Button>().ChangeScale(new Vector2(7.5f, 15f));
                classes.Add(type, btn);
                FirstMenuObjects.Add(btn);
            }

            GuiMethods.PlaceGameObjectsHorizontal(FirstMenuObjects, Vector2.Zero, 30, true);
        }

        private void SelectClass(ClassTypes type)
        {
            Data.SelectedClass = type;
            ShowHideSecondMenu();
        }


        // Base dict or osmething to hold data for each class type to show?
        // Or maybe show the weapons / with pictures or text.

        protected override void InitSecondMenu()
        {
            // Show the 
            foreach (ClassTypes type in WeaponFactory.ClassHasWeapons.Keys)
            {
                GameObject btn = ButtonFactory.Create($"{type}", true, () => { SelectClass(type); });
                btn.GetComponent<Button>().ChangeScale(new Vector2(7.5f, 15f));
                classes.Add(type, btn);
                FirstMenuObjects.Add(btn);
            }

            GuiMethods.PlaceGameObjectsHorizontal(FirstMenuObjects, Vector2.Zero, 30, true);
        }

    }
}
