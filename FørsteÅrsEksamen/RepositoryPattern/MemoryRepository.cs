namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class MemoryRepository : IRepository
    {
        private static MemoryRepository instance;

        public static MemoryRepository Instance
        { get { return instance ??= instance = new MemoryRepository(); } }

        public void Initialize()
        {
        }

        public void SaveGrids()
        {
        }

        public void LoadGrids()
        {
        }

        public void DeleteGrids()
        {
        }
    }
}