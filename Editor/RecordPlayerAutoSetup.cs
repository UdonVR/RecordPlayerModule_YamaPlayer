using System.Collections;
using System.Collections.Generic;
using UdonVR.RecordPlayer;
using UdonVR.RecordPlayer.Yamaplayer;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;
using Yamadev.YamaStream;

namespace com.udonvr.recordplayer.yamaplayerEditor
{
    public class RecordPlayerAutoSetup : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => -9999;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            return true;
        }

        [PostProcessScene(-9999)]
        private static void ProcessScene()
        {
            RecordPlayer recordPlayer = GameObject.FindObjectOfType<RecordPlayer>();
            Controller _yamaController = GameObject.FindObjectOfType<Controller>();
            if (recordPlayer == null)
            {
                Debug.LogWarning("[UdonVR][YamaPlayer AutoSetup] Record Player was not found");
                return;
            }

            if (_yamaController == null)
            {
                Debug.LogError("[UdonVR][YamaPlayer AutoSetup] YamaPlayer Controller was not found");
                return;
            }

            if (recordPlayer.videoplayerModule != null && recordPlayer.videoplayerModule.GetType() != typeof(RecordPlayer_YamaPlayer))
            {
                if (EditorUtility.DisplayDialog($"Existing Module found", $"There was a Module found attached to the record player, but it wasn't YamaPlayer. Do you want to continue YamaPlayer Auto Setup?", $"Yes", "No"))
                {

                }
                else
                {
                    Debug.LogError($"[UdonVR][YamaPlayer AutoSetup] Module found [{recordPlayer.videoplayerModule.name}] but is not for YamaPlayer, Canceling AutoSetup");
                    return;
                }
            }

            RecordPlayer_VideoplayerClass module = GameObject.FindObjectOfType<RecordPlayer_YamaPlayer>();
            if (module == null)
            {
                var loadedObject = AssetDatabase.LoadAssetAtPath("Packages/com.udonvr.recordplayer.yamaplayer/RecordplayerModule-YamaPlayer.prefab", typeof(UnityEngine.Object));
                if (loadedObject == null)
                {
                    Debug.LogError("[UdonVR][YamaPlayer AutoSetup] No Videoplayer module found");
                    return;
                }
                if (!((GameObject)PrefabUtility.InstantiatePrefab(loadedObject)).transform.TryGetComponent(out module))
                {
                    Debug.LogError("[UdonVR][YamaPlayer AutoSetup] No Videoplayer module found, failed to instantiate");
                    return;
                }
                else
                {
                    Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Instantiate YamaPlayer videoplayer module: [{module.name}]");
                    recordPlayer.videoplayerModule = module;
                    if (module.GetType() != typeof(RecordPlayer_YamaPlayer))
                    {
                        Debug.LogError("[UdonVR][YamaPlayer AutoSetup] Created module found but is not for YamaPlayer, Canceling AutoSetup");
                        return;
                    }
                }
            }
            else if (recordPlayer.videoplayerModule == null)
            {
                recordPlayer.videoplayerModule = module;
            }

            if (module.recordPlayer == null)
            {
                module.recordPlayer = recordPlayer;
                Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Module's recordPlayer Reference was null, binding to [{recordPlayer.name}]");
            }
            else
            {
                Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Module's recordPlayer Reference already filled in, skipping");
            }

            RecordPlayer_YamaPlayer yamaModule = (RecordPlayer_YamaPlayer)module;

            if (yamaModule.yamaController == null)
            {
                yamaModule.yamaController = _yamaController;
                Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Module's YamaPlayer Controller Reference was null, binding to [{_yamaController.name}]");
            }
            else
            {
                Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Module's YamaPlayer Controller Reference already filled in, skipping");
            }

            // Bind companion RecordPlayer_YamaPlayer_Listener
            if (yamaModule.yamaListener == null)
            {
                if (!yamaModule.TryGetComponent(out yamaModule.yamaListener))
                {
                    yamaModule.yamaListener = GameObject.FindObjectOfType<RecordPlayer_YamaPlayer_Listener>();
                }

                if (yamaModule.yamaListener != null)
                {
                    Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Module's yamaListener Reference was null, binding to [{yamaModule.yamaListener.name}]");
                }
                else
                {
                    Debug.LogError("[UdonVR][YamaPlayer AutoSetup] RecordPlayer_YamaPlayer_Listener was not found on module or in scene");
                }
            }

            if (yamaModule.yamaListener != null && yamaModule.yamaListener.recordPlayerProxy == null)
            {
                yamaModule.yamaListener.recordPlayerProxy = yamaModule;
                Debug.Log($"[UdonVR][YamaPlayer AutoSetup] Listener's recordPlayerProxy Reference was null, binding to [{yamaModule.name}]");
            }
        }
    }
}