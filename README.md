# YaNco - Yet another .NET Connector
.NET Connector for SAP Netweaver RFC

Stable                     |  Latest                   |
---------------------------|---------------------------|
[![NuGet stable](https://img.shields.io/nuget/v/Dbosoft.YaNco.svg?style=flat-square)](https://www.nuget.org/packages/Dbosoft.YaNco) | [![NuGet pre](https://img.shields.io/nuget/vpre/Dbosoft.YaNco.svg?style=flat-square)](https://www.nuget.org/packages/Dbosoft.YaNco)

# This the README for version v5!
The readme for maintenance release v4 can be found here: https://github.com/dbosoft/YaNco/tree/support/4.3

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

To use YaNco you need to obtain _SAP NW RFC SDK 750_ from _[SAP Support Portal](https://launchpad.support.sap.com/#/softwarecenter/template/products/_APP=00200682500000001943&_EVENT=DISPHIER&HEADER=N&FUNCTIONBAR=Y&EVENT=TREE&TMPL=INTRO_SWDC_SP_AD&V=MAINT&REFERER=CATALOG-PATCHES&ROUTENAME=products/By%20Category%20-%20Additional%20Components)_.

A prerequisite to download is having a **customer or partner account** on _SAP Support Portal_ and if you
are SAP employee please check [SAP Note 1037575 - Software download authorizations for SAP employees](https://launchpad.support.sap.com/#/notes/1037575).

_SAP NW RFC SDK 750_ is fully backwards compatible, supporting all NetWeaver systems, from today, down to release R/3 4.6C.
You can therefore always use the newest version released on SAP Support Portal and connect to older systems as well.

## Getting started

The easiest way to get started is by installing [the available NuGet package](https://www.nuget.org/packages/Dbosoft.YaNco). Take a look at the [Using](#using) section learning how to configure and use YaNco. Go to the [Build](#build) section to find out how to build YaNco. 

**Samples and articles**

- [Creating a SAP DMS library with YaNco (Part 1)](https://dbosoft.eu/en-us/blog/creating-a-sap-dms-library-with-yanco-part-1)
- [Creating a SAP DMS library with YaNco (Part 2)](https://dbosoft.eu/en-us/blog/creating-a-sap-dms-library-with-yanco-part-2)
- [Sample app: Using YaNco in ASP.NET Core](https://github.com/dbosoft/YaNco/tree/main/samples/netcore3.1/SAPWebAPI)
- [Sample app: Command line tool to create sales orders](https://github.com/dbosoft/YaNco/tree/main/samples/netcore3.1/CreateSalesOrder)
- [Sample app: IDoc Receiver - Export Material Master](https://github.com/dbosoft/YaNco/tree/main/samples/net6.0/ExportMATMAS)
- [SAPHub - How to build reliable and scaleable SAP integrated microservices with Hosuto and YaNco](https://github.com/dbosoft/SAPHub) 

Please note that most samples have not yet been updated to version 5!

## Using

### Prepare connection
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
With these settings you can now create a ConnectionBuilder instance and use it to build a connection builder function.

```csharp

var connectionBuilder = new ConnectionBuilder(settings);
var connFunc = connectionBuilder.Build();

```

The connection builders Build() method returns a function that can be reused to open connections. 

Under the hood the ConnectionBuilder also creates also a SAPRfcRuntime instance. The SAPRfcRuntime abstracts between the SAP Netweaver RFC SDK and YaNco and encapsulates all I/O between YaNco and the RFC SDK.

You can customize the runtime on the ConnectionBuilder with the ConfigureRuntime() method. For example to add a logger:

```csharp
var connectionBuilder = new ConnectionBuilder(connFunc)
    .ConfigureRuntime(c => 
        c.WithLogger(new MyLogger()));
```

>**Please note:** In versions below 5.0 we used the IRfcRuntime interface implemented by type RfcRuntime. IRfcRuntime and RfcRuntime are now deprecated.  
The new **SAPRfcRuntime** has a different concept (see below for functional IO patterns). Therefore ConfigureRuntime now configures the runtime settings used to create a new SAPRfcRuntime.

### RfcContext

For classic OO usage the RfcContext is the easiest method call functions from .NET to SAP. You can open a RfcContext directly from the connection function that you build with ConnectionBuilder.Build()

```csharp

var connectionBuilder = new ConnectionBuilder(settings);
var connFunc = connectionBuilder.Build();

using (var context = new RfcContext(connFunc))
{
   ...

}
```

The RfcContext will automatically open and close the connection and is disposeable.

### Calling ABAP Function Modules 

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
  
**Input mapping**

For Input (importing / changing ) arguments you can pass the value with the methods SetField, SetStructure and SetTable or a combination of all. 
For example to set values for a table you pass a IEnumerable to be processed to the SetTable method and provide a mapping function for each record in the IEnumerable:

```csharp
var userNamesSearch = new string[] {"A*", "B*", "C*"};

var userList = await context.CallFunction("BAPI_USER_GETLIST",
    Input:f => f.SetTable("SELECTION_RANGE", userNamesSearch , 
        (structure,userName) => structure
                .SetField("PARAMETER", "USERNAME")
                .SetField("SIGN", "I")
                .SetField("OPTION", "CP")
                .SetField("LOW", userName)
        ),

    Output: f=> f.MapTable("USERLIST", s=>s.GetField<string>("USERNAME"))
).IfLeftAsync(l=>throw new Exception(l.Message));

foreach (var userName in userList)
{
    Console.WriteLine(userName);
}
  ```

<br/>  

### Functional programming usage

YaNco is build to be used in [functional programming](https://en.wikipedia.org/wiki/Functional_programming). Functional programming allows you to make your code more reliable and move your code toward declarative and functional code rather than imperative.

In functional code you typical start with your own Runtime instance:

```csharp
var runtime = SAPRfcRuntime.New();
var connectionEffect = new ConnectionBuilder<SAPRfcRuntime>(settings)
    .Build();
```

Please note the type argument on the `ConnectionBuilder`.  
The Build method now returns an IO effect (`Aff<RT, IConnection>`) that is not executed immediately, but only when the IO effect is called with runtime. 

```csharp
var fin = await connectionEffect.Run(runtime);
fin.IfFail(error => error.Throw());
```

Using this concept, you can chain multiple effects to build the call to the SAP system: 
```csharp
using static Dbosoft.YaNco.SAPRfc<Dbosoft.YaNco.Live.SAPRfcRuntime>;

var call = useConnection(connectionEffect, connection=> 
    from userName in callFunction(connection, "BAPI_USER_GET_DETAIL", f=> 
        f.SetField("USERNAME", "SAP*"), 
        f=> f.GetField<string>("USERNAME"))
    select userName);

var fin = await call.Run(runtime);
fin.IfFail(error => error.Throw());
```

The call from above is without side effects, that means it will not cause any I/O without the runtime.

The static using imports methods of `SAPRfc<RT>` so you can call useConnection and callFunction without any type. 
You can also declare your own static classes where runtime is a type parameter, so you can replace SAPRfcRuntime with another runtime, e. g. for testing. 

You can find a more general description of this concept in the language.ext wiki: https://github.com/louthy/language-ext/wiki/How-to-deal-with-side-effects


### Calling functions from SAP to .NET


**ABAP Callbacks**

ABAP callbacks allows the backend system to call functions on the client.  
There is build in support for the RFC_START_PROGRAM callback, that is used by SAP to request start of additional programs like saprfc and saphttp. 
To register a start program callback you use the method `WithStartProgramCallback` of the `ConnectionBuilder`: 

```csharp
var connectionBuilder = new ConnectionBuilder(settings)
    .WithStartProgramCallback(callback)
    
    StartProgramDelegate callback = command =>
    {
        // validate and check the start request and start processes if necessary
        // return ok if everything works or a error
        return RfcErrorInfo.Ok();
    };
  ```

You can register also other functions using following syntax:  

```csharp
var connectionBuilder = new ConnectionBuilder(settings)
    .WithFunctionHandler("ZYANCO_SERVER_FUNCTION_1",
        cf => cf
            .Input(i =>
                i.GetField<string>("SEND"))
            .Process(Console.WriteLine)
            .Reply((_, f) => f
                .SetField("RECEIVE", "Hello from YaNco")))
 ```
 
In this example a function with the name ZYANCO_SERVER_FUNCTION_1 has to exist on the backend server, with two parameters (and CHAR field SEND and a CHAR field RECEIVE). 

The registered function handler consists of 3 chained steps: 
- Input mapping  
  Extraction of values from the incoming function call and return extracted value for further processing. Any error here will stop the chain. Mapping features are the   same as in `CallFunction` output mapping.
- Processing  
  The extracted value will be passed as argument to the process function. In this example the input is just written to the console. 
  The process function can return a output value, that is passed to last step in chain. 
- Reply mapping  
  The reply step sets the values of the response (same as Input mapping in `CallFunction`). If you have no reply you can also call `NoReply` to end the chain.  
  
<br/>

**RFC Servers**

RFC servers can process RFC calls that have their origin in the SAP backend.  
Instead of a opening a client connection a RFC server registers itself on the SAP system gateway. A pure RFC Server therefore needs no client connection at all.  

However in practice also a client connection is used in most RFC Servers to obtain function and type metadata. 
YaNco supports both server only RFC Servers and RFC Servers with client connections: 

```csharp

var serverSettings = new Dictionary<string, string>
{
    { "SYSID", _configuration["saprfc:sysid"] },  // required for servers
    { "PROGRAM_ID", _configuration["saprfc:program_id"] },
    { "GWHOST", _configuration["saprfc:ashost"] },
    { "GWSERV", _configuration["saprfc:gateway"] },
    { "REG_COUNT", "1" },  // number of servers

};

var serverBuilder = new ServerBuilder(serverSettings)
    .WithFunctionHandler(
        "ZYANCO_SERVER_FUNCTION_1",

        //build function definition
        b => b
            .AddChar("SEND", RfcDirection.Import, 30)
            .AddChar("RECEIVE", RfcDirection.Export, 30),
        cf => cf
            .Input(i =>
                i.GetField<string>("SEND"))
            .Process(s =>
            {
                Console.WriteLine($"Received message from backend: {s}");
                cancellationTokenSource.Cancel();
                
            })
            .Reply((_, f) => f
                .SetField("RECEIVE", "Hello from YaNco")));
 ```


 or by lookup of function metadata from client connection: 


 ```csharp
var serverBuilder = new ServerBuilder(serverSettings)
.WithClientConnection(clientSettings, c => c
    .WithFunctionHandler("ZYANCO_SERVER_FUNCTION_1",
        cf => cf
            .Input(i =>
                i.GetField<string>("SEND"))
            .Process(Console.WriteLine)
            .Reply((_, f) => f
                .SetField("RECEIVE", "Hello from YaNco"))))
 ```

After configuring the RFC Server it can be started like this: 

 ```csharp
using var rfcServer = serverBuilder
    .Build()
    .StartOrException();
 ```

**Transactional RFC**  
Transactional RFC (tRFC) is used in SAP Systems to synchronize transactions cross system boundaries. A tRFC call is identified by a unique transaction id that has is announced to the receiving side before the actual function call is send. 

Assuming SAP is sending a tRFC call to your application following steps will happen during a tRFC call: 
1. **Check tRFC**  
  In that step you save the incoming tRFC and verify if it was not allready saved before  

2. **Send tRFC call**  
   The actual call will now be send. The recipient should process the data from the call but should not process it further.  

3. **Commit or Rollback**  
   In case of a commit data can now be processed further or it has to be rolled back. 

4. **Confirm**  
   Transaction is completed and can be removed or other cleanup operations can be executed. 
      

To handle these steps in a RFC Server you can register a transactional RFC handler that will be called for each of these steps: 

 ```csharp
var serverBuilder = new ServerBuilder(serverSettings)
 .WithTransactionalRfc(new MyTransactionRfcHandler())

 // MyTransactionRfcHandler has to implement interface 
 // ITransactionalRfcHandler<RT>

public interface ITransactionalRfcHandler<RT>
{
    Eff<RT,RfcRc> OnCheck( 
            IRfcHandle rfcHandle, string transactionId);
    
    ...
}
 ```

A sample implementation can be found in [samples/net6.0/ExportMATMAS](https://github.com/dbosoft/YaNco/tree/main/samples/net6.0/ExportMATMAS). This sample demonstrates how to receive IDocs with YaNco.  


## Build

We use Visual Studio 2022 for building. 

As explained above you have to obtain _SAP NW RFC Library 750_ from _SAP Support Portal_. 
But the SDK is only required to run test applications, so just building works without the RFC SDK. 

If you download the SDK use the x64 version and copy to DLLs from lib folder to a directory in your PATH environment variable. 


## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/dbosoft/YaNco/tags). 

## Authors

* **Frank Wagner** - *Initial work* - [fw2568](https://github.com/fw2568)

See also the list of [contributors](https://github.com/Dbosoft/YaNco/contributors) who participated in this project.

## Commercial support

The creators of YaNco, [dbosoft](https://www.dbosoft.eu), offer [professional support plans](https://support.dbosoft.eu/hc/en-us/articles/360021454080-YaNco-Support-Plans) which we strongly recommend for any organization using YaNco on a commercial basis. 

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
