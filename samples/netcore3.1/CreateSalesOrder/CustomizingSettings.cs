namespace CreateSalesOrder
{
    /// <summary>
    /// Settings from configuration, should be used in Bind call to salesSettings.
    /// </summary>
    internal class CustomizingSettings
    {
        public string DocumentType { get; set; }
        public string SalesOrganization { get; set; }

        public string DistributionChannel { get; set; }
        public string Division { get; set; }

    }
}