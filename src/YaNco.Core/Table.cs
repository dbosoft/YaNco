﻿using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    internal class Table : TypeDescriptionDataContainer, ITable
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
                var handle = _rfcRuntime.Options.CloneTableForRowEnumerator 
                    ? _rfcRuntime.CloneTable(_handle) 
                    : _handle.Apply(h => Prelude.Right(h).Bind<RfcError>());

                return handle
                    .Map(clonedHandle => new TableRowEnumerator(_rfcRuntime, Prelude.Some(clonedHandle)))
                    .Match(
                        r => new EnumeratorAdapter<Structure>(r),
                        l => new EnumeratorAdapter<Structure>(new TableRowEnumerator(_rfcRuntime, Prelude.None)));
            }
        }

        public Either<RfcError, IStructure> AppendRow()
        {
            return _rfcRuntime.AppendTableRow(_handle).Map(sh => (IStructure) new Structure(sh, _rfcRuntime));
        }

    }
}