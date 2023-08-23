using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsService
{
    public partial class HardwareService : ServiceBase
    {

        private Timer timer;
        private DatabaseManager databaseManager;
        private HardwareMonitor hardwareMonitor;

        public DatabaseManager DatabaseManager { get; set; }
        public HardwareMonitor HardwareMonitor { get; set; }


        private string connString;
        public HardwareService()
        {
            InitializeComponent();
            connString = "Data Source=hardwaremonitor.db;Version=3;";
            ServiceName = "HardwareService";
            databaseManager = new DatabaseManager(connString);
           
            hardwareMonitor = new HardwareMonitor(connString); //fills the list about hardware info
            
            timer = new Timer();
            timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds; // Set the interval to 5mins
            timer.Elapsed += TimerElapsed;
        }
       

      

        protected override void OnStart(string[] args)
        {
           timer.Start();
        }

        protected override void OnStop()
        {
           timer.Stop();
        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            List<HardwareInfo> hardwareInfoList = hardwareMonitor.HardwareInfoList;

            //I will use the sum of my cpu, disk, memory utilization to store as a value in records
            Utilization utilization = new Utilization();
            int utilizationValue = utilization.GetCPUUtilization()+ Convert.ToInt32(utilization.GetDiskUtilization())+ Convert.ToInt32(utilization.GetMemoryUtilization());
           
            using (SQLiteConnection connection = new SQLiteConnection(connString))
            {
                connection.Open();
                  using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Records (HardwareTypeId, Value, CreateDate) VALUES (@HardwareTypeId, @Value, @CreateDate);", connection))
                  {

                    //do this for every hardware in the list
                    for (int i = 0; i < hardwareInfoList.Count; i++)
                    {
                        command.Parameters.AddWithValue("@HardwareTypeId", hardwareInfoList[i].Id);
                        command.Parameters.AddWithValue("@Value", utilizationValue);
                        command.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                  }
            }

        }
        public void GenerateReport()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connString))
            {
                connection.Open();
                string query = @"SELECT r.Value, r.CreateDate, h.Model, h.AdditionalInfo
                         FROM Records r
                         INNER JOIN HardwareTypes h ON (r.HardwareTypeId = h.Id);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                using (StreamWriter sw = new StreamWriter("report.csv"))
                {
                    sw.WriteLine("Value,CreateDate,Model,AdditionalInfo");
                    while (reader.Read())
                    {
                        sw.WriteLine($"{reader.GetInt32(0)},{reader.GetDateTime(1)},{reader.GetString(2)},{reader.GetString(3)}");
                    }
                }
            }
        }


    }
}
