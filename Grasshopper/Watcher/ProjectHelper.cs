using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Grasshopper.Kernel;

namespace ScriptParasite.Watcher;

public class ProjectHelper
{
    public static void EnsureVsCodeSettings(string workspaceRoot)
    {
        var settingsPath = Path.Combine(workspaceRoot, ".vscode", "settings.json");
        if (File.Exists(settingsPath))
            return;

        var rhinoCodePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".rhinocode", "py39-rh8");

        var pythonExe = Environment.OSVersion.Platform == PlatformID.Unix
            ? Path.Combine(rhinoCodePath, "bin", "python3")
            : Path.Combine(rhinoCodePath, "python.exe");

        var siteEnvsPath = Path.Combine(rhinoCodePath, "site-envs");
        var defaultEnv = Directory.Exists(siteEnvsPath)
            ? Directory.GetDirectories(siteEnvsPath, "default-*").FirstOrDefault()
            : null;

        var extraPaths = new List<string>
        {
            Path.Combine(rhinoCodePath, "site-stubs"),
            Path.Combine(rhinoCodePath, "site-rhinopython"),
            Path.Combine(rhinoCodePath, "site-rhinoghpython"),
            Path.Combine(rhinoCodePath, "site-interop"),
        };

        if (defaultEnv != null)
            extraPaths.Add(defaultEnv);

        var extraPathsJson = string.Join(",\n    ", extraPaths.Select(p => $"\"{Escape(p)}\""));

        var settings = $$"""
                         {
                           "python.analysis.extraPaths": [
                             {{extraPathsJson}}
                           ],
                           "python.defaultInterpreterPath": "{{Escape(pythonExe)}}"
                         }
                         """;

        Directory.CreateDirectory(Path.Combine(workspaceRoot, ".vscode"));
        File.WriteAllText(settingsPath, settings);
    }

    private static string Escape(string path) => path.Replace("\\", "\\\\");
    
    public static bool EnsureProjectCsharp(string scriptFilename)
    {
        var directory = Path.GetDirectoryName(scriptFilename);
        // find if a csproj is already there, or any of the parent directories have a csproj, if so, use that one instead of writing a new one.
        var currentDir = directory;
        while (currentDir != null)        {
            var csprojFiles = Directory.GetFiles(currentDir, "*.csproj");
            if (csprojFiles.Length > 0)            {
                return true;
            }
            currentDir = Path.GetDirectoryName(currentDir);
        }
        var project = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9</TargetFramework>
    <LangVersion>10</LangVersion>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Debug|AnyCPU'"">
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""GH_IO"">
      <HintPath>%ghio%\GH_IO.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include=""Grasshopper"">
      <HintPath>%grasshopper%</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include=""RhinoCommon"">
      <HintPath>%rhinocommon%</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>
</Project>";
        
        if (directory == null)
        {
            return false;
        }
        var projectFile = Path.Combine(directory, "GrasshopperScripts.csproj");
        var grasshopperDir = Assembly.GetAssembly(typeof(GH_Component))!.Location;
        var rhinoCommonDir = Assembly.GetAssembly(typeof(Rhino.RhinoDoc))!.Location;
        var ghIoDir = Assembly.GetAssembly(typeof(GH_IO.Serialization.GH_IWriter))!.Location;
        project = project.Replace("%grasshopper%", grasshopperDir);
        project = project.Replace("%rhinocommon%", rhinoCommonDir);
        project = project.Replace("%ghio%", ghIoDir);
        try
        {
            File.WriteAllText(projectFile, project);
            return true;
        } catch (Exception ex)
        {
            return false;
        }

        return false;
    }
}