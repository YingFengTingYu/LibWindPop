cd ../src
dotnet publish -c Release -f net8.0 -o ../build/publish/managed
dotnet publish /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r linux-x64 -o ../build/publish/nativeaot/static/linux-x64
cp libwindpop.h ../build/publish/nativeaot/static/linux-x64/LibWindPop.h
dotnet publish /p:NativeLib=Shared\;PublishAot=true -c Release -f net8.0 -r linux-x64 -o ../build/publish/nativeaot/shared/linux-x64
cp libwindpop.h ../build/publish/nativeaot/shared/linux-x64/LibWindPop.h
cd ../build
