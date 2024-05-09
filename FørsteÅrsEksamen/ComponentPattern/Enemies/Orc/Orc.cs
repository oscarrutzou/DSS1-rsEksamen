using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies.Orc
{
    //Asser

    public abstract class Orc : Enemy
    {
        protected Orc(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
