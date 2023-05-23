rd /s /q ..\src\obj
dotnet publish ..\src\LibWindPop.csproj /p:NativeLib=Static;PublishAot=true -c Release -f net8.0 -r win-x64 -o ..\build\publish\nativeaot\static\win-x64
copy ..\src\libwindpop.h ..\build\publish\nativeaot\static\win-x64\LibWindPop.h
