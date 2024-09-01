using Godot;
using PinGod.Modes;
using System.Linq;

/// <summary>A Godot scene for attract display. This is a custom attract scene overriding pingod-addons attract<para/>
/// Gets scores from NetProcDataGame</summary>
public partial class AttractProc : Attract
{
	private PinGodGameProc _pinGodProcGame;

    public override void _EnterTree()
	{
		base._EnterTree();
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
    }

	/// <summary>Build highscore lists. Top scores come from a NetProcDataGame controller database and the base implentation not used</summary>
	public override void GetHighScores()
	{
		//base.GetHighScores();

		_highScoresList?.Clear();
		var scores = _pinGodProcGame?.NetProcGame?.GetTopScores();
		if (scores?.Count() > 0) 
			_highScoresList = scores.Select(s => new HighScore { Name = s.Player.Name, Points = s.Points, Created = s.GamePlayed.Ended }).ToList();
	}
}
