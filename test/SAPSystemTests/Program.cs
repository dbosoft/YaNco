using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dbosoft.YaNco;
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

            configurationBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("tests:repeats", "10"),
                new KeyValuePair<string, string>("tests:rows", "10")
            });
            configurationBuilder.AddEnvironmentVariables("saprfc");
            configurationBuilder.AddCommandLine(args);
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

            var rows = Convert.ToInt32(config["tests:rows"]);
            var repeats = Convert.ToInt32(config["tests:repeats"]);

            Console.WriteLine($"Test rows: {rows}");
            Console.WriteLine($"Test repeats: {repeats}");


            StartProgramDelegate callback = command =>
            {
                var programParts = command.Split(' ');
                var arguments = command.Replace(programParts[0], "");
                var p = Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\" + programParts[0] + ".exe",
                    arguments.TrimStart());

                return RfcErrorInfo.Ok();
            };

            var runtime = new RfcRuntime(new SimpleConsoleLogger());


            Task<Either<RfcErrorInfo, IConnection>> ConnFunc() => Connection.Create(settings, runtime);

            using (var context = new RfcContext(ConnFunc))
            {
                await context.Ping();
                
                long totalTest1 = 0;
                long totalTest2 = 0;

                for (var run = 0; run < repeats; run++)
                {
                    Console.WriteLine($"starting Test Run {run+1} of {repeats}\tTest 01");
                    totalTest1 += await RunPerformanceTest01(context, rows);
                    Console.WriteLine($"starting Test Run {run+1} of {repeats}\tTest 02");
                    totalTest2 += await RunPerformanceTest02(context, rows);

                    GC.Collect();

                }

                Console.WriteLine("Total runtime Test 01: " + totalTest1);
                Console.WriteLine("Total runtime Test 02: " + totalTest2);
                Console.WriteLine("Average runtime Test 01: " + totalTest1 / repeats);
                Console.WriteLine("Average runtime Test 02: " + totalTest2 / repeats);

            }
        }

        private static async Task<long> RunPerformanceTest01(IRfcContext context, int rows=0)
        {
            var watch = Stopwatch.StartNew();

            var result = await context.CallFunction("ZYANCO_PT_READ_1",
                func => SetRows(func, rows),
                func => func.BindAsync(f =>
                    from resultTable in f.GetTable("ET_DATA")
                    from row in resultTable.Rows.Map(s =>
                        from char40 in s.GetField<string>("FIELD_CHAR40")
                        from char01 in s.GetField<string>("FIELD_CHAR01")
                        from int04 in s.GetField<int>("FIELD_INT4")
                        //from s in s.GetField<string>("FIELD_STRING")

                        select new TestData
                        {
                            Char01 = char01,
                            Char40 = char40,
                            Int04 = int04
                        }
                    ).Traverse(l => l)

                    select row)).MapAsync(x => x.ToArray());

            watch.Stop();
            return watch.ElapsedMilliseconds;

        }

        private static async Task<long> RunPerformanceTest02(IRfcContext context, int rows = 0)
        {
            var watch = Stopwatch.StartNew();

            var result = await context.CallFunction("ZYANCO_PT_READ_2",
                func => SetRows(func, rows),
                func => func.BindAsync(f =>
                    from resultTable in f.GetTable("ET_DATA")
                    from row in resultTable.Rows.Map(s =>
                        from char40 in s.GetField<string>("FIELD_CHAR40")
                        from char01 in s.GetField<string>("FIELD_CHAR01")
                        from int04 in s.GetField<int>("FIELD_INT4")
                        from str in s.GetField<string>("FIELD_STRING")

                        select new TestData
                        {
                            Char01 = char01,
                            Char40 = char40,
                            Int04 = int04,
                            String = str
                        }
                    ).Traverse(l => l)
                    select row)).MapAsync(x => x.ToArray());

            watch.Stop();
            return watch.ElapsedMilliseconds;

        }

        private static Task<Either<RfcErrorInfo, IFunction>> SetRows(Task<Either<RfcErrorInfo, IFunction>> func, in int rows)
        {
            return rows == 0 ? func : func.SetField("IV_UP_TO", rows);
        }
    }

    public class TestData
    {
        public string Char40 { get; set; }
        public string Char01 { get; set; }
        public int Int04 { get; set; }

        public string String { get; set; }
        
    }
}