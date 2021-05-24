# YaNco - Yet another .NET Connector
.NET Connector for SAP Netweaver RFC

Stable                     |  Latest                   |
---------------------------|---------------------------|
[![NuGet stable](https://img.shields.io/nuget/v/Dbosoft.YaNco.svg?style=flat-square)](https://www.nuget.org/packages/Dbosoft.YaNco) | [![NuGet pre](https://img.shields.io/nuget/vpre/Dbosoft.YaNco.svg?style=flat-square)](https://www.nuget.org/packages/Dbosoft.YaNco)

## Description

This library provides an alternative SAP .NET Connector based on the _SAP NetWeaver RFC Library_.

**Features**:
- thin, modern layer above native SAP Netweaver RFC SDK
- DI container friendly API
- Functional programming friendly API (using [Language.Ext](https://github.com/louthy/language-ext))
- ABAP callbacks support (not possible with sapnco, see SAP note [2297083](https://launchpad.support.sap.com/#/notes/2297083)). 


## Platforms & Prerequisites

**.NET**

The library requires .NET Framework >= 4.7.1 or .NET Core 2.0 or higher. 

Supported platforms: Windows, Linux and MacOS.


**Windows: C++ Runtimes**

On Windows the Visual Studio 2013 (VC++ 12.0) runtime library has to be installed. 
Library can be downloaded here: https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads


**SAP Netweaver RFC SDK**

To use and build YaNco you need to obtain _SAP NW RFC SDK 750_ from _[SAP Support Portal](https://launchpad.support.sap.com/#/softwarecenter/template/products/_APP=00200682500000001943&_EVENT=DISPHIER&HEADER=N&FUNCTIONBAR=Y&EVENT=TREE&TMPL=INTRO_SWDC_SP_AD&V=MAINT&REFERER=CATALOG-PATCHES&ROUTENAME=products/By%20Category%20-%20Additional%20Components)_.

A prerequisite to download is having a **customer or partner account** on _SAP Support Portal_ and if you
are SAP employee please check [SAP Note 1037575 - Software download authorizations for SAP employees](https://launchpad.support.sap.com/#/notes/1037575).

You can automate the download of the SAP NW RFC SDK with the dotnet tool [sapnwrfc-download](https://www.nuget.org/packages/Dbosoft.SAPNwRfc.DownloadTool).  
See https://www.dbosoft.eu/en-us/blog/sap-netweaver-rfc-sdk-download-tool for details. 

_SAP NW RFC SDK 750_ is fully backwards compatible, supporting all NetWeaver systems, from today, down to release R/3 4.6C.
You can therefore always use the newest version released on Service Marketplace and connect to older systems as well.

## Getting started

The easiest way to get started is by installing [the available NuGet package](https://www.nuget.org/packages/Dbosoft.YaNco). Take a look at the [Using](#using) section learning how to configure and use YaNco. Go to the [Build](#build) section to find out how to build YaNco. 

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
With these settings you can now create a ConnectionBuilder instance and use it to open a RfcContext.

```csharp

var connectionBuilder = new ConnectionBuilder(settings);
var connFunc = connectionBuilder.Build();

using (var context = new RfcContext(connFunc))
{
   ...

}
```
The connection builders Build() method returns a function that can be reused to open additional connections. The RfcContext will do that internally in case the connection breaks.

Under the hood the ConnectionBuilder also creates also a RfcRuntime instance. The RfcRuntime is a low level API that you will typical never use directly. 
But you can customize it on the ConnectionBuilder with the ConfigureRuntime() method. For example to add a logger:

```csharp
var connectionBuilder = new ConnectionBuilder(connFunc)
    .ConfigureRuntime(c => 
        c.WithLogger(new MyLogger()));
```


**Calling ABAP Function Modules**

We provide a extension method on the RFCContext that supports a syntax similar to the ABAP call function command, except that it is using function callbacks to pass or retrieve data: 

- *IMPORTING* parameters are passed in the *Input* function
- *EXPORTING* parameters are retured in the *Output* function
- *CHANGING* and *TABLES* parameters can be used in both functions 

```csharp
using (var context = new RfcContext(connFunc))
{
    await context.CallFunction("DDIF_FIELDLABEL_GET",
            Input: f => f
                .SetField("TABNAME", "USR01")
                .SetField("FIELDNAME", "BNAME"),
            Output: f => f
                .GetField<string>("LABEL"))

        // this is from language.ext to extract the value from a either
        .Match(r => Console.WriteLine($"Result: {r}"), // should return: User Name
            l => Console.WriteLine($"Error: {l.Message}"));
}
  ```
The Result of the function is a Either<L,R> type (see language.ext [Either left right monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Either_L_R.htm)). The Match call at the end either writes the result (right value) or a rfc error (left value). 

**Structures**

Structures can be set or retrieved the same way. Another example extracting company code details (you may have to change the company code if you try this example):

```csharp
using (var context = new RfcContext(connFunc))
{
    await context.CallFunction("BAPI_COMPANYCODE_GETDETAIL",
            Input: f => f
                .SetField("COMPANYCODEID", "1000"),
            Output: f => f
                .MapStructure("COMPANYCODE_DETAIL", s=> s
                    .GetField<string>("COMP_NAME"))
        )
        .Match(r => Console.WriteLine($"Result: {r}"),
            l => Console.WriteLine($"Error: {l.Message}"));

}
  ```

Alternatively, you can also use a LINQ syntax:

```csharp
using (var context = new RfcContext(connFunc))
{
    await context.CallFunction("BAPI_COMPANYCODE_GETDETAIL",
        Input: f => f
            .SetField("COMPANYCODEID", "1000"),
        Output: f => f
            .MapStructure("COMPANYCODE_DETAIL", s =>
                from name in s.GetField<string>("COMP_NAME")
                select name
            ))

            .Match(r => Console.WriteLine($"Result: {r}"),
                l => Console.WriteLine($"Error: {l.Message}"));

}
  ```
Especially for complex structures, the LINQ syntax is often easier to read.

**Tables**

Getting table results is possible by iterating over the table rows to retrieve the table structures. Here an example to extract all company code name and descriptions:

```csharp
using (var context = new RfcContext(connFunc))
{
    await context.CallFunction("BAPI_COMPANYCODE_GETLIST",
            Output: f => f
                .MapTable("COMPANYCODE_LIST", s =>
                    from code in s.GetField<string>("COMP_CODE")
                    from name in s.GetField<string>("COMP_NAME")
                    select (code, name)))
        .Match(
            r =>
            {
                foreach (var (code, name) in r)
                {
                    Console.WriteLine($"{code}\t{name}");
                }
            },
            l => Console.WriteLine($"Error: {l.Message}"));

}
  ```


## Build

We use Visual Studio 2019 for building. 

As explained above you have to obtain _SAP NW RFC Library 750_ from _SAP Support Portal_. 
Download both the x64 and the x86 versions and place then in the repository folder nwrfcsdk/x64 and nwrfcsdk/x86.

The sdk is required to build the test application and reference helper library. 


## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/dbosoft/YaNco/tags). 

## Authors

* **Frank Wagner** - *Initial work* - [fw2568](https://github.com/fw2568)

See also the list of [contributors](https://github.com/Dbosoft/YaNco/contributors) who participated in this project.

## Commercial support

The creators of YaNco, [dbosoft](https://www.dbosoft.eu), offer [professional support plans](https://support.dbosoft.eu/hc/en-us/articles/360021454080-YaNco-Support-Plans)  which we strongly recommend for any organisations using YaNco on a commercial basis. 

They includes:

- Prioritised resolution of any bugs. If you find a bug that’s blocking you, we’ll prioritise it and release a hot fix as soon as it’s ready.
- Prioritised resolution and escalation of issues. If there’s a possible issue or question, we’ll prioritise dealing with it.
- Prioritised feature requests: Get new features that are important to you added first.
- Personalised support and guidance via email, telephone or video. Speak to one of our team for advice and best practices on how to best manage deployments.
- Discounts on training and coaching services



## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Trademark notice

SAP, Netweaver are trademarks of SAP SE
