using Dbosoft.YaNco.TypeMapping;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

// ReSharper disable once InconsistentNaming
[PublicAPI]
public interface HasFieldMapper<RT> where RT : struct, HasFieldMapper<RT>
{
    Eff<RT, IFieldMapper> FieldMapperEff { get; }

}