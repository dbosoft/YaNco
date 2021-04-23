using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public static class FunctionalDataContainerExtensions
    {
        public static EitherAsync<RfcErrorInfo, TDataContainer> SetField<TDataContainer, T>(this EitherAsync<RfcErrorInfo, TDataContainer> self, string name, T value)
            where TDataContainer : IDataContainer
        {
            return self.Bind(s => s.SetField(name, value).Map(u => s).ToAsync());
        }

        
        public static EitherAsync<RfcErrorInfo, TDataContainer> SetStructure<TDataContainer, TResult>(this EitherAsync<RfcErrorInfo, TDataContainer> self, string structureName, Func<EitherAsync<RfcErrorInfo, IStructure>, TResult> map)
            where TDataContainer : IDataContainer
        {
             return self.Bind(dc =>
                dc.GetStructure(structureName).Use(used => used.ToAsync().Apply(map).Apply(_=>used)).ToAsync().Map(_=>dc));
        }

        public static EitherAsync<RfcErrorInfo, TDataContainer> SetTable<TDataContainer, TInput, TResult>(
            this EitherAsync<RfcErrorInfo, TDataContainer> self, string tableName,
            IEnumerable<TInput> inputList,
            Func<EitherAsync<RfcErrorInfo, IStructure>, TInput, TResult> map)
            where TDataContainer : IDataContainer
            => SetTable(self, tableName, () => inputList, map);

        public static EitherAsync<RfcErrorInfo, TDataContainer> SetTable<TDataContainer, TInput, TResult>(this EitherAsync<RfcErrorInfo, TDataContainer> self, string tableName, Func<IEnumerable<TInput>> inputListFunc, Func<EitherAsync<RfcErrorInfo, IStructure>, TInput, TResult> map)
            where TDataContainer : IDataContainer
        {
            return self.Bind(dc => dc.GetTable(tableName).Use(used => used.Map(table => (dc, table, inputListFunc))

                .Map(t => t.inputListFunc().Map(
                    input => t.table.AppendRow().ToAsync()
                        .Apply(row => map(row, input).Apply(_ => row))
                ).Traverse(l => l)).Apply(_ => used)).ToAsync().Map(_=>dc));

        }

        static public Either<RfcErrorInfo, IEnumerable<TResult>> MapStructure<TResult>(this ITable table, Func<IStructure,Either<RfcErrorInfo, TResult>> mapperFunc)
        {
            return table.Rows.Map(mapperFunc).Traverse(l=>l);
        }

        static public Either<RfcErrorInfo, IEnumerable<TResult>> MapTable<TResult>(this IDataContainer self, string tableName, Func<IStructure, Either<RfcErrorInfo, TResult>> mapperFunc)
        {
            return self.GetTable(tableName).Bind(t => t.MapStructure(mapperFunc));
        }

        static public Either<RfcErrorInfo, TResult> MapStructure<TResult>(this IDataContainer self, string structureName, Func<IStructure, Either<RfcErrorInfo, TResult>> mapperFunc)
        {
            return self.GetStructure(structureName).Bind(mapperFunc);

        }

    }
}
