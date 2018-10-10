namespace Contiva.SAP.NWRfc
{
    public class Structure : DataContainer, IStructure
    {
        public Structure(IDataContainerHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
        }
    }
}