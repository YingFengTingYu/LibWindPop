rm -rf ../src/obj
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r osx-arm64 -o ../build/publish/nativeaot/shared/osx-arm64
cp ../src/libwindpop.h ../build/publish/nativeaot/shared/osx-arm64/LibWindPop.h
