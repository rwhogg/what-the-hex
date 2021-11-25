// Copyright 2021 Bob "Wombat" Hogg
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

[Generator]
class GodotBuildInfoGenerator: ISourceGenerator
{
    private string GodotVersion;

    public void Initialize(GeneratorInitializationContext context)
    {
        Process godotProcess = new Process();
        // FIXME: the ".exe" will not be present when building on non-WSL Linux
        godotProcess.StartInfo.FileName = "godot.exe";
        godotProcess.StartInfo.Arguments = "--version";
        godotProcess.StartInfo.CreateNoWindow = true;
        godotProcess.StartInfo.RedirectStandardOutput = true;
        godotProcess.StartInfo.UseShellExecute = false;
        godotProcess.Start();
        StreamReader versionReader = godotProcess.StandardOutput;
        godotProcess.WaitForExit();
        GodotVersion = versionReader.ReadToEnd().Replace("\n", "").Replace("\r", "");
        godotProcess.Dispose();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        string sourceCode = String.Format(
            "using System;\npublic static class GodotVersionInfo\n{{\n    public static string GetGodotInfo()\n    {{\n        return {0};\n    }}\n}}\n",
            "\"" + GodotVersion + "\""
        );
        context.AddSource("GodotVersionInfo", SourceText.From(sourceCode, Encoding.UTF8));
    }
}
