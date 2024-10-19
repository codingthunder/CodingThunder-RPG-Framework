using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class NuGetConfigInstaller
{
    private static string editorPrefsKey = "NuGetConfigInstalled";

    static NuGetConfigInstaller()
    {
        if (EditorPrefs.HasKey(editorPrefsKey))
        {
            return;
        }

        // Define source paths (inside your package)
        string packagePath = "Packages/com.codingthunder.rpgframework/Config/";
        string destinationPath = "Assets/";

        // Ensure destination directory exists
        //if (!Directory.Exists(destinationPath))
        //{
        //    Directory.CreateDirectory(destinationPath);
        //}

        // Copy files
        CopyFile(packagePath, destinationPath, "packages.config");
        CopyFile(packagePath, destinationPath, "NuGet.config");

        // Optional: Show a message when done
        Debug.Log("NuGet configuration files have been copied to the Assets folder.");

        EditorPrefs.SetBool(editorPrefsKey, true);
    }

    // Method to copy files from the package to the destination
    private static void CopyFile(string sourceDir, string destDir, string fileName)
    {
        string sourceFile = Path.Combine(sourceDir, fileName);
        string destFile = Path.Combine(destDir, fileName);

        if (File.Exists(sourceFile))
        {
            File.Copy(sourceFile, destFile, true);
        }
        else
        {
            Debug.LogWarning($"File not found: {sourceFile}");
        }
    }
}
