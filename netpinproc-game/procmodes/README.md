# Modes - P-ROC

These are `NetPinProc.IGameController` modes that are added to the `NetPinProc` game instances `ModeQueue`.

Scenes for the mode should go in `res://scenes`, for example `res://scenes/AttractMode` to match the `PROC` mode name.

### Mode Scenes
Scenes are not required for P-ROC modes but they can be added when the mode starts in `"ModeStarted"` and removed in `"ModeEnded"`.
The scene you send to the mode should be the full asset path:
- For the attract it is `"res://scenes/AttractMode/AttractProc.tscn"`

### Add / Remove Scenes in PROC Modes
To add a scene on UI thread:
- `_pingod.CallDeferred("AddModeScene", ATTRACT_SCENE);`

To remove a scene on UI thread:
- `_pingod.CallDeferred("RemoveModeScene", ATTRACT_SCENE);`


### Default Basic Modes
- MachineSwitchHandlerMode
- AttractMode
- ScoreDisplayMode
- MyMode
- ServiceMode
