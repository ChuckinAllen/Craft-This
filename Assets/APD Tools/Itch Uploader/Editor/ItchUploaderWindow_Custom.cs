using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using aPrioriDigital.Unity.Tooling.Itch;
using UnityEditor;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Threading;
using System;
using System.Collections.Concurrent;

public class ItchUploaderWindow_Custom : EditorWindow
{
    private ItchUploaderWindow_Custom instance;
    private Texture2D logo;
    private ItchUploader uploader;
    private bool isLoading = false;
    private bool hasButler = false, isLoggedIn = false, uploading = false, uploadSuccessful = false, uploadFailed = false;
    private bool invalidGame = false, invalidUser = false;
    private bool enableAdvanced = false;
    private bool isWebBuild = false;
    private ItchUploader.UploadArgs uploadArgs = new ItchUploader.UploadArgs();
    private ItchUploader.UploadArgsValidation validationState;

    private ConcurrentQueue<string> uploadOutput = new ConcurrentQueue<string>();
    private const int MAX_UPLOAD_OUTPUT = 6;

    [MenuItem("Tools/APD/Builds/Itch Uploader Custom")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ItchUploaderWindow_Custom window = (ItchUploaderWindow_Custom)EditorWindow.GetWindow(typeof(ItchUploaderWindow_Custom), false, "Itch Uploader Custom");
        window.instance = window;
        window.maxSize = new Vector2(350f, 500f);
        window.minSize = window.maxSize;
        window.logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/APD Tools/Itch Uploader/Resources/ibu_banner.png", typeof(Texture2D));
        window.uploader = new ItchUploader(Debug.Log);
        window.uploader.StateChanged += window.Uploader_StateChanged;
        window.Show();
    }

    new void Show()
    {
        base.Show();
        Task.Run(HasButler).ContinueWith((t) => Refresh(), TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void Uploader_StateChanged(object sender, ItchUploader.StateChangedEventArgs e)
    {
        isLoading = e.IsLoading;
    }

    void Refresh()
    {
        instance.Focus();
        instance.Repaint();
        instance.OnGUI();
    }

    void OnGUI()
    {
        // Check if logo has unloaded and if so reload
        if (logo == null)
        {
            logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/APD Tools/Itch Uploader/Resources/ibu_banner.png", typeof(Texture2D));
        }
        EditorGUI.DrawPreviewTexture(new Rect(0, 0, position.width, 128), logo);

        // Add spacing for formatting reasons
        for (int i = 0; i < 22; i++)
        {
            EditorGUILayout.Space();
        }

        if (uploading || uploadSuccessful || uploadFailed)
        {
            GUILayout.BeginVertical();
            
            foreach (var s in uploadOutput)
            {
                GUILayout.Label(s, EditorStyles.wordWrappedLabel);
            }
            
            GUILayout.EndVertical();

            EditorGUILayout.Separator();

            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 14, fontStyle = FontStyle.Bold };
            if (uploadSuccessful)
            {
                GUILayout.Label("Upload Successful", style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (GUILayout.Button("Close Window"))
                {
                    instance.Close();
                }
            }

            if(uploadFailed)
            {
                GUILayout.Label("Upload Failed", style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (GUILayout.Button("Go Back"))
                {
                    hasButler = true;
                    isLoggedIn = true;
                    uploading = false;
                    uploadSuccessful = false;
                    uploadFailed = false;
                    while(uploadOutput.Count > 0)
                    {
                        uploadOutput.TryDequeue(out _);
                    }
                }
            }
        }
        else if (isLoading)
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 24, fontStyle = FontStyle.Bold };
            GUILayout.Label("Loading", style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }
        else
        {
            if (!isLoggedIn)
            {
                if (GUILayout.Button("Login"))
                {
                    Task.Run(Login).ContinueWith((t) => Refresh(), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
            {
                DisplayUploaderGUI();
                EditorGUILayout.Separator();
                if (GUILayout.Button("Logout"))
                {
                    Task.Run(Logout).ContinueWith((t) => Refresh(), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }

    #region UploadFormGUI
    public void DisplayUploaderGUI()
    {
        DisplayGeneralSection();

        DisplayAdvancedSection();

        DisplayUploadSection();
    }

    private void DisplayGeneralSection()
    {
        if (validationState?.Errors.Contains(nameof(uploadArgs.Path)) ?? false)
        {
            EditorGUILayout.HelpBox("Please select a build", MessageType.Error);
        }
        if (uploadArgs.Path.Length > 0)
        {
            GUILayout.Label($"{uploadArgs.Path}/{uploadArgs.ZipFile}", EditorStyles.wordWrappedLabel);
        }
        if (GUILayout.Button("Select Build"))
        {
            uploadArgs.Path = EditorUtility.OpenFilePanel("Select build to upload", Application.dataPath, "zip");
            if (uploadArgs.Path.Length == 0)
            {
                return;
            }
            uploadArgs.BuildDirectory = Path.GetDirectoryName(uploadArgs.Path) + "/";
            uploadArgs.ZipFile = Path.GetFileName(uploadArgs.Path);
        }


        if (validationState?.Errors.Contains(nameof(uploadArgs.UserName)) ?? false)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please enter your username", MessageType.Error);
            EditorGUILayout.Space();
        }
        if (invalidUser)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Username was invalid", MessageType.Error);
            EditorGUILayout.Space();
        }
        uploadArgs.UserName = EditorGUILayout.TextField("User Name: ", uploadArgs.UserName.ToLower());


        if (validationState?.Errors.Contains(nameof(uploadArgs.GameName)) ?? false)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please enter the name of the game", MessageType.Error);
            EditorGUILayout.Space();
        }
        if (invalidGame)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Game was invalid", MessageType.Error);
            EditorGUILayout.Space();
        }
        uploadArgs.GameName = EditorGUILayout.TextField("Game Name: ", uploadArgs.GameName.ToLower());

        if (isWebBuild = EditorGUILayout.Toggle("Web Build", isWebBuild))
        {
            Debug.Log("WebBuild");

            //validationState?.Errors.Contains(nameof(uploadArgs.GameName));
            //return;
        }
        else
        {
            uploadArgs.Channel = (ItchChannel)EditorGUILayout.EnumPopup("Channel:", uploadArgs.Channel);
        }
    }

    private void DisplayAdvancedSection()
    {
        EditorGUILayout.Space();

        enableAdvanced = EditorGUILayout.Foldout(enableAdvanced, "Advanced Settings");

        if (enableAdvanced)
        {
            uploadArgs.VersionId = EditorGUILayout.TextField("Version ID: ", uploadArgs.VersionId);
        }
    }

    private void DisplayUploadSection()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("Upload", GUILayout.Height(50)))
        {
            // Check variables are valid
            if (uploadArgs.Path.Length == 0 || uploadArgs.BuildDirectory.Length == 0 || uploadArgs.ZipFile.Length == 0
                || uploadArgs.UserName.Length == 0 || uploadArgs.GameName.Length == 0) { return; }

            if (enableAdvanced && uploadArgs.VersionId.Length == 0) { return; }

            isLoading = true;
            Task.Run(Upload).ContinueWith((t) => Refresh(), TaskScheduler.FromCurrentSynchronizationContext());
        }
    } 
    #endregion

    #region Tasks
    async Task Login()
    {
        var result = await uploader.Login();
        isLoggedIn = result;
    }

    async Task Logout()
    {
        var result = await uploader.Logout();
        isLoggedIn = !result;
    }

    async Task HasButler()
    {
        var result = await uploader.ButlerInstalled();
        hasButler = result;
    }

    async Task Upload()
    {

        invalidGame = invalidUser = false;
        validationState = uploader.ValidateUploadArgs(uploadArgs);

        if (validationState.Errors.Count > 0)
            return;

        uploader.OutputDataReceived += Uploader_DataReceived;
        uploader.ErrorDataReceived += Uploader_DataReceived;


        uploading = true;
        var result = await uploader.Upload(uploadArgs);

        validationState = null;
        uploadFailed = !(uploadSuccessful = result);
    } 
    #endregion

    private void Uploader_DataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
    {
        if (!String.IsNullOrEmpty(e.Data.Trim()))
        {
            uploadOutput.Enqueue(e.Data.Trim());
            if (uploadOutput.Count > MAX_UPLOAD_OUTPUT)
            {
                while (!uploadOutput.TryDequeue(out _) && uploadOutput.Count > MAX_UPLOAD_OUTPUT) { }
            }
            if(e.Data.Trim().Contains("invalid game"))
            {
                invalidGame = true;
            }
            if(e.Data.Trim().Contains("invalid target (bad user)"))
            {
                invalidUser = true;
            }
        }
        Task.Run(() => Task.Run(() => { }).ContinueWith((t) => Repaint(), TaskScheduler.FromCurrentSynchronizationContext()));
    }
}
