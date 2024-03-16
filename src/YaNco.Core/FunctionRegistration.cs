using System;
using System.Linq;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

/// <summary>
/// The global function registration that holds all function registrations of the process
/// </summary>
[PublicAPI]
public class FunctionRegistration : IFunctionRegistration
{
    public static FunctionRegistration Instance = new();
    private HashMap<string, RegistrationRecord> _registrations;

    public bool IsFunctionRegistered(string sysId, string functionName)
    {
        var key = $"{sysId}_{functionName}";

        return _registrations.ContainsKey(key);
    }

    public void Add(string sysId, string functionName, IDisposable holder)
    {
        var key = $"{sysId}_{functionName}";
        _registrations = _registrations.Add(key, new RegistrationRecord(sysId, functionName, holder));
    }

    public void RemoveForSystem(string sysId)
    {
        var sysReg = _registrations.Filter(x => x.Value.SysId == sysId)
            .Select(x=>x.Key)
            .ToArray();

        foreach (var reg in sysReg)
        {
            _registrations[reg].Holder.Dispose();
        }

        _registrations = _registrations.RemoveRange(sysReg);

    }


    public void Remove(string sysId, string functionName)
    {
        var key = $"{sysId}_{functionName}";

        if (!_registrations.ContainsKey(key))
            return;

        _registrations[key].Holder.Dispose();
        _registrations = _registrations.Remove(key);

    }

    public void Dispose()
    {
        foreach (var registration in _registrations)
        {
            registration.Value.Holder.Dispose();
        }
        _registrations = HashMap<string, RegistrationRecord>.Empty;
    }

    private readonly struct RegistrationRecord
    {
        public readonly string SysId;
        public readonly string FunctionName;
        public readonly IDisposable Holder;

        public RegistrationRecord(string sysId, string functionName, IDisposable holder)
        {
            SysId = sysId;
            FunctionName = functionName;
            Holder = holder;
        }
    }
}