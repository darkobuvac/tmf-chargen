# dotnet-tmfchargen

`dotnet-tmfchargen` is a CLI tool for generating classes that contain characteristic names defined in the specifications of TMF (TeleManagement Forum) Service and Resource Catalogs. This tool helps streamline the development process by scaffolding characteristic classes based on the provided specifications, making it easier to work with TMF standards in your applications.

## Features

- **Scaffold characteristic classes**: Automatically generate C# classes with properties based on TMF Service and Resource Catalog specifications.
- **Support for multiple specifications**: You can generate classes for multiple specifications at once.
- **Customizable output**: Configure the namespace for the generated class file.
- **Flexible service URL configuration**: Define the service hostname, port, and full catalog URL.

## Installation

### Prerequisites

Before using `dotnet-tmfchargen`, make sure you have the following:

- [.NET SDK](https://dotnet.microsoft.com/download) (version compatible with this tool)
- Access to the repository where the `dotnet-tmfchargen` tool is stored.

### Building the Tool

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd <repository-folder>
   ```


### Usage

```
INFO: dotnet-tmfchargen

Usage: dotnet-tmfchargen [options] [value]

The following arguments are required to run:
    --type              Specifies the type of specification to scaffold characteristics for. Supported values are:
                        - `service`
                        - `resource`
    --namespace         The namespace for the generated class file.
The following arguments are optional:
    --host              Hostname to use when constructing URLs for catalog management services. Defaults to `127.0.0.1`.
    --port              Port number of the service. Defaults to `40207`.
    --spec-name         The name of the specification(s) whose characteristics will be used for class generation. Multiple specifications can be separate by commas. If none provided, all specs will be fetched from catalog.
    --catalog           Absolute URL for the catalog management service. If not provided, a URL is constructed using `--host` and `--port`.
```
