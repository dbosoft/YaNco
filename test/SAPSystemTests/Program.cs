using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contiva.SAP.NWRfc;
using LanguageExt;
using Microsoft.Extensions.Configuration;

namespace SAPSystemTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationBuilder =
                new ConfigurationBuilder();

            configurationBuilder.AddUserSecrets<Program>();
            var config = configurationBuilder.Build();

            var settings = new Dictionary<string, string>
            {
                {"ashost", config["saprfc:ashost"]},
                {"sysnr", config["saprfc:sysnr"]},
                {"client", config["saprfc:client"]},
                {"user", config["saprfc:username"]},
                {"passwd", config["saprfc:password"]},
                {"lang", "EN"}

            };

            var runtime = new RfcRuntime(new SimpleConsoleLogger());

            Task<Either<RfcErrorInfo, IConnection>> ConnFunc() => Connection.Create(settings, runtime);

            using (var context = new RfcContext(ConnFunc))
            {
                await context.Ping();
                using (var memBuffer = new MemoryStream())
                {
                    var intBytes = BitConverter.GetBytes(1);
                    memBuffer.Write(intBytes, 0, intBytes.Length);

                    var setterCall = await context.CallFunctionAsUnit("Z_SET_COUNTER", Input:
                        func => func.BindAsync(f =>
                            f.SetFieldBytes("SET_VALUE", memBuffer.GetBuffer(), memBuffer.Length)));

                    setterCall.Match(r =>
                        {

                        },
                        l =>
                        {

                        });

                    var res = await context.CallFunction("Z_GET_COUNTER",
                        Output: func => func.GetField<int>("GET_VALUE"));
                    res.Match(r =>
                        {

                        },
                        l =>
                        {

                        });
                }

                var resStructure = await context.CallFunction("BAPI_DOCUMENT_GETDETAIL2",
                    Input: func => func
                        .SetField("DOCUMENTTYPE", "AW")
                        .SetField("DOCUMENTNUMBER", "AW001200")
                        .SetField("DOCUMENTVERSION", "01")
                        .SetField("DOCUMENTPART", "000"),
                    Output: func=> func.BindAsync(f => f.GetStructure("DOCUMENTDATA"))
                        .BindAsync(s =>s.GetField<string>("DOCUMENTVERSION")));

                resStructure.Match(r =>
                    {

                    },
                    l =>
                    {

                    });
            }
        }
    }
}
