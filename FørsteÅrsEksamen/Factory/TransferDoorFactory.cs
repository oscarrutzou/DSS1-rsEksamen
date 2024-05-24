using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.WorldObjects;
using Microsoft.Xna.Framework;


namespace FørsteÅrsEksamen.Factory
{
    public static class TransferDoorFactory
    {
        public static GameObject Create()
        {
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetSprite(GameManagement.TextureNames.Pixel);
            sr.ShouldDraw = false;
            sr.IsCentered = false;

            Collider collider = go.AddComponent<Collider>();
            collider.SetCollisionBox(32, 48, Vector2.Zero); // The size of our door and therefore our exit

            go.AddComponent<TransferDoor>();
            
            return go;
        }
    }
}
