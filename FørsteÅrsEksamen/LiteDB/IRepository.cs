using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;

namespace FørsteÅrsEksamen.LiteDB
{
    public interface IRepository
    {
        public void SaveGrid(Grid grid);
        public GameObject LoadGrid(string gridName);
        public bool DoesGridExits(string gridName);
    }
}
