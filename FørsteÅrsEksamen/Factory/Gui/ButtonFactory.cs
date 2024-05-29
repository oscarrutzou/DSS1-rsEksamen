using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement;
using System;

namespace DoctorsDungeon.Factory.Gui
{
    // Stefan
    public static class ButtonFactory
    {
        public static GameObject Create(string text, bool invokeActionOnFullScale, Action onClick, TextureNames textureName = TextureNames.SmallBtn)
        {
            GameObject roomBtn = new();
            roomBtn.Transform.Scale = new(6, 6);
            roomBtn.Type = GameObjectTypes.Gui;
            roomBtn.AddComponent<SpriteRenderer>().SetSprite(textureName);
            roomBtn.AddComponent<Animator>();
            roomBtn.AddComponent<Collider>();
            roomBtn.AddComponent<Button>(text, invokeActionOnFullScale, onClick);

            return roomBtn;
        }
    }
}