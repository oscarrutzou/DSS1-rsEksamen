using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement.Scenes;
using LiteDB;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DoctorsDungeon.LiteDB
{
    // Oscar
    public class DB
    {
        #region Start
        private static DB instance;

        public static DB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DB();
                    DataBasePath = GetStartPath();
                }
                return instance;
            }
        }

        public static string DataBasePath;

        private static string GetStartPath()
        {
            string baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectoryPath, $"data");
            Directory.CreateDirectory(path);

            return Path.Combine(path, "LiteDB.db");
        }
        #endregion

        #region Grid
        public void SaveGrid(Grid grid)
        {
            if (grid == null) return;

            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<GridData> gridCollection = db.GetCollection<GridData>();

            GridData savedData = gridCollection.FindById(grid.Name);

            if (savedData == null)
            {
                SaveNewGrid(gridCollection, grid);
            }
            else
            {
                if (grid.Width != savedData.GridSize[0] || grid.Height != savedData.GridSize[1])
                {
                    gridCollection.Delete(grid.Name);
                    SaveNewGrid(gridCollection, grid);
                    return;
                }

                UpdateGrid(gridCollection, grid, savedData);
            }
        }

        /// <summary>
        /// To update the grids cells
        /// </summary>
        /// <param name="grid"></param>
        private void UpdateGrid(ILiteCollection<GridData> gridCollection, Grid grid, GridData oldSavedData)
        {
            List<CellData> newCells = new List<CellData>();

            foreach (CellData oldCellData in oldSavedData.Cells)
            {
                Point point = new(oldCellData.PointPosition[0], oldCellData.PointPosition[1]);

                GameObject cellGo = grid.Cells[point];

                Cell cell = cellGo.GetComponent<Cell>();

                CellData newCellData = new()
                {
                    Cell_ID = oldCellData.Cell_ID,
                    PointPosition = oldCellData.PointPosition,
                    Room_Nr = cell.RoomNr,
                    Cell_Type = cell.CellWalkableType,
                };

                newCells.Add(newCellData);
            }

            oldSavedData.Cells = newCells;

            gridCollection.Update(oldSavedData);
        }

        private GridData SaveNewGrid(ILiteCollection<GridData> gridCollection, Grid grid)
        {
            GridData newData = new()
            {
                Grid_Name = grid.Name,
                Position = new float[] { grid.StartPostion.X, grid.StartPostion.Y },
                GridSize = new int[] { grid.Width, grid.Height },
                Cells = GetNewCells(grid),
            };

            gridCollection.Insert(newData);

            return newData;
        }

        private List<CellData> GetNewCells(Grid grid)
        {
            List<CellData> cells = new List<CellData>();
            foreach (GameObject go in grid.Cells.Values)
            {
                Point gridpos = go.Transform.GridPosition;
                Cell cell = go.GetComponent<Cell>();

                // The cell data we want to save
                CellData cellData = new()
                {
                    Cell_ID = Guid.NewGuid(), // A unique key
                    PointPosition = new int[] { gridpos.X, gridpos.Y },
                    Room_Nr = cell.RoomNr,
                    Cell_Type = cell.CellWalkableType,
                };

                cells.Add(cellData);
            }
            return cells;
        }

        public GameObject LoadGrid(string gridname)
        {
            using var db = new LiteDatabase(DataBasePath);
            ILiteCollection<GridData> gridCollection = db.GetCollection<GridData>();

            GameObject gridGo = new();

            GridData savedGrid = gridCollection.FindById(gridname);

            if (savedGrid == null) return null;

            Grid grid = gridGo.AddComponent<Grid>(savedGrid.Grid_Name,
                                                  new Vector2(savedGrid.Position[0], savedGrid.Position[1]),
                                                  savedGrid.GridSize[0],
                                                  savedGrid.GridSize[1]);

            foreach (CellData cellData in savedGrid.Cells)
            {
                Point gridPos = new(cellData.PointPosition[0], cellData.PointPosition[1]);
                GameObject cellGo = CellFactory.Create(grid, gridPos, cellData.Cell_Type, cellData.Room_Nr); // Create the cell object
                grid.Cells.Add(gridPos, cellGo); // Adds to Cell Dict so we can use it later.
                cellGo.IsEnabled = InputHandler.Instance.DebugMode;
                GameWorld.Instance.Instantiate(cellGo); //Spawn it into the world.
            }

            return gridGo;
        }
        #endregion

        public SaveFileTestData LoadGame()
        {
            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<SaveFileTestData> saveCollection = db.GetCollection<SaveFileTestData>();

            SaveFileTestData savedData = saveCollection.FindById(SaveData.CurrentSaveID);

            if (savedData == null) return null;

            SaveData.Currency = savedData.Currency;
            SaveData.UnlockedWeapons = savedData.Unlocked_Weapons;
            SaveData.UnlockedClasses = savedData.Unlocked_Classes;

            return savedData;
        }

        public SaveFileTestData SaveGame(int currentSaveID)
        {
            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<SaveFileTestData> saveCollection = db.GetCollection<SaveFileTestData>();

            SaveFileTestData savedData = saveCollection.FindById(currentSaveID);

            if (savedData == null) // No save file
            {
                //Empty savedata, no run
                savedData = GetNewSaveData(currentSaveID);

                saveCollection.Insert(savedData);
            }
            else
            {
                // What should updates, make sure everything from the run has been loaded
                savedData.Currency = SaveData.Currency;
                savedData.Unlocked_Classes = SaveData.UnlockedClasses;
                savedData.Unlocked_Weapons = SaveData.UnlockedWeapons;
                savedData.Last_Login = DateTime.Now;

                if (savedData.RunData != null && SaveData.Player != null) // Make sure the scene has been initalized
                {
                    // Update rundata
                    savedData = UpdateRun(savedData);
                }

                saveCollection.Update(savedData);
            }

            return savedData;
        }

        private SaveFileTestData UpdateRun(SaveFileTestData savedData)
        {
            savedData.RunData.Room_Reached = SaveData.Level_Reached;
            savedData.RunData.Time_Left = SaveData.Time_Left;

            // Update player
            string potionName = SaveData.Player.ItemInInventory == null ? string.Empty : SaveData.Player.ItemInInventory.Name;
            savedData.RunData.PlayerData.Health = SaveData.Player.CurrentHealth;
            savedData.RunData.PlayerData.Potion_Name = potionName;
            return savedData;
        }

        private SaveFileTestData GetNewSaveData(int currentSaveID)
        {
            return new()
            {
                Save_ID = currentSaveID,
                Currency = SaveData.Currency,
                Unlocked_Classes = SaveData.UnlockedClasses,
                Unlocked_Weapons = SaveData.UnlockedWeapons,
            };
        }

        public void UpdateLoadRun(int currentSaveID)
        {
            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<SaveFileTestData> saveCollection = db.GetCollection<SaveFileTestData>();

            SaveFileTestData savedData = saveCollection.FindById(currentSaveID);

            GameObject playerGo = new();

            if (savedData.RunData == null)
            {
                playerGo = PlayerFactory.Create(SaveData.SelectedClass, SaveData.SelectedWeapon);
                Player player = playerGo.GetComponent<Player>();
                SaveData.Player = player;


                PlayerTestData playerData = new()
                {
                    Health = player.CurrentHealth,
                    Class_Type = SaveData.SelectedClass,
                    Weapon_Type = SaveData.SelectedWeapon,
                };

                RunTestData runData = new()
                {
                    Room_Reached = SaveData.Level_Reached,
                    Time_Left = SaveData.Time_Left,
                    PlayerData = playerData,
                };

                savedData.RunData = runData;

                saveCollection.Update(savedData);
            }
            else
            {
                #region Load Player
                // First load player
                PlayerTestData loadPlayerData = savedData.RunData.PlayerData;
                playerGo = PlayerFactory.Create(loadPlayerData.Class_Type, loadPlayerData.Weapon_Type);
                Player player = playerGo.GetComponent<Player>();
                player.CurrentHealth = loadPlayerData.Health;

                if (loadPlayerData.Potion_Name != null)
                {
                    GameObject potionGo = ItemFactory.Create(playerGo);
                    potionGo.IsEnabled = false;
                    GameWorld.Instance.Instantiate(potionGo); // So the potion gets loaded with its awake and instantiate

                    player.ItemInInventory = potionGo.GetComponent<Potion>();
                    player.ItemInInventory.Name = loadPlayerData.Potion_Name;
                }

                SaveData.Player = player;
                #endregion

                savedData = UpdateRun(savedData);

                saveCollection.Update(savedData);
            }

            GameWorld.Instance.Instantiate(playerGo);
        }

        public List<SaveFileTestData> LoadAllSaveFiles()
        {
            List<SaveFileTestData> saveFiles = new();

            using var db = new LiteDatabase(DataBasePath);
            ILiteCollection<SaveFileTestData> saveCollection = db.GetCollection<SaveFileTestData>();

            for (int i = 1; i <= SaveData.MaxSaveID; i++)
            {
                SaveFileTestData data = saveCollection.FindById(i);

                if (data == null) continue;

                saveFiles.Add(data);
            }

            return saveFiles;
        }

        public void DeleteRun(int currentSaveID)
        {
            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<SaveFileTestData> saveCollection = db.GetCollection<SaveFileTestData>();

            SaveFileTestData savedData = saveCollection.FindById(currentSaveID);

            if (savedData == null) return;

            savedData.RunData = null;
            saveCollection.Update(savedData);
        }

        public void DeleteSave(int currentSaveID)
        {
            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<SaveFileTestData> saveCollection = db.GetCollection<SaveFileTestData>();

            SaveFileTestData savedData = saveCollection.FindById(currentSaveID);

            if (savedData == null) return;

            saveCollection.Delete(savedData.Save_ID);

            SaveData.SetBaseValues();
        }


        #region Extra Methods
        public void UnlockClass(ClassTypes classType)
        {
            if (SaveData.UnlockedClasses.Contains(classType)) return;

            SaveData.UnlockedClasses.Add(classType);

            SaveGame(SaveData.CurrentSaveID);
        }

        public void UnlockWeapon(WeaponTypes unlockedWeapon)
        {
            // Only add the weapon if it's not already in the list
            if (SaveData.UnlockedWeapons.Contains(unlockedWeapon)) return;

            SaveData.UnlockedWeapons.Add(unlockedWeapon);

            SaveGame(SaveData.CurrentSaveID);
        }

        public void AddCurrency(int amount)
        {
            SaveData.Currency += amount;

            SaveGame(SaveData.CurrentSaveID);
        }

        /// <summary>
        /// Overrides the data if the player has enough currency
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>To check if you can remove that much</returns>
        public bool RemoveCurrency(int amount)
        {
            if (SaveData.Currency - amount < 0)
            {
                return false;
            }
            SaveData.Currency -= amount;

            SaveGame(SaveData.CurrentSaveID);

            return true;
        }

        public async void CheckChangeDungeonScene()
        {
            // After it has saved the palyer it will change scene
            await Task.Run(() => { SaveGame(SaveData.CurrentSaveID); });

            int newRoomNr = SaveData.Level_Reached + 1;

            if (newRoomNr <= SaveData.MaxRooms)
            {
                // Change to next dungeon scene
                GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, newRoomNr);
            }
            else // Player won
            {
                AddCurrency(100); // 100 for winning
                SaveData.HasWon = true;
                SaveData.LostByTime = false;
                OnChangeSceneEnd();
            }
        }

        public void OnChangeSceneEnd()
        {
            DeleteRun(SaveData.CurrentSaveID);

            GameWorld.Instance.ChangeScene(SceneNames.EndMenu);
        }

        #endregion

    }

}