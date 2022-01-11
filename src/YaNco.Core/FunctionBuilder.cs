using System;
using System.Collections.Generic;
using System.Linq;
using Dbosoft.YaNco.Internal;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class FunctionBuilder
    {
        private readonly string _functionName;
        private readonly IDictionary<string, RfcParameterDescription> _parameters = new Dictionary<string, RfcParameterDescription>();


        public FunctionBuilder(string functionName)
        {
            _functionName = functionName;
        }

        public FunctionBuilder AddParameter(RfcParameterDescription parameter)
        {
            _parameters.Add(parameter.Name, parameter);
            return this;
        }

        public FunctionBuilder AddChar(string name, RfcDirection direction, uint length, bool optional = true, string defaultValue = null)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.CHAR, direction, length, length * 2, 0, optional, defaultValue));
        }

        public FunctionBuilder AddInt(string name, RfcDirection direction, bool optional = true, int defaultValue = 0)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.INT, direction, 0, 0, 0, optional, defaultValue.ToString()));
        }

        public FunctionBuilder AddLong(string name, RfcDirection direction, bool optional = true, long defaultValue = 0)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.INT8, direction, 0, 0, 0, optional, defaultValue.ToString()));
        }

        public FunctionBuilder AddString(string name, RfcDirection direction, bool optional = true, uint length = 0, string defaultValue = null)
        {
            return AddParameter(new RfcParameterDescription(name, RfcType.STRING, direction, length, length*2, 0, optional, defaultValue));
        }

        public FunctionBuilder AddStructure(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true)
        {
            return AddTyped(name, RfcType.STRUCTURE, direction, typeHandle, optional);
        }

        public FunctionBuilder AddStructure(string name, RfcDirection direction, IStructure structure, bool optional = true)
        {
            return structure.GetTypeDescription()
                .Match(
                    Right: r => AddTyped(name, RfcType.STRUCTURE, direction, r, optional),
                    Left: l => throw new ArgumentException("Argument is not a valid type handle", nameof(structure)));
        }

        public FunctionBuilder AddTable(string name, RfcDirection direction, ITable table, bool optional = true)
        {
            return table.GetTypeDescription()
                .Match(
                    Right: r => AddTyped(name, RfcType.STRUCTURE, direction, r, optional),
                    Left: l => throw new ArgumentException("Argument is not a valid type handle", nameof(table)));
        }

        public FunctionBuilder AddTable(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true)
        {
            return AddTyped(name, RfcType.TABLE, direction, typeHandle, optional);
        }

        private FunctionBuilder AddTyped(string name, RfcType type, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true)
        {
            if (!(typeHandle is TypeDescriptionHandle handle))
                throw new ArgumentException("Argument has to be of type TypeDescriptionHandle", nameof(typeHandle));

            var ptr = handle.Ptr;
            return AddParameter(new RfcParameterDescription(name, type, direction, 0, 0, 0, optional, null) { TypeDescriptionHandle = ptr });
        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> Build()
        {

            var functionHandle = Api.CreateFunctionDescription(_functionName, out var errorInfo);
            if (functionHandle == null)
                return errorInfo;

            if (_parameters.Values.Select(parameter => Api.AddFunctionParameter(functionHandle, parameter, out errorInfo)).Any(rc => rc != RfcRc.RFC_OK))
            {
                return errorInfo;
            }

            return functionHandle;
        }

    }

}
