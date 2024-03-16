using LanguageExt;

namespace Dbosoft.YaNco;

internal abstract class TypeDescriptionDataContainer : DataContainer
{
    protected readonly IDataContainerHandle Handle;

    protected TypeDescriptionDataContainer(IDataContainerHandle handle, SAPRfcDataIO io) 
        : base(handle, io)
    {
        Handle = handle;
    }

    protected override Either<RfcError, RfcFieldInfo> GetFieldInfo(string name)
    {
        return IO.GetTypeDescription(Handle).Use(used => used
            .Bind(handle => IO.GetTypeFieldDescription(handle, name)));

    }
}