using Godot;
using NetPinProc.Domain;
using PinGod.Game;
using PinGod.Modes;

/// <summary>A P-ROC (PinGodProcMode) but reusing the default PinGod ScoreDisplay <para/>
/// This uses a custom score display overriding the `pingod-addons` score mode/display from `scenes/ScoreModePROC.cs` <para/>
/// </summary>
public class ScoreDisplayProcMode : PinGodProcMode
{
    private ScoreModePROC _scoreDisplay;

    private PinGodGameProc _pingod;

    private Node _sceneInstance;

    /// <summary>Custom class of <see cref="ScoreMode"/>. This needs to be custom so we can use the P-ROC game controller players.</summary>
    const string SCORE_MODE_SCENE = "res://netpinproc-game/scenes/ScoreMode/ScoreModePROC.tscn";

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

    /// <summary>Cleans up, removes from canvas and frees this mode. <para/>
    /// For this mode it would usually be when the game ends.<para/>
    /// This will <see cref="PinGodProcMode.RemoveChildSceneFromCanvasLayer"/></summary>
    public override void ModeStopped()
    {
        base.ModeStopped();
        if (_sceneInstance != null)
            RemoveChildSceneFromCanvasLayer(_sceneInstance);
    }

    /// <summary>Invokes AddModeScene (<see cref="SCORE_MODE_SCENE>"/>) on Godot PinGodProcGame</summary>
    public override void ModeStarted()
    {
        base.ModeStarted();

        Game.Logger.Log("Score mode started", NetPinProc.Domain.PinProc.LogLevel.Info);

        var variant = _pingod.CallDeferred("AddModeScene", SCORE_MODE_SCENE);

        _sceneInstance = variant.As<Node>();

        if(_sceneInstance == null) {
            Game.Logger.Log("SCORE MODE SCENE NOT INSTANCED", NetPinProc.Domain.PinProc.LogLevel.Error);
        }
    }

    /// <summary>Emits godot signal ScoresUpdated for scenes to pick up on</summary>
    internal void UpdateScores()
    {
        //_sceneInstance?
        //.CallDeferred("OnScoresUpdated");
        _pingod.CallDeferred("emit_signal", "ScoresUpdated");
    }
}