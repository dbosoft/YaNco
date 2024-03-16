using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

[PublicAPI]
public interface IFunctionBuilder<RT>
    where RT : struct, HasSAPRfcFunctions<RT>
{
    IFunctionBuilder<RT> AddParameter(RfcParameterDescription parameter);
    IFunctionBuilder<RT> AddChar(string name, RfcDirection direction, uint length, bool optional = true, string defaultValue = null);
    IFunctionBuilder<RT> AddInt(string name, RfcDirection direction, bool optional = true, int defaultValue = 0);
    IFunctionBuilder<RT> AddLong(string name, RfcDirection direction, bool optional = true, long defaultValue = 0);
    IFunctionBuilder<RT> AddString(string name, RfcDirection direction, bool optional = true, uint length = 0, string defaultValue = null);
    IFunctionBuilder<RT> AddStructure(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true);
    IFunctionBuilder<RT> AddStructure(string name, RfcDirection direction, IStructure structure, bool optional = true);
    IFunctionBuilder<RT> AddTable(string name, RfcDirection direction, ITable table, bool optional = true);
    IFunctionBuilder<RT> AddTable(string name, RfcDirection direction, ITypeDescriptionHandle typeHandle, bool optional = true);
    Eff<RT, IFunctionDescriptionHandle> Build();
}