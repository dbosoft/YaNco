using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public static class FunctionalDataContainerExtensions
    {
        public static Either<RfcErrorInfo, TDataContainer> SetField<TDataContainer, T>(this Either<RfcErrorInfo, TDataContainer> self, string name, T value)
            where TDataContainer : IDataContainer
        {
            return self.Bind(s => s.SetField(name, value).Map(u=> s));
        }

        public static Task<Either<RfcErrorInfo, TDataContainer>> SetField<TDataContainer, T>(this Task<Either<RfcErrorInfo, TDataContainer>> self, string name, T value)
            where TDataContainer : IDataContainer
        {
            return self.BindAsync(s => s.SetField(name, value).Map(u => s));
        }

        
        public static Task<Either<RfcErrorInfo, TDataContainer>> SetStructure<TDataContainer>(this Task<Either<RfcErrorInfo, TDataContainer>> self, string structureName, Func<Either<RfcErrorInfo, IStructure>, Either<RfcErrorInfo, IStructure>> map)
            where TDataContainer : IDataContainer
        {
            return self.BindAsync(dc => dc.GetStructure(structureName).Apply(map).Map(u => dc));
        }

        public static Task<Either<RfcErrorInfo, TDataContainer>> SetTable<TDataContainer, TInput>(
            this Task<Either<RfcErrorInfo, TDataContainer>> self, string tableName,
            IEnumerable<TInput> inputList,
            Func<Either<RfcErrorInfo, IStructure>, TInput, Either<RfcErrorInfo, IStructure>> map)
            where TDataContainer : IDataContainer
            => SetTable(self, tableName, () => inputList, map);

        public static Task<Either<RfcErrorInfo, TDataContainer>> SetTable<TDataContainer, TInput>(this Task<Either<RfcErrorInfo, TDataContainer>> self, string tableName, Func<IEnumerable<TInput>> inputListFunc, Func<Either<RfcErrorInfo, IStructure>, TInput, Either<RfcErrorInfo, IStructure>> map)
            where TDataContainer : IDataContainer
        {
            return self.BindAsync(dc => dc.GetTable(tableName).Map(table => (dc, table, inputListFunc))

                    .Bind(t => t.inputListFunc().Map(
                        input => t.table.AppendRow().Apply(row=>map(row,input))
                    ).Traverse(l => l).Map(_ => dc)));

        }
    }
}
