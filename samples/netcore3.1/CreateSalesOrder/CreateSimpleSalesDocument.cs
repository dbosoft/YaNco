using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.Extensions.Configuration;

namespace CreateSalesOrder
{
    public class CreateSimpleSalesDocument : RootCommand
    {
        public CreateSimpleSalesDocument()
            : base("Creates a sales document in SAP system.")
        {
            AddArgument(new Argument<string>("customerNo", "Number (Id) of the customer"));
            AddArgument(new Argument<string>("productId", "Id of the product"));
            AddOption(new Option<int>(
                new[] { "--quantity", "-q" }, () => 1, "Quantity ordered"));
        }

        public new class Handler : ICommandHandler
        {
            private readonly IRfcContext _rfcContext;
            private readonly CustomizingSettings _customizingSettings = new CustomizingSettings();

            public string CustomerNo { get; set; }
            public int Quantity { get; set; }
            public string ProductId { get; set; }

            public static IEnumerable<int> OneLine = Enumerable.Range(0, 1);

            public Handler(IRfcContext rfcContext, IConfiguration configuration)
            {
                _rfcContext = rfcContext;
                configuration.GetSection("salesSettings").Bind(_customizingSettings);


            }

            public int Invoke(InvocationContext context)
            {
                throw new NotImplementedException();
            }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                return _rfcContext.CallFunction("BAPI_SALESDOCU_CREATEFROMDATA1",
                        Input: f => f
                            // sets the input fields
                            .SetStructure("SALES_HEADER_IN",s => s
                                .SetField("DOC_TYPE", _customizingSettings.DocumentType ?? "")
                                .SetField("SALES_ORG", _customizingSettings.SalesOrganization ?? "")
                                .SetField("DISTR_CHAN", _customizingSettings.DistributionChannel ?? "")
                                .SetField("DIVISION", _customizingSettings.Division ?? "")
                            )
                            .SetStructure("SALES_HEADER_INX", s => s 
                                .SetField("UPDATEFLAG", "I") // this enables handling of X structures
                                .SetField("DOC_TYPE", _customizingSettings.DocumentType != null ? "X": "")
                                .SetField("SALES_ORG", _customizingSettings.SalesOrganization != null ? "X" : "")
                                .SetField("DISTR_CHAN", _customizingSettings.DistributionChannel != null ? "X" : "")
                                .SetField("DIVISION", _customizingSettings.Division != null ? "X" : "")
                            )
                            //we only support documents with one item line
                            .SetTable("SALES_ITEMS_IN", OneLine, (s, _) => s
                                .SetField("MATERIAL", ProductId)
                            )
                            .SetTable("SALES_ITEMS_INX", OneLine, (s, _) => s
                                .SetField("MATERIAL", "X")
                            )
                            .SetTable("SALES_PARTNERS", OneLine, (s, _) => s
                                .SetField("PARTN_ROLE", "AG")
                                .SetField("PARTN_NUMB", CustomerNo)
                            )
                            .SetTable("SALES_SCHEDULES_IN", OneLine, (s, _) => s
                                //item numbers are not necessary, the function will automatically assign it to the item
                                .SetField("REQ_DATE", DateTime.Now)
                                .SetField("REQ_QTY", Quantity)
                            )
                            .SetTable("SALES_SCHEDULES_INX", OneLine, (s, _) => s
                                .SetField("REQ_DATE", "X")
                                .SetField("REQ_QTY", "X")
                            ),
                        Output: f => f
                            .HandleReturnTable() //map result of RETURN table to RfcErrorInfo
                            .GetField<string>("SALESDOCUMENT_EX")

                    )
                    .CommitAndWait(_rfcContext)
                    //alternative to:
                    //.Bind(result => _rfcContext.CommitAndWait().Map(_ => result))
                    .MatchAsync(
                        LeftAsync:async l=>
                        {
                            await Console.Error.WriteLineAsync($"Failed to create sales document. Error: {l.Message}");
                            return -1;
                        },
                        RightAsync: async result => {
                            await Console.Out.WriteLineAsync($"Created Sales document: {result}");
                            return 0;
                        });

            }
        }
    }
}