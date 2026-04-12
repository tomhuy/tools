using System;
using System.IO;

namespace ETLTools.Infrastructure.Tests.TestUtilities;

public class FileSystemTestHelper : IDisposable
{
    public string TestDirectory { get; private set; }

    public FileSystemTestHelper()
    {
        TestDirectory = Path.Combine(Path.GetTempPath(), "ETLToolsTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(TestDirectory);
    }

    public string CreateTestCsprojFile(string fileName, string version)
    {
        var filePath = Path.Combine(TestDirectory, fileName);
        var content = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <AssemblyVersion>{version}</AssemblyVersion>
    <FileVersion>{version}</FileVersion>
  </PropertyGroup>
</Project>";
        
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(filePath, content);
        return filePath;
    }

    public void CreateTestProjectStructure()
    {
        var projectsDir = Path.Combine(TestDirectory, "Projects");
        Directory.CreateDirectory(projectsDir);

        CreateTestCsprojFile(Path.Combine("Projects", "Test1.ETL.csproj"), "2026.2.3.1");
        CreateTestCsprojFile(Path.Combine("Projects", "Test2.ETL.csproj"), "2026.2.3.2");
        CreateTestCsprojFile(Path.Combine("Projects", "ShareProject.ETL.csproj"), "2026.2.3.1");
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(TestDirectory))
            {
                Directory.Delete(TestDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}
