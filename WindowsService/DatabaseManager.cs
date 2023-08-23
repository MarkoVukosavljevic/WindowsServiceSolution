using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace WindowsService
{

    public class DatabaseManager
    {
        private SQLiteConnection _connection;
        public SQLiteConnection Connection { get { return _connection; } }
        public DatabaseManager(string connectionString) {

            InitializeDatabase(connectionString);
        }

        public void InitializeDatabase(string connString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connString))
            {
                connection.Open();

                string createHardwareTypesTable = "CREATE TABLE IF NOT EXISTS HardwareTypes (Id INTEGER PRIMARY KEY AUTOINCREMENT, Model VARCHAR(30), AdditionalInfo varchar(30));";
                string createRecordsTable = "CREATE TABLE IF NOT EXISTS Records (Id INTEGER PRIMARY KEY AUTOINCREMENT, HardwareTypeId INTEGER, Value INTEGER, CreateDate DATETIME, CONSTRAINT hrd_fk FOREIGN KEY(HardwareTypeId) REFERENCES HardwareTypes(Id) ON DELETE CASCADE);";
                string insertHardwareTypesTableData = @"INSERT INTO HardwareTypes(Id, Model, AdditionalInfo) VALUES (1, 'Intel Core i5', 'CPU, serial number: 1234');
                                                        INSERT INTO HardwareTypes(Id, Model, AdditionalInfo) VALUES (2, 'Samsung 500gb', 'HDD1, serial number: 2345');
                                                        INSERT INTO HardwareTypes(Id, Model, AdditionalInfo) VALUES (3, 'Toshiba 250gb', 'HDD2, serial number: 3456');";

                using (SQLiteCommand command = new SQLiteCommand(createHardwareTypesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(createRecordsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(insertHardwareTypesTableData, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }


}



