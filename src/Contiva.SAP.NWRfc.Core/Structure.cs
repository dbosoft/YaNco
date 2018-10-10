namespace Contiva.SAP.NWRfc
{
    internal class Structure : DataContainer, IStructure
    {
        public Structure(IDataContainerHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
        }
    }
}