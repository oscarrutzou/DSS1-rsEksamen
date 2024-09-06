using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.Factory;
using ShamansDungeon.GameManagement.Scenes;
using LiteDB;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ShamansDungeon.LiteDB;

// Oscar
public class DB
{
    #region Start

    private static DB _instance;

    // Used a singleton so we can get the path when we first use our database
    public static DB Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DB();
                DataBasePath = GetStartPath();
            }
            return _instance;
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

    #endregion Start

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
                CollisionNr = cell.CollisionNr,
                RoomNr = cell.RoomNr,
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
                CollisionNr = cell.CollisionNr,
                RoomNr = cell.RoomNr,
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
            GameObject cellGo = CellFactory.Create(grid, gridPos, cellData.Cell_Type, cellData.CollisionNr, cellData.RoomNr); // Create the cell object
            grid.Cells.Add(gridPos, cellGo); // Adds to Cell Dict so we can use it later.
            cellGo.IsEnabled = InputHandler.Instance.DebugMode;
            GameWorld.Instance.Instantiate(cellGo); //Spawn it into the world.

            Cell cell = cellGo.GetComponent<Cell>();

            // Add the cell to the dictionary based on its CollisionNr
            if (!grid.CellsCollisionDict.TryGetValue(cellData.CollisionNr, out var cellList))
            {
                cellList = new List<GameObject>();
                grid.CellsCollisionDict.Add(cellData.CollisionNr, cellList);
            }
            cellList.Add(cellGo);
        }

        return gridGo;
    }

    #endregion Grid

    public SaveFileData LoadGame()
    {
        using var db = new LiteDatabase(DataBasePath);

        ILiteCollection<SaveFileData> saveCollection = db.GetCollection<SaveFileData>();

        SaveFileData savedData = saveCollection.FindById(SaveData.CurrentSaveID);

        if (savedData == null) return null;

        SaveData.Currency = savedData.Currency;
        SaveData.UnlockedWeapons = savedData.Unlocked_Weapons;
        SaveData.UnlockedClasses = savedData.Unlocked_Classes;
        SaveData.HasCompletedFullTutorial = savedData.HasCompletedFullTutorial;

        return savedData;
    }

    public SaveFileData SaveGame(int currentSaveID, bool saveRun = true)
    {
        using var db = new LiteDatabase(DataBasePath);

        ILiteCollection<SaveFileData> saveCollection = db.GetCollection<SaveFileData>();

        SaveFileData savedData = saveCollection.FindById(currentSaveID);

        if (savedData == null) // No save file
        {
            //Empty savedata, no run
            savedData = GetNewSaveData(currentSaveID);

            saveCollection.Insert(savedData);
        }
        else
        {
            // What should updates in the new data
            savedData.Currency = SaveData.Currency;
            savedData.Unlocked_Classes = SaveData.UnlockedClasses;
            savedData.Unlocked_Weapons = SaveData.UnlockedWeapons;
            savedData.HasCompletedFullTutorial = SaveData.HasCompletedFullTutorial;
            savedData.Last_Login = DateTime.Now;

            if (saveRun && savedData.RunData != null && SaveData.Player != null) // Make sure the scene has been initalized
            {
                // Update rundata
                savedData = UpdateRun(savedData);
            }

            saveCollection.Update(savedData);
        }

        return savedData;
    }

    private SaveFileData UpdateRun(SaveFileData savedData)
    {
        savedData.RunData.Room_Reached = SaveData.Level_Reached;
        savedData.RunData.Time_Left = SaveData.Time_Left;

        // Update player
        string potionName = SaveData.Player.ItemInInventory == null ? string.Empty : SaveData.Player.ItemInInventory.Name;

        Health playerHealth = SaveData.Player.GameObject.GetComponent<Health>();
        savedData.RunData.PlayerData.Health = playerHealth.CurrentHealth;
        savedData.RunData.PlayerData.Potion_Name = potionName;
        return savedData;
    }

    private SaveFileData GetNewSaveData(int currentSaveID)
    {
        return new()
        {
            Save_ID = currentSaveID,
            Currency = SaveData.Currency,
            Unlocked_Classes = SaveData.UnlockedClasses,
            Unlocked_Weapons = SaveData.UnlockedWeapons,
            HasCompletedFullTutorial = SaveData.HasCompletedFullTutorial,
        };
    }

    public void UpdateLoadRun(int currentSaveID)
    {
        using var db = new LiteDatabase(DataBasePath);

        ILiteCollection<SaveFileData> saveCollection = db.GetCollection<SaveFileData>();

        SaveFileData savedData = saveCollection.FindById(currentSaveID);

        GameObject playerGo = new();

        if (savedData.RunData == null) // First time loaded in a new run
        {
            // Makes the player, with full health
            playerGo = PlayerFactory.Create(SaveData.SelectedClass, SaveData.SelectedWeapon);
            Player player = playerGo.GetComponent<Player>();
            SaveData.Player = player;

            Health playerHealth = playerGo.GetComponent<Health>();

            // Could ekstrakt here, to make code more readable
            PlayerData playerData = new()
            {
                Health = playerHealth.CurrentHealth,
                Class_Type = SaveData.SelectedClass,
                Weapon_Type = SaveData.SelectedWeapon,
            };

            RunData runData = new()
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
            PlayerData loadPlayerData = savedData.RunData.PlayerData;
            playerGo = PlayerFactory.Create(loadPlayerData.Class_Type, loadPlayerData.Weapon_Type);
            Player player = playerGo.GetComponent<Player>();
            Health playerHealth = playerGo.GetComponent<Health>();

            playerHealth.CurrentHealth = loadPlayerData.Health;

            if (loadPlayerData.Potion_Name != null)
            {
                GameObject potionGo = ItemFactory.Create(playerGo);
                potionGo.IsEnabled = false;
                GameWorld.Instance.Instantiate(potionGo); // So the potion gets loaded with its awake and instantiate

                player.ItemInInventory = potionGo.GetComponent<Potion>();
                player.ItemInInventory.Name = loadPlayerData.Potion_Name;
            }

            SaveData.Player = player;

            #endregion Load Player

            savedData = UpdateRun(savedData);

            saveCollection.Update(savedData);
        }

        GameWorld.Instance.Instantiate(playerGo);
    }

    public List<SaveFileData> LoadAllSaveFiles()
    {
        List<SaveFileData> saveFiles = new();

        using var db = new LiteDatabase(DataBasePath);
        ILiteCollection<SaveFileData> saveCollection = db.GetCollection<SaveFileData>();

        for (int i = 1; i <= SaveData.MaxSaveID; i++)
        {
            SaveFileData data = saveCollection.FindById(i);

            if (data == null) continue;

            saveFiles.Add(data);
        }

        return saveFiles;
    }

    public void DeleteRun(int currentSaveID)
    {
        using var db = new LiteDatabase(DataBasePath);

        ILiteCollection<SaveFileData> saveCollection = db.GetCollection<SaveFileData>();

        SaveFileData savedData = saveCollection.FindById(currentSaveID);

        if (savedData == null) return;

        savedData.RunData = null;
        saveCollection.Update(savedData);
    }

    public void DeleteSave(int currentSaveID)
    {
        using var db = new LiteDatabase(DataBasePath);

        ILiteCollection<SaveFileData> saveCollection = db.GetCollection<SaveFileData>();

        SaveFileData savedData = saveCollection.FindById(currentSaveID);

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

        SaveGame(SaveData.CurrentSaveID, false);
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

    public void CheckChangeDungeonScene()
    {
        // After it has saved the palyer it will change scene
        SaveGame(SaveData.CurrentSaveID);

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
        List<WeaponTypes> data = SaveData.UnlockedWeapons;
        GameWorld.Instance.ChangeScene(SceneNames.EndMenu);
    }

    #endregion Extra Methods
}