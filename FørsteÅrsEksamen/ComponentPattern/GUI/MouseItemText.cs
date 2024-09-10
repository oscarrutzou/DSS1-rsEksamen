using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern.Weapons;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;
using ShamansDungeon.GameManagement.Scenes;
using ShamansDungeon.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShamansDungeon.Other;
using ShamansDungeon.LiteDB;

namespace ShamansDungeon.ComponentPattern.GUI;

public class MouseItemText : Component
{
    private SpriteRenderer _sr;
    private Collider _col;
    private Potion _itemOnHover; // Would have the base stuff inside a master class called item, with the text.
    private Vector2 _textOffset = new Vector2(15, 20);
    private readonly Vector2 _itemTextOffset = new(0, -80);
    private readonly Vector2 _backpackOffset = new(-50, 10);
    private Vector2 _backpackTextOffset = new Vector2(15, 80);

    private bool _showBackpackText, _hasPlayedHoverSound;
    public MouseItemText(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        _sr = GameObject.GetComponent<SpriteRenderer>();
        _sr.SetSprite(TextureNames.MouseItemInfo);
        _sr.SetLayerDepth(LayerDepth.UIBackGroundNearCursor);
        _col = GameObject.GetComponent<Collider>();
        _col.SetColliderLayer(ColliderLayer.Gui, new List<ColliderLayer>() { ColliderLayer.BackPackIcon });
    }

    public override void Update()
    {
        _sr.ShouldDrawSprite = false;
        if (SaveData.Player == null) return; 

        if (CheckPotionsInWorld() || CheckBackpackIcon())
        {
            // Play sound once
            PlaySoundOnce();
        }
        else
        {
            // Not hovering over either icon or potion, reset sound
            _hasPlayedHoverSound = false;
        }


    }

    // Check if its on the potion in the world
    private bool CheckPotionsInWorld()
    {
        foreach (GameObject otherGo in SceneData.Instance.GameObjectLists[GameObjectTypes.Items])
        {
            if (!otherGo.IsEnabled) continue;

            Collider otherCollider = otherGo.GetComponent<Collider>();
            if (otherCollider == null) continue;

            if (otherCollider.Contains(InputHandler.Instance.MouseInWorld))
            {
                _showBackpackText = false;

                Potion potion = otherGo.GetComponent<Potion>();
                _itemOnHover = potion;
                _sr.SetSprite(TextureNames.MouseItemInfo);
                _sr.ShouldDrawSprite = true;
                _sr.IsCentered = true;
                _col.CenterCollisionBox = true;

                GameObject.Transform.Position += _itemTextOffset; // We can do this since we set the pos each frame in MouseIcon
                return true;
            }
        }
        return false;
    }

    // Check if its on the item in the backpack.
    private bool CheckBackpackIcon()
    {
        if (SaveData.Player.ItemInInventory == null) return false;

        foreach (ColliderLayer layer in SceneData.Instance.ColliderMeshLists.Keys)
        {
            if (!_col.LayersToCollideWith.Contains(layer)) continue;

            foreach (GameObject otherGo in SceneData.Instance.ColliderMeshLists[layer])
            {
                if (!otherGo.IsEnabled) continue;

                Collider otherCollider = otherGo.GetComponent<Collider>();
                if (otherCollider == null) continue;

                if (otherCollider.Contains(InputHandler.Instance.MouseOnUI))
                {
                    _showBackpackText = true;

                    _sr.SetSprite(TextureNames.MosueBackpackText);
                    _sr.ShouldDrawSprite = true;
                    _sr.IsCentered = false;
                    _col.CenterCollisionBox = false;

                    GameObject.Transform.Position += _backpackOffset; // We can do this since we set the pos each frame in MouseIcon
                    return true;                    
                }
            }
        }

        return false;
    }

    private void PlaySoundOnce()
    {
        if (_hasPlayedHoverSound) return;

        _hasPlayedHoverSound = true;
        GlobalSounds.PlaySound(SoundNames.ButtonHover, maxAmountPlaying: 5, soundVolume: 0.7f, enablePitch: true);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!_sr.ShouldDrawSprite) return;
        string text;
        Vector2 startTextCorner;
        if (_showBackpackText)
        {
            if (SaveData.Player.ItemInInventory == null) return;
            text = SaveData.Player.ItemInInventory.FullPotionText;
            startTextCorner = _col.LeftTopPosRectangle + _backpackTextOffset;
        }
        else
        {
            text = _itemOnHover.FullPotionText;
            startTextCorner = _col.LeftTopPosRectangle + _textOffset;
        }

        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, startTextCorner, BaseMath.TransitionColor(GameWorld.TextColor), 0, Vector2.Zero, 0.8f, SpriteEffects.None, SpriteRenderer.GetLayerDepth(LayerDepth.UITextNearCursor));
    }
}
