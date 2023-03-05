using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IFunctionBuilder
    {
        IFunctionBuilder AddParameter(RfcParameterDescription parameter);
        IFunctionBuilder AddChar(string name, RfcDirection direction, uint length, bool optional = true, string defaultValue = null);
        IFunctionBuilder AddInt(string name, RfcDirection direction, bool optional = true, int defaultValue = 0);
        IFunctionBuilder AddLong(string name, RfcDirection direction, bool optional = true, long defaultValue = 0);
        IFunctionBuilder AddString(string name, RfcDirection direction, bool optional = true, uint length = 0, string defaultValue = null);
        IFunctionBuilder AddStructure(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true);
        IFunctionBuilder AddStructure(string name, RfcDirection direction, IStructure structure, bool optional = true);
        IFunctionBuilder AddTable(string name, RfcDirection direction, ITable table, bool optional = true);
        IFunctionBuilder AddTable(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true);
        Either<RfcError, IFunctionDescriptionHandle> Build();
    }
}