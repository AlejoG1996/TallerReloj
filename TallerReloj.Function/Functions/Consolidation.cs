using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerReloj.Common.Responses;
using TallerReloj.Function.Entities;

namespace TallerReloj.Function.Functions
{
    public static class Consolidation
    {
        [FunctionName("Consolidation")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer,
            [Table("Reloj", Connection = "AzureWebJobsStorage")] CloudTable RelojTable,
            [Table("Consolidate", Connection = "AzureWebJobsStorage")] CloudTable Consolidate,
            ILogger log)
        {
            log.LogInformation($"Consolidation  completed function excecute {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("Consolidado", QueryComparisons.Equal, false);

            TableQuery<RelojEntity> query = new TableQuery<RelojEntity>().Where(filter);

            TableQuerySegment<RelojEntity> filtrocompletado = await RelojTable.ExecuteQuerySegmentedAsync(query, null);

            //creat list
            List<RelojEntity> Lista = filtrocompletado.OrderBy(x => x.IdEmpleado).ThenBy(x => x.Fecha).ToList();
            if (Lista.Count < 1)
            {
            }
            else
            {
                for (int i = 0; i < Lista.Count(); i++)
                {


                    try
                    {
                        if (Lista[i].IdEmpleado == Lista[i + 1].IdEmpleado)
                        {
                            //diferencia de tiempo
                            TimeSpan difference = Lista[i + 1].Fecha - Lista[i].Fecha;
                            int minutes = Convert.ToInt32(difference.TotalMinutes);
                            
                           
                            //validar existente
                            string filters = TableQuery.GenerateFilterConditionForDate("Fecha", QueryComparisons.Equal, Convert.ToDateTime(Lista[i].Fecha.ToString("yyyy-MM-dd")));
                            TableQuery<TimeEntity> querys = new TableQuery<TimeEntity>().Where(filters);
                            TableQuerySegment<TimeEntity> filtrocompletados = await Consolidate.ExecuteQuerySegmentedAsync(querys, null);
                            //creat list
                            List<TimeEntity> Listatwo = filtrocompletados.OrderBy(x => x.IdEmpleado).ToList();
                            
                            int cont = 0;
                            for(int j=0; j<Listatwo.Count(); j++)
                            {
                                string id = Convert.ToString(Listatwo[j].RowKey);
                                if (Lista[i].IdEmpleado == Listatwo[j].IdEmpleado )
                                {



                                    cont++;

                                    TableOperation findOperation = TableOperation.Retrieve<TimeEntity>("TIME", id);
                                    TableResult findResulta = await Consolidate.ExecuteAsync(findOperation);
                                    TimeEntity todoEntity = (TimeEntity)findResulta.Result;
                                    todoEntity.Minute = todoEntity.Minute+minutes;
                                    


                                    TableOperation addOperation = TableOperation.Replace(todoEntity);
                                    await Consolidate.ExecuteAsync(addOperation);

                                    //cambiar estado del consolidado
                                    string id1 = Convert.ToString(Lista[i].RowKey);

                                    TableOperation findOperations = TableOperation.Retrieve<RelojEntity>("RELOJ", id1);
                                    TableResult findResultados = await RelojTable.ExecuteAsync(findOperations);
                                    RelojEntity relojentity = (RelojEntity)findResultados.Result;
                                    relojentity.Consolidado = true;


                                    TableOperation addOperations = TableOperation.Replace(relojentity);
                                    await RelojTable.ExecuteAsync(addOperations);


                                    //cambiar estado del consolidado
                                    string id2 = Convert.ToString(Lista[i + 1].RowKey);

                                    TableOperation findOperationss = TableOperation.Retrieve<RelojEntity>("RELOJ", id2);
                                    TableResult findResultadoss = await RelojTable.ExecuteAsync(findOperationss);
                                    RelojEntity relojentitys = (RelojEntity)findResultadoss.Result;
                                    relojentitys.Consolidado = true;


                                    TableOperation addOperationss = TableOperation.Replace(relojentitys);
                                    await RelojTable.ExecuteAsync(addOperationss);



                                }
                            }
                            if (cont == 0)
                            {
                                //ingresar registro
                                TimeEntity timeentity = new TimeEntity
                                {
                                    ETag = "*",
                                    PartitionKey = "TIME",
                                    RowKey = Guid.NewGuid().ToString(),
                                    IdEmpleado = Lista[i].IdEmpleado,
                                    Fecha = Convert.ToDateTime(Lista[i].Fecha.ToString("yyyy-MM-dd")),
                                    Minute = minutes

                                };


                                await Consolidate.ExecuteAsync(TableOperation.Insert(timeentity));

                                //cambiar estado del consolidado
                                string id1 = Convert.ToString(Lista[i].RowKey);

                                TableOperation findOperations = TableOperation.Retrieve<RelojEntity>("RELOJ", id1);
                                TableResult findResultados = await RelojTable.ExecuteAsync(findOperations);
                                RelojEntity relojentity = (RelojEntity)findResultados.Result;
                                relojentity.Consolidado = true;


                                TableOperation addOperations = TableOperation.Replace(relojentity);
                                await RelojTable.ExecuteAsync(addOperations);


                                //cambiar estado del consolidado
                                string id2 = Convert.ToString(Lista[i + 1].RowKey);

                                TableOperation findOperationss = TableOperation.Retrieve<RelojEntity>("RELOJ", id2);
                                TableResult findResultadoss = await RelojTable.ExecuteAsync(findOperationss);
                                RelojEntity relojentitys = (RelojEntity)findResultadoss.Result;
                                relojentitys.Consolidado = true;


                                TableOperation addOperationss = TableOperation.Replace(relojentitys);
                                await RelojTable.ExecuteAsync(addOperationss);

                            }















                        }
                    }
                    catch (Exception ex)
                    {

                        string error = ex.Message;
                    }



                }
            }















        }
    }
}
