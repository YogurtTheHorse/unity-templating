using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace YogurtTheHorse.Unity.Templating
{
    public static class TemplateInstaller
    {
        public static void SetupFolders(UnityTemplate template)
        {
        }

        public static void SetupScopedRegistries(UnityTemplate template)
        {
            foreach (var registry in template.scopedRegistries)
            {
                AddScopedRegistry(registry);
            }
        }

        public static void SetupRequiredPackages(UnityTemplate template)
        {
            Client.AddAndRemove(template.requiredPackages);
        }

        private static Request<RegistryInfo> AddScopedRegistry(UnityTemplate.ScopedRegistry registry) =>
            (Request<RegistryInfo>)typeof(Client)
                .GetMethods(BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
                .First(m => m.Name == "AddScopedRegistry" &&
                            typeof(Request<RegistryInfo>).IsAssignableFrom(m.ReturnType))
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