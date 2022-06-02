using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public static class FunctionalDataContainerExtensions
    {

        public static Either<RfcErrorInfo, TDataContainer> SetField<TDataContainer, T>(this Either<RfcErrorInfo, TDataContainer> self, string name, T value)
            where TDataContainer : IDataContainer
        {
            return self.Bind(s => s.SetField(name, value).Map(u => s));
        }

        public static Either<RfcErrorInfo, T> GetField<T>(this Either<RfcErrorInfo, IFunction> self, string name)
        {
            return self.Bind(s => s.GetField<T>(name));
        }

        public static Either<RfcErrorInfo, TDataContainer> SetStructure<TDataContainer, TResult>(this Either<RfcErrorInfo, TDataContainer> self, string structureName, Func<Either<RfcErrorInfo, IStructure>, Either<RfcErrorInfo, TResult>> map)
            where TDataContainer : IDataContainer
        {
             return self.Bind(dc =>
                dc.GetStructure(structureName).Use(used => used.Apply(map)).Map(_=>dc));
        }

        public static Either<RfcErrorInfo, TDataContainer> SetTable<TDataContainer, TInput, TResult>(
            this Either<RfcErrorInfo, TDataContainer> self, string tableName,
            IEnumerable<TInput> inputList,
            Func<Either<RfcErrorInfo, IStructure>, TInput, Either<RfcErrorInfo, TResult>> map)
            where TDataContainer : IDataContainer
            => SetTable(self, tableName, () => inputList, map);

        public static Either<RfcErrorInfo, TDataContainer> SetTable<TDataContainer, TInput, TResult>(this Either<RfcErrorInfo, TDataContainer> self, string tableName, Func<IEnumerable<TInput>> inputListFunc, Func<Either<RfcErrorInfo, IStructure>, TInput, Either<RfcErrorInfo, TResult>> map)
            where TDataContainer : IDataContainer
        {
            return self.Bind(dc => dc.GetTable(tableName).Use(used => used.Map(table => (dc, table, inputListFunc))

                .Bind(t => t.inputListFunc().Map(
                    input => t.table.AppendRow()
                        .Apply(row => map(row, input).Bind(_ => row))
                ).Traverse(l => l).Map(_=> dc))));

        }

        static public Either<RfcErrorInfo, IEnumerable<TResult>> MapStructure<TResult>(this Either<RfcErrorInfo,ITable> eitherTable, Func<IStructure,Either<RfcErrorInfo, TResult>> mapperFunc)
        {
            return eitherTable.Bind(table=> table.Rows.Map(mapperFunc).Traverse(l=>l));
        }

        static public Either<RfcErrorInfo, IEnumerable<TResult>> MapTable<TCont,TResult>(this Either<RfcErrorInfo,TCont> self, string tableName, Func<IStructure, Either<RfcErrorInfo, TResult>> mapperFunc)
            where TCont: IDataContainer
        {
            return self.Map(s =>s.GetTable(tableName)).Bind(t => t.MapStructure(mapperFunc));
        }

        static public Either<RfcErrorInfo, TResult> MapStructure<TCont,TResult>(this Either<RfcErrorInfo,TCont> self, string structureName, Func<IStructure, Either<RfcErrorInfo, TResult>> mapperFunc)
            where TCont : IDataContainer
        {
            return self.Bind(s=>s.GetStructure(structureName).Bind(mapperFunc));

        }

    }
}
