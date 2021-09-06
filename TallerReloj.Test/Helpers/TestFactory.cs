using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TallerReloj.Common.Models;
using TallerReloj.Function.Entities;

namespace TallerReloj.Test.Helpers
{
    public class TestFactory
    {
        public static RelojEntity GetRelojEntity()
        {
            return new RelojEntity
            {
                ETag = "*",
                PartitionKey = "RELOJ",
                RowKey = Guid.NewGuid().ToString(),
                IdEmpleado = 999,
                Fecha = DateTime.UtcNow,
                Tipo = 0,
                Consolidado = false

            };
        }

        public static TimeEntity GetTimeEntity()
        {
            return new TimeEntity
            {
                ETag = "*",
                PartitionKey = "RELOJ",
                RowKey = Guid.NewGuid().ToString(),
                IdEmpleado = 999,
                Fecha = DateTime.UtcNow,
                Minute = 0

            };
        }

        public static Reloj GetReloj()
        {
            return new Reloj
            {
                
                IdEmpleado = 999,
                Fecha = DateTime.UtcNow,
                Tipo = 0,
                Consolidado = false

            };
        }

        public static Function.Entities.RelojEntity GetRelojEntitys()
        {
            return new Function.Entities.RelojEntity
            {
                ETag = "*",
                PartitionKey = "RELOJ",
                RowKey = Guid.NewGuid().ToString(),
                IdEmpleado = 999,
                Fecha = DateTime.UtcNow,
                Tipo = 0,
                Consolidado = false

            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid RelojId, RelojEntity relojRequest)
        {
            string request = JsonConvert.SerializeObject(relojRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request),
                Path = $"/{RelojId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(DateTime RelojDate, TimeEntity relojRequest)
        {
            string request = JsonConvert.SerializeObject(relojRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request),
                Path = $"/{RelojDate}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid RelojId)
        {
            
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                
                Path = $"/{RelojId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest( RelojEntity relojRequest)
        {
            string request = JsonConvert.SerializeObject(relojRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request)
                
            };
        }

        public static DefaultHttpRequest CreateHttpRequests(Reloj relojRequest)
        {
            string request = JsonConvert.SerializeObject(relojRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenereteStreamFromString(request)

            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {

            return new DefaultHttpRequest(new DefaultHttpContext());
            
        }

        public static RelojEntity GetTodoRequest()
        {
            return new RelojEntity
            {
               IdEmpleado=99,
               Fecha=DateTime.UtcNow,
               Tipo=0,
               Consolidado=false
            };
        }

        public static Stream GenereteStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
