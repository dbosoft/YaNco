using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcLibraryIO
{
    Either<RfcError, System.Version> GetVersion();
    Either<RfcError, Unit> SetTraceDirectory(string traceDirectory);
    Either<RfcError, Unit> SetMaximumTraceFiles(int traceFiles);
    Either<RfcError, Unit> SetCpicTraceLevel(int traceLevel);

    Either<RfcError, Unit> LoadCryptoLibrary(string libraryPath);
    Either<RfcError, Unit> SetIniDirectory(string iniDirectory);
    Either<RfcError, Unit> ReloadRfcIni();

}