using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

[PublicAPI]
public interface IRfcContext<RT> : IDisposable where RT : struct, HasCancel<RT>
{
    /// <summary>
    /// Creates a RFC function <see cref="IFunction"/> from function name.
    /// </summary>
    /// <param name="name">Name of the function as defined in SAP.</param>
    /// <returns>A async effect <see cref="Aff{RT,IFunction}"/> with <see cref="IFunction"/> as success result.</returns>
    Aff<RT,IFunction> CreateFunction(string name);


    /// <summary>
    /// Calls the function specified in parameter <param name="function">function</param>
    /// </summary>
    /// <param name="function">The function to be invoked.</param>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> with <see cref="Unit"/> as success result.</returns>
    Aff<RT, Unit> InvokeFunction(IFunction function);

    /// <summary>
    /// Checks if connection to SAP backend could be established.
    /// </summary>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> with <see cref="Unit"/> as success result.</returns>
    Aff<RT, Unit> Ping();

    /// <summary>
    /// Commits current SAP transaction in backend without waiting. 
    /// </summary>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> with <see cref="Unit"/> as success result.</returns>
    Aff<RT, Unit> Commit();

    /// <summary>
    /// Commits current SAP transaction in backend with waiting for posting to be completed. 
    /// </summary>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> with <see cref="Unit"/> as success result.</returns>
    Aff<RT, Unit> CommitAndWait();

    /// <summary>
    /// Rollback of current SAP transaction in backend. 
    /// </summary>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> with <see cref="Unit"/> as success result.</returns>
    Aff<RT, Unit> Rollback();

    /// <summary>
    /// Explicit request to open a connection to the SAP backend.
    /// </summary>
    /// <remarks>
    /// Normally it is not necessary to access the connection directly.
    /// However you can access the connection directly to perform low level operations on the connection.
    /// </remarks>
    /// <returns>A async effect <see cref="Aff{RT,IConnection}"/> with <see cref="IConnection"/> as success result.</returns>
    Aff<RT,IConnection> GetConnection();

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Calls a SAP RFM as async effect with input and output 
    /// </summary>
    /// <remarks>
    /// The input parameter of this method is a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The input type <typeparam name="TInput"></typeparam> itself is not used any more
    /// after calling the input mapping.
    /// The output parameter of this method is also a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The output type <typeparam name="TResult"></typeparam> is returned after processing the ABAP function.
    ///
    /// You should use the methods defined on <see cref="IDataContainer"/> within the mapping functions to map from .NET
    /// types to SAP function fields and back from SAP function fields to .NET. 
    /// </remarks>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="functionName">name of the function as defined in SAP backend</param>
    /// <param name="Input">Input function lifted in either monad.</param>
    /// <param name="Output">Output function lifted in either monad.</param>
    /// <returns>Result of output mapping function as a async effect <see cref="Aff{RT,A}"/> </returns>
    Aff<RT, TResult> CallFunction<TInput, TResult>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output);

    /// <summary>
    /// Calls a SAP RFM as async effect with output and no input.
    /// </summary>
    /// <remarks>
    /// The output parameter of this method is also a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The output type <typeparam name="TResult"></typeparam> is returned after processing the ABAP function.
    ///
    /// You should use the methods defined on <see cref="IDataContainer"/> within the mapping functions to map from .NET
    /// types to SAP function fields and back from SAP function fields to .NET. 
    /// </remarks>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="functionName">name of the function as defined in SAP backend</param>
    /// <param name="Output">Output function lifted in either monad.</param>
    /// <returns>Result of output mapping function as a async effect <see cref="Aff{RT,A}"/> </returns>
    Aff<RT, TResult> CallFunction<TResult>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output);

    /// <summary>
    /// Calls a SAP RFM as async effect with no output and no input.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="functionName">name of the function as defined in SAP backend</param>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> </returns>

    Aff<RT, Unit> InvokeFunction(
        string functionName);

    /// <summary>
    /// Calls a SAP RFM as async effect with input and no output.
    /// </summary>
    /// <remarks>
    /// The input parameter of this method is a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The input type <typeparam name="TInput"></typeparam> itself is not used any more
    /// after calling the input mapping.
    /// You should use the methods defined on <see cref="IDataContainer"/> within the mapping functions to map from .NET
    /// types to SAP function fields and back from SAP function fields to .NET. 
    /// </remarks>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="functionName">name of the function as defined in SAP backend</param>
    /// <param name="Input">Input function lifted in either monad.</param>
    /// <returns>A async effect <see cref="Aff{RT,Unit}"/> </returns>
    Aff<RT, Unit> InvokeFunction<TInput>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input);

    // ReSharper restore InconsistentNaming

}

[PublicAPI]
public interface IRfcContext : IDisposable
{
    /// <summary>
    /// Creates a RFC function <see cref="IFunction"/> from function name.
    /// </summary>
    /// <param name="name">Name of the function as defined in SAP.</param>
    /// <returns>A <see cref="EitherAsync{RfcError,IFunction}"/> with any rfc error as left state and function as right state.</returns>
    EitherAsync<RfcError, IFunction> CreateFunction(string name);

    /// <summary>
    /// Creates a RFC function <see cref="IFunction"/> from function name.
    /// </summary>
    /// <param name="name">Name of the function as defined in SAP.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="EitherAsync{RfcError,IFunction}"/> with any rfc error as left state and function as right state.</returns>
    EitherAsync<RfcError, IFunction> CreateFunction(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Calls the function specified in parameter <param name="function">function</param>
    /// </summary>
    /// <param name="function">The function to be invoked.</param>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> InvokeFunction(IFunction function);

    /// <summary>
    /// Calls the function specified in parameter <param name="function"></param>
    /// </summary>
    /// <param name="function">The function to be invoked.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if connection to SAP backend could be established.
    /// </summary>
    /// <returns>A <see cref="EitherAsync{RfcError,IRfcContext}"/> with any rfc error as left state and <seealso cref="IRfcContext"/> as right state for chaining.</returns>
    EitherAsync<RfcError, IRfcContext> Ping();

    /// <summary>
    /// Checks if connection to SAP backend could be established.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="EitherAsync{RfcError,IRfcContext}"/> with any rfc error as left state and <seealso cref="IRfcContext"/> as right state for chaining.</returns>
    EitherAsync<RfcError, IRfcContext> Ping(CancellationToken cancellationToken);

    /// <summary>
    /// Commits current SAP transaction in backend without waiting. 
    /// </summary>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> Commit();

    /// <summary>
    /// Commits current SAP transaction in backend without waiting. 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> Commit(CancellationToken cancellationToken);

    /// <summary>
    /// Commits current SAP transaction in backend with waiting for posting to be completed. 
    /// </summary>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> CommitAndWait();

    /// <summary>
    /// Commits current SAP transaction in backend with waiting for posting to be completed. 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> CommitAndWait(CancellationToken cancellationToken);

    /// <summary>
    /// Rollback of current SAP transaction in backend. 
    /// </summary>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> Rollback();

    /// <summary>
    /// Rollback of current SAP transaction in backend. 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    EitherAsync<RfcError, Unit> Rollback(CancellationToken cancellationToken);

    /// <summary>
    /// Async pattern compatible overload of <see cref="Ping()"/>
    /// </summary>
    /// <returns></returns>
    Task<Either<RfcError, IRfcContext>> PingAsync();

    /// <summary>
    /// Async pattern compatible overload of <see cref="Ping(CancellationToken)"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Either<RfcError, IRfcContext>> PingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Async pattern compatible overload of <see cref="Commit()"/>
    /// </summary>
    /// <returns></returns>
    Task<Either<RfcError, Unit>> CommitAsync();

    /// <summary>
    /// Async pattern compatible overload of <see cref="Commit(CancellationToken)"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Either<RfcError, Unit>> CommitAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Async pattern compatible overload of <see cref="CommitAndWait()"/>
    /// </summary>
    /// <returns></returns>
    Task<Either<RfcError, Unit>> CommitAndWaitAsync();

    /// <summary>
    /// Async pattern compatible overload of <see cref="CommitAndWait(CancellationToken)"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Either<RfcError, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Async pattern compatible overload of <see cref="Rollback()"/>
    /// </summary>
    /// <returns></returns>
    Task<Either<RfcError, Unit>> RollbackAsync();

    /// <summary>
    /// Async pattern compatible overload of <see cref="Rollback(CancellationToken)"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Either<RfcError, Unit>> RollbackAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Explicit request to open a connection to the SAP backend.
    /// </summary>
    /// <remarks>
    /// Normally it is not necessary to access the connection directly. However if you would like to
    /// access <seealso cref="IRfcRuntime"/> you can use this method to first open a connection and then to interact directly with the
    /// SAP backend via <see cref="IConnection.RfcRuntime"/> property of the opened connection.
    /// </remarks>
    /// <returns>A <see cref="EitherAsync{RfcError,IConnection}"/> with any rfc error as left state and <seealso cref="IConnection"/> as right state.</returns>
    EitherAsync<RfcError, IConnection> GetConnection();


}