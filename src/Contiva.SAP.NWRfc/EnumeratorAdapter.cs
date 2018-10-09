using System.Collections;
using System.Collections.Generic;

namespace Contiva.SAP.NWRfc
{
    public class EnumeratorAdapter<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;
        public EnumeratorAdapter(IEnumerator<T> e)
        {
            _enumerator = e;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }
        // Rest omitted 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}