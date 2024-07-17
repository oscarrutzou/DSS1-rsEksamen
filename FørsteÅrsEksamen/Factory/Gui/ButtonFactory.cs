using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement;
using System;

namespace DoctorsDungeon.Factory.Gui;

// Stefan
public static class ButtonFactory
{
    public static GameObject Create(string text, bool invokeActionOnFullScale, Action onClick, TextureNames textureName = TextureNames.SmallBtn)
    {
        GameObject roomBtn = new();
        roomBtn.Transform.Scale = new(6, 6);
        roomBtn.Type = GameObjectTypes.Gui;
        SpriteRenderer sr = roomBtn.AddComponent<SpriteRenderer>();
        sr.SetSprite(textureName);
        roomBtn.AddComponent<Animator>();
        Collider col = roomBtn.AddComponent<Collider>();
        col.SetCollisionBox(sr.Sprite.Width, sr.Sprite.Height);

        roomBtn.AddComponent<Button>(text, invokeActionOnFullScale, onClick);

        return roomBtn;
    }
}