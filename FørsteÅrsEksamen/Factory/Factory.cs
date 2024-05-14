using FørsteÅrsEksamen.ComponentPattern;

namespace FørsteÅrsEksamen.Factory
{
    // Oscar
    public abstract class Factory
    {
        public virtual GameObject Create()
        {
            return new GameObject();
        }
    }
}