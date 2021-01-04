# Option Expressions for .NET Core

Enable functions and variables in your app's configuration with Option Expressions. This library is intended for apps that use dependency injection and [Microsoft.Extensions.Options](https://docs.microsoft.com/en-us/dotnet/core/extensions/options).

## Features
* Default set of functions and variables available are included out of the box.
* Extensible with custom functions and variables.
* Integration with `IOptions`, `IOptionsSnapshot` and `IOptionsMonitor`.
* Works with any configuration provider including JSON, commandline, etc.

## Quick Start
To enable Option Expressions, add `UseOptionExpressions()` to your host builder code.
```csharp
using OptionExpressions;

using IHost host = Host
    .CreateDefaultBuilder()
    .ConfigureServices(services => {
        services.AddTransient<MyService>();
    })
    .UseOptionExpressions()
    .Build();

await host.StartAsync();
```
```csharp
[EnableExpressions] // enable expressions on all properties in this class
public class MyServiceOptions
{
    public string AppName { get; set; }

    [EnableExpressions] // OR - enable expressions on certain properties only
    public bool Enabled { get; set; }
}

public class MyService
{
    public MyService(IOptions<MyServiceOptions> options)
    {
        string appName = options.Value.ApppName;
    }
}
```

#### JSON
```json
{
    "AppName": "[concat(assemblyName, ' - ', assemblyVersion)]",
    "Enabled": "[myCustomFunction()]"
}
```

#### Commandline
`app.exe --AppName=[concat(assemblyName, ' - ', assemblyVersion)] --Enabled=[myCustomFunction()]`

## Literals
Option Expressions support **string**, **integer**, and **boolean** literals.
```
[concat('This is a string!', 42, true)]
```

## Customization
```csharp
.UseOptionExpressions(configure =>
{
    // By default, expressions must be wrapped with [ and ] to be executed at runtime.
    // Set these to an empty string if you want every config value to be evaluated.
    configure.Prefix = "{{"; // expressions must start with {{
    configure.Suffix = "}}"; // expressions must end with }}

    // Global enable/disable. Classes and properties may override this via the [EnableExpressions] attribute.
    configure.EnableVariables = true;
    configure.EnableFunctions = true;

    // Define custom variables that can be used in expressions.
    configure.RegisterVariable("hello", "Hello, World!");

    // Define custom functions that can be used in expressions.
    configure.RegisterFunction("isDebug", args =>
    {
        #if DEBUG
            return true;
        #else
            return false;
        #endif
    });
})
```

You may enable or disable functions and variables per class or property with the `EnableExpressions` attribute.
```csharp
[EnableExpressions(enableFunctions: false)] // Disable expression functions for this class
public class MyServiceOptions
{
    public string AppName { get; set; }

    [EnableExpressions(enableVariables: false)] // Disable variables for this property
    public bool Enabled { get; set; }
}
```

## Default functions and variables
When you use `UseOptionExpressions` in your host builder, these variables and functions will be registered by default.

### Functions
| Function | Result |
| -------- | ----------- |
| concat(val1, val2, ...) | Concatenates given arguments into a single string. |
| strlen(val) | Gets the length of a string. |

### Variables
| Variable Name | Value |
| ------------- | ----------- |
| assemblyName | The name of the entry assembly. |
| assemblyVersion | The version of the entry assembly. |

## Compiling
This library uses ANTLR to automatically generate parser code. Since ANTLR is a Java tool, you'll need Java installed to build the solution. Please note that Java is NOT required to use this library. It's required only if you want to compile it.

You'll need to download ANTLR too. [Please read this file for more information](https://github.com/msaville8/OptionExpressions/tree/main/tools/antlr).

## Future Development
There are plans to add support for these features in the near future:
* Binary expressions (boolean and arithmetic)
* Floating-point numbers

## License
This library is licensed under the [MIT License](./LICENSE).

## Contributions
All contributions to this project will be greatly appreciated! Feel free to open a pull request if you see something you can improve.
