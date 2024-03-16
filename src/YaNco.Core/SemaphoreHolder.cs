using System;
using System.Threading;

namespace Dbosoft.YaNco;

internal class SemaphoreHolder : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public SemaphoreHolder(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }


    public void Dispose()
    {
        _semaphore.Release();
    }
}