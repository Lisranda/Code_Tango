using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class DeployableLoader : MonoBehaviour {
	public static DeployableLoader instance;
	Dictionary<string, Deployables> deployableModels;

	void Awake () {
		instance = this;
		deployableModels = new Dictionary<string, Deployables> ();
		LoadModelsFromXML ();
	}

	void LoadModelsFromXML () {
		XDocument xmlImport = XDocument.Load ("Assets/Resources/Config/Deployables.xml");
		foreach (XElement e in xmlImport.Root.Elements ()) {
			string name = e.Element ("name").Value;
			int sizeX = int.Parse(e.Element ("sizeX").Value);
			int sizeY = int.Parse(e.Element ("sizeY").Value);
			float travelSpeed = float.Parse(e.Element ("travelSpeed").Value);
			deployableModels.Add (name, Deployables.CreateModels (name, travelSpeed, sizeX, sizeY));
		}
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
