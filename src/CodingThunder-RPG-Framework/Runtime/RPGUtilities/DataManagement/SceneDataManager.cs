using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using System.Linq;
using CodingThunder.RPGUtilities.DataManagement;

/// <summary>
/// Use this to grab any object from a scene by its name.
/// It's just a little simpler than calling GameObject.Find().
/// If you just write $$Scene, it'll return the active Scene itself.
/// As I write that, I realize that's probably broken. Try fetching the active scene at your own risk.
/// </summary>
public class SceneDataManager : MonoBehaviour
{
    private HashSet<GameObject> nonDestroyedObjects = new HashSet<GameObject>();

    public static SceneDataManager Instance;

    public void RegisterDontDestroyOnLoad(GameObject otherObject)
    {
        if (nonDestroyedObjects.Contains(otherObject))
        {
            return;
        }

        nonDestroyedObjects.Add(otherObject);
        DontDestroyOnLoad(otherObject);

    }

    public void DeregisterDontDestroyOnLoad(GameObject otherGameObject)
    {
        if (nonDestroyedObjects.Contains(otherGameObject))
        {
            nonDestroyedObjects.Remove(otherGameObject);
        }
    }

	private void Awake()
	{
		if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        LookupResolver.Instance.RegisterRootKeyword("Scene", Lookup);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	/// <summary>
	/// This can lookup 3 steps deep. GameObject->Property, or GameObject->ComponentInChildren->Property.
	/// It can't go any deeper because screw you, that's why.
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public object Lookup(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        var id_chain = id.Split('.').ToList();
        return Lookup(id_chain);
    }

    /// <summary>
    /// I don't know, something something something.
    /// </summary>
    /// <param name="id_chain"></param>
    /// <returns></returns>
    public object Lookup(List<string> id_chain) 
    {
        if (id_chain == null ||  id_chain.Count == 0 )
        {
            return SceneManager.GetActiveScene();
        }

        if (id_chain[0] == "Scene")
        {
            id_chain.RemoveAt(0);
        }

        if (id_chain.Count == 0)
        {
            return SceneManager.GetActiveScene();
        }

        var gObject = GameObject.Find(id_chain[0]);

        if (gObject == null)
        {
            gObject = FindInactiveObjectByName(id_chain[0]);
        }

        id_chain.RemoveAt(0);
        return gObject;
    }

    /// <summary>
    /// Breadth-first search. Long-term, we should probably just register the damn objects in a dictionary.
    /// But get it working first, then worry about getting it working fast.
    /// Why the hell Unity doesn't include inactive objects in GameObject.Find is beyond me.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject FindInactiveObjectByName(string name)
    {
        // Get all root objects in the active scene
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        // Find all objects marked with DontDestroyOnLoad
        GameObject[] dontDestroyObjects = nonDestroyedObjects.ToArray();

        // Combine both arrays (scene root objects and DontDestroyOnLoad objects)
        GameObject[] allRootObjects = rootObjects.Concat(dontDestroyObjects).ToArray();

        Queue<GameObject> queue = new Queue<GameObject>();

        // Enqueue all root objects
        foreach (GameObject rootObject in allRootObjects)
        {
            queue.Enqueue(rootObject);
        }

        // Perform a breadth-first search
        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();

            // Check if the current object's name matches
            if (current.name == name)
            {
                return current;
            }

            // Enqueue all children of the current object
            foreach (Transform child in current.transform)
            {
                queue.Enqueue(child.gameObject);
            }
        }

        return null; // Object not found
    }
}
