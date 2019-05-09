using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    internal abstract class TypeDescriptionDataContainer : DataContainer
    {
        private readonly IDataContainerHandle _handle;
        private readonly IRfcRuntime _rfcRuntime;

        protected TypeDescriptionDataContainer(IDataContainerHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
            _handle = handle;
            _rfcRuntime = rfcRuntime;
        }

        protected override Either<RfcErrorInfo, RfcFieldInfo> GetFieldInfo(string name)
        {
            return _rfcRuntime.GetTypeDescription(_handle).Use(used => used
                .Bind(handle => _rfcRuntime.GetTypeFieldDescription(handle, name)));

        }
    }
}