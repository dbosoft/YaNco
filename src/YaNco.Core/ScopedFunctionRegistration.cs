using System;
using LanguageExt;

namespace Dbosoft.YaNco;

/// <summary>
/// a local scoped function registration that will dispose only it's own registrations
/// </summary>
public class ScopedFunctionRegistration : IFunctionRegistration
{
    private HashSet<Registration> _registrations;

    public void Dispose()
    {
        foreach (var (sysId, functionName) in _registrations)
        {
            FunctionRegistration.Instance.Remove(sysId, functionName);
        }

        _registrations = HashSet<Registration>.Empty;
            
    }

    public bool IsFunctionRegistered(string sysId, string functionName)
    {
        return FunctionRegistration.Instance.IsFunctionRegistered(sysId, functionName);
    }

    public void Add(string sysId, string functionName, IDisposable holder)
    {
        var reg = new Registration(sysId, functionName);

        _registrations = _registrations.Add(reg);
        FunctionRegistration.Instance.Add(sysId, functionName, holder);

    }

    private readonly struct Registration : IEquatable<Registration>
    {
        public readonly string SysId;
        public readonly string FunctionName;

        public Registration(string sysId, string functionName)
        {
            SysId = sysId;
            FunctionName = functionName;
        }

        public bool Equals(Registration other)
        {
            return SysId == other.SysId && FunctionName == other.FunctionName;
        }

        public override bool Equals(object obj)
        {
            return obj is Registration other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SysId != null ? SysId.GetHashCode() : 0) * 397) ^ (FunctionName != null ? FunctionName.GetHashCode() : 0);
            }
        }

        public void Deconstruct(out string sysId, out string functionName)
        {
            sysId = SysId;
            functionName = FunctionName;
        }
    }
}