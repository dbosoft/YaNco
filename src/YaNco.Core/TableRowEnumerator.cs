using System.Collections;
using System.Collections.Generic;
using Dbosoft.YaNco.Traits;
using LanguageExt;

namespace Dbosoft.YaNco;

internal class TableRowEnumerator : IEnumerator<Structure>
{
    private Option<ITableHandle> _handle;
    private readonly SAPRfcDataIO _io;
    private Option<Structure> _currentRow;
    private bool _first = true;

    public TableRowEnumerator(SAPRfcDataIO io, Option<ITableHandle> handle)
    {
        _io = io;
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

        var hasNext = _handle.Map(h => _io.MoveToNextTableRow(h)).Traverse(l => l)
            .Match(_ => true, _ => false);

        if(hasNext)
            ReadCurrentRow();

        return hasNext;
    }

    public Unit ReadCurrentRow()
    {
        _currentRow= _handle
            .Map(_io.GetCurrentTableRow).Traverse(l => l)
            .Map(sho => sho.Map(sh => new Structure(sh, _io)))
            .Match(r => r, _ => Prelude.None);

        return Unit.Default;
    }

    public void Reset()
    {
        _first = true;

        _handle.IfSome(h =>
        {
            _ = _io.MoveToFirstTableRow(h).Map(_ => ReadCurrentRow());
        });
    }

    public Structure Current
    {
        get { return _currentRow.MatchUnsafe(s => s, () => null); }
    }

    object IEnumerator.Current => Current;
}