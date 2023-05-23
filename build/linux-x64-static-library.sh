rm -rf ../src/obj
dotnet publish ../src/LibWindPop.csproj /p:NativeLib=Static\;PublishAot=true -c Release -f net8.0 -r linux-x64 -o ../build/publish/nativeaot/static/linux-x64
cp ../src/libwindpop.h ../build/publish/nativeaot/static/linux-x64/LibWindPop.h
