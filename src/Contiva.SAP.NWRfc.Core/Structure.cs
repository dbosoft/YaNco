namespace Contiva.SAP.NWRfc
{
    internal class Structure : TypeDescriptionDataContainer, IStructure
    {
        public Structure(IDataContainerHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
        }
    }
}