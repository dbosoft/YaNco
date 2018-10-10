using System.Collections.Generic;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public interface ITable : IDataContainer
    {
        IEnumerable<IStructure> Rows { get; }
        Either<RfcErrorInfo, IStructure> AppendRow();
    }
}