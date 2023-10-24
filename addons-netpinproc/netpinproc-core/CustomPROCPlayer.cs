using NetPinProc.Domain.Players;

public class CustomPROCPlayer : Player
{
    public CustomPROCPlayer(string name) : base(name) { }

    public bool[] TargetsCompleted { get; set; } = new bool[4];
}
