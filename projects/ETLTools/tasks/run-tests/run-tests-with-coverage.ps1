# ============================================
# Lifes - Run Tests With Coverage Report
# ============================================

param(
    [switch]$SkipBuild,
    [switch]$SkipReport,
    [string]$Filter = ""
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Lifes - Test Runner with Coverage" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")
$testResultsDir = Join-Path $projectRoot "TestResults"
$coverageReportDir = Join-Path $projectRoot "CoverageReport"
$runsettingsPath = Join-Path $projectRoot "coverlet.runsettings"

# Change to project root
Push-Location $projectRoot

try {
    # Check if reportgenerator is installed
    $reportGenInstalled = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"
    if (-not $reportGenInstalled) {
        Write-Host "ReportGenerator tool not found. Installing..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-reportgenerator-globaltool
        Write-Host "ReportGenerator installed successfully!" -ForegroundColor Green
        Write-Host ""
    }

    # Clean previous results
    Write-Host "Cleaning previous test results..." -ForegroundColor Yellow
    if (Test-Path $testResultsDir) {
        Remove-Item -Recurse -Force $testResultsDir
    }
    if (Test-Path $coverageReportDir) {
        Remove-Item -Recurse -Force $coverageReportDir
    }
    Write-Host "Cleanup complete!" -ForegroundColor Green
    Write-Host ""

    # Build solution if not skipped
    if (-not $SkipBuild) {
        Write-Host "Building solution..." -ForegroundColor Yellow
        dotnet build --configuration Debug
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Build failed!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Build successful!" -ForegroundColor Green
        Write-Host ""
    }

    # Run tests with coverage
    Write-Host "Running tests with code coverage..." -ForegroundColor Yellow
    Write-Host ""

    $testCommand = "dotnet test --no-build --collect:`"XPlat Code Coverage`" --results-directory `"$testResultsDir`" --settings `"$runsettingsPath`""

    if ($Filter -ne "") {
        Write-Host "   Filter: $Filter" -ForegroundColor Gray
        $testCommand += " --filter `"$Filter`""
    }

    Invoke-Expression $testCommand

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "Some tests failed!" -ForegroundColor Red
        Write-Host ""
        
        if (-not $SkipReport) {
            Write-Host "Generating coverage report anyway..." -ForegroundColor Yellow
        }
    } else {
        Write-Host ""
        Write-Host "All tests passed!" -ForegroundColor Green
        Write-Host ""
    }

    # Generate HTML report if not skipped
    if (-not $SkipReport) {
        Write-Host "Generating HTML coverage report..." -ForegroundColor Yellow
        
        $coverageFiles = Get-ChildItem -Path $testResultsDir -Filter "coverage.cobertura.xml" -Recurse
        
        if ($coverageFiles.Count -eq 0) {
            Write-Host "No coverage files found!" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "   Found $($coverageFiles.Count) coverage file(s)" -ForegroundColor Gray
        
        $reportsPattern = Join-Path $testResultsDir "**\coverage.cobertura.xml"
        reportgenerator -reports:"$reportsPattern" -targetdir:"$coverageReportDir" -reporttypes:"Html;HtmlSummary;Badges;TextSummary"
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to generate coverage report!" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "Coverage report generated!" -ForegroundColor Green
        Write-Host ""
        
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Coverage Summary" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        
        $summaryFile = Join-Path $coverageReportDir "Summary.txt"
        if (Test-Path $summaryFile) {
            Get-Content $summaryFile | ForEach-Object {
                if ($_ -match "Line coverage:") {
                    Write-Host $_ -ForegroundColor Green
                } elseif ($_ -match "Branch coverage:") {
                    Write-Host $_ -ForegroundColor Yellow
                } else {
                    Write-Host $_
                }
            }
        }
        
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        
        $reportIndexPath = Join-Path $coverageReportDir "index.html"
        Write-Host "Opening coverage report in browser..." -ForegroundColor Cyan
        Start-Process $reportIndexPath
        
        Write-Host ""
        Write-Host "Done!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Report location: $reportIndexPath" -ForegroundColor Gray
        Write-Host "Test results:    $testResultsDir" -ForegroundColor Gray
        Write-Host ""
    } else {
        Write-Host "Skipping report generation" -ForegroundColor Gray
        Write-Host ""
    }

    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
}
finally {
    Pop-Location
}
