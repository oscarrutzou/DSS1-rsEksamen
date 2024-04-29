using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Grid;
using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using System.Reflection;
using SharpDX.Direct3D9;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class FileRepository : IRepository
    {
        private static FileRepository instance;

        public static FileRepository Instance
        { get { return instance ??= instance = new FileRepository(); } }

        private readonly string folderPath;

        public FileRepository()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            folderPath = Path.Combine(exeDirectory, "data");
            Directory.CreateDirectory(folderPath);
        }

        public void Initialize()
        {
        }

        public void SaveGrid(Grid grid, string savedDescription)
        {
            DeleteExistingGrids();

            string path = Path.Combine(folderPath, $"{savedDescription}.txt");
            FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            try
            {
                stream.SetLength(0);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine($"Start Postition (World coordinates): {grid.StartPostion.X}, {grid.StartPostion.Y}");
                writer.WriteLine($"Grids Size (Width, Height): {grid.Width}, {grid.Height}");

                writer.WriteLine("Grid Cells (Position.X, Postion.Y, Cell Type):");
                for (int y = 0; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        GameObject cellGo = grid.Cells[new Point(x, y)];
                        Point gridpos = cellGo.Transform.GridPosition;
                        Cell cell = cellGo.GetComponent<Cell>();
                        writer.WriteLine($"{gridpos.X}, {gridpos.Y}, {cell.CellWalkableType}");
                    }
                }

                writer.Flush();
            }
            finally
            {
                stream.Close();
            }
        }

        public GameObject GetGrid(string description)
        {
            GameObject gridGo = new();
            string gridName = null;
            string fullPathName = null;

            string[] files = Directory.GetFiles(folderPath, "grid*.txt");
            // Search though each file, to find the correct
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string[] parts = fileName.Split('_');

                if (parts.Length == 3 && parts[2] == description)
                {
                    fullPathName = file;
                    gridName = parts[2];
                    break;
                }
            }
            if (gridName == null)
            {
                throw new Exception($"Haven't found '{description}' in appdata.");
            }

            string path = Path.Combine(folderPath, fullPathName); //Know the path is there
            FileStream stream = File.OpenRead(path);

            Grid grid;

            try
            {
                StreamReader reader = new StreamReader(stream);

                string[] startPosParts = reader.ReadLine().Split(':')[1].Split(',');
                Vector2 startPos = new Vector2(float.Parse(startPosParts[0]), float.Parse(startPosParts[1]));

                string[] gridSizeParts = reader.ReadLine().Split(':')[1].Split(',');
                int width = int.Parse(gridSizeParts[0]);
                int height = int.Parse(gridSizeParts[1]);

                grid = gridGo.AddComponent<Grid>(gridName, startPos, width, height);

                reader.ReadLine(); //There is the line "Grid Cells (Position.X, Postion.Y, Cell Type):" here, so we skip it

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(",");

                    Point gridPos = new Point(int.Parse(parts[0]), int.Parse(parts[1]));

                    CellWalkableType type = (CellWalkableType)Enum.Parse(typeof(CellWalkableType), parts[2]);

                    GameObject cellGo = CellFactory.Create(grid, gridPos, type);
                    grid.Cells.Add(gridPos, cellGo);
                    GameWorld.Instance.Instantiate(cellGo);
                }
            }
            finally
            {
                stream.Close();
            }

            return gridGo;
        }

        private void DeleteExistingGrids()
        {
            // Get all files that start with "grid"
            string[] files = Directory.GetFiles(folderPath, "grid*.txt");

            // Delete each file
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public void DeleteGrid(string description)
        {
            string file = FindGridFile(description);
            if (file != null)
            {
                File.Delete(file);
            }
        }

        public bool DoesGridExist(string description)
        {
            return FindGridFile(description) != null;
        }

        private string FindGridFile(string description)
        {
            string[] files = Directory.GetFiles(folderPath, "grid*.txt");
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string[] parts = fileName.Split('_');

                if (parts.Length == 3 && parts[2] == description)
                {
                    return file;
                }
            }
            return null;
        }
    }
}