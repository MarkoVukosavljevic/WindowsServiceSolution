using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsService;

namespace WindowsServiceTest
{
    [TestFixture]
    internal class HardwareMonitorTests
    {
        [Test]
        public void HardwareMonitor_InitializesHardwareInfoList()
        {
            // Arrange
            var mockConnection = new Mock<SQLiteConnection>();
            mockConnection.Setup(c => c.Open());

            var mockCommand = new Mock<SQLiteCommand>();
            mockCommand.Setup(c => c.ExecuteReader()).Returns(MockDataReader());

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var hardwareMonitor = new HardwareMonitor(mockConnection.Object.ToString());


           int countList = hardwareMonitor.HardwareInfoList.Count;
            // Assert
            Assert.AreEqual(3, countList); //i will assume there are 3 records in MockDataReader
        }

        private SQLiteDataReader MockDataReader()
        {
            var mockDataReader = new Mock<SQLiteDataReader>();
            mockDataReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(false); // End of data

            mockDataReader.Setup(r => r.GetInt32(0)).Returns(1); // Mock Id values
            mockDataReader.Setup(r => r.GetString(1)).Returns("Test Model");
            mockDataReader.Setup(r => r.GetString(2)).Returns("Test Info");

            return mockDataReader.Object;
        }

    }
}
