using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TallerReloj.Common.Models;
using TallerReloj.Function.Entities;
using TallerReloj.Function.Functions;
using TallerReloj.Test.Helpers;
using Xunit;

namespace TallerReloj.Test.Tests
{
    public class RelojApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateEntry_Should_Return_200()
        {
            //Arrenge
            MockCloudTableReloj mockReloj = new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RelojEntity todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            //Act
            IActionResult response = await RelojApi.CreateEntry(request, mockReloj, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }


        [Fact]
        public async void UpdateReloj_Should_Return_200()
        {
            //Arrenge
            MockCloudTableReloj mockReloj = new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RelojEntity todoRequest = TestFactory.GetTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);

            //Act
            IActionResult response = await RelojApi.UpdateEntry(request, mockReloj, todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }

        [Fact]
        public async void DeleteReloj_Should_Return_200()
        {
            //Arrenge
            MockCloudTableReloj mockreloj= new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Function.Entities.RelojEntity todoRequest = TestFactory.GetRelojEntitys();
            RelojEntity todoRequestS = TestFactory.GetTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequestS);
            DefaultHttpRequest request1 = TestFactory.CreateHttpRequest(todoRequestS);
            //Act
            IActionResult response = await RelojApi.DeleteEntry( request, todoRequest, mockreloj, todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }

        [Fact]
        public async void GetByIdReloj_Should_Return_200()
        {
            //Arrenge
            MockCloudTableReloj mockReloj = new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RelojEntity todoRequest = TestFactory.GetRelojEntity();
            Guid todoId = Guid.NewGuid();
            
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);

            //Act
            IActionResult response =  RelojApi.GetEntryById(request, todoRequest, todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }

        [Fact]
        public async void GetAllReloj_Should_Return_200()
        {  
            //error
            //Arrenge
            MockCloudTableReloj mockReloj = new MockCloudTableReloj(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            RelojEntity todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //Act
            IActionResult response = await RelojApi.GetAllEntry(request, mockReloj, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }

       
        

    }

    
}
