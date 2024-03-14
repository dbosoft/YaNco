using LanguageExt;

namespace Dbosoft.YaNco;

public interface HasSAPRfcApi<RT> where RT : struct, HasSAPRfcApi<RT>
{
    Eff<RT, SAPRfcFieldIO> RfcFieldsEff { get; }
    Eff<RT, SAPRfcStructureIO> RfcStructuresEff { get; }
    Eff<RT, SAPRfcTableIO> RfcTablesEff { get; }
    Eff<RT, SAPRfcTypeIO> RfcTypesEff { get; }
    Eff<RT, SAPRfcFunctionIO> RfcFunctionsEff { get; }
    Eff<RT, SAPRfcConnectionIO> RfcConnectionsIO { get; }

}