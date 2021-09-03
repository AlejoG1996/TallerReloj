using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
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

            if (relojito.IdEmpleado <= 0  || relojito.Tipo < 0 || relojito.Tipo > 1)
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
            if( relojito.Tipo > 0 || relojito.Tipo < 1)
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



    }
}
