using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsService;

namespace WindowsServiceTest
{
    [TestFixture]
    internal class HardwareServiceTests
    {


        [Test]
        public void TimerElapsed_InsertsDataIntoRecordsTable()
        {
            // Arrange
            var mockDatabaseManager = new Mock<DatabaseManager>();
            mockDatabaseManager.Setup(dm => dm.Connection).Returns(new SQLiteConnection("Data Source=:memory:;Version=3;"));

            var mockHardwareMonitor = new Mock<HardwareMonitor>();
            var hardwareInfoList = new List<HardwareInfo>
        {
            new HardwareInfo { Id = 1, Model = "Test Model 1", AdditionalInfo = "Test Info 1" },
            new HardwareInfo { Id = 2, Model = "Test Model 2", AdditionalInfo = "Test Info 2" }
        };
            //i will make some fake hardwareinfos
            mockHardwareMonitor.Setup(hm => hm.HardwareInfoList).Returns(hardwareInfoList);

            var mockUtilization = new Mock<Utilization>();
            mockUtilization.Setup(u => u.GetCPUUtilization()).Returns(50);
            mockUtilization.Setup(u => u.GetDiskUtilization("C:")).Returns(20);
            mockUtilization.Setup(u => u.GetMemoryUtilization()).Returns(70);

            var hardwareService = new HardwareService
            {
                DatabaseManager = mockDatabaseManager.Object,
                HardwareMonitor = mockHardwareMonitor.Object,
            };

            // Act
            hardwareService.TimerElapsed(null, null);

            // Assert
            mockDatabaseManager.Verify(dm => dm.Connection, Times.Once);
            mockUtilization.Verify(u => u.GetCPUUtilization(), Times.Once);
            mockUtilization.Verify(u => u.GetDiskUtilization("C:"), Times.Once);
            mockUtilization.Verify(u => u.GetMemoryUtilization(), Times.Once);

           
        }

        [Test]
        public void GenerateReport_CreatesReportFile()
        {
            // Arrange
            var mockDatabaseManager = new Mock<DatabaseManager>();
            mockDatabaseManager.Setup(dm => dm.Connection).Returns(new SQLiteConnection("Data Source=:memory:;Version=3;"));

            var hardwareService = new HardwareService
            {
                DatabaseManager = mockDatabaseManager.Object
            };

            // Act
            hardwareService.GenerateReport();

            // Assert
            Assert.IsTrue(File.Exists("report.csv")); // we want to check if the report is created
        }
    }
}
