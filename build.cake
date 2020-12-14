var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument("packageVersion", "0.0.1");
var prerelease = Argument("prerelease", "");

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

Task("Publish")
    .IsDependentOn("RunUnitTests")
    .Does(() =>
    {
        foreach(var project in projects.Where(p => !p.IsTestProject))
        {
            var publishSettings = new DotNetCorePublishSettings()
                {
                    Configuration = configuration,
                    OutputDirectory = System.IO.Path.Combine("publish", project.Name),
                    ArgumentCustomization = args => args.Append("--no-restore")
                };

            if (!string.IsNullOrEmpty(project.Runtime))
            {
                publishSettings.Runtime = project.Runtime;
            }

            DotNetCorePublish(project.FullPath, publishSettings);
        }
        
    });
Task("Default")
    .IsDependentOn("RunUnitTests");

RunTarget(target);