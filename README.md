jetbrains-mirror
================

This tool produces a local mirror of the official [JetBrains plugin repository](https://plugins.jetbrains.com) for use
in airgapped environments.

It's currently in a prototype stage, and isn't very finished.


## Basic Usage
```bash
dotnet jetbrains-mirror.dll --versions RD-191.7141.355 CL-191.7479.33
```

Various options are available to configure which versions to mirror. See `--help` for detailed information.

<sub>Background created using https://www.jetbrains.com/goodies/code2art/</sub>
