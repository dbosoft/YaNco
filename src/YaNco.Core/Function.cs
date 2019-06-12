using LanguageExt;

namespace Dbosoft.YaNco
{
    internal class Function : DataContainer, IFunction
    {
        public IFunctionHandle Handle { get; private set; }
        private readonly IRfcRuntime _rfcRuntime;

        internal Function(IFunctionHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
            Handle = handle;
            _rfcRuntime = rfcRuntime;
        }

        protected override Either<RfcErrorInfo, RfcFieldInfo> GetFieldInfo(string name)
        {
            return _rfcRuntime.GetFunctionDescription(Handle).Use(used => used
                .Bind(handle => _rfcRuntime.GetFunctionParameterDescription(handle, name)).Map(r => (RfcFieldInfo) r));

        }
    }
}