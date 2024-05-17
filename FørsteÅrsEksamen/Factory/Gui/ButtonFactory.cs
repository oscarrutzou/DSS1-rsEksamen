using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using System;

namespace FørsteÅrsEksamen.Factory.Gui
{
    public static class ButtonFactory
    {
        public static GameObject Create(string text, Action onClick)
        {
            GameObject roomBtn = new();
            roomBtn.Transform.Scale = new(8, 4);
            roomBtn.Type = GameObjectTypes.Gui;
            roomBtn.AddComponent<SpriteRenderer>().SetSprite(GameManagement.TextureNames.Cell);
            roomBtn.AddComponent<Collider>();
            roomBtn.AddComponent<Button>(text, onClick);

            return roomBtn;
        }
    }
}