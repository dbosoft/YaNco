using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcConnectionIO
{
    Either<RfcError, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams);
    Either<RfcError, Unit> CancelConnection(IConnectionHandle connectionHandle);
    Either<RfcError, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle);
    Either<RfcError, ConnectionAttributes> GetConnectionAttributes(IConnectionHandle connectionHandle);

}