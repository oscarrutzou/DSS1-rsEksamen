using FørsteÅrsEksamen.ComponentPattern;
using System.Numerics;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.Factory
{
    //Asser
    internal class ItemFactory : Factory
    {
        public override GameObject Create()
        {
            GameObject itemGo = new GameObject();
            itemGo.Transform.Scale = new Vector2(4, 4);
            SpriteRenderer sr = itemGo.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.WoodSword);
            sr.SetLayerDepth(LAYERDEPTH.WorldForeground);
            itemGo.AddComponent<Collider>();
            itemGo.AddComponent<PickupableItem>();

            return itemGo;
        }
    }
}
