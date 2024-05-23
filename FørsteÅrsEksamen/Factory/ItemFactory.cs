using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement;
using System.Numerics;

namespace FørsteÅrsEksamen.Factory
{
    //Asser
    public static class ItemFactory
    {
        public static GameObject Create(GameObject playerGo)
        {
            GameObject itemGo = new GameObject();
            itemGo.Transform.Scale = new Vector2(4, 4);
            SpriteRenderer sr = itemGo.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.HealthPotion);
            sr.SetLayerDepth(LAYERDEPTH.WorldForeground);
            itemGo.AddComponent<Collider>();
            itemGo.AddComponent<PickupableItem>(playerGo);

            return itemGo;
        }
    }
}