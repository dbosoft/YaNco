using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
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
            return self.BindAsync(dc => dc.GetStructure(structureName).Use(used => used.Apply(map).Map(u => dc)));
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
            return self.BindAsync(dc => dc.GetTable(tableName).Use(used=> used.Map(table => (dc, table, inputListFunc))

                    .Bind(t => t.inputListFunc().Map(
                        input => t.table.AppendRow().Apply(row=>map(row,input))
                    ).Traverse(l => l).Map(_ => dc))));

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
