using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class PostgreRepository : IRepository
    {

        private static FileRepository instance;

        public static FileRepository Instance
        { get { return instance ??= instance = new FileRepository(); } }

        public void Initialize()
        {

        }

        #region Grid

        public void SaveGrid(Grid grid)
        {

        }
        
        public void DeleteGrid(string description)
        {

        }

        public bool DoesGridExist(string description)
        {
            return false;
        }

        public GameObject GetGrid(string description)
        {
            return null;
        }
        #endregion

        public void DeleteDatabase()
        {

        }

    }
}