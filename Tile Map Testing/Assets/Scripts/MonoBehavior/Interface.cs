using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour {
	public static Interface instance;

	void Awake (){
		instance = this;
	}


}
