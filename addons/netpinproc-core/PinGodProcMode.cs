using Godot;
using PinGod.Core.Service;
using PinGod.Core;
using NetPinProc.Domain;
using NetPinProc.Domain.Mode;
using NetPinProc.Domain.PinProc;

/// <summary>
/// Base PROC Mode with PinGod. This is a P-ROC Mode but with an added CanvasLayer from Godot. <para/>
/// A standard P-ROC Mode:IMode can be used instead that don't need any scenes or to interact with Godot <para/>
/// 
/// In the MainScene this holds a CanvasLayer named Modes, we insert this current modes CanvasLayer into the Modes<para/>
/// 
/// - Display layer order:
///     P-ROC modes have a layer property which is usually for the DMD (dot matrix) and we translate the mode priority to layer order.<para/>
///     
/// When the mode is removed (ModeStopped) from the games modes then this Canvaslayer is cleared from the Modes (CanvasLayer). <para/>
/// NOTE: This base mode needs the Resources autoload plug-in running to load packed scenes.<para/>
///   - You would add the scene that you're loading into this layer in the Resources.tscn.<para/>
///   - This scene is then preloaded in Resources.tscn.<para/>
///   - When we need to add to this layer we can get it from the preloaded resources.<para/>
/// </summary>
public abstract class PinGodProcMode : Mode //P-ROC Mode
{
    /// <summary>reference to the netproc database instance of a game<para/>
	/// Use GetAudit, Adjustment and SaveData to save all changes to database<para/></summary>
    protected readonly PinGodNetProcDataGameController NetProcDataGame;

    protected readonly bool loadDefaultScene;

    [Export(PropertyHint.File)] protected string defaultScene = "res://addons/netpinproc-scenes/Logos/PinGodLogoScene.tscn";

    /// <summary>Initializes a base P-ROC mode for PinGod. <para/> <see cref="CreateCanvasLayer"/></summary>
    /// <param name="game"></param>
    /// <param name="priority">Layer on the canvas is a priority</param>
    /// <param name="pinGod">Need this to grab the root</param>
    /// <param name="defaultScene">path to default scene to load</param>
    /// <param name="loadDefaultScene">Load default scene and into the tree when the mode starts?</param>
    public PinGodProcMode(IGameController game,
		string name,
		int priority,
		IPinGodGame pinGod,
		string defaultScene = null,
		bool loadDefaultScene = false) :
		base(game, priority)
    {
        Name = name;

        this.defaultScene = defaultScene;
        this.loadDefaultScene = loadDefaultScene;

        PinGodGameProc = pinGod as PinGodGameProc;
        NetProcDataGame = game as PinGodNetProcDataGameController;        
    }

    /// <summary>This modes Layer. Add scenes to this layer.</para>
    /// The Godot CanvasLayer has a Layer property for priority.</summary>
    public CanvasLayer CanvasLayer { get; private set; }

    public string Name { get; }

    /// <summary>Is an IPinGodGame. Use this to get at the ModesCanvas and Resources</summary>
    public PinGodGameProc PinGodGameProc { get; }

    /// <summary>Reference to the scene. Will be assigned if loadDefaultScene set to true</summary>
    public Node ModeSceneInstance { get; private set; }

    /// <summary>adds the node to the canvas layer. calls deferred on Godots `add_child`<para/>
    /// A Canvas layer will be created if not available</summary>
    /// <param name="node"></param>
    public virtual Node AddMainModeSceneToCanvas(Node node)
	{
		if(CanvasLayer == null)
        {
            CreateCanvasLayer(Name, Priority);
        }            

        Logger.Debug($"adding child scene to canvas: " + node.Name);
        CanvasLayer.CallDeferred("add_child", node);

        return node;
	}

    /// <summary>Creates a CanvasLayer with mode / layer priority to hold this mode in Godot scenes</summary>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="addAsChild">add the CanvasLayer to the modes canvas layer</param>
    public virtual void CreateCanvasLayer(string name, int priority, bool addAsChild = true)
	{
		if (PinGodGameProc.ModesCanvasLayer != null && CanvasLayer == null)
		{
			CanvasLayer = new CanvasLayer() { Layer = priority, Name = name };
            if (addAsChild)
            {
                PinGodGameProc.ModesCanvasLayer?.CallDeferred("add_child", CanvasLayer);
            }
		}
		else { Logger.Warning(nameof(PinGodGameMode), ": not creating new scene canvas layer, CanvasLayer found"); }
	}

    /// <summary>Instantiates a Node from a packed scene in the <see cref="PinGodGameProc._resources"/></summary>
    /// <param name="resourceName">should be single name from resource but a full res:// path should also work</param>
    /// <returns>Returns a packed scene instantiated</returns>
    public virtual Node CreateSceneResourceInstance(
        string resourceName)
    {
        if (PinGodGameProc._resources != null)
        {
            var scene = PinGodGameProc._resources?
                .GetResource(resourceName.GetBaseName()) as PackedScene;

            return scene?.Instantiate();
        }
        else 
        {
            Logger.WarningRich(nameof(AttractMode), nameof(ModeStarted), ": [color=yellow]no resources found, can't create attract scene[/color]");
            return null; 
        }
    }

	/// <summary>Loads a packed scene, instantiates it and adds it as a child to the canvas<para/>
	/// This doesn't use the pre loaded resources, rather it loads from the res:// path, so you probably only want to do smaller scenes here<para/>
	/// Or scenes that are rarely used and not needed to pre load</summary>
	/// <param name="scenePath">res://</param>
	public virtual void LoadSceneIntoCanvas(string scenePath)
	{
		if (PinGodGameProc.ModesCanvasLayer != null && CanvasLayer != null)
		{
			var res = GD.Load<PackedScene>(scenePath);
			var inst = res.Instantiate();
			CanvasLayer?.AddChild(inst);
            PinGodGameProc.ModesCanvasLayer.AddChild(CanvasLayer);
		}
	}

    /// <summary>Load a new scene into this modes canvas<para/>
    /// For example if the mode was Attract it would appear above the default scene in "/Modes/AttractMode/Attract"<para/> 
    /// /Modes/AttractMode/MyAddedScene</summary>
    /// <param name="node"></param>
    public virtual void LoadChildSceneIntoCanvas(Node node)
    {
        if (PinGodGameProc.ModesCanvasLayer != null && CanvasLayer == null)
            CreateCanvasLayer(Name, Priority);

        if (CanvasLayer != null)
        {
            CanvasLayer?.CallDeferred("add_child", node);
            Logger.Debug($"{nameof(LoadChildSceneIntoCanvas)}: added {node.Name} to {Name}");
        }            
    }

    /// <summary>Mode started. This loads the default scene for this mode from the resources if <see cref="loadDefaultScene"/> is set<para/>
    /// If loading a scene then a CanvasLayer will be created under this modes names<para/>
    /// In the scene tree the canvas layer would be under `Modes/Attract/Attract` for example</summary>
    public override void ModeStarted()
    {
        base.ModeStarted();

        Game.Logger.Log(LogLevel.Debug, $"{nameof(ModeStarted)}: {Name}");

        if (loadDefaultScene)
        {
            if (string.IsNullOrWhiteSpace(defaultScene))
            {
                Logger.Warning(nameof(PinGodGameMode), ": load default scene but no default scene was set.");
            }
            else
            {
                Game.Logger.Log(LogLevel.Debug, $"{nameof(ModeStarted)}: loading default scene resource {defaultScene}");
                ModeSceneInstance = AddMainModeSceneToCanvas(CreateSceneResourceInstance(defaultScene));
            }
        }
    }

    /// <summary>Removes the <see cref="CanvasLayer"/> from the ModesCanvas</summary>
    public override void ModeStopped()
	{
		if(CanvasLayer != null)
		{
            Logger.Debug(nameof(PinGodProcMode), "removing mode scene from Modes:", Name);
            PinGodGameProc.CallDeferred(nameof(PinGodGameProc.RemoveModeScene), Name);
		}        
		base.ModeStopped();
	}

    /// <summary>Removes the Node from this modes canvas layer<para/>
    /// The Node is also queue freed to be deleted</summary>
    /// <param name="node"></param>
    /// <param name="delete">runs Queue free, use in most cases</param>
    public virtual void RemoveChildScene(
		Node node,
		bool delete = true)
	{
		if(!node?.IsQueuedForDeletion() ?? false)
		{
            CanvasLayer?.RemoveChild(node);
			if(delete)
				node.QueueFree();
        }			
	}
}
