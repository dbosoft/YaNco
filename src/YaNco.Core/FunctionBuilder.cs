using System;
using System.Collections.Generic;
using Dbosoft.YaNco.Internal;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class FunctionBuilder<RT> : IFunctionBuilder<RT>
        where RT : struct, HasSAPRfcFunctions<RT>

    {
        private readonly string _functionName;
        private readonly IDictionary<string, RfcParameterDescription> _parameters = new Dictionary<string, RfcParameterDescription>();


        public FunctionBuilder(string functionName)
        {
            _functionName = functionName;
        }

        public IFunctionBuilder<RT> AddParameter(RfcParameterDescription parameter)
        {
            _parameters.Add(parameter.Name, parameter);
            return this;
        }

        public IFunctionBuilder<RT> AddChar(string name, RfcDirection direction, uint length, bool optional = true, string defaultValue = null)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.CHAR, direction, length, length * 2, 0, optional, defaultValue));
        }

        public IFunctionBuilder<RT> AddInt(string name, RfcDirection direction, bool optional = true, int defaultValue = 0)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.INT, direction, 0, 0, 0, optional, defaultValue.ToString()));
        }

        public IFunctionBuilder<RT> AddLong(string name, RfcDirection direction, bool optional = true, long defaultValue = 0)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.INT8, direction, 0, 0, 0, optional, defaultValue.ToString()));
        }

        public IFunctionBuilder<RT> AddString(string name, RfcDirection direction, bool optional = true, uint length = 0, string defaultValue = null)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.STRING, direction, length, length * 2, 0, optional, defaultValue));
        }

        public IFunctionBuilder<RT> AddStructure(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true)
        {
            return AddTyped(name, RfcType.STRUCTURE, direction, typeHandle, optional);
        }

        public IFunctionBuilder<RT> AddStructure(string name, RfcDirection direction, IStructure structure, bool optional = true)
        {
            return structure.GetTypeDescription()
                .Match(
                    Right: r => AddTyped(name, RfcType.STRUCTURE, direction, r, optional),
                    Left: l => throw new ArgumentException("Argument is not a valid type handle", nameof(structure)));
        }

        public IFunctionBuilder<RT> AddTable(string name, RfcDirection direction, ITable table, bool optional = true)
        {
            return table.GetTypeDescription()
                .Match(
                    Right: r => AddTyped(name, RfcType.STRUCTURE, direction, r, optional),
                    Left: l => throw new ArgumentException("Argument is not a valid type handle", nameof(table)));
        }

        public IFunctionBuilder<RT> AddTable(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true)
        {
            return AddTyped(name, RfcType.TABLE, direction, typeHandle, optional);
        }

        private IFunctionBuilder<RT> AddTyped(string name, RfcType type, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true)
        {
            if (!(typeHandle is TypeDescriptionHandle handle))
                throw new ArgumentException("Argument has to be of type TypeDescriptionHandle", nameof(typeHandle));

            var ptr = handle.Ptr;
            return AddParameter(new RfcParameterDescription(name, type, direction, 0, 0, 0, optional, null) { TypeDescriptionHandle = ptr });
        }

        public Eff<RT,IFunctionDescriptionHandle> Build()
        {
            return from io in default(RT).RfcFunctionsEff
            from functionHandle in io.CreateFunctionDescription(_functionName).ToEff(l=>l)
            from mapped in _parameters.Values.Map(parameter =>
                    io.AddFunctionParameter(functionHandle, parameter))
                .Traverse(l => l).Map(e => functionHandle)
                .ToEff(l => l)
            select functionHandle;


        }

    }

}