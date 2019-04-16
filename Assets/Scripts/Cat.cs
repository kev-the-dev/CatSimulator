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
	LITTER_SCOOPER,
	LASER_POINTER
}

// Behavior script for the cat. Manages the cats behaviors, stats, and personality
public class Cat : BaseCat
{
	// The cat's current activity/behavior/goal
	CatActivity activity;
	
	NavMeshAgent agent;
	Animator animator;
	bool forced_walk;
	bool forced_sit;
	
	// Autosave
	float last_autosave_time;
	const float AUTOSAVE_PERIOD_SECONDS = 60F;

	// Cat AI
	BehaviorTree autonomousCatBehaviorTree;
	BehaviorTree userInteractionBehaviorTree;
	Context contextObject;
	// Coroutine variables
	private bool waiting;
	public const float BT_TRAVERSAL_INTERVAL = 1F;	// Traverse the tree every second
	
	// Petting / brushing / summoning related variables
	Vector3 inFrontOfUserPosition;
	bool is_drag;
	double drag_start_time;
	public float time_of_last_user_interaction {get; private set;}
	
	// Can the cat currently use catnip? (if it is currently on catnip, using more will not do anything in order to prevent catnip buffs from stacking)
	public bool on_catnip {get; private set;}
	
	// UI Buttons
	private Button hand_button, brush_button, food_button, laser_button, litter_button;
	public SelectedTool selected_tool {get; private set;}
	public Texture2D hand_cursor;
	public Texture2D brush_cursor;
	public Texture2D food_cursor;
	public Texture2D laser_cursor;
	public Texture2D litter_cursor;

	// Laser pointer GameObject
	GameObject laserPointer;
	// Laser pointer GameObject's script
	LaserPointer laserPointerScript;
	
	float last_update_time;

    // Start is called before the first frame update
    void Start()
    {
		last_autosave_time = 0F;
		// Initialize agent
		agent = GetComponent<NavMeshAgent>();
		// Initialize animator
		animator = GameObject.Find("Cat_Model_Latest").GetComponent<Animator>();
		forced_walk = false;
		forced_sit = false;
		
		// Cat position for petting / brushing
		inFrontOfUserPosition = new Vector3(0F, -0.5F, -5F);
		
		// Get the laser pointer GameObject
		laserPointer = GameObject.Find("Laser Pointer");
		// Get the laser pointer GameObject's attached script
		laserPointerScript = laserPointer.GetComponent<LaserPointer>();
		// Deactivate laser pointer game object to turn it off
		laserPointer.SetActive(false);
		// Cat is not on catnip
		on_catnip = false;
		
		// Get Buttons
		hand_button = GameObject.Find("hand_button").GetComponent<Button>();
		brush_button = GameObject.Find("brush_button").GetComponent<Button>();
		food_button = GameObject.Find("food_button").GetComponent<Button>();
		laser_button = GameObject.Find("laser_button").GetComponent<Button>();
		litter_button = GameObject.Find("litter_button").GetComponent<Button>();
		// Initialize listeners
		hand_button.onClick.AddListener(delegate {SelectTool(SelectedTool.HAND);});
		brush_button.onClick.AddListener(delegate {SelectTool(SelectedTool.BRUSH);});
		food_button.onClick.AddListener(delegate {SelectTool(SelectedTool.FOOD);});
		laser_button.onClick.AddListener(delegate {SelectTool(SelectedTool.LASER_POINTER);});
		litter_button.onClick.AddListener(delegate {SelectTool(SelectedTool.LITTER_SCOOPER);});
		// Default to hand tool
		SelectTool(SelectedTool.HAND);

		// Previous save should always exist (otherwise adoption center would be loaded)
		// If for some reason we got here, crash.
		if (!GameSave.Valid()) {
			Debug.Log("LivingRoom loaded without a save! Quiting.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
			return;
#endif
		}

		// Load previous save / adopted cat
		Load();

		// Start off idle
		activity = new CatActivity( CatActivityEnum.Idle );
		
		// Initialize variables needed by behavior tree nodes
		contextObject = new Context( gameObject, ref personality, ref stats, ref activity );
		getPointDelegate getPointDel = laserPointerScript.getLaserIntersectionPoint;
		waiting = false;
		rng = new System.Random();
		
		// Construct the cat's behavior tree
        autonomousCatBehaviorTree = new BehaviorTree(	new SelectorNode	( 	contextObject,
		
																				/* Energy Sequence */				new SequenceNode 	(	contextObject, 
																																			new CheckEnergyNode ( contextObject ),
																																			new SleepNode ( contextObject , GameObject.Find("SleepingParticles"))
																																		),
																				/* Bladder Sequence */				new SequenceNode	(	contextObject,
																																			new CheckBladderNode ( contextObject ),
																																			new GoToObjectNode ( contextObject, GameObject.Find("Litterbox") ),
																																			new RelieveBladderNode ( contextObject )
																																		),
																				/* Hunger Sequence */				new SequenceNode 	( 	contextObject,
																																			new CheckFullnessNode ( contextObject ),
																																			new CheckObjectStatusNode ( contextObject, GameObject.Find("food_in_bowl") ),
																																			new GoToObjectNode ( contextObject, GameObject.Find("Food Bowl") ),
																																			new EatNode ( contextObject, GameObject.Find("food_in_bowl") )
																																		),
																				/* Chase Laser Pointer Sequence */	new SequenceNode 	( 	contextObject,
																																			new CheckObjectStatusNode ( contextObject, laserPointer ),
																																			new GoToDynamicPointNode ( contextObject, getPointDel ),
																																			new ChaseLaserNode ( contextObject )
																																		),
																				/* Chase Toy Ball Sequence */		new SequenceNode	(	contextObject,
																																			new CheckFunNode (contextObject),
																																			new CoinFlipNode (contextObject, rng, 15F),
																																			new CheckObjectStatusNode ( contextObject, GameObject.Find("Toy Ball") ),
																																			new PlayNode ( contextObject ),
																																			new GoToObjectNode ( contextObject, GameObject.Find("Toy Ball") )
																																		),
																				/* Chase Toy Mouse Sequence */		new SequenceNode	(	contextObject,
																																			new CheckFunNode (contextObject),
																																			new CheckObjectStatusNode ( contextObject, GameObject.Find("Mouse")),
																																			new PlayNode ( contextObject ),
																																			new GoToObjectNode ( contextObject, GameObject.Find("Mouse") )
																																		),
																				/* Wandering Sequence */			new SequenceNode 	(	contextObject,
																																			new WaitNode ( contextObject, 5F),
																																			new GoToRandomPointNode ( contextObject ),
																																			new IdleNode ( contextObject )
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

    // Update is called once per frame
    void Update()
    {
		// autosave if it's time
		if (Time.time - last_autosave_time > AUTOSAVE_PERIOD_SECONDS)
		{
			Save();
			last_autosave_time = Time.time;
			Debug.Log("Autosaving");
		}

		// Set walking / sitting animiation
		float velocity = agent.velocity.magnitude;
		if (velocity < 0.2) {
			animator.SetBool("moving", false);
			forced_walk = false;
			if (!forced_sit && animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) {
				animator.SetFloat("speed", 1.0F);
				animator.Play("Walk", -1, 0.9F);
				forced_sit = true;
			}
		} else {
			forced_sit = false;
			animator.SetBool("moving", true);
			if (!forced_walk && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
				animator.Play("Idle", -1, 0.8F);
				forced_walk = true;
			}
			animator.SetFloat("speed", velocity);
		}

		// Update achievements
		achievements.GetNewUnlocks();

		// TODO: autosave
		//  Save game when S is pressed
        if(Input.GetKeyDown (KeyCode.S))
            Save();
		// Load game when L is pressed
        if (Input.GetKeyDown (KeyCode.L))
            Load();
		// If R is pressed, reset
		if (Input.GetKeyDown (KeyCode.R)) {
			CreateNew();
			GameSave.Clear();
		}

		// Calcuate time delta since last update, in seconds, fps-independent calculations
		float dt = Time.time - last_update_time;
		achievements.time_played += dt;
		last_update_time = Time.time;

		// Update cat stats with current state
		personality.UpdateStats(ref stats, activity, dt);

		// Update UI
		stats.UpdateUI();

		// Run behavior tree. If a tree is "paused", it will not run.
		StartCoroutine(runTree(Time.time));
		
		// If cat is currently interacting with user, the camera should follow the cat
		if (userInteractionBehaviorTree.paused == false)
		{
			Camera.main.transform.LookAt(gameObject.transform); // Main camera look at cat
		}

		// Log current state
		//Debug.Log(activity);
        //Debug.Log(stats);
		//Debug.Log(achievements);
    }
	
	// Coroutine to run BT once every set interval
	IEnumerator runTree(float _startTime)
	{	
		// Do not execute coroutine if it is already running
		if (waiting)
		{
			yield break;
		}
		
		// Begin waiting
		waiting = true;
		
		// Traverse behavior trees
		//Debug.Log(string.Format("Running trees... Time = {0}", _startTime));
		autonomousCatBehaviorTree.run(_startTime);
		userInteractionBehaviorTree.run(_startTime);
		//Debug.Log(string.Format("Finished running trees... Time = {0}", Time.time));
		
		yield return new WaitForSeconds(BT_TRAVERSAL_INTERVAL);
		
		waiting = false;
	}

	// Change the currently selected tool
	void SelectTool(SelectedTool tool)
	{
		// If no change, nothing to do
		if (tool == selected_tool) return;
		
		// If selected tool is not the laser pointer, ensure the laser pointer GameObject is turned off
		if (tool != SelectedTool.LASER_POINTER)
		{
			laserPointer.SetActive(false);
		}
		
		// Log the change in tool
		Debug.Log(string.Format("Selected Tool {0}", tool));
		
		Vector2 offset = new Vector2(0, 32);
		
		if (SelectedTool.HAND == tool)
		{
			Cursor.SetCursor(hand_cursor, offset, CursorMode.Auto);
		} 
		else if (SelectedTool.BRUSH == tool) 
		{
			Cursor.SetCursor(brush_cursor, offset, CursorMode.Auto);
		} 
		else if (SelectedTool.FOOD == tool) 
		{
			Cursor.SetCursor(food_cursor, offset, CursorMode.Auto);
		} 
		else if (SelectedTool.LASER_POINTER == tool) 
		{
			Cursor.SetCursor(laser_cursor, offset, CursorMode.Auto);
			laserPointer.SetActive(true);
		}
		else if (SelectedTool.LITTER_SCOOPER == tool)
		{
			Cursor.SetCursor(litter_cursor, offset, CursorMode.Auto);
		}

		selected_tool = tool;
	}
	
	void OnMouseDown ()
	{
		//Debug.Log("Clicked on cat.");
		
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
				achievements.num_pets++;
			}
			// If using brush tool, register as brushing
			else if (selected_tool == SelectedTool.BRUSH)
			{
				activity.current = CatActivityEnum.BeingBrushed;
				achievements.num_brushes++;
			}
		}


 	}
	
	public IEnumerator useCatnip()
    {
        // Before Wait period:
		Debug.Log("Using Catnip...");
		activity.current = CatActivityEnum.OnCatnip;
		on_catnip = true;
		// Apply stat buffs
		stats.fun_buff.Value = stats.fun_buff.Value * CatStats.CATNIP_FUN_BUFF;
		stats.fun_debuff.Value = stats.fun_debuff.Value * CatStats.CATNIP_FUN_DEBUFF;
		stats.energy_debuff.Value = stats.energy_debuff.Value * CatStats.CATNIP_ENERGY_DEBUFF;
		agent.speed = agent.speed * CatStats.CATNIP_SPEED_BOOST;
		
		
        yield return new WaitForSeconds(CatnipScript.CATNIP_TIME_DURATION);
        

		// After Wait period:
		Debug.Log("Catnip effects have worn off.");
		activity.current = CatActivityEnum.Idle;
		on_catnip = false;
		// Remove stat buffs
		stats.fun_buff.Value = stats.fun_buff.Value / CatStats.CATNIP_FUN_BUFF;
		stats.fun_debuff.Value = CatStats.DEFAULT_BUFF_VALUE;
		stats.energy_debuff.Value = stats.energy_debuff.Value / CatStats.CATNIP_ENERGY_DEBUFF;
		agent.speed = agent.speed / CatStats.CATNIP_SPEED_BOOST;
		
    }
	
	public void resetActivity()
	{
		activity.current = CatActivityEnum.Idle;
	}
	
	public CatActivityEnum getCurrentActivity()
	{
		return activity.current;
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
