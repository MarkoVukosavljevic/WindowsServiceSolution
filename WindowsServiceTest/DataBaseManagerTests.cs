using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using WindowsService;

namespace WindowsServiceTest
{
    [TestFixture]
    public class DatabaseManagerTests
    {
        private string connectionString = "Data Source=:memory:;Version=3;";

        [Test]
        public void InitializeDatabase_CreatesTablesAndInsertsData()
        {
            // Arrange
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // i will create in-memory database and use it in the test
                var databaseManager = new DatabaseManager(connectionString);
                

                
                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM HardwareTypes;", connection))
                {
                    int hardwareTypesCount = Convert.ToInt32(command.ExecuteScalar());
                    Assert.AreEqual(3, hardwareTypesCount); 
                    //we know our table has 3 rows so we compare it with that number
                }

                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM Records;", connection))
                {
                    int recordsCount = Convert.ToInt32(command.ExecuteScalar());
                    Assert.AreEqual(0, recordsCount);
                    //recordsCount should always be 0 on the start
                }
            }
        }
    }
}
