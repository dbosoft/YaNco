using System;
using System.Threading;
using LanguageExt;

namespace Dbosoft.YaNco
{
    /// <summary>
    /// Interface for a SAP Connection
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Commits current SAP transaction and waits for posting.
        /// </summary>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait();

        /// <summary>
        /// Commits current SAP transaction and waits for posting.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> for cancellation</param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken);

        /// <summary>
        /// Commits current SAP transaction without waiting for posting.
        /// </summary>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> Commit();

        /// <summary>
        /// Commits current SAP transaction without waiting for posting.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> for cancellation</param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken);

        /// <summary>
        /// Rollback of current SAP transaction.
        /// </summary>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> Rollback();

        /// <summary>
        /// Rollback of current SAP transaction.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> for cancellation</param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken);

        /// <summary>
        /// Creates a structure from a SAP dictionary type name. The structure has to be disposed after using it. 
        /// </summary>
        /// <remarks>It is not checked if the type name is a structure type.</remarks>
        /// <param name="name">SAP structure type name</param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,IStructure}"/> with any rfc error as left state and created structure as right state.</returns>
        EitherAsync<RfcErrorInfo, IStructure> CreateStructure(string name);

        /// <summary>
        /// Creates a callable function from a SAP function name. The function has to be disposed after using it. 
        /// </summary>
        /// <param name="name">SAP function name</param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,IFunction}"/> with any rfc error as left state and created function as right state.</returns>

        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name);

        /// <summary>
        /// Calls a function (sends the input to SAP backend and sets output in <see cref="IFunction"/>).
        /// </summary>
        /// <param name="function">SAP function interface</param>
        /// <remarks>
        /// The function input parameters have to be set before calling this method. 
        /// When the SAP backend has processed the call, function parameters will contain the data set by the SAP backend. 
        /// </remarks>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function);


        /// <summary>
        /// Calls a function (sends the input to SAP backend and sets output in <see cref="IFunction"/>).
        /// </summary>
        /// <param name="function">SAP function interface</param>
        /// <remarks>
        /// The function input parameters have to be set before calling this method. 
        /// When the SAP backend has processed the call, function parameters will contain the data set by the SAP backend. 
        /// </remarks>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> for cancellation</param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken);

        [Obsolete("Use method WithStartProgramCallback of ConnectionBuilder instead. This method will be removed in next major release.")]
        EitherAsync<RfcErrorInfo, Unit> AllowStartOfPrograms(StartProgramDelegate callback);

        /// <summary>
        /// Cancels any running backend communication.
        /// </summary>
        /// <remarks>
        /// This method cancels any running communication with the SAP backend. The connection will be in a invalid state after cancellation and
        /// has to be recreated for further calls. 
        /// </remarks>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,Unit}"/> with any rfc error as left state and Unit as right state.</returns>
        EitherAsync<RfcErrorInfo, Unit> Cancel();


        /// <summary>
        /// Gets connection attributes. This call will open the connection.
        /// </summary>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,ConnectionAttributes}"/> with any rfc error as left state and the attributes as right state.</returns>
        EitherAsync<RfcErrorInfo, ConnectionAttributes> GetAttributes();


        /// <summary>
        /// Flag if connection is already disposed. 
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Runtime used for the connection
        /// </summary>
        IRfcRuntime RfcRuntime { get;  }

    }
}