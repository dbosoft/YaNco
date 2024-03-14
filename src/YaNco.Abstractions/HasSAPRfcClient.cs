using System;
using System.Runtime;
using System.Threading;
using LanguageExt;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006

namespace Dbosoft.YaNco;


public interface HasSAPRfcFunctions<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, SAPRfcFunctionIO> RfcFunctionsEff { get; }
}

public interface HasSAPRfcConnection<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, SAPRfcConnectionIO> RfcConnectionEff { get; }
}

public interface HasSAPRfcServer<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, SAPRfcServerIO> RfcServerEff { get; }
}

public interface HasEnvSettings<TSettings>
    where TSettings : SAPRfcRuntimeSettings
{
    SAPRfcRuntimeEnv<TSettings> Env { get; }
}

public interface HasCancelFactory<RT> : HasCancel<RT> where RT : struct, HasCancel<RT>
{
    RT WithCancelToken(CancellationToken token);

}