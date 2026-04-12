# Testing Guidelines

This document provides guidelines and best practices for writing tests in the Lifes project.

## General Principles

1.  **Test the "What", not the "How"**: Focus on testing behavior and business rules rather than implementation details.
2.  **Keep Tests Fast**: Unit tests should be extremely fast. If a test is slow, it might be an integration test.
3.  **One Concept per Test**: Each test should verify a single behavior or scenario.
4.  **No Logic in Tests**: Avoid if-statements, loops, or complex calculations in test code.

## Test Naming Convention

We use the following pattern: `MethodUnderTest_Scenario_ExpectedResult`

*   **Good**: `Increment_SameDay_BuildShouldBeIncremented`
*   **Good**: `ScanProjectsAsync_PathNotExists_ShouldReturnFailure`
*   **Bad**: `Test1`, `CheckVersion`, `MyTest`

## AAA Pattern (Arrange, Act, Assert)

All tests should follow the AAA structure:

```csharp
[Fact]
public void MatchesFilter_ValidEtlProject_ShouldReturnTrue()
{
    // Arrange
    var projectFile = new ProjectFile { FileName = "MyProject.ETL.csproj" };

    // Act
    var result = projectFile.MatchesFilter();

    // Assert
    result.Should().BeTrue();
}
```

## Mocking Strategy

*   Use `Moq` for all external dependencies in Unit Tests.
*   Centralize common mocks in `AppMockFactory.cs`.
*   Always use `It.IsAny<T>()` for parameters that aren't relevant to the specific test scenario.
*   Verify method calls only when necessary (e.g., in commands to ensure a service was called).

## Assertions

Use `FluentAssertions` for more readable and expressive assertions:

*   `result.Should().BeTrue();`
*   `list.Should().HaveCount(3);`
*   `action.Should().Throw<ArgumentNullException>();`

## Integration Testing

*   Use `FileSystemTestHelper` to create a controlled environment for tests that interact with the disk.
*   Always clean up after tests (use `IDisposable` or `try-finally`).
*   Mock components that are too slow or have side effects (like Git operations), but test the actual logic of the component under test.

## Coverage Requirements

We aim for high coverage on core logic:
*   **Domain**: High priority (>= 80%)
*   **Application**: Medium priority (>= 70%)
*   **Infrastructure**: Focused on core services (>= 60%)
*   **Presentation**: Focused on ViewModel logic (>= 50%)
