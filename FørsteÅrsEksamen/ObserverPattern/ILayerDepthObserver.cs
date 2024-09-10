using ShamansDungeon.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ObserverPattern
{
    public interface ILayerDepthObserver
    {
        void OnLayerDepthChanged(SpriteRenderer spriteRenderer);
    }
}
