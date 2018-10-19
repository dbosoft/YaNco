# Contiva.SAP.NWRfc - .NET SAP RFC API based on SAP Netweaver RFC SDK
## Description

This library provides a alternative SAP .NET Connector based on the _SAP NetWeaver RFC Library_,


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


Using an open connection, we can invoke remote function calls (RFC).


## Installation & Documentation

For further details on connection parameters, _pyrfc_ installation and usage,
please refer to [_pyrfc_ documentation](http://sap.github.io/PyRFC),
complementing _SAP NW RFC Library_ [programming guide and documentation](http://service.sap.com/rfc-library)
provided on SAP Service Marketplace.
