using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using Npgsql;
using System.Collections.Generic;
using System;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class PostgreRepository : IRepository
    {

        private static PostgreRepository instance;

        public static PostgreRepository Instance
        { get { return instance ??= instance = new PostgreRepository(); } }

        private readonly string connectionString = "Host=localhost;Username=postgres;Password=1234;DataBase=DoctorsDungeon";
        public NpgsqlDataSource DateSource { get; private set; }


        public void Initialize()
        {
            DateSource = NpgsqlDataSource.Create(connectionString);
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

        public T SelectFromList<T>(string writeBeforeCheck, List<T> list)
        {
            Console.WriteLine($"\n{writeBeforeCheck}");
            string input = Console.ReadLine();

            // Check if the input is a number
            if (int.TryParse(input, out int index) && index > 0 && index <= list.Count)
            {
                return list[index - 1];
            }
            else
            {
                Console.WriteLine("Invalid input. Prøv igen.");
                return SelectFromList(writeBeforeCheck, list); // Return the result of the recursive call
            }
        }

        /// <summary>
        /// Updates a single value in a table
        /// </summary>
        /// <typeparam name="T1">The new value typ</typeparam>
        /// <typeparam name="T2">The search value type</typeparam>
        /// <param name="table"></param>
        /// <param name="attribute">The attribute the new value should change</param>
        /// <param name="newValue">The new changed value</param>
        /// <param name="whereAttribute">The search condition</param>
        /// <param name="searchValue">The search condition value</param>
        public void UpdateValue<T1, T2>(string table, string attribute, T1 newValue, string whereAttribute, T2 searchValue)
        {
            NpgsqlCommand cmd = DateSource.CreateCommand(
                $"UPDATE {table} SET {attribute} = $1 WHERE {whereAttribute} = $2"
            );
            cmd.Parameters.AddWithValue(newValue);
            cmd.Parameters.AddWithValue(searchValue);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a value from a vable
        /// </summary>
        /// <typeparam name="T">The return value</typeparam>
        /// <typeparam name="T1">The where value</typeparam>
        /// <param name="table">The table to search</param>
        /// <param name="selectAttribute">The value to search for</param>
        /// <param name="whereAttribute">The where attribute, unique like a ID</param>
        /// <param name="whereValue">The value of the where attribute, that it needs to find</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetValue<T, T1>(string table, string selectAttribute, string whereAttribute, T1 whereValue)
        {
            NpgsqlCommand cmd = DateSource.CreateCommand(
                $"SELECT ({selectAttribute}) FROM {table} WHERE {whereAttribute} = $1;"
            );

            cmd.Parameters.AddWithValue(whereValue);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    object value = reader[0];
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else
                {
                    throw new InvalidOperationException("No rows returned.");
                }
            }
        }


        public void DeleteDatabase()
        {

        }
    }
}