namespace FørsteÅrsEksamen.RepositoryPattern
{
    public interface IRepository
    {
        void Initialize();

        void SaveGrids();

        void DeleteGrids();

        void LoadGrids();
    }
}