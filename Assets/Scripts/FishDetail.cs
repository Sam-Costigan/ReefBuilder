using UnityEngine;
using System.Collections;

public class FishDetail : MonoBehaviour {
	public string fishType;
	public string fishbio;
	public bool isSelected= false;

	public bool expand = false;
	// Use this for initialization
	private GUI button ;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setVisble(bool v){
		isSelected = v;
	}


	void OnGUI(){
		
		if (!isSelected) {
			return;
		}

		if (GUI.Button (new Rect (700, 40, 50, 30), "detail")) {

			expand = !expand;
		}
		if (expand) {
			GUI.TextArea (new Rect (700, 70, 100, 100), fishbio);
		}






	}

}
