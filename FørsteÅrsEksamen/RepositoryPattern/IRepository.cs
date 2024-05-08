using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Grid;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public interface IRepository
    {
        void Initialize();

        void SaveGrid(Grid grid);

        GameObject GetGrid(string description);
        bool DoesGridExist(string description);
        void DeleteGrid(string description);
        void DeleteDatabase();
    }
}