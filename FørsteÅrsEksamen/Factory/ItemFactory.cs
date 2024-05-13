using FørsteÅrsEksamen.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.Factory
{
    //Asser
    internal class ItemFactory : Factory
    {
        public override GameObject Create()
        {
            GameObject itemGo = new GameObject();
            itemGo.Transform.Scale = new Vector2(4, 4);
            itemGo.AddComponent<SpriteRenderer>();
            itemGo.AddComponent<Collider>();

            return itemGo;
        }
    }
}
