using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WindowsService
{
    public class HardwareMonitor
    {
         private List<HardwareInfo> hardwareInfoList = new List<HardwareInfo>();

        public List<HardwareInfo> HardwareInfoList { get; }
        public HardwareMonitor(string connString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connString))
            {
                connection.Open();
                string query = @"SELECT Id,Model,AdditionalInfo
                                 FROM HardwareTypes";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HardwareInfo hinfo = new HardwareInfo();
                        hinfo.Id = reader.GetInt32(0);
                        hinfo.Model = reader.GetString(1);
                        hinfo.AdditionalInfo = reader.GetString(2);

                        hardwareInfoList.Add(hinfo);
                    }
                }
            }
        }
    }
}