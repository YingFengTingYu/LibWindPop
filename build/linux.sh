dotnet publish ../src/LibWindPop.csproj -c Release -f net8.0 -o ../build/publish/managed
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r linux-x64 -o ../build/publish/nativeaot/static/linux-x64
cp -i ../src/libwindpop.h ../build/publish/nativeaot/static/linux-x64/LibWindPop.h
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r linux-arm64 -o ../build/publish/nativeaot/static/linux-arm64
cp -i ../src/libwindpop.h ../build/publish/nativeaot/static/linux-arm64/LibWindPop.h
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r linux-x64 -o ../build/publish/nativeaot/shared/linux-x64
cp -i ../src/bin/Release/net8.0/linux-x64/native/LibWindPop.a ../build/publish/nativeaot/shared/linux-x64/LibWindPop.a
cp -i ../src/libwindpop.h ../build/publish/nativeaot/shared/linux-x64/LibWindPop.h
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r linux-arm64 -o ../build/publish/nativeaot/shared/linux-arm64
cp -i ../src/bin/Release/net8.0/linux-arm64/native/LibWindPop.a ../build/publish/nativeaot/shared/linux-arm64/LibWindPop.a
cp -i ../src/libwindpop.h ../build/publish/nativeaot/shared/linux-arm64/LibWindPop.h
