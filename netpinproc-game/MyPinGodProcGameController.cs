using Godot;
using NetPinProc.Domain;
using NetPinProc.Domain.Mode;
using NetPinProc.Domain.PinProc;
using System.Collections.Generic;
using System.Linq;

namespace PinGodAddOns.netpinproc_game
{
    /// <summary>Custom PROC Game controller using the implementation from `addons/netpinproc`</summary>
    public class MyPinGodProcGameController : PinGodNetProcDataGameController
    {
        public MyPinGodProcGameController(
            MachineType machineType,
            IPinGodGame pinGodGame,
            bool deleteOnInit,
            ILogger logger = null,
            bool simulated = false,
            MachineConfiguration configuration = null) : 
                base(machineType, pinGodGame, deleteOnInit, logger, simulated, configuration)
        {
        }

        #region PROC Game Modes
        public AttractMode _AttractMode;       
        private MachineSwitchHandlerMode _machineSwitchHandlerMode;
        private MyMode _myMode;
        private ScoreDisplayProcMode _scoreDisplay;
        private ServiceMode _serviceMode;
        #endregion

        /// <summary>Overrides to add a <see cref="MyProcPlayer"/></summary>
        /// <returns></returns>
        public override IPlayer AddPlayer()
        {
            //todo: name the player when need to change from eg: Player 1
            var p = base.AddPlayer();
            p = new MyProcPlayer(p.Name);
            _scoreDisplay?.UpdateScores();
            return p;
        }

        /// <summary>Use the Godot call deferred to use this method</summary>
        /// <param name="modeName"></param>
        public override void AddMode(string modeName)
        {
            switch (modeName)
            {
                case "service":
                    Modes.Add(_serviceMode);
                    break;
                case "attract":
                    GodotResourcesReady();
                    break;
                default:
                    break;
            }
        }

        /// <summary>Add points to the CurrentPlayer and update the Godot display</summary>
        /// <param name="p">points</param>
        public override void AddPoints(long p)
        {
            base.AddPoints(p);
            _scoreDisplay?.UpdateScores();
        }

        /// <summary><see cref=""/></summary>
        public override void BallEnded()
        {
            base.BallEnded();

            //TODO: remove modes on ball starting,
            //these modes remove when a new ball ends
            Modes.Remove(_myMode);
            _scoreDisplay?.UpdateScores();
        }

        public override void BallStarting()
        {
            base.BallStarting();

            _myMode = new MyMode(this, 10, PinGodProcGame);
            Modes.Add(_myMode);
            _scoreDisplay?.UpdateScores();
            _ballSave.Start(now: false);
        }

        /// <summary>Game ended, invoke Godot ready to reset game</summary>
        public override void GameEnded()
        {
            //this.EmitSignal(nameof(GameEnded));
            base.GameEnded();
            Modes.Remove(_scoreDisplay);

            this.GodotResourcesReady();
        }

        /// <summary>Should be called when Godot is ready or on a reset.<para/>
        /// This calls reset on the PROC game controller</summary>
        public override void GodotResourcesReady()
        {
            base.GodotResourcesReady();

            _AttractMode = new AttractMode(this, 12, PinGodProcGame);
            _serviceMode = new ServiceMode(this, PinGodProcGame, priority: 10, defaultScene: "res://addons/netpinproc-servicemode/ServiceModePROC.tscn".GetBaseName());
            _scoreDisplay = new ScoreDisplayProcMode(this, (PinGodGameProc)PinGodProcGame, priority: 2);
            _machineSwitchHandlerMode = new MachineSwitchHandlerMode(this, (PinGodGameProc)PinGodProcGame);
            _ballSave = new BallSave(this, "shootAgain", "plungerLane") { AllowMultipleSaves = false, Priority = 25 };
            SetupBallSearch();
            Reset();
        }

        public override void GameStarted()
        {
            base.GameStarted();

            Modes.Add(_scoreDisplay);

        }

        /// <summary>Use the godot call deferred to use this if invoking from Godot</summary>
        /// <param name="modeName"></param>
        public override void RemoveMode(string modeName)
        {
            switch (modeName)
            {
                case "service":
                    Modes.Remove(_serviceMode);
                    break;
                case "attract":
                    Modes.Remove(_AttractMode);
                    break;
                default:
                    break;
            }
        }

        /// <summary>Resets ball saves and adds our base modes to the queue<para/>
        /// Calls the base implementation to create to a trough and to reset</summary>
        public override void Reset()
        {
            base.Reset();

            //_troughMode.EnableBallSave(true);        

            //link the trough to ball save
            Trough.BallSaveCallback = new AnonDelayedHandler(_ballSave.LaunchCallback);
            Trough.NumBallsToSaveCallback = new GetNumBallsToSaveHandler(_ballSave.GetNumBallsToSave);
            _ballSave.TroughEnableBallSave = new BallSaveEnable(Trough.EnableBallSave);

            Modes.Add(_AttractMode);
            Modes.Add(_machineSwitchHandlerMode);
            Modes.Add(_ballSave);
            Modes.Add(_ballSearch);

            Logger.Log($"MODES RUNNING:" + Modes.Modes.Count);
        }

        /// <summary> Is this done in NetPinProc? </summary>
        private void SetupBallSearch()
        {
            var coils = Config.PRCoils.Where(x => x.Search > 0)
                        .Select(n => n.Name)
                        .ToArray();

            var resetDict = new Dictionary<string, string>();
            var resetSw = Config.PRSwitches
                .Where(x => string.IsNullOrWhiteSpace(x.SearchReset));
            foreach (var sw in resetSw)
            {
                resetDict.Add(sw.Name, sw.SearchReset);
            }

            var stopDict = new Dictionary<string, string>();
            var stopSw = Config.PRSwitches
                .Where(x => string.IsNullOrWhiteSpace(x.SearchStop));
            foreach (var sw in stopSw)
            {
                stopDict.Add(sw.Name, sw.SearchStop);
            }

            _ballSearch = new BallSearch(this, 12, coils, resetDict, stopDict, null);
        }
    }
}
