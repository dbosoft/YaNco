using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco.Live;

public class SAPRfcEnv
{
    public static SAPRfcEnv New(Option<ILogger> logger, IFieldMapper fieldMapper, SAPRfcDataIO dataIO) =>
        new(logger, fieldMapper, dataIO);

    public Option<ILogger> Logger { get; }
    public IFieldMapper FieldMapper { get; }
    public SAPRfcDataIO DataIO { get; }

    private SAPRfcEnv(Option<ILogger> logger, IFieldMapper fieldMapper, SAPRfcDataIO dataIO)
    {
        Logger = logger;
        FieldMapper = fieldMapper;
        DataIO = dataIO;
    }

}