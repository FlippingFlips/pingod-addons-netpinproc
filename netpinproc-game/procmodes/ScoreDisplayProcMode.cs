using NetPinProc.Domain;
using PinGod.Game;
using PinGod.Modes;

/// <summary>A P-ROC (PinGodProcMode) but reusing the default PinGod ScoreDisplay <para/>
/// This uses a custom score display overriding the `pingod-addons` score mode/display from `scenes/ScoreModePROC.cs` <para/>
/// </summary>
public class ScoreDisplayProcMode : PinGodProcMode
{
    /// <summary>Mode scene and custom script overriding the base PinGod Addons</summary>
    const string SCORE_MODE_SCENE = "res://netpinproc-game/scenes/ScoreMode/ScoreModePROC.tscn";

    private ScoreModePROC _scoreDisplay;

    /// <summary>Gets the score display scene from Resources and adds it to the Godot tree</summary>
    /// <param name="game"></param>
    /// <param name="priority"></param>
    /// <param name="pinGod"></param>
    /// <param name="defaultScene"></param>
    /// <param name="loadDefaultScene"></param>
    public ScoreDisplayProcMode(IGameController game,
        PinGodGame pinGod,
        string name = nameof(ScoreDisplayProcMode),
        int priority = 1,
        string defaultScene = SCORE_MODE_SCENE,
        bool loadDefaultScene = true) 
        : base(game, name, priority, pinGod, defaultScene, loadDefaultScene) { }

    /// <summary>Invokes AddModeScene (<see cref="SCORE_MODE_SCENE>"/>) on Godot PinGodProcGame</summary>
    public override void ModeStarted()
    {
        //call this to add the default scene
        base.ModeStarted();

        //instance of the scene if need to call deferred on it
        _scoreDisplay = ModeSceneInstance as ScoreModePROC;
    }

    /// <summary>Cleans up, removes from canvas and frees this mode. <para/>
    /// For this mode it would usually be when the game ends.<para/>
    /// This will <see cref="PinGodProcMode.RemoveChildScene"/></summary>
    public override void ModeStopped()
    {
        //clear up layers from the Modes Canvas
        base.ModeStopped();
    }

    /// <summary>invokes Godot's `emit_signal` with `ScoresUpdated`</summary>
    internal void UpdateScores() => PinGodGameProc.CallDeferred("emit_signal", "ScoresUpdated");
}