using NetPinProc.Domain.Players;

/// <summary>This player derives from a P-ROC player (NetPinProc.Domain)</summary>
public class MyProcPlayer : Player
{
    public MyProcPlayer(string name) : base(name) { }

    public bool[] TargetsCompleted { get; set; } = new bool[4];
}
