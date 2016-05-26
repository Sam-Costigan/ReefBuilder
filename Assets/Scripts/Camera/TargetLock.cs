using UnityEngine;
using System.Collections;

public class TargetLock : MonoBehaviour {

    private Vector3 touchPos = Vector2.zero;
    private bool init = false;
	private GameScript gameScript;
	// Use this for initialization
	void Start () {
		gameScript = GameObject.Find ("GameController").GetComponent<GameScript> ();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            touchPos = Input.mousePosition;
            init = true;
        }

        if (init && !Input.GetMouseButton(0) && (Input.mousePosition - touchPos).magnitude < 5)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
            RaycastHit[] rays = Physics.RaycastAll(ray, 1000);
            float minDist = float.MaxValue;
            Transform closest = null;

            foreach(RaycastHit hit in rays)
            {
                //If it's the right kind of object..
                if (hit.transform.GetComponent<CameraCanSnapTo>() != null)
                {
                   if(hit.distance < minDist)
                    {
                        minDist = hit.distance;
                        closest = hit.transform;
                    }
                }
            }
            if (closest != null && closest != Camera.main.GetComponent<MouseOrbit>().target)
            {
				Debug.Log (closest.name);
				if (closest.name == "CenterObject") {
					gameScript.buildMode ();
				} else {
					gameScript.focusMode (closest.gameObject);
				}

				Camera.main.GetComponent<MouseOrbit>().target = closest;
                Camera.main.GetComponent<MouseOrbit>().distance = 15;
            }

        }

     

    }
}
