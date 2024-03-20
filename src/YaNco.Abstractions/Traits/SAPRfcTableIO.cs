using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcTableIO
{
    RfcRuntimeOptions Options { get; }

    Either<RfcError, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name);
    Either<RfcError, ITableHandle> CloneTable(ITableHandle tableHandle);

    Either<RfcError, int> GetTableRowCount(ITableHandle tableHandle);
    Either<RfcError, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle);
    Either<RfcError, IStructureHandle> AppendTableRow(ITableHandle tableHandle);
    Either<RfcError, Unit> MoveToNextTableRow(ITableHandle tableHandle);
    Either<RfcError, Unit> MoveToFirstTableRow(ITableHandle tableHandle);

}