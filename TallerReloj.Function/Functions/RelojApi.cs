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
            [Table("Reloj", Connection = "UseDevelopmentStorage")] CloudTable RelojTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new entry.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Reloj relojito = JsonConvert.DeserializeObject<Reloj>(requestBody);

            if (string.IsNullOrEmpty(relojito?.IdEmpleado.ToString()) || relojito.IdEmpleado <= 0 ||
                string.IsNullOrEmpty(relojito?.Tipo.ToString()) || relojito.Tipo < 0 || relojito.Tipo > 1)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "the input needs employee id and type"
                });
            }
            DateTime date = DateTime.Now;
            string dat = date.Date.ToString("dd-MM-yyyy");
            RelojEntity relojentity = new RelojEntity
            {

                Fecha = DateTime.Parse(dat),
                Hora = DateTime.UtcNow,
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
    }
}
