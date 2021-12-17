using GitVersion.Logging;
using GitVersion.OutputVariables;

namespace GitVersion.BuildAgents;

public class BuildKite : BuildAgentBase
{
    public BuildKite(IEnvironment environment, ILog log) : base(environment, log)
    {
    }

    public const string EnvironmentVariableName = "BUILDKITE";

    protected override string EnvironmentVariable { get; } = EnvironmentVariableName;

    public override bool CanApplyToCurrentContext()
    {
        Console.WriteLine($"The BUILDKITE environment variable={Environment.GetEnvironmentVariable(EnvironmentVariable)}");
        Console.WriteLine($"The BUILDKITE_BRANCH environment variable={Environment.GetEnvironmentVariable("BUILDKITE_BRANCH")}");
        Console.WriteLine($"The BUILDKITE_TAG environment variable={Environment.GetEnvironmentVariable("BUILDKITE_BRANCH")}");
        Console.WriteLine($"The BUILDKITE_PULL_REQUEST environment variable={Environment.GetEnvironmentVariable("BUILDKITE_PULL_REQUEST")}");
        return Environment.GetEnvironmentVariable(EnvironmentVariable)?.Trim('"').Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    
    public override string GenerateSetVersionMessage(VersionVariables variables) =>
        string.Empty; // There is no equivalent function in BuildKite.

    public override string[] GenerateSetParameterMessage(string name, string value) =>
        Array.Empty<string>(); // There is no equivalent function in BuildKite.

    // TODO could be nice to add a WriteIntegration() to write the version variables out as build meta data
    // https://buildkite.com/docs/pipelines/build-meta-data
    
    public override string? GetCurrentBranch(bool usingDynamicRepos)
    {
        var currentBranch = Environment.GetEnvironmentVariable("BUILDKITE_BRANCH") ??
                           Environment.GetEnvironmentVariable("BUILDKITE_TAG") ??
                           Environment.GetEnvironmentVariable("BUILDKITE_PULL_REQUEST");

        Console.WriteLine($"The BuildKite Agent thinks the current branch is: {currentBranch}");
        return currentBranch?.Trim('"');
    }
        

    public override bool PreventFetch() => true;
}