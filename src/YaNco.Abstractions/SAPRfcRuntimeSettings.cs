using Dbosoft.YaNco.Traits;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco;

/// <summary>
/// This class holds the runtime settings for the SAP RFC runtime
/// </summary>
/// <remarks>
/// If you would like to inject your own implementations for logger
/// and field mapper via dependency injection, you can register this class in your DI container.
/// </remarks>
public class SAPRfcRuntimeSettings
{
    public readonly IFieldMapper FieldMapper;
    public readonly ILogger Logger;
    public readonly RfcRuntimeOptions Options;

    /// <summary>
    /// Creates a new instance of the runtime settings with the given field mapper
    /// </summary>
    /// <param name="fieldMapper">the field mapper instance</param>
    public static SAPRfcRuntimeSettings New(IFieldMapper fieldMapper) 
        => new(null, fieldMapper, new RfcRuntimeOptions());

    /// <summary>
    /// Creates a new instance of the runtime settings with the given logger and field mapper
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="fieldMapper">the field mapper instance</param>
    public static SAPRfcRuntimeSettings New(ILogger logger, IFieldMapper fieldMapper) 
        => new(logger, fieldMapper, new RfcRuntimeOptions());

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <remarks>
    /// You can pass null for logger if you don't want to use logging.
    /// The default field mapper can be created by calling <see cref="T:RfcMappingConfigurer{RT}.CreateDefaultFieldMapper"/> or
    /// <see cref="T:SAPRfcRuntime.Default"/>.
    /// </remarks>
    /// <param name="logger"></param>
    /// <param name="fieldMapper"></param>
    /// <param name="options"></param>
    public SAPRfcRuntimeSettings(ILogger logger, IFieldMapper fieldMapper, RfcRuntimeOptions options)
    {
        FieldMapper = fieldMapper;
        Logger = logger;
        Options = options;
    }

    public SAPRfcDataIO RfcDataIO { get; set; }
    public SAPRfcFunctionIO RfcFunctionIO { get; set; }
    public SAPRfcConnectionIO RfcConnectionIO { get; set; }
    public SAPRfcServerIO RfcServerIO { get; set; }
}