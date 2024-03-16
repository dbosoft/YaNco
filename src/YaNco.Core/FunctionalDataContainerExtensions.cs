using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco;

public static class FunctionalDataContainerExtensions
{
    /// <summary>
    /// This methods sets a value of a field in the <see cref="IDataContainer"/>, e.g. a function or structure field.
    /// </summary>
    /// <remarks>
    /// Use this method to set the value of function or structure field when mapping from .NET to SAP in the input mapping of
    /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>. 
    /// </remarks>
    /// <typeparam name="TDataContainer">Type of data container (a structure, function or table). </typeparam>
    /// <typeparam name="T">type of field value.</typeparam>
    /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/>monad.</param>
    /// <param name="name">Name of the field</param>
    /// <param name="value">Value to be set.</param>
    /// <returns></returns>
    public static Either<RfcError, TDataContainer> SetField<TDataContainer, T>(this Either<RfcError, TDataContainer> self, string name, T value)
        where TDataContainer : IDataContainer
    {
        return self.Bind(s => s.SetField(name, value).Map(_ => s));
    }

    public static Either<RfcError, T> GetField<T>(this Either<RfcError, IFunction> self, string name)
    {
        return self.Bind(s => s.GetField<T>(name));
    }

    public static Either<RfcError, TDataContainer> SetStructure<TDataContainer, TResult>(this Either<RfcError, TDataContainer> self, string structureName, Func<Either<RfcError, IStructure>, Either<RfcError, TResult>> map)
        where TDataContainer : IDataContainer
    {
        return self.Bind(dc =>
            dc.GetStructure(structureName).Use(used => used.Apply(map)).Map(_=>dc));
    }

    public static Either<RfcError, TDataContainer> SetTable<TDataContainer, TInput, TResult>(
        this Either<RfcError, TDataContainer> self, string tableName,
        IEnumerable<TInput> inputList,
        Func<Either<RfcError, IStructure>, TInput, Either<RfcError, TResult>> map)
        where TDataContainer : IDataContainer
        => SetTable(self, tableName, () => inputList, map);

    public static Either<RfcError, TDataContainer> SetTable<TDataContainer, TInput, TResult>(this Either<RfcError, TDataContainer> self, string tableName, Func<IEnumerable<TInput>> inputListFunc, Func<Either<RfcError, IStructure>, TInput, Either<RfcError, TResult>> map)
        where TDataContainer : IDataContainer
    {
        return self.Bind(dc => dc.GetTable(tableName).Use(used => used.Map(table => (dc, table, inputListFunc))

            .Bind(t => t.inputListFunc().Map(
                input => t.table.AppendRow()
                    .Apply(row => map(row, input).Bind(_ => row))
            ).Traverse(l => l).Map(_=> dc))));

    }

    public static Either<RfcError, IEnumerable<TResult>> MapStructure<TResult>(this Either<RfcError, ITable> eitherTable, Func<IStructure,Either<RfcError, TResult>> mapperFunc)
    {
        return eitherTable.Bind(table=> table.Rows.Map(mapperFunc).Traverse(l=>l));
    }

    public static Either<RfcError, IEnumerable<TResult>> MapTable<TCont,TResult>(this Either<RfcError, TCont> self, string tableName, Func<IStructure, Either<RfcError, TResult>> mapperFunc)
        where TCont: IDataContainer
    {
        return self.Map(s =>s.GetTable(tableName)).Bind(t => t.MapStructure(mapperFunc));
    }

    public static Either<RfcError, TResult> MapStructure<TCont,TResult>(this Either<RfcError, TCont> self, string structureName, Func<IStructure, Either<RfcError, TResult>> mapperFunc)
        where TCont : IDataContainer
    {
        return self.Bind(s=>s.GetStructure(structureName).Bind(mapperFunc));

    }

}