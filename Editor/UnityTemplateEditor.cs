using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YogurtTheHorse.Unity.Templating.Editor.Progression;

namespace YogurtTheHorse.Unity.Templating.Editor
{
    [CustomEditor(typeof(UnityTemplate))]
    public class UnityTemplateEditor : UnityEditor.Editor
    {
        private UnityTemplate _template;

        private bool _createFoldersSelected = true;
        private bool _copyFilesSelected = true;
        private bool _installRegistriesSelected = true;
        private bool _installPackagesSelected = true;

        private float _progress = 0;
        private string _progressState = "Idle";
        private bool _isInstalling = false;

        private void OnEnable()
        {
            _template = (UnityTemplate)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            GUILayout.Label("Installation Options", EditorStyles.boldLabel);

            _createFoldersSelected = EditorGUILayout.Toggle("Create Folders", _createFoldersSelected);
            _copyFilesSelected = EditorGUILayout.Toggle("Copy files", _copyFilesSelected);
            _installRegistriesSelected = EditorGUILayout.Toggle("Install Registries", _installRegistriesSelected);
            _installPackagesSelected = EditorGUILayout.Toggle("Install Packages", _installPackagesSelected);

            GUILayout.Space(20);

            if (_isInstalling)
            {
                EditorGUILayout.LabelField("Progress", _progressState);

                var rect = GUILayoutUtility.GetRect(18, 18, "TextField");
                EditorGUI.ProgressBar(rect, _progress, $"{_progress * 100:0.0}%");

                GUILayout.Space(20);
            }
            else
            {
                if (GUILayout.Button("Install"))
                {
                    InstallTemplateStepByStep();
                }
            }
        }

        private void InstallTemplateStepByStep()
        {
            _isInstalling = true;
            _progress = 0;
            _progressState = "Starting Installation...";

            var installList = new List<Func<IProgress>>();

            if (_createFoldersSelected)
            {
                installList.Add(() => TemplateInstaller.SetupFolders(_template.folders, _template.createGitKeepFiles));
            }

            if (_copyFilesSelected)
            {
                installList.Add(() => TemplateInstaller.CopyFiles(_template.files, _template.overwriteFiles));
            }

            if (_installRegistriesSelected)
            {
                installList.Add(() => TemplateInstaller.SetupScopedRegistries(_template.scopedRegistries));
            }

            if (_installPackagesSelected)
            {
                installList.Add(() => TemplateInstaller.SetupRequiredPackages(_template.requiredPackages));
            }

            Install(installList);
        }

        private void Install(List<Func<IProgress>> installList)
        {
            IProgress current = null;

            EditorApplication.update += UpdateProgress;

            void UpdateProgress()
            {
                if (current == null || current.IsCompleted)
                {
                    if (installList.Count == 0)
                    {
                        Debug.Log("Installation completed");

                        _progressState = "Installation Completed";
                        _progress = 1.0f;
                        _isInstalling = false;

                        EditorApplication.update -= UpdateProgress;
                        Repaint();
                        return;
                    }

                    current = installList[0]();
                    installList.RemoveAt(0);
                }
                else
                {
                    _progress = current.Progress;
                    _progressState = current.State;
                    Repaint();
                }
            }
        }
    }
}