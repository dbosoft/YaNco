using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.Functional;
using Dbosoft.YaNco;
using LanguageExt;

namespace SAPSystemTests;

/// <summary>
/// A pool of RFC agents that limits the number of concurrent SAP RFC connections.
/// Each agent holds its own <see cref="RfcContext"/> and processes calls sequentially
/// via an internal queue. Incoming requests are distributed round-robin across agents.
/// Broken connections are automatically replaced on the next request.
/// An optional <c>maxPending</c> parameter limits the number of queued requests
/// to provide backpressure to callers.
/// </summary>
public class SapAgentPool : IDisposable
{
    private readonly IAgent<Func<AgentState, Task>>[] _agents;
    private readonly AgentState[] _states;
    private readonly SemaphoreSlim _throttle;
    private int _roundRobin;
    private int _pending;

    public SapAgentPool(Func<EitherAsync<RfcError, IConnection>> connFunc,
        int poolSize = 3, int maxPending = 0)
    {
        _throttle = maxPending > 0 ? new SemaphoreSlim(maxPending) : null;
        _states = new AgentState[poolSize];
        _agents = Enumerable.Range(0, poolSize)
            .Select(i =>
            {
                var state = new AgentState(connFunc);
                _states[i] = state;
                return Agent.Start<AgentState, Func<AgentState, Task>>(
                    initialState: state,
                    process: async (s, fn) =>
                    {
                        await fn(s);
                        return s;
                    });
            })
            .ToArray();
    }

    public async Task<Either<RfcError, T>> Execute<T>(
        Func<RfcContext, Task<Either<RfcError, T>>> fn)
    {
        if (_throttle != null && !await _throttle.WaitAsync(TimeSpan.FromSeconds(30)))
            return new RfcError(new RfcErrorInfo(
                RfcRc.RFC_EXTERNAL_FAILURE,
                RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                "", "Agent pool saturated", "", "E", "", "", "", "", ""));

        var tcs = new TaskCompletionSource<Either<RfcError, T>>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        Interlocked.Increment(ref _pending);
        var idx = (Interlocked.Increment(ref _roundRobin) & 0x7FFFFFFF) % _agents.Length;

        _agents[idx].Tell(async state =>
        {
            try
            {
                var result = await fn(state.Context);
                var needsRecreate = result.Match(_ => false, IsConnectionError);
                if (needsRecreate)
                {
                    state.RecreateContext();
                }
                else
                {
                    // Reset the server context before returning the connection to the
                    // pool, so the next caller starts from a clean server-side state.
                    var resetResult = await state.Context.GetConnection()
                        .Bind(c => c.ResetServerContext()).ToEither();
                    resetResult.IfLeft(err =>
                    {
                        if (IsConnectionError(err))
                            state.RecreateContext();
                    });
                }
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                state.RecreateContext();
                tcs.SetResult(new RfcError(new RfcErrorInfo(
                    RfcRc.RFC_EXTERNAL_FAILURE,
                    RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                    "", ex.Message, "", "E", "", "", "", "", "")));
            }
            finally
            {
                Interlocked.Decrement(ref _pending);
                _throttle?.Release();
            }
        });

        return await tcs.Task;
    }

    private static bool IsConnectionError(RfcError err)
    {
        return err.Rc is RfcRc.RFC_COMMUNICATION_FAILURE
            or RfcRc.RFC_CLOSED
            or RfcRc.RFC_TIMEOUT
            or RfcRc.RFC_INVALID_HANDLE;
    }

    public void Dispose()
    {
        var sw = Stopwatch.StartNew();
        while (Volatile.Read(ref _pending) > 0 && sw.ElapsedMilliseconds < 5000)
            Thread.Sleep(50);

        foreach (var state in _states)
            state.Dispose();

        _throttle?.Dispose();
    }

    public class AgentState : IDisposable
    {
        private readonly Func<EitherAsync<RfcError, IConnection>> _connFunc;
        public RfcContext Context { get; private set; }

        public AgentState(Func<EitherAsync<RfcError, IConnection>> connFunc)
        {
            _connFunc = connFunc;
            Context = new RfcContext(connFunc);
        }

        public void RecreateContext()
        {
            Context.Dispose();
            Context = new RfcContext(_connFunc);
        }

        public void Dispose() => Context.Dispose();
    }
}