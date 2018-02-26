using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTracker : MonoBehaviour {
	public int level;
	public int x;
	public int y;
	public enum Layer {Floor, Wall, Deployables, Overlay};
	public Layer layer;
}
