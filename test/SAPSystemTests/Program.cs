using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using KellermanSoftware.CompareNetObjects;
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

            
            using (var context = new RfcContext(
                new ConnectionBuilder(settings)
                    .ConfigureRuntime(rc=> rc.WithLogger(new SimpleConsoleLogger()))
                    .Build()))
            {
                await context.PingAsync();

                await RunIntegrationTests(context);

                long totalTest1 = 0;
                long totalTest2 = 0;

                for (var run = 0; run < repeats; run++)
                {
                    Console.WriteLine($"starting Test Run {run + 1} of {repeats}\tTest 01");
                    totalTest1 += await RunPerformanceTest01(context, rows);
                    Console.WriteLine($"starting Test Run {run + 1} of {repeats}\tTest 02");
                    totalTest2 += await RunPerformanceTest02(context, rows);

                    GC.Collect();

                }

                Console.WriteLine("Total runtime Test 01: " + totalTest1);
                Console.WriteLine("Total runtime Test 02: " + totalTest2);
                Console.WriteLine("Average runtime Test 01: " + totalTest1 / repeats);
                Console.WriteLine("Average runtime Test 02: " + totalTest2 / repeats);

            }
        }

        private static async Task RunIntegrationTests(IRfcContext context)
        {
            Console.WriteLine("*** BEGIN OF Integration Tests ***");

            await RunIntegrationTest01(context);

            Console.WriteLine("*** END OF Integration Tests ***");
        }
        private static async Task RunIntegrationTest01(IRfcContext context)
        {
            Console.WriteLine("Integration Tests 01 (I/O field)");

            // ReSharper disable StringLiteralTypo
            var inputData = new TypesTestData
            {
                ACCP = "123456",
                CHAR = "AB",
                CLNT = "ABC",
                CUKY = "ABCDE",
                CURR = 9.12,
                DATS = DateTime.MaxValue.Date,
                DEC = 1.999M,
                FLTP = +1E-6143,
                INT1 = 1,
                INT2 = 32767,
                INT4 = 2147483647,
                LANG = "A",
                LCHR = RandomString(256),
                LRAW = RandomByteArray(256),
                NUMC = "123",
                PREC = 99,
                QUAN = 99.123,
                RAW = RandomByteArray(10),
                SSTRING = RandomString(10),
                TIMS = default(DateTime).Add(new TimeSpan(23, 59,59)),
                STRING = "ABCDE",
                RAWSTRING = RandomByteArray(300),
                UNIT = "ABC"
            };

            var outputData = await context.CallFunction("ZYANCO_IT_1",
                Input: f => f.SetStructure("IS_IN",
                    s => s
                    .SetField("FIELD_ACCP", inputData.ACCP)
                    .SetField("FIELD_CHAR", inputData.CHAR)
                    .SetField("FIELD_CLNT", inputData.CLNT)
                    .SetField("FIELD_CUKY", inputData.CUKY)
                    .SetField("FIELD_CURR", inputData.CURR)
                    .SetField("FIELD_DATS", inputData.DATS)
                    .SetField("FIELD_DEC", inputData.DEC)
                    .SetField("FIELD_FLTP", inputData.FLTP)
                    .SetField("FIELD_INT1", inputData.INT1)
                    .SetField("FIELD_INT2", inputData.INT2)
                    .SetField("FIELD_INT4", inputData.INT4)
                    .SetField("FIELD_LANG", inputData.LANG)
                    .SetField("FIELD_LCHR", inputData.LCHR)
                    .SetField("FIELD_LRAW", inputData.LRAW)
                    .SetField("FIELD_NUMC", inputData.NUMC)
                    .SetField("FIELD_PREC", inputData.PREC)
                    .SetField("FIELD_QUAN", inputData.QUAN)
                    .SetField("FIELD_RAW", inputData.RAW)
                    .SetField("FIELD_SSTRING", inputData.SSTRING)
                    .SetField("FIELD_TIMS", inputData.TIMS)
                    .SetField("FIELD_STRING", inputData.STRING)
                    .SetField("FIELD_RAWSTRING", inputData.RAWSTRING)
                    .SetField("FIELD_UNIT", inputData.UNIT)
                ),
                Output: f => f.MapStructure("ES_ECHO", s =>
                    // ReSharper disable InconsistentNaming
                    // ReSharper disable IdentifierTypo
                    from fieldACCP in s.GetField<string>("FIELD_ACCP")
                    from fieldCHAR in s.GetField<string>("FIELD_CHAR")
                    from fieldCLNT in s.GetField<string>("FIELD_CLNT")
                    from fieldCUKY in s.GetField<string>("FIELD_CUKY")
                    from fieldCURR in s.GetField<double>("FIELD_CURR")
                    from fieldDATS in s.GetField<DateTime>("FIELD_DATS")
                    from fieldDEC in s.GetField<decimal>("FIELD_DEC")
                    from fieldFLTP in s.GetField<double>("FIELD_FLTP")
                    from fieldINT1 in s.GetField<int>("FIELD_INT1")
                    from fieldINT2 in s.GetField<short>("FIELD_INT2")
                    from fieldINT4 in s.GetField<int>("FIELD_INT4")
                    from fieldLANG in s.GetField<string>("FIELD_LANG")
                    from fieldLCHR in s.GetField<string>("FIELD_LCHR")
                    from fieldLRAW in s.GetField<byte[]>("FIELD_LRAW")
                    from fieldNUMC in s.GetField<string>("FIELD_NUMC")
                    from fieldPREC in s.GetField<int>("FIELD_PREC")
                    from fieldQUAN in s.GetField<double>("FIELD_QUAN")
                    from fieldRAW in s.GetField<byte[]>("FIELD_RAW")
                    from fieldSSTRING in s.GetField<string>("FIELD_SSTRING")
                    from fieldTIMS in s.GetField<DateTime>("FIELD_TIMS")
                    from fieldSTRING in s.GetField<string>("FIELD_STRING")
                    from fieldRAWSTRING in s.GetField<byte[]>("FIELD_RAWSTRING")
                    from fieldUNIT in s.GetField<string>("FIELD_UNIT")
                        // ReSharper restore InconsistentNaming
                        // ReSharper restore IdentifierTypo

                        select new TypesTestData
                    {
                        ACCP = fieldACCP,
                        CHAR = fieldCHAR,
                        CLNT = fieldCLNT,
                        CUKY = fieldCUKY,
                        CURR = fieldCURR,
                        DATS = fieldDATS,
                        DEC = fieldDEC,
                        FLTP = fieldFLTP,
                        INT1 = fieldINT1,
                        INT2 = fieldINT2,
                        INT4 = fieldINT4,
                        LANG = fieldLANG,
                        LCHR = fieldLCHR,
                        LRAW = fieldLRAW,
                        NUMC = fieldNUMC,
                        PREC = fieldPREC,
                        QUAN = fieldQUAN,
                        RAW = fieldRAW,
                        SSTRING = fieldSSTRING,
                        TIMS = fieldTIMS,
                        STRING = fieldSTRING,
                        RAWSTRING = fieldRAWSTRING,
                        UNIT = fieldUNIT

                    })).Match(
                        r => r,
                        l =>
                        {
                            Console.WriteLine(l.Message);
                            return new TypesTestData();
                        });
            // ReSharper restore StringLiteralTypo


            CompareLogic compareLogic = new CompareLogic();
            var result = compareLogic.Compare(inputData, outputData);

            Console.WriteLine(!result.AreEqual ? result.DifferencesString : "Test succeed");
        }
        private static async Task<long> RunPerformanceTest01(IRfcContext context, int rows=0)
        {
            var watch = Stopwatch.StartNew();

            var result = await context.CallFunction("ZYANCO_PT_READ_1",
                func => SetRows(func, rows),
                func => func.MapTable("ET_DATA", s =>
                    from char40 in s.GetField<string>("FIELD_CHAR40")
                    from char01 in s.GetField<string>("FIELD_CHAR01")
                    from int04 in s.GetField<int>("FIELD_INT4")
                    //from s in s.GetField<string>("FIELD_STRING")

                    select new TestData
                    {
                        Char01 = char01,
                        Char40 = char40,
                        Int04 = int04
                    })).ToEither();

            watch.Stop();
            return watch.ElapsedMilliseconds;

        }

        private static async Task<long> RunPerformanceTest02(IRfcContext context, int rows = 0)
        {
            var watch = Stopwatch.StartNew();

            var result = await context.CallFunction("ZYANCO_PT_READ_2",
                func => SetRows(func, rows),
                func => func.MapTable("ET_DATA", s =>
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
                    )).ToEither();

            watch.Stop();
            return watch.ElapsedMilliseconds;

        }

        private static Either<RfcErrorInfo, IFunction> SetRows(Either<RfcErrorInfo, IFunction> func, in int rows)
        {
            return rows == 0 ? func : func.SetField("IV_UP_TO", rows);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static byte[] RandomByteArray(int size)
        {
            var rnd = new Random();
            var b = new byte[size]; // convert kb to byte
            rnd.NextBytes(b);
            return b;
        }
    }



    public class TestData
    {
        public string Char40 { get; set; }
        public string Char01 { get; set; }
        public int Int04 { get; set; }

        public string String { get; set; }
        
    }

    public class TypesTestData
    {
        // ReSharper disable InconsistentNaming
        public string ACCP { get; set; }
        public string CHAR { get; set; }
        public string CLNT { get; set; }
        public string CUKY { get; set; }
        public double CURR { get; set; }
        public DateTime DATS { get; set; }
        public decimal DEC { get; set; }
        public double FLTP { get; set; }
        public int INT1 { get; set; }
        public short INT2 { get; set; }
        public int INT4 { get; set; }
        public string LANG { get; set; }
        public string LCHR { get; set; }
        public byte[] LRAW { get; set; }
        public string NUMC { get; set; }
        public int PREC { get; set; }
        public double QUAN { get; set; }

        public byte[] RAW { get; set; }
        public string SSTRING { get; set; }
        public DateTime TIMS { get; set; }
        public string STRING { get; set; }
        public byte[] RAWSTRING { get; set; }
        public string UNIT { get; set; }

        // ReSharper restore InconsistentNaming



    }
}