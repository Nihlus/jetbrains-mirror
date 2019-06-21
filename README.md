jetbrains-mirror
================

This tool produces a local mirror of the official [JetBrains plugin repository](https://plugins.jetbrains.com) for use
in airgapped environments.

It's currently in a prototype stage, and isn't very finished.


## Basic Usage
```bash
dotnet jetbrains-mirror.dll
```

No options are currently available. The application will only mirror the latest plugins for Rider. No metadata files 
will be created.


<sub>Background created using https://www.jetbrains.com/goodies/code2art/</sub>
