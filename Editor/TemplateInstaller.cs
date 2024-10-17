using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
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
        public static IProgress CopyAssets(UnityTemplate.AssetsToCopy[] assetsToCopies, string relativeDirectory) =>
            new LambdaProgress(
                "Copy files", () =>
                {
                    var rootDirectory = Directory.GetParent(Application.dataPath)!.FullName;

                    foreach (var assetToCopy in assetsToCopies)
                    {
                        // todo: add glob support somehow...
                        switch (assetToCopy.type)
                        {
                            case UnityTemplate.AssetsToCopy.AssetType.UnityAsset:
                                AssetDatabase.CopyAsset(assetToCopy.source, assetToCopy.destination);
                                break;

                            case UnityTemplate.AssetsToCopy.AssetType.RelativeFile:
                                var absoluteDestination = Path.Combine(rootDirectory, assetToCopy.destination);
                                var absoluteSource = Path.Combine(relativeDirectory, assetToCopy.source);
                                var isDirectory = assetToCopy.destination.EndsWith(Path.DirectorySeparatorChar)
                                                  || assetToCopy.destination.EndsWith(Path.AltDirectorySeparatorChar);
                                var fileDestination = isDirectory
                                    ? Path.Join(absoluteDestination, Path.GetFileName(assetToCopy.source))
                                    : absoluteDestination;

                                var parentDirectory = Path.GetDirectoryName(fileDestination);
                                if (!Directory.Exists(parentDirectory))
                                {
                                    Directory.CreateDirectory(parentDirectory!);
                                }

                                Debug.Log($"Copying {absoluteSource} to {fileDestination}");
                                File.Copy(absoluteSource, fileDestination, true);
                                break;
                        }
                    }

                    return 1;
                }
            );

        public static IProgress SetupScopedRegistries(UnityTemplate.ScopedRegistry[] scopedRegistries) =>
            new ParallelProgress(
                "Install scopes",
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