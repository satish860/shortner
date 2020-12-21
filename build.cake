#addin "nuget:?package=Cake.Docker&version=0.11.0"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument("packageVersion", "0.0.1");
var prerelease = Argument("prerelease", "");

var buildNumber = EnvironmentVariable<int>("GITHUB_RUN_NUMBER",1);
var IsCI = EnvironmentVariable<bool>("CI",false)
var buildKey = "LOCAL-BUILD";
var uniqueTag = DateTime.UtcNow.ToString("yyyy.MM.dd") + "." + buildNumber;

class ProjectInformation
{
    public string Name { get; set; }
    public string FullPath { get; set; }
    public string Runtime { get; set; }
    public bool IsTestProject { get; set; }
}

string packageVersion;
List<ProjectInformation> projects;

Setup(context =>
{
    if (BuildSystem.IsLocalBuild && string.IsNullOrEmpty(prerelease))
    {
        prerelease = "-local";
    }

    packageVersion = $"{version}{prerelease}";

    projects = GetFiles("./**/*.csproj").Select(p => new ProjectInformation
    {
        Name = p.GetFilenameWithoutExtension().ToString(),
        FullPath = p.GetDirectory().FullPath,
        Runtime = null,
        IsTestProject = p.GetFilenameWithoutExtension().ToString().EndsWith(".Tests")
    }).ToList();

    Information("Building Shortner v{0}", packageVersion);
});

Task("Clean")
    .Does(() =>
        {
            CleanDirectory("publish");
            CleanDirectory("package");

            var cleanSettings = new DotNetCoreCleanSettings { Configuration = configuration };

            foreach(var project in projects)
            {
                DotNetCoreClean(project.FullPath, cleanSettings);
            }
        });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        foreach(var project in projects)
        {
            var restoreSettings = new DotNetCoreRestoreSettings();

            if (!string.IsNullOrEmpty(project.Runtime))
            {
                restoreSettings.Runtime = project.Runtime;
            }

            DotNetCoreRestore(project.FullPath, restoreSettings);
        }
    });

    Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        foreach(var project in projects)
        {
            var buildSettings = new DotNetCoreBuildSettings()
                {
                    Configuration = configuration,
                    NoRestore = true
                };

            if (!string.IsNullOrEmpty(project.Runtime))
            {
                buildSettings.Runtime = project.Runtime;
            }

            DotNetCoreBuild(project.FullPath, buildSettings);
        }
    });

Task("RunUnitTests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach(var project in projects.Where(p => p.IsTestProject))
        {
            DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings { Configuration = configuration });
        }
    });

Task("BuildDockerImage")
    .IsDependentOn("RunUnitTests")
    .Does(() =>
{
        var lastCommit = "unspecified";
		using(var process = StartAndReturnProcess("git", new ProcessSettings
		{
			Arguments				= "log -1 --format=%H",
			RedirectStandardError	= true,
			RedirectStandardOutput	= true
		}))
		{
			process.WaitForExit();

			if (process.GetExitCode() != 0)
			{
				var error = string.Join(Environment.NewLine, process.GetStandardError());
				throw new InvalidOperationException(error);
			}

			lastCommit = process.GetStandardOutput().FirstOrDefault();
		}

    
    DockerComposeBuild(new DockerComposeBuildSettings{
        BuildArg	= new[] { $"BUILD_NUMBER={buildNumber}", $"BUILD_KEY={buildKey}", $"GIT_COMMIT={lastCommit}", $"VERSION={uniqueTag}" },
        ForceRm		= true,
		Pull		= true
    });
});

Task("PushImage")
    .IsDependentOn("BuildDockerImage")
    .WithCriteria(() => IsCI)
    .Does(() =>
{
    var finalImageName = $"docker.io/satish860/shortnerapi:{uniqueTag}";
    DockerTag("shortnerapi:latest",finalImageName);
    DockerPush(finalImageName);
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("RunUnitTests")
    .IsDependentOn("BuildDockerImage")
    .IsDependentOn("PushImage")


RunTarget(target);
