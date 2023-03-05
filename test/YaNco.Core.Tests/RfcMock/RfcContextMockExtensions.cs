using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dbosoft.YaNco;
using LanguageExt;
using Moq;
using Moq.Language.Flow;

namespace YaNco.Core.Tests.RfcMock
{
    [ExcludeFromCodeCoverage]
    public static class RfcContextMockExtensions
    {
        public static Mock<IRfcContext> SetupFunction(this Mock<IRfcContext> self, string functionName, Action<Mock<IFunction>> functionBuilder)
        {
            var function = new Mock<IFunction>();
            function.SetReturnsDefault(Prelude.Right<RfcError, Unit>(Unit.Default));

            functionBuilder(function);

            self.Setup(x => x.CreateFunction(functionName))
                .Returns(Prelude.RightAsync<RfcError, IFunction>(function.Object));

            self.Setup(x => x.InvokeFunction(function.Object))
                .Returns(Prelude.RightAsync<RfcError, Unit>(Unit.Default));

            return self;
        }

        public static Mock<TMock> SetupStructure<TMock>(this Mock<TMock> self, string structureName, Action<Mock<IStructure>> structureBuilder) where TMock: class, IDataContainer
        {
            var structure = new Mock<IStructure>();
            structure.SetReturnsDefault(Prelude.Right<RfcError, Unit>(Unit.Default));

            structureBuilder(structure);

            self.Setup(x => x.GetStructure(structureName))
                .Returns(Prelude.Right<RfcError, IStructure>(structure.Object));

            return self;
        }

        public static Mock<TMock> SetupStructure<TMock>(this Mock<TMock> self, string structureName, IDictionary<string,object> structureData) where TMock: class, IDataContainer
        {
            return self.SetupStructure(structureName, builder =>
            {
                foreach (var key in structureData.Keys)
                {
                    var value = structureData[key];
                    switch (value)
                    {
                        case int intValue:
                            builder.SetupGetField(key, intValue);
                            break;
                        case bool boolValue:
                            builder.SetupGetField(key, boolValue);
                            break;
                        default:
                            builder.SetupGetField(key, value.ToString());
                            break;
                    }                      
                }
            });
        }


        public static Mock<TMock> SetupTable<TMock>(this Mock<TMock> self, string tableName, Action<TableMockBuilder> buildTable) where TMock: class, IDataContainer
        {
            var tableBuilder = new TableMockBuilder(new Mock<ITable>());
            buildTable(tableBuilder);

            tableBuilder.Table.SetReturnsDefault(Prelude.Right<RfcError, Unit>(Unit.Default));
            tableBuilder.Table.Setup(x => x.Rows).Returns( () =>
            {
                return tableBuilder.Structures.Select(x => x.Object);
            });
            
            self.Setup(x => x.GetTable(tableName))
                .Returns(Prelude.Right<RfcError, ITable>(tableBuilder.Table.Object));

            return self;
        }

        public static Mock<TMock> SetupEmptyTable<TMock>(this Mock<TMock> self, string tableName) where TMock: class, IDataContainer
        {
            return self.SetupTable(tableName, builder => {});
        }

        public static Mock<TMock> SetupTable<TMock>(this Mock<TMock> self, string tableName, IEnumerable<IDictionary<string,object>> tableData) where TMock: class, IDataContainer
        {
            return self.SetupTable(tableName, builder =>
            {
                foreach (var data in tableData)
                {
                    builder.AddRow(s =>
                    {
                        foreach (var key in data.Keys)
                        {
                            var value = data[key];
                            switch (value)
                            {
                                case int intValue:
                                    s.SetupGetField(key, intValue);
                                    break;
                                case bool boolValue:
                                    s.SetupGetField(key, boolValue);
                                    break;
                                default:
                                    s.SetupGetField(key, value.ToString());
                                    break;
                            }
                        }
                    });                         
                }


            });
        }


        public static Mock<TMock> SetupGetField<TMock,T>(this Mock<TMock> self, string fieldName, T value) where TMock: class, IDataContainer
        {
            self.Setup(x => x.GetField<T>(fieldName)).Returns(value);
            
            return self;
        }
       

        public static Mock<TMock> SetupSetFieldAnyOfType<TMock,T>(this Mock<TMock> self) where TMock: class, IDataContainer
        {
            self.Setup(x => x.SetField(It.IsAny<string>(), It.IsAny<T>()))
                .ReturnsUnit();
            
            return self;
        }

        public static IReturnsResult<TMock> ReturnsUnit<TMock>(this ISetup<TMock, Either<RfcError,Unit>> self) where TMock: class, IDataContainer
        {
            return self.Returns(Prelude.Right<RfcError, Unit>(Unit.Default));
        }

        public static Mock<TMock> SetupAllSetField<TMock>(this Mock<TMock> self) where TMock: class, IDataContainer
        {
            self.SetupSetFieldAnyOfType<TMock,string>();
            self.SetupSetFieldAnyOfType<TMock,int>();
            self.SetupSetFieldAnyOfType<TMock,bool>();
            return self;
        }

        public static Mock<IFunction> SetupReturn(this Mock<IFunction> self)
        {
            var returnStructure = new Mock<IStructure>();

            returnStructure.Setup(x => x.GetField<string>(It.IsAny<string>()))
                .Returns(Prelude.Right<RfcError, string>(""));
            
            self.Setup(x=>x.GetStructure("RETURN")) 
                .Returns(Prelude.Right<RfcError, IStructure>(returnStructure.Object));

            return self;
        }

    }
}