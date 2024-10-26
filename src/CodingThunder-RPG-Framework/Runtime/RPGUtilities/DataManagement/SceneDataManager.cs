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
    public static SceneDataManager Instance;

	private void Awake()
	{
		if (Instance != null)
        {
            Destroy(this);
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
        id_chain.RemoveAt(0);
        return gObject;
    }
}
