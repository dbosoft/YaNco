using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.SAP.NWRfc
{
    public interface ITable : IDataContainer
    {
        IEnumerable<IStructure> Rows { get; }
        Either<RfcErrorInfo, IStructure> AppendRow();
    }
}