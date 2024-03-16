using System;

namespace Dbosoft.YaNco;

/// <summary>
/// The function registration keeps track of function handlers created and holds their references.
/// </summary>
public interface IFunctionRegistration : IDisposable
{
    bool IsFunctionRegistered(string sysId, string functionName);
    void Add(string sysId, string functionName, IDisposable holder);
        
}