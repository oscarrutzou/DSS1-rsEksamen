using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace FørsteÅrsEksamen.ComponentPattern.Enemies.Skeleton
{
    //Asser


    public abstract class Skeleton : Enemy
    {
        protected Skeleton(GameObject gameObject) : base(gameObject)
        {
        }
    }
}
