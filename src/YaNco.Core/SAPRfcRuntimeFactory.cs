using System.Threading;

namespace Dbosoft.YaNco;

// ReSharper disable once InconsistentNaming
public class RuntimeReference<RT> where RT: struct
{
    public RT Runtime { get; }

    private RuntimeReference(RT runtime)
    {
        Runtime = runtime;
    }

    public static RuntimeReference<RT> New (RT runtime)
    {
       return new RuntimeReference<RT>(runtime);
    }
}