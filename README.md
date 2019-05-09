# Dbosoft.SAP.NWRfc - .NET SAP RFC API based on SAP Netweaver RFC SDK
## Description

This library provides an alternative SAP .NET Connector based on the _SAP NetWeaver RFC Library_,


## Platforms & Prerequisites

The library requires .NET Framework 4.7.1. 
Even if the core library itself is compatible with .NET Standard 2.0, only Windows is supported as runtime environment.

The Visual Studio 2013 (VC++ 12.0) runtime library and the Visual Studio 2017 VC runtime library have be installed.  
Both libraries can be downloaded here: https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads

## Build
To build Dbosoft.SAP.NWRfc_ you need to obtain _SAP NW RFC Library 750_ from _SAP Service Marketplace_.

A prerequisite to download is having a **customer or partner account** on _SAP Service Marketplace_ and if you
are SAP employee please check [SAP Note 1037575 - Software download authorizations for SAP employees](https://launchpad.support.sap.com/#/notes/1037575).
Download both the x64 and the x86 versions and place then in the repository folder nwrfcsdk/x64 and nwrfcsdk/x86.

_SAP NW RFC Library_ is fully backwards compatible, supporting all NetWeaver systems, from today, down to release R/3 4.6C.
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
  
  
## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/dbosoft/Dbosoft.SAP.NWRfc/tags). 

## Authors

* **Frank Wagner** - *Initial work* - [fw2568](https://github.com/fw2568)

See also the list of [contributors](https://github.com/Dbosoft/SAP.NWRfc/contributors) who participated in this project.


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

