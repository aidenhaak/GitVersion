using GitVersion.BuildAgents;
using GitVersion.Core.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace GitVersion.Core.Tests.BuildAgents;

[TestFixture]
public class BuildKiteTests : TestBase
{
    private IEnvironment environment;
    private BuildKite buildServer;

    [SetUp]
    public void SetUp()
    {
        var sp = ConfigureServices(services => services.AddSingleton<BuildKite>());
        this.environment = sp.GetService<IEnvironment>();
        this.buildServer = sp.GetService<BuildKite>();
        this.environment.SetEnvironmentVariable(BuildKite.EnvironmentVariableName, "true");
    }

    [TearDown]
    public void TearDown()
    {
        this.environment.SetEnvironmentVariable(BuildKite.EnvironmentVariableName, null);
    }

    [Test]
    public void CanApplyToCurrentContextShouldBeTrueWhenEnvironmentVariableIsSet()
    {
        // Act
        var result = this.buildServer.CanApplyToCurrentContext();

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void CanApplyToCurrentContextShouldBeFalseWhenEnvironmentVariableIsNotSet()
    {
        // Arrange
        this.environment.SetEnvironmentVariable(BuildKite.EnvironmentVariableName, "");

        // Act
        var result = this.buildServer.CanApplyToCurrentContext();

        // Assert
        result.ShouldBeFalse();
    }
    
    [Test]
    public void GetCurrentBranchShouldHandleBranches()
    {
        // Arrange
        this.environment.SetEnvironmentVariable("BUILDKITE_BRANCH", $"refs/heads/{MainBranch}");

        // Act
        var result = this.buildServer.GetCurrentBranch(false);

        // Assert
        result.ShouldBe($"refs/heads/{MainBranch}");
    }

    [Test]
    public void GetCurrentBranchShouldHandleTags()
    {
        // Arrange
        this.environment.SetEnvironmentVariable("BUILDKITE_TAG", "refs/tags/1.0.0");

        // Act
        var result = this.buildServer.GetCurrentBranch(false);

        // Assert
        result.ShouldBe("refs/tags/1.0.0");
    }

    [Test]
    public void GetCurrentBranchShouldHandlePullRequests()
    {
        // Arrange
        this.environment.SetEnvironmentVariable("BUILDKITE_PULL_REQUEST", "refs/pull/1/merge");

        // Act
        var result = this.buildServer.GetCurrentBranch(false);

        // Assert
        result.ShouldBe("refs/pull/1/merge");
    }

    [Test]
    public void GetSetParameterMessage()
    {
        // Assert
        this.environment.GetEnvironmentVariable("GitVersion_Something").ShouldBeNullOrWhiteSpace();

        // Act
        var result = this.buildServer.GenerateSetParameterMessage("GitVersion_Something", "1.0.0");

        // Assert
        result.ShouldContain(s => true, 0);
    }

    [Test]
    public void SkipEmptySetParameterMessage()
    {
        // Act
        var result = this.buildServer.GenerateSetParameterMessage("Hello", string.Empty);

        // Assert
        result.ShouldBeEquivalentTo(Array.Empty<string>());
    }
    
    [Test]
    public void GetEmptyGenerateSetVersionMessage()
    {
        // Arrange
        var vars = new TestableVersionVariables("1.0.0");

        // Act
        var message = this.buildServer.GenerateSetVersionMessage(vars);

        // Assert
        message.ShouldBeEmpty();
    }
}