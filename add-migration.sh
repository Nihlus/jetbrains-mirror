#!/usr/bin/env bash

if [[ -z $1 ]]; then
    echo "Please provide a migration name."
    return 1;
fi

dotnet ef migrations add $1 --project JetBrains.Plugins.Models/JetBrains.Plugins.Models.csproj --startup-project JetBrains.Plugins/JetBrains.Plugins.csproj 

