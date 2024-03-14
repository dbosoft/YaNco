﻿using System;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;
// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006


public static class SAPRfc<RT>
    where RT : struct, HasCancel<RT>

{

    public static Aff<RT, TR> useConnection<TR>(Aff<RT, IConnection> connectionEffect, Func<IConnection, Aff<RT, TR>> mapFunc )
    {
        return from connection in connectionEffect
            from res in Prelude.use(connection, mapFunc)
            select res;
    }

    /// <summary>
    /// Creates a RFC function <see cref="IFunction"/> from function name.
    /// </summary>
    /// <param name="functionName">Name of the function as defined in SAP.</param>
    /// <returns>A <see cref="Aff{RT,IFunction}"/> with any error as left state and function as right state.</returns>
    public static Aff<RT, IFunction> createFunction(IConnection connection, string functionName)
    {
        return from function in connection.CreateFunction(functionName).ToAff(l=>l)
        select function;
    }

    /// <summary>
    /// Calls the function specified in parameter <param name="function">function</param>
    /// </summary>
    /// <param name="function">The function to be invoked.</param>
    /// <returns>A <see cref="Aff{RT,Unit}"/> with any error as left state and <seealso cref="Unit"/> as right state.</returns>
    public static Aff<RT, Unit> invokeFunction(IConnection connection, IFunction function)
    {
        return from ct in Prelude.cancelToken<RT>()
            from _ in connection.InvokeFunction(function, ct).ToAff(l => l)
            select Unit.Default;
    }


    /// <summary>
    /// This method calls a SAP RFM with input and output.
    /// </summary>
    /// <remarks>
    /// The input parameter of this method is a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The input type <typeparam name="TInput"></typeparam> itself is not used any more
    /// after calling the input mapping.
    /// The output parameter of this method is also a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The output type <typeparam name="TResult"></typeparam> is returned after processing the ABAP function.
    ///
    /// </remarks>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="functionName">name of the function as defined in SAP backend</param>
    /// <param name="Input">Input function lifted in either monad.</param>
    /// <param name="Output">Output function lifted in either monad.</param>
    /// <returns>Result of output mapping function.</returns>
    public static Aff<RT, TResult> callFunction<TInput, TResult>(
        IConnection connection,
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output) =>
        from result in createFunction(connection, functionName).Use(
            func =>
                from input in Input(Prelude.Right(func)).ToAff(l => l)
                from _ in invokeFunction(connection,func)
                from output in Output(Prelude.Right(func)).ToAff(l => l)
                select output)
        select result;

    /// <summary>
    /// This method calls a SAP RFM without input.
    /// </summary>
    /// <remarks>
    /// The output parameter of this method is a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The output type <typeparam name="TResult"></typeparam> is returned after processing the ABAP function.
    ///
    /// </remarks>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="functionName">ABAP function name</param>
    /// <param name="Output">Output function lifted in either monad.</param>
    /// <returns>Result of output mapping function.</returns>
    public static Aff<RT, TResult> callFunction<TResult>(
        IConnection connection,
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output) =>
        from result in createFunction(connection,functionName).Use(
            func =>
                from _ in invokeFunction(connection,func)
                from output in Output(Prelude.Right(func)).ToAff(l => l)
                select output)
        select result;

    /// <summary>
    /// This method calls a SAP RFM without input and output.
    /// </summary>
    /// <param name="functionName">ABAP function name</param>
    /// <returns>Unit</returns>
    public static Aff<RT,Unit> invokeFunction(
        IConnection connection,
        string functionName)
    {
        return from result in createFunction(connection,functionName).Use(
                func =>
                    from _ in invokeFunction(connection,func)
                    select Unit.Default)
            select result;

    }

    /// <summary>
    /// This method calls a SAP RFM without output.
    /// </summary>
    /// <remarks>
    /// The input parameter of this method is a function that maps from a <see cref="Either{RfcError,IFunction}"/>
    /// to any kind of type. The input type <typeparam name="TInput"></typeparam> itself is not used any more
    /// after calling the input mapping.
    /// </remarks>
    /// <param name="functionName">ABAP function name</param>
    /// <param name="Input">Input function lifted in either monad.</param>
    /// <returns>Unit</returns>
    public static Aff<RT, Unit> invokeFunction<TInput>(
        IConnection connection,
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input) =>
        from result in createFunction(connection,functionName).Use(
            func =>
                from input in Input(Prelude.Right(func)).ToAff(l => l)
                from _ in invokeFunction(connection,func)
                select Unit.Default)
        select result;

    /// <summary>
    /// Checks if connection to SAP backend could be established.
    /// </summary>
    /// <returns>A <see cref="Aff{RT,Unit}"/> with any error as left state and <seealso cref="Unit"/> as right state</returns>
    public static Aff<RT, Unit> ping(IConnection connection)
    {
        return 
            from result in createFunction(connection,"RFC_PING").Use(
            func =>
                from _ in invokeFunction(connection, func)
                select Unit.Default)
            select Unit.Default;

    }


    /// <summary>
    /// Commits current SAP transaction in backend without waiting. 
    /// </summary>
    /// <returns>A <see cref="Aff{RT,Unit}"/> with any error as left state and <seealso cref="Unit"/> as right state.</returns>
    public static Aff<RT, Unit> commit(IConnection connection)
    {
        return from ct in Prelude.cancelToken<RT>()
            from _ in connection.Commit(ct).ToAff(l => l)
            select Unit.Default;
    }

    /// <summary>
    /// Commits current SAP transaction in backend with waiting for posting to be completed. 
    /// </summary>
    /// <returns>A <see cref="Aff{RT,Unit}"/> with any error as left state and <seealso cref="Unit"/> as right state.</returns>
    public static Aff<RT, Unit> commitAndWait(IConnection connection)
    {
        return from ct in Prelude.cancelToken<RT>()
            from _ in connection.CommitAndWait(ct).ToAff(l => l)
            select Unit.Default;

    }

    /// <summary>
    /// Rollback of current SAP transaction in backend. 
    /// </summary>
    /// <returns>A <see cref="EitherAsync{RfcError,Unit}"/> with any rfc error as left state and <seealso cref="Unit"/> as right state.</returns>
    public static Aff<RT, Unit> rollback(IConnection connection)
    {
        return from ct in Prelude.cancelToken<RT>()
            from _ in connection.Rollback(ct).ToAff(l => l)
            select Unit.Default;

    }

}