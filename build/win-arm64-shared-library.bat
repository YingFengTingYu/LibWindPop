rd /s /q ..\src\obj
dotnet publish ..\src\LibWindPop.csproj /p:NativeLib=Shared;PublishAot=true -c Release -f net8.0 -r win-arm64 -o ..\build\publish\nativeaot\shared\win-arm64
copy ..\src\bin\Release\net8.0\win-arm64\native\LibWindPop.lib ..\build\publish\nativeaot\shared\win-arm64\LibWindPop.lib
copy ..\src\bin\Release\net8.0\win-arm64\native\LibWindPop.exp ..\build\publish\nativeaot\shared\win-arm64\LibWindPop.exp
copy ..\src\libwindpop.h ..\build\publish\nativeaot\shared\win-arm64\LibWindPop.h
