rd /s /q ..\src\obj
dotnet publish ..\src\LibWindPop.csproj /p:NativeLib=Static;PublishAot=true -c Release -f net8.0 -r win-arm64 -o ..\build\publish\nativeaot\static\win-arm64
copy ..\src\libwindpop.h ..\build\publish\nativeaot\static\win-arm64\LibWindPop.h
