using FørsteÅrsEksamen.ComponentPattern;
using System.Numerics;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ComponentPattern.Classes;

namespace FørsteÅrsEksamen.Factory
{
    //Asser
    internal static class ItemFactory
    {
        public static GameObject Create(GameObject playerGo)
        {
            GameObject itemGo = new GameObject();
            itemGo.Transform.Scale = new Vector2(4, 4);
            SpriteRenderer sr = itemGo.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.WoodSword);
            sr.SetLayerDepth(LAYERDEPTH.WorldForeground);
            itemGo.AddComponent<Collider>();
            itemGo.AddComponent<PickupableItem>(playerGo);

            return itemGo;
        }
    }
}
