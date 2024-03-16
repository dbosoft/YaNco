using System;

namespace Dbosoft.YaNco.Internal;

internal class FunctionHandler : IDisposable
{
    private readonly RfcFunctionDelegate _functionHandler;
    public readonly Interopt.RfcServerFunction ServerFunction;
    public readonly string SysId;
    private readonly FunctionDescriptionHandle _functionDescription;

    public FunctionHandler(string sysId, 
        FunctionDescriptionHandle functionDescription, RfcFunctionDelegate functionHandler)
    {
        SysId = sysId;
        _functionDescription = functionDescription;

        _functionHandler = functionHandler;
        ServerFunction = CallInternal;
    }

    public RfcRc CallInternal(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo)
    {
        RfcErrorInfo errorInfoLocal = default;
        var rc = _functionHandler(new RfcHandle(rfcHandle), new FunctionHandle(funcHandle)).Match(
                Right: _ => RfcRc.RFC_OK,
                l =>
                {
                    errorInfoLocal = l;
                    return l.Code;

                })
            .ConfigureAwait(true)
            .GetAwaiter().GetResult();

        errorInfo = errorInfoLocal;
        return rc;
    }

    private void ReleaseUnmanagedResources()
    {
        Interopt.RfcInstallServerFunction(SysId, _functionDescription.Ptr, null, out _);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~FunctionHandler()
    {
        ReleaseUnmanagedResources();
    }
}