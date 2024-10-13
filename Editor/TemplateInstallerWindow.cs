using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace YogurtTheHorse.Unity.Templating
{
    public class TemplateInstallerWindow : EditorWindow
    {
        public UnityTemplate templateSettings;


        [MenuItem("Window/SuperUnityBuild")]
        public static void ShowWindow()
        {
            var editorAsm = typeof(Editor).Assembly;
            var inspectorWindowsType = editorAsm.GetType("UnityEditor.InspectorWindow");

            var window = inspectorWindowsType != null
                ? GetWindow<TemplateInstallerWindow>(inspectorWindowsType)
                : GetWindow<TemplateInstallerWindow>();

            window.Show();
        }

        protected void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
            templateSettings = EditorGUILayout.ObjectField(
                "Unity Template",
                templateSettings,
                typeof(UnityTemplate),
                false
            ) as UnityTemplate;
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("Install"))
            {
                TemplateInstaller.SetupFolders(templateSettings);
                TemplateInstaller.SetupScopedRegistries(templateSettings);
                TemplateInstaller.SetupRequiredPackages(templateSettings);
            }

            EditorGUILayout.EndVertical();
        }
    }
}