using Godot;
using NetPinProc.Domain;
using NetPinProc.Domain.MachineConfig;
using System.Linq;

/// <summary>This will go through all NetProcDataGame.Switches set in the game and generate a <see cref="MatrixItemPanel"/> <para/>
/// This scene is assigned to a group named: switch_views<para/>
/// OnSwitch is hooked up to this event and will UpdateSwitch for status in the UI<para />
/// TODO: move some properties from SwitchFileConfig onto the actual switch in NetPinProcGame. ItemType for example to tell opto switches</summary>
public partial class MatrixSwitchesGridContainer : GridContainer
{
	private PinGodGameProc _pinGodProcGame;

	public override void _Ready()
	{
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
		
		var scene = ResourceLoader
			.Load<PackedScene>("res://addons/netpinproc-servicemode/Shared/MatrixItemPanel.tscn");

		int switchCount = _pinGodProcGame.NetProcGame.Switches.Count;

		//draw top column rows
		for (int i = 0; i < (8 * 16); i++)
		{
			var newScene = scene.Instantiate() as MatrixItemPanel;
			//newScene.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["column"]);
			var switchNum = i;

			if (i < switchCount && (_pinGodProcGame?.NetProcGame.Switches.ContainsKey((ushort)switchNum) ?? false))
			{				
				//game switch and set the MatrixPanel name
				var sw = _pinGodProcGame.NetProcGame.Switches[(ushort)switchNum];
                newScene.Name = sw.Name;
                this.AddChild(newScene);

                newScene.SetNameFromLabel(sw.Name);

				//config switch with more values
				var cfgSwitch = _pinGodProcGame.NetProcGame.Config
					.PRSwitches.FirstOrDefault(x => x.Name == sw.Name);

				//set up displaying any wire colous set on switches
				if (!string.IsNullOrWhiteSpace(cfgSwitch.InputWire) && !string.IsNullOrWhiteSpace(cfgSwitch.GroundWire))
				{
                    //get the wire colour names from database config
                    var inputWireColours = cfgSwitch.InputWire.Split(",");
                    var groundWireColours = cfgSwitch.GroundWire.Split(",");

                    newScene.SetWireL(inputWireColours);
                    newScene.SetWireR(groundWireColours);
                }

				//set switch state depending on the type. TODO: opto switches
				SetSwitchColor(cfgSwitch, sw, newScene);                

                //TODO: set wire colour. wire colour isn't in database it is 3 charachter colour names like. BLK, BRN, BLU. 
                //newScene.SetWireL(cfgSwitch.WireLColour, cfgSwitch.WireLColourName);
                //newScene.SetWireR(cfgSwitch.WireRColour, cfgSwitch.WireRColourName);
            }
			else
			{
				newScene.SetName("UNUSED");
                this.AddChild(newScene);
            }

			newScene.SetNum(switchNum);
			
		}
	}

	public void OnSwitch(string swName, ushort swNum, bool isClosed)
	{
		var cfgSwitch = _pinGodProcGame.NetProcGame.Config.PRSwitches.FirstOrDefault(x => x.Name == swName);
		UpdateSwitch(cfgSwitch);
	}

	private void UpdateSwitch(SwitchConfigFileEntry sw)
	{
		if (sw != null)
        {
            //get the game switch for it's state
            var gameSw = _pinGodProcGame.NetProcGame.Switches[sw.Name];

            //find the item in the grid
            var item = this.GetChild(gameSw.Number) as MatrixItemPanel;

            SetSwitchColor(sw, gameSw, item);
        }
    }

    private static void SetSwitchColor(SwitchConfigFileEntry sw, Switch gameSw, MatrixItemPanel item)
    {
        if (sw.Type == NetPinProc.Domain.PinProc.SwitchType.NO)
        {
            if (gameSw.IsClosed())
                item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
            else
            {
                if (sw.ItemType == "opto") item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["opto_no"]);
                else item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_no"]);
            }
        }
        else if (sw.Type == NetPinProc.Domain.PinProc.SwitchType.NC)
        {
            if (gameSw.IsOpen())
                item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["active"]);
            else
            {
                if (sw.ItemType == "opto") item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["opto_nc"]);
                else item.ChangePanelBackgroundColour(PinballMatrixConstants.BackgroundColours["switch_nc"]);
            }
        }
    }
}
