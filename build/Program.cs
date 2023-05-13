using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LibWindPop.Build
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string basePath = Path.GetFullPath("../../../../");
            // BflatNativeLibraryBuilder.BuildAll(basePath); // Json Error
            ManagedLibraryBuilder.BuildAll(basePath);
            NativeAOTLibraryBuilder.BuildAll(basePath, true);
            NativeAOTLibraryBuilder.BuildAll(basePath, false);
        }

        private static void MyProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private static class ManagedLibraryBuilder
        {
            private const string DotNetPath = "dotnet";

            public static void BuildAll(string basePath)
            {
                string srcPath = Path.Combine(basePath, "src", "LibWindPop.csproj");
                string buildPath = Path.Combine(basePath, "build");
                string outputPath = Path.Combine(buildPath, "publish", "managed");

                string sharedArg = $"publish {srcPath} -c Release -f net8.0 -o {outputPath}";

                RunDotnetWithArgs(sharedArg);

                static void RunDotnetWithArgs(string args)
                {
                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        myProcess.StartInfo.FileName = DotNetPath;
                        myProcess.StartInfo.Arguments = args;
                        myProcess.StartInfo.CreateNoWindow = false;
                        myProcess.StartInfo.RedirectStandardOutput = true;
                        myProcess.Start();
                        myProcess.WaitForExit();
                    }
                }
            }
        }

        private static class NativeAOTLibraryBuilder
        {
            private const string DotNetPath = "dotnet";

            public static void BuildAll(string basePath, bool staticLibrary)
            {
                if (OperatingSystem.IsWindows())
                {
                    BuildSingle(basePath, TarSys.Windows, TarArch.X86_64, staticLibrary);
                    BuildSingle(basePath, TarSys.Windows, TarArch.Arm64, staticLibrary);
                }
                if (OperatingSystem.IsLinux())
                {
                    BuildSingle(basePath, TarSys.Linux, TarArch.X86_64, staticLibrary);
                    BuildSingle(basePath, TarSys.Linux, TarArch.Arm64, staticLibrary);
                    BuildSingle(basePath, TarSys.Linux, TarArch.ArmMusl64, staticLibrary);
                }
                if (OperatingSystem.IsMacOS())
                {
                    BuildSingle(basePath, TarSys.MacOS, TarArch.X86_64, staticLibrary);
                    BuildSingle(basePath, TarSys.MacOS, TarArch.Arm64, staticLibrary);
                }
            }

            private static void BuildSingle(string basePath, TarSys sys, TarArch arch, bool staticLibrary)
            {
                string tName = staticLibrary ? "Static" : "Shared";
                int dotnetVersion = 8;
                string runtimeName = "android-arm64";

                if (sys == TarSys.Windows)
                {
                    if (arch == TarArch.X86_64)
                    {
                        runtimeName = "win-x64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else if (arch == TarArch.Arm64)
                    {
                        runtimeName = "win-arm64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else
                    {
                        Throw();
                    }
                }
                else if (sys == TarSys.Linux)
                {
                    if (arch == TarArch.X86_64)
                    {
                        runtimeName = "linux-x64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else if (arch == TarArch.Arm64)
                    {
                        runtimeName = "linux-arm64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else if (arch == TarArch.ArmMusl64)
                    {
                        runtimeName = "linux-musl-arm64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else
                    {
                        Throw();
                    }
                }
                else if (sys == TarSys.MacOS)
                {
                    if (arch == TarArch.X86_64)
                    {
                        runtimeName = "osx-x64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else if (arch == TarArch.Arm64)
                    {
                        runtimeName = "osx-arm64";
                        dotnetVersion = Math.Max(dotnetVersion, 8);
                    }
                    else
                    {
                        Throw();
                    }
                }
                else
                {
                    Throw();
                }

                string srcPath = Path.Combine(basePath, "src", "LibWindPop.csproj");
                string buildPath = Path.Combine(basePath, "build");
                string outputPath = Path.Combine(buildPath, "publish", "nativeaot", tName.ToLower(), runtimeName);
                string rawOutputPath = Path.Combine(basePath, "src", "bin", "Release", $"net{dotnetVersion}.0", runtimeName, "native");
                string sharedArg = $"publish {srcPath} /p:NativeLib={tName};PublishAot=true -c Release -f net{dotnetVersion}.0 -r {runtimeName}";
                
                if (Directory.Exists(rawOutputPath))
                {
                    Directory.Delete(rawOutputPath, true);
                }

                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = DotNetPath;
                    myProcess.StartInfo.Arguments = sharedArg;
                    myProcess.StartInfo.CreateNoWindow = false;
                    myProcess.StartInfo.RedirectStandardOutput = true;
                    myProcess.OutputDataReceived += MyProcess_OutputDataReceived;
                    myProcess.Start();
                    myProcess.BeginOutputReadLine();
                    myProcess.WaitForExit();
                }

                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }
                
                Directory.CreateDirectory(outputPath);
                foreach (string rawOutputFile in Directory.GetFiles(rawOutputPath))
                {
                    string outputFile = Path.Combine(outputPath, Path.GetFileName(rawOutputFile));

                    File.Copy(rawOutputFile, outputFile, true);
                }
                File.Copy(
                    Path.Combine(buildPath, "header", "libwindpop.h"),
                    Path.Combine(outputPath, "LibWindPop.h"),
                    true
                    );
            }
        }

        private static class BflatNativeLibraryBuilder
        {
            private const string BflatPath = "bflat";

            public static void BuildAll(string basePath)
            {
                BuildSingle(basePath, TarSys.Windows, TarArch.X86_64);
                BuildSingle(basePath, TarSys.Windows, TarArch.Arm64);
                BuildSingle(basePath, TarSys.Linux, TarArch.X86_64);
                BuildSingle(basePath, TarSys.Linux, TarArch.Arm64);
                BuildSingle(basePath, TarSys.Android, TarArch.Arm64);
            }

            private static void BuildSingle(string basePath, TarSys sys, TarArch arch)
            {
                string folderName = "android-arm64";
                string fileName = "libwindpop.so";
                string archName = "arm64";
                string osName = "linux";
                string libcName = "bionic";

                if (sys == TarSys.Windows)
                {
                    if (arch == TarArch.X86_64)
                    {
                        folderName = "win-x64";
                        fileName = "libwindpop.dll";
                        archName = "x64";
                        osName = "windows";
                        libcName = "shcrt";
                    }
                    else if (arch == TarArch.Arm64)
                    {
                        folderName = "win-arm64";
                        fileName = "libwindpop.dll";
                        archName = "arm64";
                        osName = "windows";
                        libcName = "shcrt";
                    }
                    else
                    {
                        Throw();
                    }
                }
                else if (sys == TarSys.Linux)
                {
                    if (arch == TarArch.X86_64)
                    {
                        folderName = "linux-x64";
                        fileName = "libwindpop.so";
                        archName = "x64";
                        osName = "linux";
                        libcName = "glibc";
                    }
                    else if (arch == TarArch.Arm64)
                    {
                        folderName = "linux-arm64";
                        fileName = "libwindpop.so";
                        archName = "arm64";
                        osName = "linux";
                        libcName = "glibc";
                    }
                    else
                    {
                        Throw();
                    }
                }
                else if (sys == TarSys.Android)
                {
                    if (arch == TarArch.Arm64)
                    {
                        folderName = "android-arm64";
                        fileName = "libwindpop.so";
                        archName = "arm64";
                        osName = "linux";
                        libcName = "bionic";
                    }
                    else
                    {
                        Throw();
                    }
                }
                else
                {
                    Throw();
                }
                string srcPath = Path.Combine(basePath, "src");
                string buildPath = Path.Combine(basePath, "build");
                string outputFolder = Path.Combine(buildPath, "publish", "bflat", folderName);
                string outputPath = Path.Combine(outputFolder, fileName);
                Directory.CreateDirectory(outputFolder);
                StringBuilder buildScriptBuilder = new StringBuilder();
                buildScriptBuilder.Append("build");
                foreach (string csFile in Directory.GetFiles(srcPath, "*.cs", SearchOption.TopDirectoryOnly))
                {
                    buildScriptBuilder.Append(' ');
                    buildScriptBuilder.Append(csFile);
                }
                foreach (string csFolder in Directory.GetDirectories(srcPath))
                {
                    string fdName = Path.GetFileName(csFolder);
                    if (fdName != "bin" && fdName != "obj")
                    {
                        foreach (string csFile in Directory.GetFiles(csFolder, "*.cs", SearchOption.AllDirectories))
                        {
                            buildScriptBuilder.Append(' ');
                            buildScriptBuilder.Append(csFile);
                        }
                    }
                }
                buildScriptBuilder.Append(" -r");
                foreach (string dllFile in Directory.GetFiles(Path.Combine(buildPath, "dependency"), "*.dll", SearchOption.AllDirectories))
                {
                    buildScriptBuilder.Append(' ');
                    buildScriptBuilder.Append(dllFile);
                }
                buildScriptBuilder.Append(" --target Shared -o ");
                buildScriptBuilder.Append(outputPath);
                buildScriptBuilder.Append(" --arch ");
                buildScriptBuilder.Append(archName);
                buildScriptBuilder.Append(" --os ");
                buildScriptBuilder.Append(osName);
                buildScriptBuilder.Append(" --libc ");
                buildScriptBuilder.Append(libcName);
                buildScriptBuilder.Append(" -Ot --separate-symbols --stdlib DotNet");
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = BflatPath;
                    myProcess.StartInfo.Arguments = buildScriptBuilder.ToString();
                    myProcess.StartInfo.CreateNoWindow = false;
                    myProcess.Start();
                    myProcess.WaitForExit();
                }
                File.Copy(
                    Path.Combine(buildPath, "header", "libwindpop.h"),
                    Path.Combine(outputFolder, "LibWindPop.h"),
                    true
                    );
            }
        }

        private static void Throw() => throw new Exception();
    }
}
