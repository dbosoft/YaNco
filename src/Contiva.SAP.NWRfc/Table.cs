using System.Collections.Generic;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public class Table : DataContainer{
        private readonly TableHandle _handle;
        private readonly IRfcRuntime _rfcRuntime;

        public Table(TableHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
            _handle = handle;
            _rfcRuntime = rfcRuntime;
        }
        
        public IEnumerable<Structure> Rows
        {
            get
            {
                return _rfcRuntime.CloneTable(_handle)
                    .Map(clonedHandle => new TableRowEnumerator(_rfcRuntime, clonedHandle))
                    .Match(
                        r => new EnumeratorAdapter<Structure>(r),
                        l => new EnumeratorAdapter<Structure>(new TableRowEnumerator(_rfcRuntime, Prelude.None)));
            }
        }

        public Either<RfcErrorInfo, Structure> AppendRow()
        {
            return _rfcRuntime.AppendTableRow(_handle).Map(sh => new Structure(sh, _rfcRuntime));
        }

    }
}