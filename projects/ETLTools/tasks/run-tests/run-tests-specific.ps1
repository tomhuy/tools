# ============================================
# Lifes - Run Specific Test Project
# ============================================
# Run tests for a specific test project
# ============================================

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Domain", "Application", "Infrastructure", "Presentation", "Integration", "All")]
    [string]$Project,
    
    [switch]$WithCoverage,
    [string]$Filter = ""
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "🎯 Lifes - Specific Test Runner" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Map project names to paths (relative to project root)
$projectPaths = @{
    "Domain" = "tests\Lifes.Domain.Tests"
    "Application" = "tests\Lifes.Application.Tests"
    "Infrastructure" = "tests\Lifes.Infrastructure.Tests"
    "Presentation" = "tests\Lifes.Presentation.WPF.Tests"
    "Integration" = "tests\Lifes.Integration.Tests"
}

if ($Project -eq "All") {
    Write-Host "🧪 Running all test projects..." -ForegroundColor Yellow
    $runCoverageScript = Join-Path $scriptDir "run-tests-with-coverage.ps1"
    $runQuickScript = Join-Path $scriptDir "run-tests-quick.ps1"
    
    if ($WithCoverage) {
        & $runCoverageScript -Filter $Filter
    } else {
        & $runQuickScript -Filter $Filter
    }
    exit
}

$projectPath = Join-Path $projectRoot $projectPaths[$Project]

Write-Host "📁 Test Project: $Project" -ForegroundColor Yellow
Write-Host "   Path: $projectPath" -ForegroundColor Gray
if ($Filter -ne "") {
    Write-Host "   Filter: $Filter" -ForegroundColor Gray
}
Write-Host ""

if (-not (Test-Path $projectPath)) {
    Write-Host "❌ Test project not found: $projectPath" -ForegroundColor Red
    Write-Host ""
    exit 1
}

# Change to project root
Push-Location $projectRoot

try {
    Write-Host "🧪 Running tests..." -ForegroundColor Yellow
    Write-Host ""

    $testResultsDir = Join-Path $projectRoot "TestResults"
    $coverageReportDir = Join-Path $projectRoot "CoverageReport"
    $runsettingsPath = Join-Path $projectRoot "coverlet.runsettings"

    if ($WithCoverage) {
        # Clean previous results
        if (Test-Path $testResultsDir) {
            Remove-Item -Recurse -Force $testResultsDir
        }
        if (Test-Path $coverageReportDir) {
            Remove-Item -Recurse -Force $coverageReportDir
        }
        
        # Run with coverage
        $testCommand = "dotnet test `"$projectPath`" --collect:`"XPlat Code Coverage`" --results-directory `"$testResultsDir`" --settings `"$runsettingsPath`""
        if ($Filter -ne "") {
            $testCommand += " --filter `"$Filter`""
        }
        
        Invoke-Expression $testCommand
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✅ Tests passed!" -ForegroundColor Green
            Write-Host ""
            
            # Generate report
            Write-Host "📊 Generating coverage report..." -ForegroundColor Yellow
            $reportsPattern = Join-Path $testResultsDir "**\coverage.cobertura.xml"
            reportgenerator `
                -reports:"$reportsPattern" `
                -targetdir:"$coverageReportDir" `
                -reporttypes:"Html;HtmlSummary;TextSummary"
            
            Write-Host "✅ Report generated!" -ForegroundColor Green
            Write-Host ""
            
            # Display summary
            $summaryFile = Join-Path $coverageReportDir "Summary.txt"
            if (Test-Path $summaryFile) {
                Write-Host "📈 Coverage Summary:" -ForegroundColor Cyan
                Get-Content $summaryFile
                Write-Host ""
            }
            
            # Open report
            $reportIndexPath = Join-Path $coverageReportDir "index.html"
            Start-Process $reportIndexPath
        } else {
            Write-Host ""
            Write-Host "❌ Tests failed!" -ForegroundColor Red
            Write-Host ""
        }
    } else {
        # Run without coverage
        if ($Filter -ne "") {
            dotnet test "$projectPath" --filter "$Filter" --verbosity normal
        } else {
            dotnet test "$projectPath" --verbosity normal
        }
        
        Write-Host ""
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Tests passed!" -ForegroundColor Green
        } else {
            Write-Host "❌ Tests failed!" -ForegroundColor Red
        }
    }

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
}
finally {
    # Return to original directory
    Pop-Location
}
