using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using System.Linq;
using CodingThunder.RPGUtilities.DataManagement;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance;
    Dictionary<string, Actor> actorLookup = new Dictionary<string, Actor>();

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
