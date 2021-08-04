using Cake.Frosting;
using Common.Utilities;

namespace Artifacts.Tasks
{
    [TaskName(nameof(ArtifactsMsBuildCoreTest))]
    [TaskDescription("Tests the msbuild package in docker container")]
    [IsDependentOn(typeof(ArtifactsPrepare))]
    public class ArtifactsMsBuildCoreTest : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            var shouldRun = true;
            shouldRun &= context.ShouldRun(context.IsDockerOnLinux, $"{nameof(ArtifactsMsBuildCoreTest)} works only on Docker on Linux agents.");

            return shouldRun;
        }

        public override void Run(BuildContext context)
        {
            if (context.Version == null)
                return;
            var rootPrefix = string.Empty;
            var version = context.Version.NugetVersion;

            foreach (var dockerImage in context.Images)
            {
                var (distro, targetFramework) = dockerImage;

                if (targetFramework == Constants.Version31 && distro == "centos.8-x64") continue; // TODO check why this one fails
                targetFramework = targetFramework switch
                {
                    Constants.Version31 => $"netcoreapp{targetFramework}",
                    Constants.Version50 => $"net{targetFramework}",
                    _ => targetFramework
                };

                var cmd = $"-file {rootPrefix}/scripts/Test-MsBuildCore.ps1 -version {version} -repoPath {rootPrefix}/repo/tests/integration/core -nugetPath {rootPrefix}/nuget -targetframework {targetFramework}";

                context.DockerTestArtifact(dockerImage, cmd, Constants.GitHubContainerRegistry);
            }
        }
    }
}