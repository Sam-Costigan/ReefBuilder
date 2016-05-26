using UnityEngine;
using System.Collections;

[System.Serializable]
public class FishScript : MonoBehaviour {

    private float speed = 10;
    public float minSpeed = 10;
    public float maxSpeed = 10;

    private float xRot = -0.01f;
    public float yRot = 2f;
    private float zRot = 0f;

    private bool upSpeed = true;

    public bool positiveZ = false;

	const int  EATMODE = 1;
	const int MOVETOMODE =2;
	const int  RANDMODE = 0;
	int mode =RANDMODE;
	Transform target;

    public Vector3 initialMove = new Vector3(0, 0, 0);
	// Use this for initialization
	void Start () {
        transform.Translate(initialMove);
        speed = Random.Range(minSpeed, maxSpeed);

        if(Random.Range(0, 3) == 2)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            yRot = -yRot;
        }

        transform.Translate(0, Random.Range(-1, 15), 0);

	}
	
	// Update is called once per frame
	void Update () {

		if (RANDMODE == mode) {
			randomMove ();
			return;
		}

		if (MOVETOMODE == mode) {





			RaycastHit ray;
			Physics.Raycast (gameObject.transform.position, target.position, out ray);

			if (ray.transform.gameObject == null) {
				Debug.Log ("ray was null");
				randomMove ();

			} else if (ray.transform.gameObject.GetInstanceID () == target.gameObject.GetInstanceID ()) {

				Debug.Log ("move to");
				moveToTarget ();
			} else {
				Debug.Log ("something in the way");
				randomMove ();

			}

			if (Vector3.Distance (gameObject.transform.position, target.position) < 1) {
				mode = EATMODE;
				Debug.Log ("Fish in eatmode");

			}


		}
	}




	public void setTarget(Transform target){
		this.target = target;
		mode = MOVETOMODE;
	}


	public void randomMove(){

		float runtime = Time.time;

		float currYRot = (Mathf.Sin(runtime)+2f)*yRot;

		//upSpeed = (Random.Range(0, 10) > 7);

		if (upSpeed)
		{
			//speed += speed * 0.1f;
		} else
		{
			// speed -= speed * 0.2f;
		}

		if (positiveZ)
		{
			speed = -speed;
		}

		transform.Translate(0, 0, -speed * Time.deltaTime);
		transform.Rotate(new Vector3(xRot, currYRot, zRot) * Time.deltaTime);

		if (positiveZ)
		{
			speed = -speed;
		}



	}


	public void moveToTarget(){
		float runtime = Time.time;

		float currYRot = (Mathf.Sin(runtime)+2f)*yRot;

		if (positiveZ)
		{
			speed = -speed;
		}

		Vector3 tVec = Vector3.Lerp (gameObject.transform.position, target.position,-speed * Time.deltaTime );


		transform.Translate (tVec);




		transform.Rotate(new Vector3(xRot, currYRot, zRot) * Time.deltaTime);

		if (positiveZ)
		{
			speed = -speed;
		}


}
