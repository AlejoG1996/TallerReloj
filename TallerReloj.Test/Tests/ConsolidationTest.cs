using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using TallerReloj.Function.Functions;
using TallerReloj.Test.Helpers;
using Xunit;

namespace TallerReloj.Test.Tests
{
    public class ConsolidationTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            //Arrange
            MockCloudTableReloj mockTodos = new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableReloj mockTodostodos = new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            
            //Act
            Consolidation.Run(null, mockTodos, mockTodostodos, logger);
            string message = logger.Logs[0];

            //Asert
            Assert.Contains("Consolidation  completed", message);
        }
    }
}
