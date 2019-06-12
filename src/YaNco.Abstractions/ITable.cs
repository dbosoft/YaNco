using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface ITable : IDataContainer
    {
        IEnumerable<IStructure> Rows { get; }
        Either<RfcErrorInfo, IStructure> AppendRow();
    }
}