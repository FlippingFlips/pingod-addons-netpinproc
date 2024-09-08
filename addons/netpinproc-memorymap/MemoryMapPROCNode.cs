using PinGod.Core.Service;

public partial class MemoryMapPROCNode : MemoryMapNode
{
	PinGodGameProc _pinGodProc;

	/// <summary>This is where the base memory map is created. Need to have access to PROC to create with game controller.</summary>
	public override void _EnterTree()
	{
		base._EnterTree();
		_pinGodProc = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
	}

	public override void CreateMemoryMap()
	{
		//todo: get proc gamecontroller
		if(_pinGodProc?.NetProcGame != null)
		{
			if (PinGod.Game.PinGodGame.PinGodOverrideConfig.MemoryMapEnabled)
			{
				mMap = new MemoryMapPROC(_pinGodProc.NetProcGame, this.MutexName, MapName,
                    PinGod.Game.PinGodGame.PinGodOverrideConfig.MemoryMapWriteDelay,
                    PinGod.Game.PinGodGame.PinGodOverrideConfig.MemoryMapReadDelay,
					CoilTotal, LampTotal, LedTotal, SwitchTotal);
			}				
			else { Logger.WarningRich(nameof(MemoryMapPROC), ": [color=yellow]memory map disabled exiting memory mapping[/color]"); }
		}
		else
		{
			Logger.WarningRich(nameof(MemoryMapPROC), ": [color=yellow]No PinGodProcGame found, exiting memory mapping[/color]");
			this.QueueFree();
		}
	}

	public void WriteStates()
	{
		mMap.WriteStates();        
    }
}
