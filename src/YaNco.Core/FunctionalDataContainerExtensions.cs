using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
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
        public static Either<RfcErrorInfo, TDataContainer> SetField<TDataContainer, T>(this Either<RfcErrorInfo, TDataContainer> self, string name, T value)
            where TDataContainer : IDataContainer
        {
            return self.Bind(s => s.SetField(name, value).Map(u => s));
        }

        /// <summary>
        /// This methods gets a value of a field in the <see cref="IDataContainer"/>, e.g. a function or structure field.
        /// </summary>
        /// <remarks>
        /// Use this method to get the value of function or structure field when mapping from SAP to .NET in the output mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>. 
        /// </remarks>
        /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/> monad.</param>
        /// <param name="name">Name of the field</param>
        /// <typeparam name="T">type of field value.</typeparam>
        public static Either<RfcErrorInfo, T> GetField<T>(this Either<RfcErrorInfo, IFunction> self, string name)
        {
            return self.Bind(s => s.GetField<T>(name));
        }

        /// <summary>
        /// This methods sets the values of a structure in the <see cref="IDataContainer"/>, e.g. a function, table or structure.
        /// </summary>
        /// <remarks>
        /// Use this method to map from a .NET input type to a SAP structure in the input mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>.
        ///
        /// Within the mapping function passed you have to map the fields of the structure.
        /// As a structure also implements <see cref="IDataContainer"/>
        /// you can use the mapping methods of <see cref="FunctionalDataContainerExtensions"/> to map any data from .NET to SAP fields.
        /// </remarks>
        /// <typeparam name="TDataContainer">Type of data container (a structure, function or table). </typeparam>
        /// <typeparam name="TResult">type of mapping result.</typeparam>
        /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/>monad.</param>
        /// <param name="structureName">Name of the structure</param>
        /// <param name="map">Mapping function to map from .NET to SAP structure fields.</param>
        /// <returns></returns>

        public static Either<RfcErrorInfo, TDataContainer> SetStructure<TDataContainer, TResult>(this Either<RfcErrorInfo, TDataContainer> self, string structureName, Func<Either<RfcErrorInfo, IStructure>, Either<RfcErrorInfo, TResult>> map)
            where TDataContainer : IDataContainer
        {
             return self.Bind(dc =>
                dc.GetStructure(structureName).Use(used => used.Apply(map)).Map(_=>dc));
        }

        /// <summary>
        /// This methods sets the values of a table in the <see cref="IDataContainer"/>, e.g. a function, table or structure.
        /// </summary>
        /// <remarks>
        /// Use this method to map from a .NET input type to a SAP table in the input mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>.
        ///
        /// Within the mapping function passed you have to map each line of input data to a structure.
        /// As a structure also implements <see cref="IDataContainer"/>
        /// you can use the mapping methods of <see cref="FunctionalDataContainerExtensions"/> to map any data from .NET to SAP fields.
        ///
        /// The mapping function will be called for each line in <paramref name="inputList"/>.
        /// </remarks>
        /// <typeparam name="TInput">Type of input data.</typeparam>
        /// <typeparam name="TDataContainer">Type of data container (a structure, function or table). </typeparam>
        /// <typeparam name="TResult">type of mapping result.</typeparam>
        /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/>monad.</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="inputList">Input data for the table. For each line in the input a line will be added to the table.</param>
        /// <param name="map">Mapping function to map from .NET to SAP table.</param>
        /// <returns></returns>
        public static Either<RfcErrorInfo, TDataContainer> SetTable<TDataContainer, TInput, TResult>(
            this Either<RfcErrorInfo, TDataContainer> self, string tableName,
            IEnumerable<TInput> inputList,
            Func<Either<RfcErrorInfo, IStructure>, TInput, Either<RfcErrorInfo, TResult>> map)
            where TDataContainer : IDataContainer
            => SetTable(self, tableName, () => inputList, map);

        /// <summary>
        /// This methods sets the values of a table in the <see cref="IDataContainer"/>, e.g. a function, table or structure.
        /// </summary>
        /// <remarks>
        /// Use this method to map from a .NET input type to a SAP table in the input mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>.
        ///
        /// Within the mapping function passed you have to map each line of input data to a structure.
        /// As a structure also implements <see cref="IDataContainer"/>
        /// you can use the mapping methods of <see cref="FunctionalDataContainerExtensions"/> to map any data from .NET to SAP fields.
        ///
        /// The mapping function will be called for each line from the result of <paramref name="inputListFunc"/>.
        /// </remarks>
        /// <typeparam name="TInput">Type of input data.</typeparam>
        /// <typeparam name="TDataContainer">Type of data container (a structure, function or table). </typeparam>
        /// <typeparam name="TResult">type of mapping result.</typeparam>
        /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/>monad.</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="inputListFunc">Function to generate input data for the table. For each line in the input a line will be added to the table.</param>
        /// <param name="map">Mapping function to map from .NET to SAP table.</param>
        /// <returns></returns>

        public static Either<RfcErrorInfo, TDataContainer> SetTable<TDataContainer, TInput, TResult>(this Either<RfcErrorInfo, TDataContainer> self, string tableName, Func<IEnumerable<TInput>> inputListFunc, Func<Either<RfcErrorInfo, IStructure>, TInput, Either<RfcErrorInfo, TResult>> map)
            where TDataContainer : IDataContainer
        {
            return self.Bind(dc => dc.GetTable(tableName).Use(used => used.Map(table => (dc, table, inputListFunc))

                .Bind(t => t.inputListFunc().Map(
                    input => t.table.AppendRow()
                        .Apply(row => map(row, input).Bind(_ => row))
                ).Traverse(l => l).Map(_=> dc))));

        }

        /// <summary>
        /// This methods maps the values of a SAP table to .NET.
        /// </summary>
        /// <remarks>
        /// Use this method to map each structure line of a SAP table to any .NET type. To be used in the output mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>. 
        /// </remarks>
        /// <param name="eitherTable">Table reference lifted in <see cref="Either{RfcErrorInfo,ITable}"/> monad.</param>
        /// <param name="mapperFunc">Function to map from SAP structure in the table to .NET.</param>
        /// <typeparam name="TResult">type of result data.</typeparam>

        public static Either<RfcErrorInfo, IEnumerable<TResult>> MapStructure<TResult>(this Either<RfcErrorInfo,ITable> eitherTable, Func<IStructure,Either<RfcErrorInfo, TResult>> mapperFunc)
        {
            return eitherTable.Bind(table=> table.Rows.Map(mapperFunc).Traverse(l=>l));
        }

        /// <summary>
        /// This methods maps the values of a SAP table to .NET.
        /// </summary>
        /// <remarks>
        /// Use this method to map each structure line of a SAP table to any .NET type. To be used in the output mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>. 
        /// </remarks>
        /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/> monad.</param>
        /// <param name="tableName">Name of the table field in the data container.</param>
        /// <param name="mapperFunc">Function to map from each SAP structure in the table to .NET.</param>
        /// <typeparam name="TCont">type of data container.</typeparam>
        /// <typeparam name="TResult">type of result data.</typeparam>
        public static Either<RfcErrorInfo, IEnumerable<TResult>> MapTable<TCont,TResult>(this Either<RfcErrorInfo,TCont> self, string tableName, Func<IStructure, Either<RfcErrorInfo, TResult>> mapperFunc)
            where TCont: IDataContainer
        {
            return self.Map(s =>s.GetTable(tableName)).Bind(t => t.MapStructure(mapperFunc));
        }

        /// <summary>
        /// This methods maps the values of a SAP structure to .NET.
        /// </summary>
        /// <remarks>
        /// Use this method to map a SAP structure to any .NET type. To be used in the output mapping of
        /// <seealso cref="FunctionalFunctionsExtensions.CallFunction{TRInput,TResult}"/>. 
        /// </remarks>
        /// <param name="self">Data container lifted in <see cref="Either{RfcErrorInfo,TDataContainer}"/> monad.</param>
        /// <param name="structureName">Name of the structure field in the data container.</param>
        /// <param name="mapperFunc">Function to map from the SAP structure to .NET.</param>
        /// <typeparam name="TCont">type of data container.</typeparam>
        /// <typeparam name="TResult">type of result data.</typeparam>

        public static Either<RfcErrorInfo, TResult> MapStructure<TCont,TResult>(this Either<RfcErrorInfo,TCont> self, string structureName, Func<IStructure, Either<RfcErrorInfo, TResult>> mapperFunc)
            where TCont : IDataContainer
        {
            return self.Bind(s=>s.GetStructure(structureName).Bind(mapperFunc));

        }

    }
}
