using Godot;
using Godot.Collections;
using NetPinProc.Domain;
using PinGod.Core;
using PinGod.Core.Service;
using PinGod.Game;
using System;
using System.Linq;

/// <summary>For handling window keyboard events and sending to PROC via a fake interface <para/>
/// The base class uses
/// </summary>
public partial class WindowActionsPROC : WindowActionsNode
{
    /// <summary>Add keycodes to PROC switch names eg:53, coin1 <para/>
    /// PROC switches will be found and populated when this node enters the tree</summary>
    [Export] Dictionary<int, string> keyboardToPROCswitches = new();

    private MachinePROC _machineNodePROC;    

    private System.Collections.Generic.Dictionary<int, NetPinProc.Domain.Switch> _procKeySwitches;

    public override void _EnterTree()
    {
        //call the base to setup a machine node reference from singleton
        base._EnterTree();

        _machineNodePROC = _machine as MachinePROC;

        if (_machineNodePROC == null)
            throw new NullReferenceException("No PROC machine node found");
    }

    public override void _Input(InputEvent @event)
    {        
        this.SetProcessInput(false);
        GD.Print("in proc");
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        base._UnhandledKeyInput(@event);

        if (_standardInputHandlingOn)
        {
            //quits the game. ESC
            if (InputMap.HasAction("ui_cancel"))
            {
                if (@event.IsActionPressed("ui_cancel"))
                {
                    Quit();
                }
            }

            if (InputMap.HasAction("toggle_border"))
            {
                if (@event.IsActionPressed("toggle_border"))
                {
                    ToggleBorder();
                };
            }
        }

        if (_sendPingodMachineSwitches && _machineNodePROC != null)
        {
            var key = ((int)(@event as InputEventKey).Keycode);
            if (_procKeySwitches.ContainsKey(key))
            {
                var pressed = @event.IsPressed();

                _machineNodePROC.SetSwitch(_machineNodePROC.NetProcGame, _procKeySwitches[key], pressed);
            }
        }
    }

    /// <summary>just make sure the base is called on ready to setup</summary>
    public override void _Ready()
    {
        base._Ready();

        //setup a dictionary of PROC switches
        if (keyboardToPROCswitches?.Any() ?? false)
        {
            _procKeySwitches = new System.Collections.Generic.Dictionary<int, NetPinProc.Domain.Switch>();
            foreach (var item in keyboardToPROCswitches)
            {
                var swName = item.Value;
                var sw = _machineNodePROC.NetProcGame.Switches[swName];
                _procKeySwitches.Add(item.Key, sw);
            }
        }
        else { Logger.Warning($"[{nameof(WindowActionsPROC)}]: no keyboard keycodes added for windowActions"); }

    }
}
