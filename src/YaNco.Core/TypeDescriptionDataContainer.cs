using LanguageExt;

namespace Dbosoft.YaNco
{
    internal abstract class TypeDescriptionDataContainer : DataContainer
    {
        protected readonly IDataContainerHandle Handle;
        protected readonly IRfcRuntime RfcRuntime;

        protected TypeDescriptionDataContainer(IDataContainerHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
            Handle = handle;
            RfcRuntime = rfcRuntime;
        }

        protected override Either<RfcError, RfcFieldInfo> GetFieldInfo(string name)
        {
            return RfcRuntime.GetTypeDescription(Handle).Use(used => used
                .Bind(handle => RfcRuntime.GetTypeFieldDescription(handle, name)));

        }
    }
}