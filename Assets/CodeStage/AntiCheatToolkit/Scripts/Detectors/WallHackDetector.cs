#define DEBUG
#undef DEBUG

using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Detects two types of wallhack cheating: Rigidbody patch and CharacterController patch.
	/// It creates service objects at spawnPosition within 3,3,3 cube.
	/// This cube is drawn as red wireframe gismo when you select GameObject with WallHackDetector.
	/// Please, use spawnPosition to place it at the unused space of your game to avoid any unnecessary collisions.
	/// </summary>
	/// You also may add WallHackDetector to any GameObject as usual or through the<br/>
	/// "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit > WallHack Detector" menu.
	/// 
	/// It allows you to edit and store detector's settings in inspector.<br/>
	/// <strong>Please, keep in mind you still need to call WallHackDetector.StartDetection() to start detector!</strong><br/><br/>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly this detector adds new objects to the scene while running:
	/// [WH Detector Service] container with scaled cube and 2 capsules inside, one is driven by Rigidbody, another one with CharacterController.<br/>
	/// Thus it will take up additional resources for physics calculations.</strong>
	[DisallowMultipleComponent]
	public class WallHackDetector : ActDetectorBase
	{
		private const string COMPONENT_NAME = "WallHack Detector";
		private const string SERVICE_CONTAINER_NAME = "[WH Detector Service]";

		private readonly Vector3 rigidPlayerVelocity = new Vector3(0, 0, 1f);

		internal static bool isRunning;
		
		/// <summary>
		/// World position of the service container. 
		/// Please keep in mind it will have objects within 3, 3, 3 cube.
		/// It should be placed away from your game objects to avoid collisions and false positives.
		/// </summary>
		[Tooltip("World position of the container for service objects within 3x3x3 cube (drawn as red wireframe cube in scene).")]
		public Vector3 spawnPosition;

		private int whLayer = -1;
		private GameObject serviceContainer;
		private Rigidbody rigidPlayer;
		private CharacterController charControllerPlayer;
		private float charControllerVelocity = 0;

#if DEBUG
		private bool rigidDetected = false;
		private bool controllerDetected = false;
#endif

		#region ComponentPlacement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(MENU_PATH + COMPONENT_NAME, false, 1)]
		private static void AddToScene()
		{
			SetupDetectorInScene<WallHackDetector>();
		}
#endif
		#endregion

		/// <summary>
		/// Allows reaching public properties from code. Can be null.
		/// </summary>
		public static WallHackDetector Instance { get; private set; }

		private static WallHackDetector GetOrCreateInstance
		{
			get
			{
				if (Instance == null)
				{
					WallHackDetector detector = FindObjectOfType<WallHackDetector>();
					if (detector != null)
					{
						Instance = detector;
					}
					else
					{
						if (detectorsContainer == null)
						{
							detectorsContainer = new GameObject(CONTAINER_NAME);
						}
						detectorsContainer.AddComponent<WallHackDetector>();
					}
				}
				return Instance;
			}
		}

		/// <summary>
		/// Starts detection using settings from inspector or defaults:<br/>
		/// spawnPosition = Vectro3(0, 0, 0).
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(System.Action callback)
		{
			StartDetection(callback, GetOrCreateInstance.spawnPosition);
		}

		/// <summary>
		/// Starts detection using passed servicePosition.<br/>
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="servicePosition">World position of the service 3x3x3 container.</param>
		public static void StartDetection(System.Action callback, Vector3 servicePosition)
		{
			GetOrCreateInstance.StartDetectionInternal(callback, servicePosition);
		}

		/// <summary>
		/// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			if (Instance != null) Instance.StopDetectionInternal();
		}

		/// <summary>
		/// Stops and completely disposes detector component. Game Object will be automatically destroyed as well if no other components left on it and it has no children.
		/// </summary>
		public static void Dispose()
		{
			if (Instance != null) Instance.DisposeInternal();
		}

		// preventing direct instantiation =P
		private WallHackDetector() { }

		private void Awake()
		{
			if (Init(Instance, COMPONENT_NAME))
			{
				Instance = this;
			}
		}

		private void StartDetectionInternal(System.Action callback, Vector3 servicePosition)
		{
			if (isRunning)
			{
				Debug.LogWarning("[ACTk] " + COMPONENT_NAME + " already running!");
				return;
			}

			if (!enabled)
			{
				Debug.LogWarning("[ACTk] " + COMPONENT_NAME + " disabled but StartDetection still called from somewhere!");
				return;
			}

			onDetection = callback;
			spawnPosition = servicePosition;

			InitDetector();

			isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (isRunning)
			{
				UninitDetector();
				onDetection = null;
				isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			if (!isRunning) return;

			isRunning = false;
			StopRigidModule();
			StopControllerModule();
		}

		protected override void ResumeDetector()
		{
			isRunning = true;
			StartRigidModule();
			StartControllerModule();
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this) Instance = null;
		}

		private void InitDetector()
		{
			InitCommon();
			InitRigidModule();
			InitControllerModule();

			StartRigidModule();
			StartControllerModule();
		}

		private void UninitDetector()
		{
			isRunning = false;
			StopRigidModule();
			StopControllerModule();
			Destroy(serviceContainer);
		}

		private void InitCommon()
		{
			if (whLayer == -1) whLayer = LayerMask.NameToLayer("Ignore Raycast");

			serviceContainer = new GameObject(SERVICE_CONTAINER_NAME);
			serviceContainer.layer = whLayer;
			serviceContainer.transform.position = spawnPosition;
			DontDestroyOnLoad(serviceContainer);

			GameObject wall = new GameObject("Wall");
			wall.AddComponent<BoxCollider>();
			wall.layer = whLayer;
			wall.transform.parent = serviceContainer.transform;
			wall.transform.localPosition = Vector3.zero;

			wall.transform.localScale = new Vector3(3, 3, 0.5f);
		}

		private void InitRigidModule()
		{
			GameObject player = new GameObject("RigidPlayer");
			player.AddComponent<CapsuleCollider>().height = 2;
			player.layer = whLayer;
			player.transform.parent = serviceContainer.transform;
			player.transform.localPosition = new Vector3(0.75f, 0, -1f);
			rigidPlayer = player.AddComponent<Rigidbody>();
			rigidPlayer.useGravity = false;
		}

		private void InitControllerModule()
		{
			GameObject player = new GameObject("ControlledPlayer");
			player.AddComponent<CapsuleCollider>().height = 2;
			player.layer = whLayer;
			player.transform.parent = serviceContainer.transform;
			player.transform.localPosition = new Vector3(-0.75f, 0, -1f);
			charControllerPlayer = player.AddComponent<CharacterController>();
		}

		private void StartRigidModule()
		{
			rigidPlayer.rotation = Quaternion.identity;
			rigidPlayer.angularVelocity = Vector3.zero;
			rigidPlayer.transform.localPosition = new Vector3(0.75f, 0, -1f);
			rigidPlayer.velocity = rigidPlayerVelocity;
			Invoke("StartRigidModule", 4);
		}

		private void StopRigidModule()
		{
			rigidPlayer.velocity = Vector3.zero;
			CancelInvoke("StartRigidModule");
		}

		private void StartControllerModule()
		{
			charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0, -1f);
			charControllerVelocity = 0.01f;
			Invoke("StartControllerModule", 4);
		}

		private void StopControllerModule()
		{
			charControllerVelocity = 0;
			CancelInvoke("StartControllerModule");
		}
		
		private void FixedUpdate()
		{
			if (!isRunning) return;

			if (rigidPlayer.transform.localPosition.z > 1f)
			{
#if DEBUG
				rigidDetected = true;
#endif
				StopRigidModule();

				Detect();
			}
		}

		private void Update()
		{
			if (!isRunning) return;

			if (charControllerVelocity > 0)
			{
				charControllerPlayer.Move(new Vector3(Random.Range(-0.002f, 0.002f), 0, charControllerVelocity));

				if (charControllerPlayer.transform.localPosition.z > 1f)
				{
#if DEBUG
					controllerDetected = true;
#endif
					StopControllerModule();

					Detect();
				}
			}
		}

		private void Detect()
		{
			if (onDetection != null)
			{
				onDetection();
			}

			if (autoDispose)
			{
				Dispose();
			}
			else
			{
				StopDetection();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(spawnPosition, new Vector3(3, 3, 3));
		}

#if DEBUG
		private void OnGUI()
		{
			GUILayout.Label("Rigid detected: " + rigidDetected);
			GUILayout.Label("Controller detected: " + controllerDetected);
		}
#endif
	}
}