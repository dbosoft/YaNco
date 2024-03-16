using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

[PublicAPI]
public interface IRfcServer<RT> : IDisposable where RT : struct, HasCancel<RT>
{
    bool Disposed { get; }
    EitherAsync<RfcError, Unit> Start();
    EitherAsync<RfcError, Unit> Stop(int timeout = 0);
    Aff<RT, IConnection> GetClientConnection();

    Unit AddClientConnection(Aff<RT, IConnection> connectionEffect);

    /// <summary>
    /// Adds references to the server that should be disposed when the server is disposed.
    /// </summary>
    /// <param name="disposables"></param>
    void AddReferences(IEnumerable<IDisposable> disposables);


}