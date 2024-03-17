using System;
using Dbosoft.YaNco.Traits;
using Dbosoft.YaNco.TypeMapping;
using JetBrains.Annotations;
using LanguageExt;
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco;

[Obsolete(Deprecations.RfcRuntime)]
[PublicAPI]
public interface IRfcRuntime : 
    SAPRfcServerIO, SAPRfcConnectionIO, SAPRfcFunctionIO, SAPRfcDataIO
{
    IFieldMapper FieldMapper { get; }
    Option<ILogger> Logger { get; }


}