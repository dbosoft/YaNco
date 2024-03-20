using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcStructureIO
{
    Either<RfcError, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name);
    Either<RfcError, IStructureHandle> CreateStructure(ITypeDescriptionHandle typeDescriptionHandle);
    Either<RfcError, Unit> SetStructure(IStructureHandle structureHandle, string content);

}