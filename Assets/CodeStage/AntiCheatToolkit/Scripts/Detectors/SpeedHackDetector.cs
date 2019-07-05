using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Allows to detect Cheat Engine's speed hack (and maybe some other speed hack tools) usage.
	/// Just call SpeedHackDetector.StartDetection() to use it.
	/// </summary>
	/// You also may add it to any GameObject as usual or through the<br/>
	/// "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit > Speed Hack Detector" menu.
	/// 
	/// It allows you to edit and store detector's settings in inspector.<br/>
	/// <strong>Please, keep in mind you still need to call ObscuredCheatingDetector.StartDetection() to start detector!</strong>
	[DisallowMultipleComponent]
	public class SpeedHackDetector : ActDetectorBase
	{
		private const string COMPONENT_NAME = "Speed Hack Detector";
		private const long TICKS_PER_SECOND = TimeSpan.TicksPerMillisecond * 1000;

		// maximum allowed time difference (in ticks)
		// used to compare difference between genuine ticks and vulnerable ticks
		private const int THRESHOLD = 5000000; // = 500 ms

		internal static bool isRunning;

		/// <summary> 
		/// Time (in seconds) between detector checks.
		/// </summary>
		[Tooltip("Time (in seconds) between detector checks.")]
		public float interval = 1f;

		/// <summary>
		/// Maximum false positives count allowed before registering speed hack.
		/// </summary>
		[Tooltip("Maximum false positives count allowed before registering speed hack.")]
		public byte maxFalsePositives = 3;

		/// <summary>
		/// Amount of sequential successful checks before clearing internal false positives counter.<br/>
		/// Set 0 to disable Cool Down feature.
		/// </summary>
		[Tooltip("Amount of sequential successful checks before clearing internal false positives counter.\nSet 0 to disable Cool Down feature.")]
		public int coolDown = 30;

		private byte currentFalsePositives;
		private int currentCooldownShots;
		private long ticksOnStart;
		private long vulnerableTicksOnStart;
		private long prevTicks;
		private long prevIntervalTicks;

		#region ComponentPlacement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(MENU_PATH + COMPONENT_NAME, false, 1)]
		private static void AddToScene()
		{
			SetupDetectorInScene<SpeedHackDetector>();
		}
#endif
		#endregion

		/// <summary>
		/// Allows reaching public properties from code. Can be null.
		/// </summary>
		public static SpeedHackDetector Instance { get; private set; }

		private static SpeedHackDetector GetOrCreateInstance
		{
			get
			{
				if (Instance == null)
				{
					SpeedHackDetector detector = FindObjectOfType<SpeedHackDetector>();
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
						detectorsContainer.AddComponent<SpeedHackDetector>();
					}
				}
				return Instance;
			}
		}

		/// <summary>
		/// Starts speed hack detection using settings from inspector or defaults:<br/>
		/// interval = 1, maxFalsePositives = 3, coolDown = 10.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(System.Action callback)
		{
			StartDetection(callback, GetOrCreateInstance.interval);
		}

		/// <summary>
		/// Starts speed hack detection using passed checkInterval.<br/>
		/// Other settings used from inspector or defaults:<br/>
		/// maxFalsePositives = 3, coolDown = 10.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="checkInterval">Time in seconds between speed hack checks.</param>
		public static void StartDetection(System.Action callback, float checkInterval)
		{
			StartDetection(callback, checkInterval, GetOrCreateInstance.maxFalsePositives);
		}

		/// <summary>
		/// Starts speed hack detection using passed checkInterval and maxErrors.<br/>
		/// Default (10) or inspector value used for coolDown.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="checkInterval">Time in seconds between speed hack checks.</param>
		/// <param name="falsePositives">Amount of possible false positives.</param>
		public static void StartDetection(System.Action callback, float checkInterval, byte falsePositives)
		{
			StartDetection(callback, checkInterval, falsePositives, GetOrCreateInstance.coolDown);
		}

		/// <summary>
		/// Starts speed hack detection using passed checkInterval, maxErrors and coolDown. 
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="checkInterval">Time in seconds between speed hack checks.</param>
		/// <param name="falsePositives">Amount of possible false positives.</param>
		/// <param name="shotsTillCooldown">Amount of sequential successful checks before resetting false positives counter.</param>
		public static void StartDetection(System.Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			GetOrCreateInstance.StartDetectionInternal(callback, checkInterval, falsePositives, shotsTillCooldown);
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
		private SpeedHackDetector() { }

		private void Awake()
		{
			if (Init(Instance, COMPONENT_NAME))
			{
				Instance = this;
			}
		}

		private void StartDetectionInternal(System.Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
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
			interval = checkInterval;
			maxFalsePositives = falsePositives;
			coolDown = shotsTillCooldown;

			ResetStartTicks();
			currentFalsePositives = 0;
			currentCooldownShots = 0;

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

		private void ResetStartTicks()
		{
			ticksOnStart = DateTime.UtcNow.Ticks;
			vulnerableTicksOnStart = System.Environment.TickCount * TimeSpan.TicksPerMillisecond;
			prevTicks = ticksOnStart;
			prevIntervalTicks = ticksOnStart;
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				//Debug.LogWarning("UNPAUSE");
				ResetStartTicks();
			}
		}

		private void Update()
		{
			if (!isRunning) return;

			long ticks = 0;

			ticks = DateTime.UtcNow.Ticks;

			long ticksSpentSinceLastUpdate = ticks - prevTicks;
		
			if (ticksSpentSinceLastUpdate < 0 || ticksSpentSinceLastUpdate > TICKS_PER_SECOND)
			{
				if (Debug.isDebugBuild) Debug.LogWarning("[ACTk] SpeedHackDetector: System DateTime change or > 1 second game freeze detected!");
				ResetStartTicks();
				return;
			}

			prevTicks = ticks;

			long intervalTicks = (long)(interval * TICKS_PER_SECOND);

			if (ticks - prevIntervalTicks >= intervalTicks)
			{
				long vulnerableTicks = 0;

				vulnerableTicks = System.Environment.TickCount * TimeSpan.TicksPerMillisecond;

				if (Mathf.Abs((vulnerableTicks - vulnerableTicksOnStart) - (ticks - ticksOnStart)) > THRESHOLD)
				{
					currentFalsePositives++;
					if (currentFalsePositives > maxFalsePositives)
					{
						if (Debug.isDebugBuild) Debug.LogWarning("[ACTk] SpeedHackDetector: final detection!");
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
					else
					{
						if (Debug.isDebugBuild) Debug.LogWarning("[ACTk] SpeedHackDetector: detection! Allowed false positives left: " + (maxFalsePositives - currentFalsePositives));
						currentCooldownShots = 0;
						ResetStartTicks();
					}
				}
				else if (currentFalsePositives > 0 && coolDown > 0)
				{
					if (Debug.isDebugBuild) Debug.LogWarning("[ACTk] SpeedHackDetector: success shot! Shots till Cooldown: " + (coolDown - currentCooldownShots));
					currentCooldownShots++;
					if (currentCooldownShots >= coolDown)
					{
						if (Debug.isDebugBuild) Debug.LogWarning("[ACTk] SpeedHackDetector: Cooldown!");
						currentFalsePositives = 0;
					}
				}

				prevIntervalTicks = ticks;
			}
		}
	}
}