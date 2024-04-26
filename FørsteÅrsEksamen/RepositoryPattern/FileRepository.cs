namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class FileRepository : IRepository
    {
        private static FileRepository instance;

        public static FileRepository Instance
        { get { return instance ??= instance = new FileRepository(); } }

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