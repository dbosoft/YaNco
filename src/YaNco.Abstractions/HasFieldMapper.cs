using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco;

// ReSharper disable once InconsistentNaming
public interface HasFieldMapper<RT> where RT : struct, HasFieldMapper<RT>
{
    Eff<RT, IFieldMapper> FieldMapperEff { get; }

}