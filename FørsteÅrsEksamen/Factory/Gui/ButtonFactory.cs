using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using System;

namespace FørsteÅrsEksamen.Factory.Gui
{
    public static class ButtonFactory
    {
        public static GameObject Create(string text, bool invokeActionOnFullScale, Action onClick, TextureNames textureName = TextureNames.SmallBtn, AnimNames animName = AnimNames.SmallBtn)
        {
            GameObject roomBtn = new();
            roomBtn.Transform.Scale = new(6, 6);
            roomBtn.Type = GameObjectTypes.Gui;
            roomBtn.AddComponent<SpriteRenderer>().SetSprite(textureName);
            roomBtn.AddComponent<Animator>();
            roomBtn.AddComponent<Collider>();
            roomBtn.AddComponent<Button>(text, invokeActionOnFullScale, onClick, textureName, animName);

            return roomBtn;
        }
    }
}