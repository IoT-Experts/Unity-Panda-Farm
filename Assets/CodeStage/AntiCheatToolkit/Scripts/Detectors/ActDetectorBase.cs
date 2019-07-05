using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Base class for all detectors.
	/// </summary>
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		protected const string CONTAINER_NAME = "Anti-Cheat Toolkit Detectors";
		protected const string MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

		/// <summary>
		/// Detector component will be automatically disposed after firing callback if enabled.
		/// Otherwise, it will just stop internal processes.
		/// </summary>
		/// Game Object will be automatically destroyed as well if no other components left on it and it has no children.
		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		/// <summary>
		/// Detector will survive new level (scene) load if checked.
		/// Otherwise, it will be destroyed.
		/// </summary>
		/// Game Object will be automatically destroyed as well if no other components left on it and it has no children.
		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		protected static GameObject detectorsContainer;
		protected System.Action onDetection = null;

		private bool inited;
		
#region ComponentPlacement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(MENU_PATH + "All detectors", false, 0)]
		private static void AddAllDetectorsToScene()
		{
			SetupDetectorInScene<InjectionDetector>();
			SetupDetectorInScene<ObscuredCheatingDetector>();
			SetupDetectorInScene<SpeedHackDetector>();
		}

		protected static void SetupDetectorInScene<T>() where T: ActDetectorBase
		{
			T component = FindObjectOfType<T>();
			string detectorName = typeof(T).Name;

			if (component != null)
			{
				if (component.gameObject.name == CONTAINER_NAME)
				{
					UnityEditor.EditorUtility.DisplayDialog(detectorName + " already exists!", detectorName + " already exists in scene and correctly placed on object \"" + CONTAINER_NAME + "\"", "OK");
				}
				else
				{
					int dialogResult = UnityEditor.EditorUtility.DisplayDialogComplex(detectorName + " already exists!", detectorName + " already exists in scene and placed on object \"" + component.gameObject.name + "\". Do you wish to move it to the Game Object \"" + CONTAINER_NAME + "\" or delete it from scene at all?", "Move", "Delete", "Cancel");
					switch (dialogResult)
					{
						case 0:
							GameObject container = GameObject.Find(CONTAINER_NAME);
							if (container == null)
							{
								container = new GameObject(CONTAINER_NAME);
							}
							T newComponent = container.AddComponent<T>();
							UnityEditor.EditorUtility.CopySerialized(component, newComponent);
							DestroyDetectorImmediate(component);
							break;
						case 1:
							DestroyDetectorImmediate(component);
							break;
					}
				}
			}
			else
			{
				GameObject container = GameObject.Find(CONTAINER_NAME);
				if (container == null)
				{
					container = new GameObject(CONTAINER_NAME);
				}
				container.AddComponent<T>();

				UnityEditor.EditorUtility.DisplayDialog(detectorName + " added!", detectorName + " successfully added to the object \"" + CONTAINER_NAME + "\"", "OK");
			}
		}

		private static void DestroyDetectorImmediate(ActDetectorBase component)
		{
			if (component.transform.childCount == 0 && component.GetComponentsInChildren<Component>(true).Length <= 2)
			{
				DestroyImmediate(component.gameObject);
			}
			else
			{
				DestroyImmediate(component);
			}
		}
#endif
#endregion

		private void Start()
		{
			inited = true;
		}

		protected virtual bool Init(ActDetectorBase instance, string detectorName)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				Destroy(this);
				return false;
			}

			DontDestroyOnLoad(gameObject);
			return true;
		}

		private void OnDisable()
		{
			if (!inited) return;
			PauseDetector();
		}

		private void OnEnable()
		{
			if (!inited || onDetection == null) return;
			ResumeDetector();
		}

		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

		private void OnLevelWasLoaded(int index)
		{
			if (!inited) return;
			if (!keepAlive)
			{
				DisposeInternal();
			}
		}

		protected abstract void StopDetectionInternal();
		protected abstract void PauseDetector();
		protected abstract void ResumeDetector();

		protected virtual void DisposeInternal()
		{
			StopDetectionInternal();
			Destroy(this);
		}

		protected virtual void OnDestroy()
		{
			if (transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
			{
				Destroy(gameObject);
			}
			else if (name == CONTAINER_NAME && GetComponentsInChildren<ActDetectorBase>().Length <= 1)
			{
				Destroy(gameObject);
			}
		}
	}
}
