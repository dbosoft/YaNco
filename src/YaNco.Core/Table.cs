using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    internal class Table : TypeDescriptionDataContainer, ITable
    {
        private readonly ITableHandle _handle;

        public Table(ITableHandle handle, SAPRfcDataIO io) : base(handle, io)
        {
            _handle = handle;
        }
        
        public IEnumerable<IStructure> Rows
        {
            get
            {
                var handle = IO.Options.CloneTableForRowEnumerator 
                    ? IO.CloneTable(_handle) 
                    : _handle.Apply(h => Prelude.Right(h).Bind<RfcError>());

                return handle
                    .Map(clonedHandle => new TableRowEnumerator(IO, Prelude.Some(clonedHandle)))
                    .Match(
                        r => new EnumeratorAdapter<Structure>(r),
                        l => new EnumeratorAdapter<Structure>(new TableRowEnumerator(IO, Prelude.None)));
            }
        }

        public Either<RfcError, IStructure> AppendRow()
        {
            return IO.AppendTableRow(_handle).Map(sh => (IStructure) new Structure(sh, IO));
        }

    }
}