using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;

namespace Lifes.Infrastructure.Features.VersionIncrease.Services;

/// <summary>
/// Service for reading and updating .csproj files.
/// </summary>
public class ProjectFileService : IProjectFileService
{
    private readonly ILogger<ProjectFileService> _logger;

    public ProjectFileService(ILogger<ProjectFileService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<string>> ReadVersionAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result<string>.Failure("File path cannot be empty");
            }

            if (!File.Exists(filePath))
            {
                return Result<string>.Failure($"File not found: {filePath}");
            }

            _logger.LogDebug("Reading version from {FileName}", Path.GetFileName(filePath));

            // Load XML document
            var doc = await Task.Run(() => XDocument.Load(filePath));

            // Find AssemblyVersion element
            var assemblyVersion = doc.Descendants("AssemblyVersion").FirstOrDefault();
            
            if (assemblyVersion == null || string.IsNullOrWhiteSpace(assemblyVersion.Value))
            {
                return Result<string>.Failure($"AssemblyVersion not found in {Path.GetFileName(filePath)}");
            }

            var version = assemblyVersion.Value.Trim();
            _logger.LogDebug("{FileName}: Current version {Version}", Path.GetFileName(filePath), version);

            return Result<string>.Success(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read version from {FilePath}", filePath);
            return Result<string>.Failure($"Failed to read version: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> UpdateVersionAsync(string filePath, string newVersion)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Failure("File path cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(newVersion))
        {
            return Result.Failure("New version cannot be empty");
        }

        if (!File.Exists(filePath))
        {
            return Result.Failure($"File not found: {filePath}");
        }

        // Create backup for rollback
        var backupPath = $"{filePath}.backup";
        
        try
        {
            File.Copy(filePath, backupPath, overwrite: true);
            _logger.LogDebug("Created backup for {FileName}", Path.GetFileName(filePath));

            _logger.LogInformation("Updating {FileName} to version {NewVersion}", 
                Path.GetFileName(filePath), newVersion);

            // Read all lines
            var lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);
            var encoding = DetectEncoding(filePath);
            var lineEnding = DetectLineEnding(filePath);

            // Find and replace version lines
            var (updatedLines, modified) = UpdateVersionLines(lines, newVersion);

            if (!modified)
            {
                File.Delete(backupPath);
                return Result.Failure("No version tags found");
            }

            // Write back with original encoding and line endings
            await File.WriteAllTextAsync(filePath, 
                string.Join(lineEnding, updatedLines), 
                encoding).ConfigureAwait(false);

            _logger.LogDebug("Updated version lines in {FileName}", Path.GetFileName(filePath));

            // VALIDATION: Read back and verify
            var verifyResult = await VerifyVersionUpdate(filePath, newVersion).ConfigureAwait(false);
            if (!verifyResult.IsSuccess)
            {
                // Rollback on validation failure
                _logger.LogError("Validation failed for {File}: {Error}", 
                    Path.GetFileName(filePath), verifyResult.Error);
                File.Copy(backupPath, filePath, overwrite: true);
                File.Delete(backupPath);
                return Result.Failure($"Validation failed: {verifyResult.Error}");
            }

            // Cleanup backup on success
            File.Delete(backupPath);
            _logger.LogInformation("✅ Saved {FileName}", Path.GetFileName(filePath));

            return Result.Success();
        }
        catch (Exception ex)
        {
            // Rollback on exception
            _logger.LogError(ex, "Failed to update version in {FilePath}", filePath);
            
            if (File.Exists(backupPath))
            {
                try
                {
                    File.Copy(backupPath, filePath, overwrite: true);
                    File.Delete(backupPath);
                    _logger.LogInformation("Restored {FileName} from backup", Path.GetFileName(filePath));
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to restore backup for {FilePath}", filePath);
                }
            }
            
            return Result.Failure($"Failed to update version: {ex.Message}");
        }
    }

    private async Task<Result> VerifyVersionUpdate(string filePath, string expectedVersion)
    {
        try
        {
            // Step 1: Validate XML structure
            var structureValidation = await ValidateXmlStructure(filePath).ConfigureAwait(false);
            if (!structureValidation.IsSuccess)
            {
                return Result.Failure($"XML structure invalid: {structureValidation.Error}");
            }

            // Step 2: Validate version value using standard read method
            var readResult = await ReadVersionAsync(filePath).ConfigureAwait(false);

            if (!readResult.IsSuccess)
            {
                return Result.Failure($"Could not read version: {readResult.Error}");
            }

            var actualVersion = readResult.Value;

            // Step 3: Compare expected vs actual
            if (actualVersion != expectedVersion)
            {
                return Result.Failure(
                    $"Version mismatch - Expected: {expectedVersion}, Actual: {actualVersion}");
            }

            _logger.LogDebug("Validation passed for {File}: {Version}", 
                Path.GetFileName(filePath), actualVersion);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Validation error: {ex.Message}");
        }
    }

    private async Task<Result> ValidateXmlStructure(string filePath)
    {
        try
        {
            // Try to parse XML to verify it's well-formed and valid
            var doc = await Task.Run(() => XDocument.Load(filePath)).ConfigureAwait(false);

            // Verify Project root element exists
            if (doc.Root == null || doc.Root.Name.LocalName != "Project")
            {
                return Result.Failure("Missing or invalid <Project> root element");
            }

            // Verify PropertyGroup exists
            var propertyGroup = doc.Descendants("PropertyGroup").FirstOrDefault();
            if (propertyGroup == null)
            {
                return Result.Failure("No <PropertyGroup> element found");
            }

            // Verify AssemblyVersion exists
            var assemblyVersion = doc.Descendants("AssemblyVersion").FirstOrDefault();
            if (assemblyVersion == null || string.IsNullOrWhiteSpace(assemblyVersion.Value))
            {
                return Result.Failure("<AssemblyVersion> missing or empty");
            }

            // Verify FileVersion exists
            var fileVersion = doc.Descendants("FileVersion").FirstOrDefault();
            if (fileVersion == null || string.IsNullOrWhiteSpace(fileVersion.Value))
            {
                return Result.Failure("<FileVersion> missing or empty");
            }

            // Verify both versions have same value
            if (assemblyVersion.Value != fileVersion.Value)
            {
                return Result.Failure(
                    $"Version mismatch between tags - AssemblyVersion: {assemblyVersion.Value}, " +
                    $"FileVersion: {fileVersion.Value}");
            }

            _logger.LogDebug("XML structure validation passed for {File}", 
                Path.GetFileName(filePath));

            return Result.Success();
        }
        catch (System.Xml.XmlException xmlEx)
        {
            return Result.Failure($"XML parse error: {xmlEx.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Structure validation error: {ex.Message}");
        }
    }

    private (string[] lines, bool modified) UpdateVersionLines(
        string[] lines, 
        string newVersion)
    {
        var result = new List<string>(lines);
        bool assemblyVersionFound = false;
        bool fileVersionFound = false;
        int firstPropertyGroupEnd = -1;

        for (int i = 0; i < result.Count; i++)
        {
            var line = result[i];

            // Find and replace <AssemblyVersion>
            if (line.Contains("<AssemblyVersion>") && line.Contains("</AssemblyVersion>"))
            {
                var indent = GetIndentation(line);
                result[i] = $"{indent}<AssemblyVersion>{newVersion}</AssemblyVersion>";
                assemblyVersionFound = true;
            }

            // Find and replace <FileVersion>
            if (line.Contains("<FileVersion>") && line.Contains("</FileVersion>"))
            {
                var indent = GetIndentation(line);
                result[i] = $"{indent}<FileVersion>{newVersion}</FileVersion>";
                fileVersionFound = true;
            }

            // Track first PropertyGroup for insertion
            if (line.Contains("</PropertyGroup>") && firstPropertyGroupEnd == -1)
            {
                firstPropertyGroupEnd = i;
            }
        }

        // Insert <FileVersion> if not found
        if (assemblyVersionFound && !fileVersionFound && firstPropertyGroupEnd > 0)
        {
            var assemblyVersionLine = result.FindIndex(l => 
                l.Contains("<AssemblyVersion>"));
            var indent = GetIndentation(result[assemblyVersionLine]);
            result.Insert(assemblyVersionLine + 1, 
                $"{indent}<FileVersion>{newVersion}</FileVersion>");
            fileVersionFound = true;
            _logger.LogWarning("Inserted missing <FileVersion> tag in {File}", "file");
        }

        return (result.ToArray(), assemblyVersionFound || fileVersionFound);
    }

    private string GetIndentation(string line)
    {
        var match = Regex.Match(line, @"^(\s*)");
        return match.Success ? match.Groups[1].Value : "    ";
    }

    private Encoding DetectEncoding(string filePath)
    {
        // Read BOM to detect encoding
        using var reader = new StreamReader(filePath, true);
        reader.Peek();
        return reader.CurrentEncoding;
    }

    private string DetectLineEnding(string filePath)
    {
        var content = File.ReadAllText(filePath);
        if (content.Contains("\r\n")) return "\r\n"; // Windows
        if (content.Contains("\n")) return "\n";     // Unix
        return Environment.NewLine;                   // Default
    }
}
