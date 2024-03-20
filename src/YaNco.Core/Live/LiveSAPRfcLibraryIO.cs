using System;
using Dbosoft.YaNco.Internal;
using Dbosoft.YaNco.Traits;
using LanguageExt;

namespace Dbosoft.YaNco.Live;

public readonly struct LiveSAPRfcLibraryIO : SAPRfcLibraryIO
{
    private readonly Option<ILogger> _logger;

    public LiveSAPRfcLibraryIO(Option<ILogger> logger)
    {
        _logger = logger;
    }

    public Either<RfcError, Version> GetVersion()
    {
        return Prelude.Try(Api.GetVersion).ToEither(f => RfcError.New(f));

    }

    public Either<RfcError, Unit> SetTraceDirectory(string traceDirectory)
    {
        var rc = Api.SetTraceDirectory(traceDirectory, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, Unit> SetMaximumTraceFiles(int traceFiles)
    {
        var rc = Api.SetMaximumTraceFiles(traceFiles, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, Unit> SetCpicTraceLevel(int traceLevel)
    {
        var rc = Api.SetCpicTraceLevel(traceLevel, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, Unit> LoadCryptoLibrary(string libraryPath)
    {
        var rc = Api.LoadCryptoLibrary(libraryPath, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, Unit> SetIniDirectory(string iniDirectory)
    {
        var rc = Api.SetIniDirectory(iniDirectory, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, Unit> ReloadRfcIni()
    {
        var rc = Api.ReloadIniFile(out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);
    }

}