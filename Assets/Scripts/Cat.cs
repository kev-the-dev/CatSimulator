using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum SelectedTool
{
	NONE,
	HAND,
	FOOD,
	BRUSH,
	LASER_POINTER
}

// Behavior script for the cat. Manages the cats behaviors, stats, and personality
public class Cat : MonoBehaviour
{
	// Versioning for the scripting so load/saves across different versions do not create issues
	// NOTE: MUST be incremented each time the script changes in a way that changes save/load functionality
	private const int ScriptMajorVersion = 1;

	// The cat's current stats, which appear on the HUD bars
	CatStats stats;
	// The cats permenant personality, initialized randomly
	CatPersonality personality;
	// The cat's current activity/behavior/goal
	CatActivity activity;
	// The cat's style (color, fur)
	CatStyle style;
	
	NavMeshAgent agent;
	
	// Cat AI
	BehaviorTree autonomousCatBehaviorTree;
	BehaviorTree userInteractionBehaviorTree;
	Context contextObject;
	
	// Petting / brushing / summoning related variables
	Vector3 inFrontOfUserPosition;
	bool is_drag;
	double drag_start_time;
	public float time_of_last_user_interaction {get; private set;}
	
	// UI Buttons
	private Button hand_button, brush_button, food_button, laser_button, liter_button;
	public SelectedTool selected_tool {get; private set;}
	public Texture2D hand_cursor;
	public Texture2D brush_cursor;
	public Texture2D food_cursor;
	public Texture2D laser_cursor;

	// Line for laser
	private LineRenderer laser_line;

	// Food bowl
	private Renderer food_in_bowl;
	
	float last_update_time;

    // Start is called before the first frame update
    void Start()
    {
		// Initialize agent
		agent = GetComponent<NavMeshAgent>();
		
		// Cat position for petting / brushing
		inFrontOfUserPosition = new Vector3(0F, -0.5F, -5F);
		
		// Initialize laser line
		laser_line = GameObject.Find("laser_line").GetComponent<LineRenderer>();
		
		// Initialize Buttons
		hand_button = GameObject.Find("hand_button").GetComponent<Button>();
		brush_button = GameObject.Find("brush_button").GetComponent<Button>();
		food_button = GameObject.Find("food_button").GetComponent<Button>();
		laser_button = GameObject.Find("laser_button").GetComponent<Button>();
		hand_button.onClick.AddListener(delegate {SelectTool(SelectedTool.HAND);});
		brush_button.onClick.AddListener(delegate {SelectTool(SelectedTool.BRUSH);});
		food_button.onClick.AddListener(delegate {SelectTool(SelectedTool.FOOD);});
		laser_button.onClick.AddListener(delegate {SelectTool(SelectedTool.LASER_POINTER);});
		SelectTool(SelectedTool.HAND);

		// If no previous save exists, create a new random cat
		if (!PlayerPrefs.HasKey("script_version")) {
			Debug.Log("No previous save found, creating a cat");
			CreateNew();
			Save();
		// If previous save was at a different game version, create new cat
		} else if (PlayerPrefs.GetInt("script_version") != ScriptMajorVersion) {
			Debug.Log("Previous save had a different script version, creating a cat");
			CreateNew();
			Save();
		// Otherwise load cat back from save
		} else {
			Debug.Log("Previous save found, loading");
			Load();
		}

		// Start off idle
		activity = new CatActivity( CatActivityEnum.Idle );
		
		
		contextObject = new Context( gameObject, ref personality, ref stats, ref activity );
		
		// Construct the cat's behavior tree
        autonomousCatBehaviorTree = new BehaviorTree(	new SelectorNode	( 	contextObject,
		
																				/* Energy Sequence */ 	new SequenceNode (	contextObject, 
																															new CheckEnergyNode ( contextObject ),
																															new SleepNode ( contextObject )
																														),
																				/* Hunger Sequence */	new SequenceNode ( 	contextObject,
																															new CheckFullnessNode ( contextObject ),
																															new CheckObjectStatusNode ( contextObject, GameObject.Find("food_in_bowl") ),
																															new GoToObjectNode ( contextObject, GameObject.Find("Food Bowl") ),
																															new EatNode ( contextObject, GameObject.Find("food_in_bowl") )
																														),
																				/* Wandering Sequence */new SequenceNode (	contextObject,
																															new WaitNode ( contextObject, 5F),
																															new GoToRandomPointNode ( contextObject )
																														)
																			)
													);
		autonomousCatBehaviorTree.paused = false;
										
		userInteractionBehaviorTree = new BehaviorTree ( new SequenceNode 	( 	contextObject,
																				new GoToPointNode ( contextObject, inFrontOfUserPosition ),
																				new FocusOnUserNode ( contextObject, 7F )
																			)
														);
		userInteractionBehaviorTree.paused = true;
		
		
		// Initialize last update time to now
		last_update_time = Time.time;
    }
	
	// Called when there is no save to generate a new random cat
	void CreateNew()
	{	// Initialize stats for a completely content cat
		stats = new CatStats();
		// Initialize personality to random values
		personality = CatPersonality.RandomPersonality();
		// Initialize the style to random color
		style = CatStyle.RandomStyle();
	}

    // Update is called once per frame
    void Update()
    {
		// TODO: autosave
		//  Save game when S is pressed
        if(Input.GetKeyDown (KeyCode.S))
            Save();
		// Load game when L is pressed
        if (Input.GetKeyDown (KeyCode.L))
            Load();
		// If R is pressed, reset
		if (Input.GetKeyDown (KeyCode.R))
			CreateNew();

		// Calcuate time delta since last update, in seconds, fps-independent calculations
		float dt = Time.time - last_update_time;
		last_update_time = Time.time;

		// Update cat stats with current state
		personality.UpdateStats(ref stats, activity, dt);

		// Update UI
		stats.UpdateUI();

		// Run behavior tree
		autonomousCatBehaviorTree.run(Time.time);
		userInteractionBehaviorTree.run(Time.time);
		
		if (userInteractionBehaviorTree.paused == false)
		{
			Camera.main.transform.LookAt(gameObject.transform); // Main camera look at cat
		}
		
		// If in follow laser mode, follow laser
		if (CatActivityEnum.FollowingLaser == activity.current && SelectedTool.LASER_POINTER == selected_tool) {
			GoToLaserPointer();
		}

		// Log current state
		Debug.Log(activity);
        //Debug.Log(stats);
    }

	// Set cats waypoint to whatever 3D point the cursor points to
	void GoToLaserPointer()
	{
		// Find intersection of cursor and an object
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Physics.Raycast (ray, out hit))
		{
		 // Set waypoint to this point
		 agent.destination = hit.point;
		 // Update laser visualization
		 Vector3[] line_points = {
			 Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, 0, Camera.main.nearClipPlane)),
			 hit.point
		 };
		 laser_line.positionCount = line_points.Length;
		 laser_line.SetPositions(line_points);
		}
	}

	// Load the cat from a previous save
	public void Load()
	{
		// Load personality, stats and style
		personality = CatPersonality.Load();
		stats = CatStats.Load();
		style = CatStyle.Load();

		// Load cat pose
		Quaternion r = new Quaternion(PlayerPrefs.GetFloat("pose.r.x"),
			PlayerPrefs.GetFloat("pose.r.y"),
			PlayerPrefs.GetFloat("pose.r.z"),
			PlayerPrefs.GetFloat("pose.r.w"));
		Vector3 p = new Vector3(PlayerPrefs.GetFloat("pose.p.x"),
			PlayerPrefs.GetFloat("pose.p.y"),
			PlayerPrefs.GetFloat("pose.p.z"));
		gameObject.transform.SetPositionAndRotation(p, r);
		// TODO: color, other info
		
		Debug.Log("--- LOADED --");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log(style);
		Debug.Log("-------------");
	}

	// Save the current cat to a file for later resuming play
	public void Save()
	{
		// Save personality, stats, and style
		personality.Save();
		stats.Save();
		style.Save();

		// Save pose
		PlayerPrefs.SetFloat("pose.p.x",gameObject.transform.position.x);
		PlayerPrefs.SetFloat("pose.p.y",gameObject.transform.position.y);
		PlayerPrefs.SetFloat("pose.p.z",gameObject.transform.position.z);
		PlayerPrefs.SetFloat("pose.r.x",gameObject.transform.rotation.x);
		PlayerPrefs.SetFloat("pose.r.y",gameObject.transform.rotation.y);
		PlayerPrefs.SetFloat("pose.r.z",gameObject.transform.rotation.z);
		PlayerPrefs.SetFloat("pose.r.w",gameObject.transform.rotation.w);

		PlayerPrefs.SetFloat("savetime", Time.time);
		PlayerPrefs.SetInt("script_version", ScriptMajorVersion);
		PlayerPrefs.Save();

		Debug.Log("--- SAVED --");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log(style);
		Debug.Log("-------------");
	}

	// Change the currently selected tool
	void SelectTool(SelectedTool tool)
	{
		// If no change, nothing to do
		if (tool == selected_tool) return;

		// Log the change in tool
		Debug.Log(string.Format("Selected Tool {0}", tool));
		
		// TODO: set cursor, change current activity, other behavior for each tool
		Vector2 offset = new Vector2(0, 32);
		if (SelectedTool.HAND == tool)
		{
			Cursor.SetCursor(hand_cursor, offset, CursorMode.Auto);
			laser_line.enabled = false;
		} else if (SelectedTool.BRUSH == tool) {
			Cursor.SetCursor(brush_cursor, offset, CursorMode.Auto);
			laser_line.enabled = false;
		} else if (SelectedTool.FOOD == tool) {
			Cursor.SetCursor(food_cursor, offset, CursorMode.Auto);
			laser_line.enabled = false;
		} else if (SelectedTool.LASER_POINTER == tool) {
			Cursor.SetCursor(laser_cursor, offset, CursorMode.Auto);
			laser_line.enabled = true;
			// TODO(Alex): only change activity if not otherwise busy
			activity.current = CatActivityEnum.FollowingLaser;
		}

		selected_tool = tool;
	}
	
	void OnMouseDown ()
	{
		Debug.Log("Clicked on cat.");
		
		// If mouse just went down, and the user is currently petting or brushing the cat, start counting drag time
		if ((!is_drag) && (SelectedTool.HAND == selected_tool || SelectedTool.BRUSH == selected_tool)) {
			is_drag = true;
			drag_start_time = Time.time;
		}

 	}

 	void OnMouseUp ()
	{
		// When mouse released, act based on accumulated drag
		is_drag = false;
		double drag_time = Time.time - drag_start_time;
		time_of_last_user_interaction = Time.time;

 		// A short drag is registered as a click, causing cat to begin user interaction behaviors
		if (drag_time < 0.1) 
		{
			// Switch behavior trees
			turnOnUserInteractionCatBehavior();

 		}
		// Else if cat is in front of user...
		else if ( (GetComponent<Transform>().position - inFrontOfUserPosition).magnitude <= agent.stoppingDistance )
		{
			// If using hand tool, register as petting
			if (selected_tool == SelectedTool.HAND)
			{
				activity.current = CatActivityEnum.BeingPet;
			}
			// If using brush tool, register as brushing
			else if (selected_tool == SelectedTool.BRUSH)
			{
				activity.current = CatActivityEnum.BeingBrushed;
			}
		}


 	}

 	// Pause one behavior tree and activate the other
	public void turnOnAutonomousCatBehavior()
	{
		autonomousCatBehaviorTree.paused = false;
		userInteractionBehaviorTree.paused = true;
		
		Camera.main.GetComponent<CameraScript>().Reset();
	}

 	public void turnOnUserInteractionCatBehavior()
	{
		autonomousCatBehaviorTree.paused = true;
		userInteractionBehaviorTree.paused = false;
		
		Camera.main.transform.LookAt(gameObject.transform); // Main camera look at cat
	}
}
