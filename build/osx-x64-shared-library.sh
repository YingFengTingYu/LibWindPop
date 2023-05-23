rm -rf ../src/obj
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r osx-x64 -o ../build/publish/nativeaot/shared/osx-x64
cp ../src/libwindpop.h ../build/publish/nativeaot/shared/osx-x64/LibWindPop.h
