# dotnet-sqlversioner

SQL Versioner is a .NET Tool that helps you version your SQL Server database.

## Installation

Create a `nuget.config` file in the root of the directory where you want to install the tool with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/paulalves/index.json" />
  </packageSources>
</configuration>
```

Then install the tool:

```bash
dotnet tool install --global SqlVersioner.CliTool --version 0.1.0-ci0012
```

## Usage

```bash
sqlversioner --username <username> --password <password> --server <server> --database <database> --output <output> --verbosity <verbosity>

Options:
  --user        The username to connect to the database.
  --password    The password to connect to the database.
  --server      The server to connect to.
  --database    The database to connect to.
  --output      The output folder where the scripts will be generated.
  --verbosity   The verbosity level. Allowed values are: 0 - Quiet, 1 - Minimal, 2 - Normal, 3 - Detailed.
```

Example: 

```bash
sqlversioner --username sa --password supersecret --server localhost --database MyDb --output ~/Desktop/dbs --verbosity 2
```