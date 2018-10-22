# Contiva.SAP.NWRfc - .NET SAP RFC API based on SAP Netweaver RFC SDK
## Description

This library provides an alternative SAP .NET Connector based on the _SAP NetWeaver RFC Library_,


## Platforms & Prerequisites

The library requires .NET Framework 4.7.1. 
Even if the library itself is compatible with .NET Standard 2.0, only Windows is supported as runtime environment.


On Windows platforms the Microsoft Visual C++ 2005 Service Pack 1 Redistributable Package (KB973544), or [newer](https://www.microsoft.com/en-us/download/details.aspx?id=48145), must be installed, per [SAP Note 1375494 - SAP system does not start after applying SAP kernel patch](https://launchpad.support.sap.com/#/notes/1375494).

To build _Contiva.SAP.NWRfc_ you need to obtain _SAP NW RFC Library 750_ from _SAP Service Marketplace_.

A prerequisite to download is having a **customer or partner account** on _SAP Service Marketplace_ and if you
are SAP employee please check [SAP Note 1037575 - Software download authorizations for SAP employees](https://launchpad.support.sap.com/#/notes/1037575).

_SAP NW RFC Library_ is fully backwards compatible, supporting all NetWeaver systems, from today, down to release R/3 4.0.
You can therefore always use the newest version released on Service Marketplace and connect to older systems as well.

## Usage examples

In order to call remote enabled ABAP function module (ABAP RFM), first a connection must be opened.
The connection settings have to be build from a string/string dictionary, for example from a ConfigurationBuilder.

```csharp
var configurationBuilder =
    new ConfigurationBuilder();

configurationBuilder.AddUserSecrets<Program>();
var config = configurationBuilder.Build();

var settings = new Dictionary<string, string>
{
    {"ashost", config["saprfc:ashost"]},
    {"sysnr", config["saprfc:sysnr"]},
    {"client", config["saprfc:client"]},
    {"user", config["saprfc:username"]},
    {"passwd", config["saprfc:password"]},
    {"lang", "EN"}

};
```

To open the connection create a runtime instance and a connection opening function.

```csharp
var runtime = new RfcRuntime();

Task<IConnection> ConnFunc() =>
    (from c in Connection.Create(settings, runtime)
        select c).MatchAsync(c => c, error => { return null; });
```

Using the create function you can now create a RfcContext to call RFC functions.

```csharp
using (var context = new RfcContext(ConnFunc))
{
    await context.Ping();

    var resStructure = await context.CallFunction("BAPI_DOCUMENT_GETDETAIL2",
        Input: func => func
            .SetField("DOCUMENTTYPE", "AW")
            .SetField("DOCUMENTNUMBER", "AW001200")
            .SetField("DOCUMENTVERSION", "01")
            .SetField("DOCUMENTPART", "000"),
        Output: func=> func.BindAsync(f => f.GetStructure("DOCUMENTDATA"))
            .BindAsync(s =>s.GetField<string>("DOCUMENTVERSION")));

    resStructure.Match(r =>
        {

        },
        l =>
        {

        });

}
  ```
  
  



