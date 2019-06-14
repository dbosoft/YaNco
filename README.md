# YaNco - Yet another .NET Connector
.NET Connector for SAP Netweaver RFC

Stable                     |  Prerelease               |  Build Status
---------------------------|---------------------------|---------------------------
[![NuGet stable](https://img.shields.io/nuget/v/Dbosoft.YaNco.svg?style=flat-square)](https://www.nuget.org/packages/Dbosoft.YaNco) | [![NuGet pre](https://img.shields.io/nuget/vpre/Dbosoft.YaNco.svg?style=flat-square)](https://www.nuget.org/packages/Dbosoft.YaNco) | ![](https://contiva.visualstudio.com/Internal/_apis/build/status/dbosoft.YaNco?branchName=master)

## Description

This library provides an alternative SAP .NET Connector based on the _SAP NetWeaver RFC Library_.

**Features**:
- .NET Standard / .NET Core Project Support
- DI container friendly API
- Functional programming friendly API (using [Language.Ext](https://github.com/louthy/language-ext))
- ABAP callbacks support (not possible with sapnco, see SAP note [2297083](https://launchpad.support.sap.com/#/notes/2297083)). 


## Platforms & Prerequisites

**.NET**

The library requires .NET Framework 4.7.1 or higher.
Even if the core library itself is compatible with .NET Standard 2.0, only Windows is supported as runtime environment. 
.NET Core will not work currently as it doesn' t support VC++ mixed mode (this may change for .NET Core 3.0).

**C++ Runtime**

The Visual Studio 2013 (VC++ 12.0) runtime library and the Visual Studio 2017 VC runtime library have to be installed.  
Both libraries can be downloaded here: https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads

**SAP Netweaver RFC SDK**

To use and build YaNco you need to obtain _SAP NW RFC Library 750_ from _SAP Service Marketplace_.

A prerequisite to download is having a **customer or partner account** on _SAP Service Marketplace_ and if you
are SAP employee please check [SAP Note 1037575 - Software download authorizations for SAP employees](https://launchpad.support.sap.com/#/notes/1037575).

_SAP NW RFC Library_ is fully backwards compatible, supporting all NetWeaver systems, from today, down to release R/3 4.6C.
You can therefore always use the newest version released on Service Marketplace and connect to older systems as well.

## Getting started

The easiest way to get started is by installing [the available NuGet package](https://www.nuget.org/packages/Dbosoft.YaNco). Take a look at the [Using](#using) section learning how to configure and use YaNco. Go to the [Build](#build) section to find out how to build YaNco. 

As explained above you have to obtain _SAP NW RFC Library 750_ from _SAP Service Marketplace_. Download the RFC SDK for the platform required for your project. You will only need the files from the lib directory - include them as solution item and copy the files to the build output directory.

You also have to change your project platform configuration to the corresponding platform (x64/x86).

## Using

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

The RfcRuntime is a low level API that you will typical never use directly. Instead you can use the connection function to open a RFCContext. 

```csharp
using (var context = new RfcContext(ConnFunc))
{
   ...

}
  ```

Use the RFCContext instance to call ABAP RFMs. 

**calling functions**

We provide a extension method on the RFCContext that supports a syntax similar to the ABAP call function command, except that it is using function callbacks to pass or retrieve data: 

- *IMPORTING* parameters are passed in the *Input* function
- *EXPORTING* parameters are retured in the *Output* function
- *CHANGING* and *TABLES* parameters can be used in both functions 

```csharp
using (var context = new RfcContext(ConnFunc))
{
    await context.CallFunction("DDIF_FIELDLABEL_GET",
            Input: f => f
                .SetField("TABNAME", "USR01")
                .SetField("FIELDNAME", "BNAME"),
            Output: f => f
                .GetField<string>("LABEL"))
    
    .ToAsync().Match(r => Console.WriteLine($"Result: {r}"), // should return: User Name
                      l => Console.WriteLine($"Error: {l.Message}"));
}
  ```
The Result of the function is a Either<L,R> type (see language.ext [Either left right monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Either_L_R.htm)). The Match call at the end either writes the result (right value) or a rfc error (left value). 

**Structures**

Structures can be set or retrieved the same way. Another example extracting company code details (change the company code if necessary if you try this example):

```csharp
using (var context = new RfcContext(ConnFunc))
{
    await context.CallFunction("BAPI_COMPANYCODE_GETDETAIL",
            Input: f => f
                .SetField("COMPANYCODEID", "1000"),
            Output: func => func.BindAsync(f=>f.GetStructure("COMPANYCODE_DETAIL"))
                        .BindAsync(s => s.GetField<string>("COMP_NAME")
                 )
                )
        .ToAsync().Match(r => Console.WriteLine($"Result: {r}"),
            l => Console.WriteLine($"Error: {l.Message}"));
}
  ```

This looks a bit complicated due to the nested BindAsync calls. Alternatively, you can also use a LINQ syntax:

```csharp
using (var context = new RfcContext(ConnFunc))
{
    await context.CallFunction("BAPI_COMPANYCODE_GETDETAIL",
        Input: f => f
            .SetField("COMPANYCODEID", "1000"),
        Output: func => func.BindAsync(f => 
            from s in f.GetStructure("COMPANYCODE_DETAIL")
            from name in s.GetField<string>("COMP_NAME")
            select name))
        .ToAsync().Match(r => Console.WriteLine($"Result: {r}"),
            l => Console.WriteLine($"Error: {l.Message}"));
}
  ```
Especially for complex structures, the LINQ syntax is often easier to read.

**Tables**

Getting table results is possible by iterating over the table rows to retrieve the table structures. Here an example to extract all company code name and descriptions:

```csharp
using (var context = new RfcContext(ConnFunc))
{
    await context.CallFunction("BAPI_COMPANYCODE_GETLIST",
        Output: func => func.BindAsync(f =>
            from companyTable in f.GetTable("COMPANYCODE_LIST")

            from row in companyTable.Rows.Map(s =>
              from code in s.GetField<string>("COMP_CODE")
              from name in s.GetField<string>("COMP_NAME")
              select (code, name)).Traverse(l => l)

        select row))
    .ToAsync().Match(
        r =>
            {
                foreach (var (code, name) in r)
                {
                    Console.WriteLine($"{code}\t{name}");
                }
            },
        l=> Console.WriteLine($"Error: {l.Message}"));
}
  ```


## Build

We use Visual Studio 2017 for building, 2019 may work but is not tested. 
Make sure that you have installed Visual Studio with VC and Platform Build Tools.

As explained above you have to obtain _SAP NW RFC Library 750_ from _SAP Service Marketplace_. 
Download both the x64 and the x86 versions and place then in the repository folder nwrfcsdk/x64 and nwrfcsdk/x86.



## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/dbosoft/YaNco/tags). 

## Authors

* **Frank Wagner** - *Initial work* - [fw2568](https://github.com/fw2568)

See also the list of [contributors](https://github.com/Dbosoft/YaNco/contributors) who participated in this project.


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

