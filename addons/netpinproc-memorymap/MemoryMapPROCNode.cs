using PinGod.Core;
using PinGod.Core.Service;

public partial class MemoryMapPROCNode : MemoryMapNode
{
	PinGodGameProc _pinGodProc;

	/// <summary>
	/// This is where the base memory map is created. Need to have access to PROC to create with game controller.
	/// </summary>
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
			//get enabled from the proc.cfg for convience of changing it. When live we don't want this
			this.IsEnabled = PinGodGameProc.PinGodProcConfig.MemoryMapEnabled;

			if (this.IsEnabled)
			{
				WriteDelay = PinGodGameProc.PinGodProcConfig.MemoryMapWriteDelay;
				ReadDelay = PinGodGameProc.PinGodProcConfig.MemoryMapReadDelay;

				mMap = new MemoryMapPROC(_pinGodProc.NetProcGame, this.MutexName, MapName, WriteDelay, ReadDelay,
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
