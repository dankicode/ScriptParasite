using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ScriptParasite.Watcher;

public static class ScriptTransformer
{
    public static string AddForDisk(string code, string namespaceName)
    {
        code = $"namespace {namespaceName};\n\n" + code;
        code = code.Replace("public class Script_Instance", "public abstract class Script_Instance");
        return code;
    }

    public static string RemoveForGrasshopper(string code)
    {
        // Remove file-scoped namespace line
        code = System.Text.RegularExpressions.Regex.Replace(code, @"^namespace\s+\S+;\s*\n", "", 
            System.Text.RegularExpressions.RegexOptions.Multiline);
        code = code.Replace("public abstract class Script_Instance", "public class Script_Instance");
        return code.TrimStart();
    }
}