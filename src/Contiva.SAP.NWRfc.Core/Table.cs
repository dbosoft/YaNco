using System.Collections.Generic;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    internal class Table : DataContainer, ITable
    {
        private readonly ITableHandle _handle;
        private readonly IRfcRuntime _rfcRuntime;

        public Table(ITableHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
            _handle = handle;
            _rfcRuntime = rfcRuntime;
        }
        
        public IEnumerable<IStructure> Rows
        {
            get
            {
                return _rfcRuntime.CloneTable(_handle)
                    .Map(clonedHandle => new TableRowEnumerator(_rfcRuntime, Prelude.Some(clonedHandle)))
                    .Match(
                        r => new EnumeratorAdapter<Structure>(r),
                        l => new EnumeratorAdapter<Structure>(new TableRowEnumerator(_rfcRuntime, Prelude.None)));
            }
        }

        public Either<RfcErrorInfo, IStructure> AppendRow()
        {
            return _rfcRuntime.AppendTableRow(_handle).Map(sh => (IStructure) new Structure(sh, _rfcRuntime));
        }

    }
}