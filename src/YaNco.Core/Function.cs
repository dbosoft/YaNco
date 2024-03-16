using LanguageExt;

namespace Dbosoft.YaNco
{
    internal class Function : DataContainer, IFunction
    {
        private readonly SAPRfcFunctionIO _functionIO;
        public IFunctionHandle Handle { get; private set; }

        internal Function(IFunctionHandle handle, SAPRfcDataIO io, SAPRfcFunctionIO functionIO) : base(handle, io)
        {
            _functionIO = functionIO;
            Handle = handle;
        }

        protected override Either<RfcError, RfcFieldInfo> GetFieldInfo(string name)
        {
            return _functionIO.GetFunctionDescription(Handle).Use(used => used
                .Bind(handle => _functionIO.GetFunctionParameterDescription(handle, name)).Map(r => (RfcFieldInfo) r));

        }
    }
}