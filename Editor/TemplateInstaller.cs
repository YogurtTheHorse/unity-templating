using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using YogurtTheHorse.Unity.Templating.Editor.Progression;

namespace YogurtTheHorse.Unity.Templating.Editor
{
    public static class TemplateInstaller
    {
        // todo: make it a coroutine
        public static IProgress SetupFolders(string[] folders, bool withGitKeep)
        {
            return new LambdaProgress("Add folders", () =>
            {
                foreach (var folder in folders)
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    var gitKeepPath = Path.Combine(folder, ".gitkeep");

                    if (withGitKeep && !File.Exists(gitKeepPath))
                    {
                        File.WriteAllText(gitKeepPath, "");
                    }
                }

                return 1;
            });
        }

        // todo: make it a coroutine
        public static IProgress CopyFiles(UnityTemplate.FileToCopy[] filesToCopies, bool overwrite) =>
            new LambdaProgress("Copy files", () =>
            {
                foreach (var file in filesToCopies)
                {
                    var source = Path.Combine(file.source);
                    var destination = Path.Combine(Application.dataPath, file.destination);

                    if (!File.Exists(source))
                    {
                        Debug.LogWarning($"File {source} not found");
                        continue;
                    }

                    File.Copy(source, destination, overwrite);
                }

                return 1;
            });

        public static IProgress SetupScopedRegistries(UnityTemplate.ScopedRegistry[] scopedRegistries) =>
            new ParallelProgress("Install scopes",
                scopedRegistries
                    .Select(r =>
                        {
                            var request = AddScopedRegistry(r);

                            return new PackageRequestProgress<Request<RegistryInfo>>(
                                "Add scoped registry " + r.name,
                                request
                            );
                        }
                    )
            );

        public static IProgress SetupRequiredPackages(string[] packages) =>
            new PackageRequestProgress<AddAndRemoveRequest>("Install packages", Client.AddAndRemove(packages));

        private static Request<RegistryInfo> AddScopedRegistry(UnityTemplate.ScopedRegistry registry) =>
            (Request<RegistryInfo>)typeof(Client)
                .GetMethods(BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
                .First(m =>
                    m.Name == "AddScopedRegistry" &&
                    typeof(Request<RegistryInfo>).IsAssignableFrom(m.ReturnType)
                )
                .Invoke(
                    null,
                    new object[]
                    {
                        registry.name,
                        registry.url,
                        registry.scopes
                    }
                );
    }
}