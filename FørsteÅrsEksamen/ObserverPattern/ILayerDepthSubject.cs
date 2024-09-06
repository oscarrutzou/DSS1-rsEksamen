using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ObserverPattern
{
    public interface ILayerDepthSubject
    {
        void AddObserver(ILayerDepthObserver observer);
        void NotifyLayerDepthChanged();
    }
}
