# UdonVR Record Player Module: YamaPlayer

Compatibility hook bridging the **UdonVR Record Player** (`RecordPlayer_VideoplayerClass`) with **Yamadev YamaStream (YamaPlayer)**. Because `YamaPlayerListener` requires class inheritance, this module is split across two components: `RecordPlayer_YamaPlayer` and `RecordPlayer_YamaPlayer_Listener`.

## Feature Compatibility

| Capability | Status | Implementation Details |
| :--- | :---: | :--- |
| **Editor Auto-Setup** | `Full` | Supported via `RecordPlayerAutoSetup` editor script |
| **Load URL** | `Full` | Calls `yamaController.TakeOwnership()` and `yamaController.PlayTrack(...)` |
| **External Load Detection** | `Full` | `AfterTrackLoaded()` triggers `_OnVideoLoad()` while guarding internal `Stop()` events |
| **Play / Pause / Stop** | `Full` | Uses `yamaController.Play()`, `yamaController.Pause()`, and `yamaController.Stop()` |
| **Seeking & Scrubbing** | `Full` | Calls `yamaController.SetTime(_time)` |
| **Tonearm Progress** | `Full` | Remaps `yamaController.VideoTime` / `yamaController.Duration` (`0..1`) |
| **2-Way Volume Sync** | `Full` | Syncs on `Start()`, `AfterVideoReady()`, and `AfterVolumeChanged(float)` |
| **Permission / Lock Check** | `N/A` | `_CanPlayVideo()` returns `true` |
| **Album Auto-Advance & Shuffle** | `Full` | `AfterVideoEnded()` and `AfterVideoErrorOccurred()` trigger `_PlaybackEnded()` |
| **Single-Track & Chapter Loop** | `Full` | Reloads URL or seeks to `0f` in `_loopCurrentUrl()` with chapter timestamp recovery |
| **Custom Track Titles** | `Partial` | Injects `[RecordPlayer] Track - Album - Artist` via `TrackUtils.NewTrack` on load; `_SetTitle()` is empty |

## Inspector Setup
* **Automated:** Automatically configured on scene build/process via `RecordPlayerAutoSetup`.
* **Manual:**
  1. Add both `RecordPlayer_YamaPlayer` and `RecordPlayer_YamaPlayer_Listener` to your module GameObject.
  2. On `RecordPlayer_YamaPlayer`:
     * Assign your `RecordPlayer` instance to **`recordPlayer`**.
     * Assign `RecordPlayer_YamaPlayer_Listener` to **`yamaListener`**.
     * Assign your scene's YamaPlayer `Controller` to **`yamaController`**.
  3. On `RecordPlayer_YamaPlayer_Listener`:
     * Assign `RecordPlayer_YamaPlayer` to **`recordPlayerProxy`**.

<!-- MODULE_MANIFEST_START -->
```json
{
  "schemaVersion": "1.2.0",
  "id": "YamaPlayer",
  "displayName": "Yamadev YamaPlayer",
  "repo": "[https://github.com/UdonVR/RecordPlayerModule_YamaPlayer](https://github.com/UdonVR/RecordPlayerModule_YamaPlayer)",
  "scriptPath": "RecordPlayer_YamaPlayer.cs",
  "listenerScriptPath": "RecordPlayer_YamaPlayer_Listener.cs",
  "className": "UdonVR.RecordPlayer.Yamaplayer.RecordPlayer_YamaPlayer",
  "listenerClassName": "UdonVR.RecordPlayer.Yamaplayer.RecordPlayer_YamaPlayer_Listener",
  "baseClass": "UdonVR.RecordPlayer.RecordPlayer_VideoplayerClass",
  "autoSetup": true,
  "requiredNamespaces": [
    "Yamadev.YamaStream"
  ],
  "inspectorReferences": [
    {
      "fieldName": "recordPlayer",
      "fieldType": "UdonVR.RecordPlayer.RecordPlayer",
      "required": true,
      "description": "Core RecordPlayer controller"
    },
    {
      "fieldName": "yamaListener",
      "fieldType": "UdonVR.RecordPlayer.Yamaplayer.RecordPlayer_YamaPlayer_Listener",
      "required": true,
      "description": "Companion YamaPlayerListener bridge component"
    },
    {
      "fieldName": "yamaController",
      "fieldType": "Yamadev.YamaStream.Controller",
      "required": true,
      "description": "YamaStream Controller component"
    }
  ],
  "capabilities": {
    "autoSetup": "full",
    "loadUrl": "full",
    "externalLoadDetection": "full",
    "playPauseStop": "full",
    "seekTime": "full",
    "playbackProgress": "full",
    "volumeSync": "bidirectional",
    "permissionCheck": "not_applicable",
    "autoAdvanceAndShuffle": "full",
    "loopTrack": "full",
    "customTitleMetadata": "load_only"
  },
  "notes": [
    "Requires two components due to YamaPlayerListener inheritance: RecordPlayer_YamaPlayer and RecordPlayer_YamaPlayer_Listener.",
    "Includes deferred stop verification to filter out internal Handler.Stop() calls during track transitions and player handler switches.",
    "Uses VideoPlayerType.AVProVideoPlayer in TrackUtils.NewTrack."
  ]
}
```
<!-- MODULE_MANIFEST_END -->
