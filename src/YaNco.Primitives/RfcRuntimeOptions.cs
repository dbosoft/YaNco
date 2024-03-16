namespace Dbosoft.YaNco;

public class RfcRuntimeOptions
{
    /// <summary>
    /// With this option you can disable the default behaviour that a table row
    /// is cloned when a iterator is created for that table.
    /// A cloned table is recommended as without it iterating over the table will cause the
    /// current row to be moved for all iterators created from same table.
    /// However if you can ensure that only one iterator is used at same time you can
    /// disable cloning to improve memory usage for large table.  
    /// </summary>
    public bool CloneTableForRowEnumerator { get; set; } = true;

}