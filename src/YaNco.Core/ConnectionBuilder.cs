using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class ConnectionBuilder : ConnectionBuilderBase<ConnectionBuilder, SAPRfcRuntime, SAPRfcRuntimeSettings>
    {
        public ConnectionBuilder(IDictionary<string, string> connectionParam) : base(connectionParam)
        {
        }

        public new Func<EitherAsync<RfcError, IConnection>> Build()
        {
            var baseEffect = base.Build();

            return () =>
            {
                var runtime = CreateRuntime(new CancellationTokenSource(), SAPRfcRuntime.New);
                return baseEffect.ToEither(runtime);
            };
        }

        /// <summary>
        /// Registers a action to configure the <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure">action with <see cref="RfcRuntimeConfigurer"/></param>
        /// <returns>current instance for chaining.</returns>
        /// <remarks>
        /// Multiple calls of this method will override the previous configuration action. 
        /// </remarks>
        public ConnectionBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer<SAPRfcRuntime, SAPRfcRuntimeSettings>> configure)
            => ConfigureRuntimeInternal(configure) as ConnectionBuilder;

    }


    /// <summary>
    /// This class is used to build client connections to a SAP ABAP backend.  
    /// </summary>
    public class ConnectionBuilder<RT, TSettings> : ConnectionBuilderBase<ConnectionBuilder<RT, TSettings>, RT, TSettings>
        where TSettings : SAPRfcRuntimeSettings where RT : struct,
        HasSAPRfcFunctions<RT>,
        HasCancelFactory<RT>,
        HasSAPRfcServer<RT>,
        HasSAPRfcConnection<RT>,
        HasSAPRfcLogger<RT>,
        HasSAPRfcData<RT>,
        HasEnvSettings<TSettings>
    {
        public ConnectionBuilder(IDictionary<string, string> connectionParam) : base(connectionParam)
        {
        }
    }
}
