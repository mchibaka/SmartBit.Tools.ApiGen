# SmartBit API Generator

 
Tool for generating APIs based on EF Model

## Usage

smartbit-apigen [configfile] [--options]

by default it looks for a config file called apigen.yml

Sample config `apigen.yml`

```
Assembly: bin\Debug\net8.0\myproject-with-efmodel-context.dll
OutputDirectory: Controllers
Namespace: my.project.root.namespace.Controllers
Force: false
```

## Commandline arguments

```
 -a, --assembly                 assembly path

  -o, --output-dir               Output directory

  -n, --namespace                Namespace for the generated APIs

  -f, --force                    (Default: false) Overwrite if file exists

  --help                         Display this help screen.

  --version                      Display version information.

  Configuration File (pos. 0)    (Default: apigen.yml) Path to configuration
                                 file
```

# Important 

This tool was developed for personal use.
