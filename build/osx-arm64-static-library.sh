rm -rf ../src/obj
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r osx-arm64 -o ../build/publish/nativeaot/static/osx-arm64
cp ../src/libwindpop.h ../build/publish/nativeaot/static/osx-arm64/LibWindPop.h
