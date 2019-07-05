#define DEBUG
#undef DEBUG

#define DEBUG_VERBOSE
#undef DEBUG_VERBOSE

#define DEBUG_PARANOID
#undef DEBUG_PARANOID

using System.IO;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using Debug = UnityEngine.Debug;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
using System.Diagnostics;
#endif

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Allows to detect foreign managed assemblies in your app.
	/// Just call InjectionDetector.StartDetection() to use it.
	/// </summary>
	/// You also may add it to any GameObject as usual or through the<br/>
	/// "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit > Injection Detector" menu.
	/// 
	/// It allows you to edit and store detector's settings in inspector.<br/>
	/// <strong>Please, keep in mind you still need to call InjectionDetector.StartDetection() to start detector!
	/// 
	/// Note #1: %InjectionDetector works in conjunction with "Enable Injection Detector" option at the<br/>
	/// "Window > Anti-Cheat Toolkit > Options" window.<br/>
	/// Make sure you enabled it there before using detector at runtime.
	/// 
	/// Note #2: Please, make sure you tested %InjectionDetector on target platform before releasing your app to the public.</strong><br/>
	/// <em>I also would be very happy to know if it do false positives for you!</em>
	/// 
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly only PC (including WebPlayer), iOS and Android are supported.</strong>
	[DisallowMultipleComponent]
	public class InjectionDetector : ActDetectorBase
	{
		private const string COMPONENT_NAME = "Injection Detector";

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID
		internal static bool isRunning;

		private bool signaturesAreNotGenuine;
		private AllowedAssembly[] allowedAssemblies;
		private string[] hexTable;

		#region ComponentPlacement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(MENU_PATH + COMPONENT_NAME, false, 1)]
		private static void AddToScene()
		{
			SetupDetectorInScene<InjectionDetector>();
		}
#endif
		#endregion

		/// <summary>
		/// Allows reaching public properties from code. Can be null.
		/// </summary>
		public static InjectionDetector Instance { get; private set; }

		private static InjectionDetector GetOrCreateInstance
		{
			get
			{
				if (Instance == null)
				{
					InjectionDetector detector = FindObjectOfType<InjectionDetector>();
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
						detectorsContainer.AddComponent<InjectionDetector>();
					}
				}
				return Instance;
			}
		}

		/// <summary>
		/// Starts foreign assemblies injection detection.
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
		private InjectionDetector() { }

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

#if UNITY_EDITOR
			if (!EditorPrefs.GetBool("ACTDIDEnabled", false))
			{
				Debug.LogWarning("[ACTk] " + COMPONENT_NAME + " is not enabled in Anti-Cheat Toolkit Options!\nPlease, check readme for details.");
				DisposeInternal();
				return;
			}
#if !DEBUG && !DEBUG_VERBOSE && !DEBUG_PARANOID
			if (Application.isEditor)
			{
				Debug.LogWarning("[ACTk] " + COMPONENT_NAME + " does not work in editor (check readme for details).");
				DisposeInternal();
				return;
			}
#else
			Debug.LogWarning("[ACTk] " + COMPONENT_NAME + " works in debug mode. There WILL BE false positives in editor, it's fine!");
#endif
#endif
			onDetection = callback;

			if (allowedAssemblies == null)
			{
				LoadAndParseAllowedAssemblies();
			}

			if (signaturesAreNotGenuine)
			{
				OnInjectionDetected();
				return;
			}

			if (!FindInjectionInCurrentAssemblies())
			{
				// listening for new assemblies
				AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
				isRunning = true;
			}
			else
			{
				OnInjectionDetected();
			}
		}

		protected override void StopDetectionInternal()
		{
			if (isRunning)
			{
				AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
				onDetection = null;
				isRunning = false;
			}
		}

		protected override void PauseDetector()
		{
			isRunning = false;
			AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
		}

		protected override void ResumeDetector()
		{
			isRunning = true;
			AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this) Instance = null;
		}

		private void OnInjectionDetected()
		{
			if (onDetection != null) onDetection();
			if (autoDispose)
			{
				Dispose();
			}
			else
			{
				StopDetectionInternal();
			}
		}

		private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Debug.Log("[ACTk] New assembly loaded: " + args.LoadedAssembly.FullName);
#endif
			if (!AssemblyAllowed(args.LoadedAssembly))
			{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
				Debug.Log("[ACTk] Injected Assembly found:\n" + args.LoadedAssembly.FullName);
#endif
				OnInjectionDetected();
			}
		}

		private bool FindInjectionInCurrentAssemblies()
		{
			bool result = false;
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			Assembly[] assembliesInCurrentDomain = AppDomain.CurrentDomain.GetAssemblies();
			if (assembliesInCurrentDomain.Length == 0)
			{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
				stopwatch.Stop();
				Debug.Log("[ACTk] 0 assemblies in current domain! Not genuine behavior.");
				stopwatch.Start();
#endif
				result = true;
			}
			else
			{
				foreach (Assembly ass in assembliesInCurrentDomain)
				{
#if DEBUG_VERBOSE	
				stopwatch.Stop();
				Debug.Log("[ACTk] Currenly loaded assembly:\n" + ass.FullName);
				stopwatch.Start();
#endif
					if (!AssemblyAllowed(ass))
					{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
						stopwatch.Stop();
						Debug.Log("[ACTk] Injected Assembly found:\n" + ass.FullName + "\n" + GetAssemblyHash(ass));
						stopwatch.Start();
#endif
						result = true;
						break;
					}
				}
			}

#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			stopwatch.Stop();
			Debug.Log("[ACTk] Loaded assemblies scan duration: " + stopwatch.ElapsedMilliseconds + " ms.");
#endif
			return result;
		}

		private bool AssemblyAllowed(Assembly ass)
		{
#if !UNITY_WEBPLAYER
			string assemblyName = ass.GetName().Name;
#else
			string fullname = ass.FullName;
			string assemblyName = fullname.Substring(0, fullname.IndexOf(", ", StringComparison.Ordinal));
#endif

			int hash = GetAssemblyHash(ass);
			
			bool result = false;
			for (int i = 0; i < allowedAssemblies.Length; i++)
			{
				AllowedAssembly allowedAssembly = allowedAssemblies[i];

				if (allowedAssembly.name == assemblyName)
				{
					if (Array.IndexOf(allowedAssembly.hashes, hash) != -1)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		private void LoadAndParseAllowedAssemblies()
		{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Debug.Log("[ACTk] Starting LoadAndParseAllowedAssemblies()");
			Stopwatch sw = Stopwatch.StartNew();
#endif
			TextAsset assembliesSignatures = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (assembliesSignatures == null)
			{
				signaturesAreNotGenuine = true;
				return;
			}
			
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debug.Log("[ACTk] Creating separator array and opening MemoryStream");
			sw.Start();
#endif

			string[] separator = {":"};

			MemoryStream ms = new MemoryStream(assembliesSignatures.bytes);
			BinaryReader br = new BinaryReader(ms);
			
			int count = br.ReadInt32();
			
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debug.Log("[ACTk] Allowed assemblies count from MS: " + count);
			sw.Start();
#endif

			allowedAssemblies = new AllowedAssembly[count];

			for (int i = 0; i < count; i++)
			{
				string line = br.ReadString();
#if (DEBUG_PARANOID)
				sw.Stop();
				Debug.Log("[ACTk] Line: " + line);
				sw.Start();
#endif
				line = ObscuredString.EncryptDecrypt(line, "Elina");
#if (DEBUG_PARANOID)
				sw.Stop();
				Debug.Log("[ACTk] Line decrypted : " + line);
				sw.Start();
#endif
				string[] strArr = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				int stringsCount = strArr.Length;
#if (DEBUG_PARANOID)
				sw.Stop();
				Debug.Log("[ACTk] stringsCount : " + stringsCount);
				sw.Start();
#endif
				if (stringsCount > 1)
				{
					string assemblyName = strArr[0];

					int[] hashes = new int[stringsCount - 1];
					for (int j = 1; j < stringsCount; j++)
					{
						hashes[j - 1] = int.Parse(strArr[j]);
					}

					allowedAssemblies[i] = (new AllowedAssembly(assemblyName, hashes));
				}
				else
				{
					signaturesAreNotGenuine = true;
					br.Close();
					ms.Close();
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
					sw.Stop();
#endif
					return;
				}
			}
			br.Close();
			ms.Close();
			Resources.UnloadAsset(assembliesSignatures);

#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debug.Log("[ACTk] Allowed Assemblies parsing duration: " + sw.ElapsedMilliseconds + " ms.");
#endif

			hexTable = new string[256];
			for (int i = 0; i < 256; i++)
			{
				hexTable[i] = i.ToString("x2");
			}
		}

		private int GetAssemblyHash(Assembly ass)
		{
			string hashInfo;

#if !UNITY_WEBPLAYER
			AssemblyName assName = ass.GetName();
			byte[] bytes = assName.GetPublicKeyToken();
			if (bytes.Length == 8)
			{
				hashInfo = assName.Name + PublicKeyTokenToString(bytes);
			}
			else
			{
				hashInfo = assName.Name;
			}
#else
			string fullName = ass.FullName;

			string assemblyName = fullName.Substring(0, fullName.IndexOf(", ", StringComparison.Ordinal));
			int tokenIndex = fullName.IndexOf("PublicKeyToken=", StringComparison.Ordinal) + 15;
			string token = fullName.Substring(tokenIndex, fullName.Length - tokenIndex);
			if (token == "null") token = "";
			hashInfo = assemblyName + token;
#endif

			// Jenkins hash function (http://en.wikipedia.org/wiki/Jenkins_hash_function)
			int result = 0;
			int len = hashInfo.Length;

			for (int i = 0; i < len; ++i)
			{
				result += hashInfo[i];
				result += (result << 10);
				result ^= (result >> 6);
			}
			result += (result << 3);
			result ^= (result >> 11);
			result += (result << 15);

			return result;
		}

#if !UNITY_WEBPLAYER
		private string PublicKeyTokenToString(byte[] bytes)
		{
			string result = "";

			// AssemblyName.GetPublicKeyToken() returns 8 bytes
			for (int i = 0; i < 8; i++)
			{
				result += hexTable[bytes[i]];
			}

			return result;
		}
#endif

		private class AllowedAssembly
		{
			public readonly string name;
			public readonly int[] hashes;

			public AllowedAssembly(string name, int[] hashes)
			{
				this.name = name;
				this.hashes = hashes;
			}
		}
#else
		//! @cond
		public static InjectionDetector Instance
		{
			get
			{
				Debug.LogError(COMPONENT_NAME + " is not supported on selected platform!");
				return null;
			}
		}

		public static void StopDetection()
		{
			Debug.LogError(COMPONENT_NAME + " is not supported on selected platform!");
		}

		public static void Dispose()
		{
			Debug.LogError(COMPONENT_NAME + " is not supported on selected platform!");
		}

		public static void StartDetection(System.Action callback)
		{
			Debug.LogError(COMPONENT_NAME + " is not supported on selected platform!");
		}

		protected override void PauseDetector()
		{
			
		}

		protected override void ResumeDetector()
		{
			
		}

		protected override void StopDetectionInternal()
		{
			
		}
		//! @endcond
#endif
	}
}
