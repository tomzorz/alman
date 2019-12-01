cd alman
dotnet build -c release
dotnet pack -c release -o nupkg
dotnet tool uninstall -g alman
dotnet tool install --add-source .\nupkg -g alman
cd ..