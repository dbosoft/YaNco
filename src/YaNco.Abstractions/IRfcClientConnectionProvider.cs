using System;
using LanguageExt;

namespace Dbosoft.YaNco;

public interface IRfcClientConnectionProvider : IDisposable
{
    Aff<IConnection> GetConnection();
}