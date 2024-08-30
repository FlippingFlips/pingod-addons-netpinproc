using Godot;
using Godot.Collections;
using NetPinProc.Domain;
using NetPinProc.Domain.PinProc;
using PinGod.Core;
using System;

/// <summary>
/// This script is loaded with the `res://autoload/Machine.tscn` which inherits a PinGod-MachineNode. <para/>
/// Singleton can be retrieved from the /root/ with GetNode&lt;MachinePROC&gt;("/root/Machine"). <para/>
/// EnterTree initializes a database, gets machine config
/// </summary>
public partial class MachinePROC : MachineNode
{           
	public PinGodGameProc _pinGodGameProc;

    public IGameController NetProcGame => _pinGodGameProc?.NetProcGame;

    #region Godot overrides

    /// <summary><see cref="MachineNode._EnterTree"/></summary>
    public override void _EnterTree()
    {
        _pinGodGameProc = GetNodeOrNull<PinGodGameProc>(Paths.ROOT_PINGODGAME);

        base._EnterTree();
    }

    /// <summary>Base saves the database and Dispose of the connection <see cref="MachineNode._ExitTree"/></summary>
    public override void _ExitTree()
	{
		Logger.Info(nameof(MachinePROC), ":", nameof(_ExitTree));
		base._ExitTree();
	}

    /// <summary>Process playback events. Turns off if playback is switched off.</summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        SetProcess(false);
        if (!_pinGodGameProc.GameReady) return;

        //TODO: recording
        //if (_recordPlayback != RecordPlaybackOption.Playback)
        //{
        //	SetProcess(false);
        //	Logger.Info(nameof(MachineNode), ": Playback _Process loop stopped. No recordings are being played back.");
        //	return;
        //}
        //else
        //{
        //	var cnt = _recordFile.GetQueueCount();
        //	if (cnt <= 0)
        //	{
        //		Logger.Info(nameof(MachineNode), ": playback events ended, RecordPlayback is off.");
        //		_recordPlayback = RecordPlaybackOption.Off;
        //		_recordFile.SaveRecording();
        //		if (_recordingStatusLabel != null) _recordingStatusLabel.Text = "Machine:Playback ended";
        //		return;
        //	}

        //	var evt = _recordFile.ProcessQueue(_machineLoadTime);
        //	if (evt != null)
        //	{
        //		var sw = _pinGodGameProc.PinGodProcGame.Switches[evt.EvtName];
        //		OnSwitchCommand(sw.Name, sw.Number, evt.State);
        //	}
        //}
    }

    /// <summary>
    /// Gets a PinGodGame
    /// </summary>
    public override void _Ready()
	{
		base._Ready();		
	}
	#endregion

	/// <summary>
	/// Clear any items from the machines collections 
	/// </summary>
	public void ClearMachineItems()
	{
		_coils.Clear();
		_lamps.Clear();
		_leds.Clear();
		_switches.Clear();
	}

    /// <summary>Sets a Fake PROC switch</summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    /// <param name="value"></param>
    public void OnSwitchCommand(string name, int num, byte value)
    {
        Logger.Info("MACHINE_PROC: Switch:" + num);
        //base.OnSwitchCommand(name, index, value);
        if (_pinGodGameProc != null && PinGodGameProc.PinGodProcConfig.Simulated)
        {
            SetSwitchFakeProc(_pinGodGameProc.NetProcGame, (ushort)num, value > 0 ? true : false);
        }
    }

    /// <summary>Calls base set switch but will set Fake p-roc switch first</summary>
    /// <param name="switch"></param>
    /// <param name="value"></param>
    /// <param name="fromAction"></param>
    public override void SetSwitch(PinGod.Core.Switch @switch, byte value, bool fromAction = true)
    {
        if (_pinGodGameProc != null)
        {
            //var sw = _switches[@switch.Name];
            SetSwitchFakeProc(_pinGodGameProc.NetProcGame, @switch.Name, value > 0 ? true : false);
        }
        base.SetSwitch(@switch, value, fromAction);
    }

    public virtual void SetSwitch(IGameController gc, NetPinProc.Domain.Switch @switch, bool state, bool fromAction = true)
    {
        if (_pinGodGameProc != null && PinGodGameProc.PinGodProcConfig.Simulated)
        {            
            var proc = gc?.PROC as IFakeProcDevice;
            
            if (!@switch.IsState(state))
            {
                var evtT = state ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;
                proc.AddSwitchEvent(@switch.Number, evtT);
                RecordSwitch(@switch.Name, @switch);
            }
        }
    }

    /// <summary>Sets a Fake PROC device switch</summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="fromAction"></param>
    public override void SetSwitch(string name, byte value, bool fromAction = true)
    {
        if (_pinGodGameProc != null)
        {
            SetSwitchFakeProc(_pinGodGameProc.NetProcGame, name, value > 0 ? true : false);
        }
    }

    /// <summary>Override to do nothing, ball search handled by PROC</summary>
    public override void SetupBallSearch() { }

    public override bool SwitchActionOff(string swName, InputEvent inputEvent) => false;

    /// <summary>override godot actions to the machine</summary>
    /// <param name="swName"></param>
    /// <param name="inputEvent"></param>
    /// <returns></returns>
    public override bool SwitchActionOn(string swName, InputEvent inputEvent) => false;

    internal void AddCoil(string name, byte number) => _coils.Add(name, number);

    internal void AddLamp(string name, byte number) => _lamps.Add(name, number);

    internal void AddLed(string name, byte number) => _leds.Add(name, number);

    internal void AddSwitch(string name, byte number) => _switches.Add(name, number);

    /// <summary>Calls AddSwitchEvent on the IFakePinProcDevice</summary>
    /// <param name="gc">game controller with reference to a PROCDevice</param>
    /// <param name="name">switch name</param>
    /// <param name="enabled">open or closed</param>
    internal bool SetSwitchFakeProc(
        IGameController gc,
        string name,
        bool enabled)
    {
        if (_pinGodGameProc != null && PinGodGameProc.PinGodProcConfig.Simulated)
        {
            var proc = gc?.PROC as IFakeProcDevice;
            var sw = gc.Switches[name];
            if (!sw.IsState(enabled))
            {
                var evtT = enabled ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;
                proc.AddSwitchEvent(sw.Number, evtT);
                RecordSwitch(name, sw);
                return true;
            }
        }

        return false;
    }

    /// <summary>Calls AddSwitchEvent on the IFakePinProcDevice</summary>
    /// <param name="gc">game controller with reference to a PROCDevice</param>
    /// <param name="number">switch number</param>
    /// <param name="enabled">open or closed</param>
    internal void SetSwitchFakeProc(IGameController gc, ushort number, bool enabled)
    {
        var proc = gc?.PROC as IFakeProcDevice;
        if (proc != null)
        {
            var sw = gc.Switches[number];
            var evtT = enabled ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;

            proc.AddSwitchEvent(sw.Number, evtT);

            RecordSwitch(sw.Name, sw);
        }
    }

    protected override void AddCustomMachineItems(
        Dictionary<string, byte> coils,
        Dictionary<string, byte> switches,
        Dictionary<string, byte> lamps,
        Dictionary<string, byte> leds)
    {
		base.AddCustomMachineItems(coils, switches, lamps, leds);
		Logger.Info(nameof(MachinePROC), ": P-ROC overriding ", nameof(AddCustomMachineItems));
	}

	/// <summary>Records a switch if the game is recording: TODO: does nothing</summary>
	/// <param name="name"></param>
	/// <param name="sw"></param>
	private void RecordSwitch(string name, NetPinProc.Domain.Switch sw)
	{		
		//TODO: recording
		//if (_recordPlayback == RecordPlaybackOption.Record)
		//{						
		//	byte state = sw.StateString() == "closed" ? (byte)0 : (byte)1;

		//	_recordFile.RecordSwitchEvent(name, state, _machineLoadTime);

		//	Logger.Verbose($"recorded switch: {name} | {state}");
		//}
	}
}
