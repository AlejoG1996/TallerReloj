using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TallerReloj.Common.Models;
using TallerReloj.Common.Responses;
using TallerReloj.Function.Entities;

namespace TallerReloj.Function.Functions
{
    public static class RelojApi
    {
        [FunctionName(nameof(CreateEntry))]
        public static async Task<IActionResult> CreateEntry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Reloj")] HttpRequest req,
            [Table("Reloj", Connection = "AzureWebJobsStorage")] CloudTable RelojTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new entry.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Reloj relojito = JsonConvert.DeserializeObject<Reloj>(requestBody);

            if (relojito.IdEmpleado <= 0 || relojito.Tipo < 0 || relojito.Tipo > 1)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "the input needs employee id and type"
                });
            }


            RelojEntity relojentity = new RelojEntity
            {

                Fecha = DateTime.UtcNow,
                ETag = "*",
                PartitionKey = "RELOJ",
                RowKey = Guid.NewGuid().ToString(),
                IdEmpleado = relojito.IdEmpleado,
                Tipo = relojito.Tipo,
                Consolidado = false



            };

            TableOperation addOperation = TableOperation.Insert(relojentity);
            await RelojTable.ExecuteAsync(addOperation);

            string message = "New entry stored in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = relojentity
            });
        }

        [FunctionName(nameof(UpdateEntry))]
        public static async Task<IActionResult> UpdateEntry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Reloj/{id}")] HttpRequest req,
            [Table("Reloj", Connection = "AzureWebJobsStorage")] CloudTable RelojTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for entry: {id}, received");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Reloj relojito = JsonConvert.DeserializeObject<Reloj>(requestBody);

            //validate todo id
            TableOperation findOperation = TableOperation.Retrieve<RelojEntity>("RELOJ", id);
            TableResult findResult = await RelojTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "the  employee not found"
                });
            }

            //update entry
            RelojEntity relojEntity = (RelojEntity)findResult.Result;
            relojEntity.Tipo = relojito.Tipo;
            if (relojito.Tipo > 0 || relojito.Tipo < 1)
            {
                relojEntity.Tipo = relojEntity.Tipo;
            }






            TableOperation addOperation = TableOperation.Replace(relojEntity);
            await RelojTable.ExecuteAsync(addOperation);

            string message = $"entry: {id} update in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = relojEntity
            });
        }



        [FunctionName(nameof(GetAllEntry))]
        public static async Task<IActionResult> GetAllEntry(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Reloj")] HttpRequest req,
           [Table("Reloj", Connection = "AzureWebJobsStorage")] CloudTable RelojTable,
           ILogger log)
        {
            log.LogInformation("Get all entry received.");





            TableQuery<RelojEntity> query = new TableQuery<RelojEntity>();
            TableQuerySegment<RelojEntity> relojs = await RelojTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "retrieve all entry";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = relojs
            });
        }


        [FunctionName(nameof(GetEntryById))]
        public static IActionResult GetEntryById(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Reloj/{id}")] HttpRequest req,
          [Table("Reloj", "RELOJ", "{id}", Connection = "AzureWebJobsStorage")] RelojEntity relojentity,
          string id,
          ILogger log)
        {
            log.LogInformation($"Get to entry by id {id} received.");


            if (relojentity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Entry not found"
                });

            }



            string message = $" entry:  {relojentity.RowKey}, retrieve";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = relojentity
            });
        }

        [FunctionName(nameof(DeleteEntry))]
        public static async Task<IActionResult> DeleteEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Reloj/{id}")] HttpRequest req,
        [Table("Reloj", "RELOJ", "{id}", Connection = "AzureWebJobsStorage")] RelojEntity relojentity,
        [Table("Reloj", Connection = "AzureWebJobsStorage")] CloudTable RelojTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"delete entry  {id} received.");


            if (relojentity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Entry not found"
                });

            }


            await RelojTable.ExecuteAsync(TableOperation.Delete(relojentity));

            string message = $" entry:  {relojentity.RowKey}, deleted";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = relojentity
            });
        }

        [FunctionName(nameof(GetConsolidateByDate))]
        public static async Task<IActionResult> GetConsolidateByDate(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Consolidate/{Fecha}")] HttpRequest req,
          [Table("Consolidate", Connection = "AzureWebJobsStorage")] CloudTable Consolidate,
          string  Fecha,
          ILogger log)
        {
            log.LogInformation($"Get to entry by id {Fecha} received.");

            string filter = TableQuery.GenerateFilterConditionForDate("Fecha", QueryComparisons.Equal, Convert.ToDateTime(Fecha));
            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> filtrocompletado = await Consolidate.ExecuteQuerySegmentedAsync(query, null);
            //creat list
            List<TimeEntity> Lista = filtrocompletado.OrderBy(x => x.Fecha).ToList();



            string message = $" entry:  {Fecha}, retrieve";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = Lista
            });
        }

    }
}
