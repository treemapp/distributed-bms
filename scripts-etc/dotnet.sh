# dotnet.sh

dotnet new console -n ProjectName

dotnet build

dotnet run

# Publish single executable (bundle .net) (in bin\Release\net10.0\win-x64\publish\):

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# .. gives thing.exe & appsettings.json

# if target machine has .net 10:

dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:AssemblyName=BeckhoffInspector

dotnet restore

dotnet add package YamlDotNet --version 18.1.0
