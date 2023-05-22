dotnet publish ../src/LibWindPop.csproj -c Release -f net8.0 -o ../build/publish/managed
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r osx-x64 -o ../build/publish/nativeaot/static/osx-x64
cp ../src/libwindpop.h ../build/publish/nativeaot/static/osx-x64/LibWindPop.h
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r osx-x64 -o ../build/publish/nativeaot/shared/osx-x64
cp ../src/libwindpop.h ../build/publish/nativeaot/shared/osx-x64/LibWindPop.h
