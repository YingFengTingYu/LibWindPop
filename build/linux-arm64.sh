dotnet publish ../src/LibWindPop.csproj -c Release -f net8.0 -o ../build/publish/managed
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r linux-arm64 -o ../build/publish/nativeaot/static/linux-arm64
cp ../src/libwindpop.h ../build/publish/nativeaot/static/linux-arm64/LibWindPop.h
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r linux-arm64 -o ../build/publish/nativeaot/shared/linux-arm64
cp ../src/libwindpop.h ../build/publish/nativeaot/shared/linux-arm64/LibWindPop.h
