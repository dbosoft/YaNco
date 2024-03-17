using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcTypeIO
{
    Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer);
    Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IConnectionHandle connectionHandle, string typeName);
    Either<RfcError, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle);

    Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
        int index);

    Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
        string name);


}