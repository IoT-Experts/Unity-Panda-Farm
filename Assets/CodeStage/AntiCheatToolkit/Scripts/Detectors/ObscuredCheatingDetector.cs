using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Detects cheating of any Obscured type (except ObscuredPrefs, it has own detection features) used in project.
	/// It allows cheaters to find desired (fake) values in memory and change them, keeping original values secure.
	/// It's like a cheese in the mouse trap - cheater try to change some obscured value and get caught on it.
	/// Just call ObscuredCheatingDetector.StartDetection() to use it.
	/// </summary>
	/// You also may add it to any GameObject as usual or through the<br/>
	/// "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit > Obscured Cheating Detector" menu.
	/// 
	/// It allows you to edit and store detector's settings in inspector.<br/>
	/// <strong>Please, keep in mind you still need to call ObscuredCheatingDetector.StartDetection() to start detector!</strong>
	[DisallowMultipleComponent]
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		private const string COMPONENT_NAME = "Obscured Cheating Detector";
		internal static bool isRunning;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.
		/// </summary>
		[HideInInspector]
		public float floatEpsilon = 0.0001f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.
		/// </summary>
		[HideInInspector] 
		public float vector2Epsilon = 0.1f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.
		/// </summary>
		[HideInInspector]
		public float vector3Epsilon = 0.1f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.
		/// </summary>
		[HideInInspector]
		public float quaternionEpsilon = 0.1f;

		#region ComponentPlacement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(MENU_PATH + COMPONENT_NAME, false, 1)]
		private static void AddToScene()
		{
			SetupDetectorInScene<ObscuredCheatingDetector>();
		}
#endif
		#endregion

		/// <summary>
		/// Allows reaching public properties from code. Can be null.
		/// </summary>
		public static ObscuredCheatingDetector Instance { get; private set; }

		private static ObscuredCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (Instance == null)
				{
					ObscuredCheatingDetector detector = FindObjectOfType<ObscuredCheatingDetector>();
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
						detectorsContainer.AddComponent<ObscuredCheatingDetector>();
					}
				}
				return Instance;
			}
		}

		/// <summary>
		/// Starts all Obscured types cheating detection.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(System.Action callback)
		{
			GetOrCreateInstance.StartDetectionInternal(callback);
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
		private ObscuredCheatingDetector() { }

		private void Awake()
		{
			if (Init(Instance, COMPONENT_NAME))
			{
				Instance = this;
			}
		}

		private void StartDetectionInternal(System.Action callback)
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
			isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (isRunning)
			{
				onDetection = null;
				isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			isRunning = false;
		}

		protected override void ResumeDetector()
		{
			isRunning = true;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this) Instance = null;
		}

		internal void OnCheatingDetected()
		{
			if (onDetection != null)
			{
				onDetection();

				if (autoDispose)
				{
					Dispose();
				}
				else
				{
					StopDetection();
				}
			}
		}
	}
}