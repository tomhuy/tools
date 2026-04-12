using Lifes.Core.Interfaces;
using Moq;

namespace Lifes.Application.Tests.TestUtilities;

public static class AppMockFactory
{
    public static Mock<IProjectScanner> CreateProjectScannerMock()
    {
        return new Mock<IProjectScanner>();
    }

    public static Mock<IProjectFileService> CreateProjectFileServiceMock()
    {
        return new Mock<IProjectFileService>();
    }

    public static Mock<IVersionService> CreateVersionServiceMock()
    {
        return new Mock<IVersionService>();
    }

    public static Mock<ISettingsService> CreateSettingsServiceMock()
    {
        return new Mock<ISettingsService>();
    }
}
