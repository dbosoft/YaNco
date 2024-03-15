using System;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class RfcServerClientConfigurer<RT, TSettings> where RT : struct, 
        HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, 
        HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasEnvSettings<TSettings>, HasCancelFactory<RT>, HasSAPRfcServer<RT>
        where TSettings : SAPRfcRuntimeSettings
    {
        private readonly ConnectionBuilder<RT, TSettings> _builder;

        public RfcServerClientConfigurer(ConnectionBuilder<RT, TSettings> builder)
        {
            _builder = builder;

        }

        /// <summary>
        /// This method registers a function handler from a SAP function name. 
        /// </summary>
        /// <param name="functionName">Name of function</param>
        /// <param name="calledFunc">function handler</param>
        /// <returns>current instance for chaining</returns>
        /// <remarks>
        /// The metadata of the function is retrieved from the backend. Therefore the function
        /// must exists on the SAP backend system.
        /// Function handlers are registered process wide (in the SAP NW RFC Library).and mapped to backend system id. 
        /// Multiple registrations of same function and same backend id will therefore have no effect.
        /// </remarks>
        public RfcServerClientConfigurer<RT, TSettings> WithFunctionHandler(string functionName,
            Func<CalledFunction<RT, TSettings>, Aff<RT, Unit>> calledFunc)
        {
            _builder.WithFunctionHandler(functionName, calledFunc);
            return this;
        }

    }
}