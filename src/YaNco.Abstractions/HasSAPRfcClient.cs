using System;
using LanguageExt;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006

namespace Dbosoft.YaNco;

[Typeclass("*")]
public interface HasSAPRfcClient<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, IRfcClientConnectionProvider> RfcClientConnectionEff { get; }
}
