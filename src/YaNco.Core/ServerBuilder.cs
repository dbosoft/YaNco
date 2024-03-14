using System;
using System.Collections.Generic;
using System.Threading;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco
{

    public class ServerBuilder : ServerBuilderBase<ServerBuilder, SAPRfcRuntime, SAPRfcRuntimeSettings>
    {
        public ServerBuilder(IDictionary<string, string> serverParam) : base(serverParam)
        {
        }

        /// <summary>
        /// Registers a action to configure the <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure">action with <see cref="RfcRuntimeConfigurer"/></param>
        /// <returns>current instance for chaining.</returns>
        /// <remarks>
        /// Multiple calls of this method will override the previous configuration action. 
        /// </remarks>
        public ServerBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer<SAPRfcRuntime, SAPRfcRuntimeSettings>> configure)
            => ConfigureRuntimeInternal(configure) as ServerBuilder;


        public new EitherAsync<RfcError, IRfcServer<SAPRfcRuntime>> Build()
        {
            var runtime = CreateRuntime(new CancellationTokenSource(), SAPRfcRuntime.New);
            return base.Build().ToEither(runtime);
        }
    }


    public class ServerBuilder<RT, TSettings> : ServerBuilderBase<ServerBuilder<RT, TSettings>, RT, TSettings>
        where RT : struct, HasSAPRfcServer<RT>,
        HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>,
        HasEnvSettings<TSettings>, HasCancelFactory<RT>
        where TSettings : SAPRfcRuntimeSettings
    {
        public ServerBuilder(IDictionary<string, string> serverParam) : base(serverParam)
        {
        }
    }
}