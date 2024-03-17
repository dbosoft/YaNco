using System;
using System.Collections.Generic;
using LanguageExt;
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco;

public static class SAPRfcServer<RT>
    where RT : struct, HasSAPRfcServer<RT>, HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, IHasEnvRuntimeSettings
{
    public static Eff<RT, RfcServerAttributes> getServerAttributes(IRfcHandle handle)
    {
        return default(RT).RfcServerEff.Bind(io => io.GetServerCallContext(handle).ToEff(l => l));
    }

    public static Eff<RT, Aff<RT,IRfcServer<RT>>> buildServer(IDictionary<string,string> serverParam,
        Action<ServerBuilder<RT>> configure)
    {
        return Prelude.Eff(() =>
        {
            var builder = new ServerBuilder<RT>(serverParam);
            configure(builder);
            return builder.Build();

        });
        
    }

    public static Aff<RT, Unit> useServer(Aff<RT, IRfcServer<RT>> serverEffect, Func<IRfcServer<RT>, Aff<RT, Unit>> ma)
    {
        var startAff = serverEffect.Bind(s => s.Start().Map(_ => s).ToAff(l=>l));

        return Prelude.use(startAff, ma);
    }

    public static Aff<RT, Unit> stopServer(IRfcServer<RT> server)
    {
        return server.Stop().ToAff(l => l).WithRuntime<RT>();
    }

}