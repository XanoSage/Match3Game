using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
#if UNITY_EDITOR
	private static bool WasDestroyed;
#endif

	private static T m_Instance = null;
	public static T Instance
	{
		get
		{
			// Instance requiered for the first time, we look for it
			if (m_Instance == null)
			{
#if UNITY_EDITOR
				var t = typeof(T);
				if (WasDestroyed)
				{
					Debug.LogWarning("Re-creating " + t.Name);
					WasDestroyed = false;
				}
#endif
				m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

				// Object not found, we create a temporary one
				if (m_Instance == null)
				{
					m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
					// Problem during the creation, this should not happen
					if (m_Instance == null)
					{
						Debug.Log( "Problem during the creation of " + typeof(T).ToString());
					}
				}
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool Exists { get { return m_Instance != null; } }

	// If no other monobehaviour request the instance in an awake function
	// executing before this one, no need to search the object.
	/*
    private void Awake()
    {
        if( m_Instance == null )
        {
            m_Instance = this as T;
            m_Instance.Init();
        }
    }
 	*/
	// This function is called when the instance is used the first time
	// Put all the initializations you need here, as you would do in Awake
	public virtual void Init() { }

	protected virtual void OnDestroying() { }

	// Make sure the instance isn't referenced anymore when the user quit, just in case.
	protected void OnDestroy()
	{
		OnDestroying();
		m_Instance = null;
#if UNITY_EDITOR
		WasDestroyed = true;
#endif
	}
}