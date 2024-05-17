

using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.Factory;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    /// <summary>
    /// Dont use this class other that in the method that saves the game!
    /// </summary>
    public static class DBSaveFile
    {
        public static SaveFileData OverrideSaveFileData()
        {
            SaveFileData fileData = new()
            {
                Save_ID = SaveFileManager.CurrentSaveID,
                Currency = SaveFileManager.Currency,
            };

            using var saveFileDB = new DataBase(CollectionName.SaveFile);

            saveFileDB.SaveOverrideSingle(fileData, fileData.Save_ID, x => x.Save_ID == fileData.Save_ID);

            return fileData;
        }

        public static void SaveWeaponData(SaveFileData saveFileData)
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

            var newWeaponDataList = CreateAndSaveWeapons(weaponDB);

            foreach (var newWeaponData in newWeaponDataList)
            {
                var newWeaponLink = new SaveFileHasUnlockedWeapon()
                {
                    Save_ID = saveFileData.Save_ID,
                    Weapon_ID = newWeaponData.Weapon_ID,
                };

                weaponLinkDB.SaveSingle(newWeaponLink);
            }
        }

        private static List<UnlockedWeaponData> CreateAndSaveWeapons(DataBase weaponDB)
        {
            List<UnlockedWeaponData> newWeaponDataList = new();
            foreach (WeaponTypes type in SaveFileManager.UnlockedWeapons)
            {
                newWeaponDataList.Add(new UnlockedWeaponData()
                {
                    Weapon_ID = Guid.NewGuid(),
                    Weapon_Type = type,
                });
            }


            weaponDB.SaveAll(newWeaponDataList);

            return newWeaponDataList;
        }


        public static void SaveClassData(SaveFileData saveFileData)
        {
            using var classLinkDB = new DataBase(CollectionName.SaveFileHasClass);
            using var classDB = new DataBase(CollectionName.UnlockedClass);

            var linkedClasses = classLinkDB.GetCollection<SaveFileHasUnlockedClass>()
                                          .Find(link => link.Save_ID == saveFileData.Save_ID)
                                          .ToList();

            foreach (var linkedClass in linkedClasses)
            {
                classDB.Delete<UnlockedClassData>(linkedClass.Class_ID);
                classLinkDB.Delete<SaveFileHasUnlockedClass>(linkedClass.Class_ID);
            }

            var newClassDataList = CreateAndSaveClasses(classDB);

            foreach (var newClassData in newClassDataList)
            {
                var newClassLink = new SaveFileHasUnlockedClass()
                {
                    Save_ID = saveFileData.Save_ID,
                    Class_ID = newClassData.Class_ID,
                };

                classLinkDB.SaveSingle(newClassLink);
            }
        }

        private static List<UnlockedClassData> CreateAndSaveClasses(DataBase classDB)
        {
            var newClassDataList = new List<UnlockedClassData>();

            foreach (ClassTypes classType in SaveFileManager.UnlockedClasses)
            {
                var newClassData = new UnlockedClassData()
                {
                    Class_ID = Guid.NewGuid(),
                    Class_Type = classType,
                };

                newClassDataList.Add(newClassData);
            }

            classDB.SaveAll(newClassDataList);

            return newClassDataList;
        }
    }
}
