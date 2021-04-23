using System.Collections;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    internal class TableRowEnumerator : IEnumerator<Structure>
    {
        private Option<ITableHandle> _handle;
        private readonly IRfcRuntime _rfcRuntime;
        private Option<Structure> _currentRow;
        private bool _first = true;

        public TableRowEnumerator(IRfcRuntime rfcRuntime, Option<ITableHandle> handle)
        {
            _rfcRuntime = rfcRuntime;
            _handle = handle;
            Reset();
        }

        public void Dispose()
        {
            _handle.IfSome(s => s.Dispose());
            _handle = Prelude.None;
        }

        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return _currentRow.IsSome;
            }

            var hasNext = _handle.Map(h => _rfcRuntime.MoveToNextTableRow(h)).Traverse(l => l)
                .Match(r => true, l => false);

            if(hasNext)
                ReadCurrentRow();

            return hasNext;
        }

        public Unit ReadCurrentRow()
        {
            _currentRow= _handle
                .Map(_rfcRuntime.GetCurrentTableRow).Traverse(l => l)
                .Map(sho => sho.Map(sh => new Structure(sh, _rfcRuntime)))
                .Match(r => r, l => Prelude.None);

            return Unit.Default;
        }

        public void Reset()
        {
            _first = true;

            _handle.IfSome(h =>
            {
                _rfcRuntime.MoveToFirstTableRow(h).Map(u => ReadCurrentRow());
            });
        }

        public Structure Current
        {
            get { return _currentRow.MatchUnsafe(s => s, () => null); }
        }

        object IEnumerator.Current => Current;
    }
}