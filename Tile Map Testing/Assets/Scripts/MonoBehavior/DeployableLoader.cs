using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployableLoader : MonoBehaviour {

	public static DeployableLoader instance;
	Dictionary<string, Deployables> deployableModels;

	void Awake () {
		instance = this;
		LoadModels ();
	}

	void LoadModels () {
		deployableModels = new Dictionary<string, Deployables> ();
		deployableModels.Add ("Cabinet", Deployables.LoadDeployables ("Cabinet", 0.5f, 1, 1));
		deployableModels.Add ("Bed", Deployables.LoadDeployables ("Bed", 0.5f, 1, 2));
	}

	public Deployables GetModel (string model) {
		if (deployableModels.ContainsKey (model))
			return deployableModels [model];
		else {
			Debug.LogError ("Trying to build a Deployable from a model that doesn't exist: " + model);
			return null;
		}
	}
}
