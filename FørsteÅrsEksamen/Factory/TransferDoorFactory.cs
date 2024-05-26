using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.WorldObjects;
using Microsoft.Xna.Framework;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.Factory
{
    public static class TransferDoorFactory
    {
        public static GameObject Create()
        {
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.BackgroundDecoration);
            sr.SetSprite(TextureNames.DoorClosed);
            sr.IsCentered = false;

            go.AddComponent<Collider>();
            go.AddComponent<TransferDoor>();
            
            return go;
        }
    }
}
