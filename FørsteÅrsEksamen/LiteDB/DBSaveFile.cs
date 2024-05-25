using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.Factory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FørsteÅrsEksamen.LiteDB
{
    /// <summary>
    /// Dont use this class other that in the method that saves the game!
    /// </summary>
    public static class DBSaveFile
    {

        public static List<SaveFileData> LoadSaveFiles()
        {
            List<SaveFileData> saveFiles = new();
            using var db = new DataBase(CollectionName.SaveFile);

            for (int i = 1; i <= SaveData.MaxSaveID; i++)
            {
                SaveFileData data = db.FindOne<SaveFileData>(x => x.Save_ID == i);
                if (data == null) continue;
                saveFiles.Add(data);
            }
            return saveFiles;
        }

        public static SaveFileData LoadFileData(int currentSaveID)
        {
            using var db = new DataBase(CollectionName.SaveFile);
            
            SaveFileData data = db.FindOne<SaveFileData>(x => x.Save_ID == currentSaveID);
            
            return data;
        }

        public static SaveFileData LoadSaveFileData(int currentSaveID, bool overrideData = false)
        {
            using var db = new DataBase(CollectionName.SaveFile);

            SaveFileData data = db.FindOne<SaveFileData>(x => x.Save_ID == currentSaveID);

            if (data == null || overrideData) // If dosent exit make a new
            {
                return OverrideSaveFileData(db);
            }

            return data;
        }

        private static SaveFileData OverrideSaveFileData(DataBase saveFileDB)
        {
            SaveFileData fileData = new()
            {
                Save_ID = SaveData.CurrentSaveID,
                Currency = SaveData.Currency,
            };

            saveFileDB.SaveOverrideSingle(fileData, fileData.Save_ID, x => x.Save_ID == fileData.Save_ID);

            return fileData;
        }

        public static SaveFileData OverrideSaveFileData(SaveFileData fileData)
        {
            using var saveFileDB = new DataBase(CollectionName.SaveFile);

            saveFileDB.SaveOverrideSingle(fileData, fileData.Save_ID, x => x.Save_ID == fileData.Save_ID);

            return fileData;
        }

        #region Weapon

        public static List<WeaponTypes> LoadSaveWeaponType(SaveFileData saveFileData, bool overrideSave = false)
        {
            List<WeaponTypes> weaponTypes = new();

            using var weaponLinkDB = new DataBase(CollectionName.SaveFileHasWeapon);
            using var weaponDB = new DataBase(CollectionName.UnlockedWeapon);

            List<SaveFileHasUnlockedWeapon> linkedWeapons = weaponLinkDB.GetCollection<SaveFileHasUnlockedWeapon>()
                .Find(link => link.Save_ID == saveFileData.Save_ID)
                .ToList();

            // First time Load or Override save
            if (linkedWeapons.Count == 0 || overrideSave)
            {
                return OverrideSaveWeaponData(saveFileData, weaponLinkDB, weaponDB);
            }

            // Load
            foreach (SaveFileHasUnlockedWeapon linkedWeapon in linkedWeapons)
            {
                UnlockedWeaponData weaponData = weaponDB.FindOne<UnlockedWeaponData>(x => x.Weapon_ID == linkedWeapon.Weapon_ID);
                if (weaponData != null)
                {
                    weaponTypes.Add(weaponData.Weapon_Type);
                }
            }

            return weaponTypes;
        }

        private static List<WeaponTypes> OverrideSaveWeaponData(SaveFileData saveFileData, DataBase weaponLinkDB, DataBase weaponDB)
        {
            List<WeaponTypes> weaponTypes = new();

            List<SaveFileHasUnlockedWeapon> currentLinkedWeapons = weaponLinkDB.GetCollection<SaveFileHasUnlockedWeapon>()
              .Find(link => link.Save_ID == saveFileData.Save_ID)
              .ToList();

            List<UnlockedWeaponData> weaponDataList = new();

            // Gets the current types
            foreach (SaveFileHasUnlockedWeapon link in currentLinkedWeapons)
            {
                UnlockedWeaponData weaponData = weaponDB.FindOne<UnlockedWeaponData>(x => x.Weapon_ID == link.Weapon_ID);

                // Get the weapon data
                weaponTypes.Add(weaponData.Weapon_Type);
            }

            // Adds if there are any weapon types that are missing
            foreach (WeaponTypes weaponType in SaveData.UnlockedWeapons)
            {
                if (weaponTypes.Contains(weaponType)) continue;

                weaponTypes.Add(weaponType);

                //Add a new weapon type and a link
                var newWeaponData = new UnlockedWeaponData()
                {
                    Weapon_ID = Guid.NewGuid(),
                    Weapon_Type = weaponType,
                };
                weaponDB.SaveSingle(newWeaponData);

                var newLink = new SaveFileHasUnlockedWeapon()
                {
                    Save_ID = saveFileData.Save_ID,
                    Weapon_ID = newWeaponData.Weapon_ID,
                };
                weaponLinkDB.SaveSingle(newLink);
            }

            return weaponTypes;
        }

        public static void DeleteWeapon(SaveFileData saveFileData)
        {
            using var weaponLinkDB = new DataBase(CollectionName.SaveFileHasWeapon);
            using var weaponDB = new DataBase(CollectionName.UnlockedWeapon);

            var linkedWeapons = weaponLinkDB.GetCollection<SaveFileHasUnlockedWeapon>()
                .Find(link => link.Save_ID == saveFileData.Save_ID)
                .ToList();

            foreach (var linkedWeapon in linkedWeapons)
            {
                weaponDB.Delete<UnlockedWeaponData>(linkedWeapon.Weapon_ID);
                weaponLinkDB.Delete<SaveFileHasUnlockedWeapon>(linkedWeapon.Weapon_ID);
            }
        }

        #endregion Weapon

        #region Class

        public static List<ClassTypes> LoadSaveClassType(SaveFileData saveFileData, bool overrideSave = false)
        {
            List<ClassTypes> classTypes = new();

            using var classLinkDB = new DataBase(CollectionName.SaveFileHasClass);
            using var classDB = new DataBase(CollectionName.UnlockedClass);

            List<SaveFileHasUnlockedClass> linkedClasses = classLinkDB.GetCollection<SaveFileHasUnlockedClass>()
                .Find(link => link.Save_ID == saveFileData.Save_ID)
                .ToList();

            // First time Load or Override save
            if (linkedClasses.Count == 0 || overrideSave)
            {
                return OverrideSaveClassData(saveFileData, classLinkDB, classDB);
            }

            // Load
            foreach (SaveFileHasUnlockedClass linkedClass in linkedClasses)
            {
                UnlockedClassData classData = classDB.FindOne<UnlockedClassData>(x => x.Class_ID == linkedClass.Class_ID);
                if (classData != null)
                {
                    classTypes.Add(classData.Class_Type);
                }
            }

            return classTypes;
        }

        private static List<ClassTypes> OverrideSaveClassData(SaveFileData saveFileData, DataBase classLinkDB, DataBase classDB)
        {
            List<ClassTypes> classTypes = new();

            List<SaveFileHasUnlockedClass> currentLinkedClasses = classLinkDB.GetCollection<SaveFileHasUnlockedClass>()
              .Find(link => link.Save_ID == saveFileData.Save_ID)
              .ToList();

            List<UnlockedClassData> classDataList = new();

            // Gets the current types
            foreach (SaveFileHasUnlockedClass link in currentLinkedClasses)
            {
                UnlockedClassData classData = classDB.FindOne<UnlockedClassData>(x => x.Class_ID == link.Class_ID);

                // Get the class data
                classTypes.Add(classData.Class_Type);
            }

            // Adds if there are any class types that are missing
            foreach (ClassTypes classType in SaveData.UnlockedClasses)
            {
                if (classTypes.Contains(classType)) continue;

                classTypes.Add(classType);

                //Add a new class type and a link
                var newClassData = new UnlockedClassData()
                {
                    Class_ID = Guid.NewGuid(),
                    Class_Type = classType,
                };
                classDB.SaveSingle(newClassData);

                var newLink = new SaveFileHasUnlockedClass()
                {
                    Save_ID = saveFileData.Save_ID,
                    Class_ID = newClassData.Class_ID,
                };
                classLinkDB.SaveSingle(newLink);
            }

            return classTypes;
        }

        public static void DeleteClass(SaveFileData saveFileData)
        {
            using var classLinkDB = new DataBase(CollectionName.SaveFileHasClass);
            using var classDB = new DataBase(CollectionName.UnlockedClass);

            var linkedClasses = classLinkDB.GetCollection<SaveFileHasUnlockedClass>()
                              .Find(link => link.Save_ID == saveFileData.Save_ID)
                              .ToList();

            // Delete current linked classes
            foreach (var linkedClass in linkedClasses)
            {
                classDB.Delete<UnlockedClassData>(linkedClass.Class_ID);
                classLinkDB.Delete<SaveFileHasUnlockedClass>(linkedClass.Class_ID);
            }
        }

        #endregion Class
    }
}