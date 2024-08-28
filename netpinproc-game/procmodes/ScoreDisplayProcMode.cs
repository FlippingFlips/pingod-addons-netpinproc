using Godot;
using NetPinProc.Domain;
using PinGod.Game;
using PinGod.Modes;

/// <summary>A P-ROC (PinGodProcMode) but reusing the default PinGod ScoreDisplay <para/>
/// This uses a custom score display overriding the `pingod-addons` score mode/display from `scenes/ScoreModePROC.cs` <para/>
/// </summary>
public class ScoreDisplayProcMode : PinGodProcMode
{
    /// <summary>Custom class of <see cref="ScoreMode"/>. This needs to be custom so we can use the P-ROC game controller players.</summary>
    const string SCORE_MODE_SCENE = "res://netpinproc-game/scenes/ScoreMode/ScoreModePROC.tscn";

    private PinGodGameProc _pingod;
    private Node _sceneInstance;
    private ScoreModePROC _scoreDisplay;
    /// <summary>Gets the score display scene from Resources and adds it to the Godot tree</summary>
    /// <param name="game"></param>
    /// <param name="priority"></param>
    /// <param name="pinGod"></param>
    /// <param name="defaultScene"></param>
    /// <param name="loadDefaultScene"></param>
    public ScoreDisplayProcMode(IGameController game, PinGodGame pinGod, string name = nameof(ScoreDisplayProcMode), int priority = 1, string defaultScene = null, bool loadDefaultScene = true) 
        : base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
    {
        _pingod = pinGod as PinGodGameProc;
    }

    /// <summary>Invokes AddModeScene (<see cref="SCORE_MODE_SCENE>"/>) on Godot PinGodProcGame</summary>
    public override void ModeStarted()
    {
        base.ModeStarted();

        Game.Logger.Log("Score ModeStarted. Calling: AddModeScene",
            NetPinProc.Domain.PinProc.LogLevel.Info);

        //return the score mode scene after adding it.
        //we don't need to do this but this is a way to check it has loaded
        var variant = _pingod.CallDeferred("AddModeScene", SCORE_MODE_SCENE);
        _sceneInstance = variant.As<Node>();

        //log an error if missing, TODO: should throw
        if (_sceneInstance == null)
            Game.Logger.Log("SCORE MODE SCENE NOT INSTANCED", NetPinProc.Domain.PinProc.LogLevel.Error);
    }

    /// <summary>Cleans up, removes from canvas and frees this mode. <para/>
    /// For this mode it would usually be when the game ends.<para/>
    /// This will <see cref="PinGodProcMode.RemoveChildSceneFromCanvasLayer"/></summary>
    public override void ModeStopped()
    {
        //remove child layers
        if (_sceneInstance != null)
            RemoveChildSceneFromCanvasLayer(_sceneInstance);

        //clear up layers from ModesCanvas
        base.ModeStopped();        
    }
    /// <summary>Emits a Godot signal ScoresUpdated for scenes to pick up on</summary>
    internal void UpdateScores() => _pingod.CallDeferred("emit_signal", "ScoresUpdated");
}