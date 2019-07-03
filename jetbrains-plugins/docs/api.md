The JetBrains Plugin API
========================

This document contains some general development notes on how the JetBrains IDEs interact with the plugin API.

base url: plugins.jetbrains.com (http)

## Typedefs
ProductBuild (RD-191.7141.355) 
    Contains product and build information.

UUID
    Client-unique UUID.

PluginID (TFS; com.intellij.plugins.watcher)
    A unique plugin name.

StandardTheme (DEFAULT; DARCULA)
    An all-caps name of a standard theme in the application. Seems to be an enum of some kind, defaulting to "DEFAULT"

## Loading & Searching
/api/iconNotAvailable (always 404)
    ulong updateId
    StandardTheme theme 

Example output: 

```
Can't find UpdateIcon with id 64097, theme: DARCULA
```

/api/icon (302 redirect to actual file or (/api/iconNotAvailable)
    PluginID pluginId
    StandardTheme theme 

/api/search
    bool is_featured_search (optional)
    ProductBuild build (required))
    ulong max (optional. Never seen more than 10k)
    orderBy (enum? optional, have seen "update", "date", "downloads", "rating")

/api/searchSuggest
    string term
    string productCode

## Plugins
/plugins/list
    UUID uuid
    ProductBuild build
    
/pluginManager
    action (download)
    PluginId id
    ProductBuild build
    UUID uuid
    
## Files
/files/<plugin-numeric-id>/<plugin-update-id>/icon/pluginIcon.svg
/files/<plugin-numeric-id>/<plugin-update-id>/icon/pluginIcon_dark.svg
/files/<plugin-numeric-id>/<plugin-update-id>/<filename>
    updateId (numeric id)
    pluginId (numeric id)
    family (enum? Seen INTELLIJ)
    UUID uuid
    code (product code; RD, CL, etc)
    build (product build)

