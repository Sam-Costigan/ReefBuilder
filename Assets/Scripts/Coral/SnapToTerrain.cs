using UnityEngine;
using System.Collections;

/** 
    
     Snaps an object to terrain and allows the object to be locked (for example, when
    the coral is placed).  When unlocked, the object will follow the mouse/touch on screen.
    
    On creation, the state will be UNLOCKED.  This means when the coral is created, whether
    it's through the UI or a hotkey or any other means, it will be 'attached' to the mouse.
    
    SetLocked(bool locked) should be called externally when the user wants to place the 
    coral, e.g when the mouse is released, using coral-object.GetComponent<SnapToTerrain>().SetLocked(false)  

    Some notes:

    SetLocked will return true if the desired setting is possible at the current time (so it will return false if
    the coral can not currently be placed, for example if it's not on appropriate terrain)

    bool CanPlace() can be called to check whether the object can currently be placed.  This can be used
    for things like highlighting the object red/green depending on whether it can be placed.

    IMPORTANT: DIRECTLY AFTER INSTANTIATION, 'terrain' SHOULD BE SET TO THE TERRAIN WE WANT TO PLACE ON.

    */
public class SnapToTerrain : MonoBehaviour {

    private bool locked = false;
    private bool onTerrain = false;

    public GameObject terrain;
    public float defaultDistanceFromCamera = 2;

	public Vector3 initialEulerRotation = new Vector3 (0, 0, 0);
	public Vector3 positionOffset = new Vector3(0, 0, 0);

	private float rotationSpeed = 50f;
	private float rotAroundForward = 0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (locked)
        {
            return;
        }

        //Store the mouse/touch position in screenPos
        Vector2 screenPos = Vector2.zero;
        if (!locked)
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touches.Length != 0)
                {
                    screenPos = Input.touches[0].position;
                }
            }
            else
            {
               
                if (Input.GetMouseButton(0))
                {
                    screenPos = Input.mousePosition;
                }
            }
        }

        //Find the 3D position of the screen point we have.  If there is terrain there lock to that, and change the angle to something appropriate (mean of normals in an area or something?).
        
        float maxDist = 1000;
        float defaultDist = defaultDistanceFromCamera;
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        float minDist = float.MaxValue;
		onTerrain = false;
        foreach(GameObject terrain in rocks)
        {
            if (terrain.GetComponent<MeshCollider>().Raycast(ray, out hit, maxDist) && hit.distance < minDist)
            {

                minDist = hit.distance;

                Vector3 point = hit.point;
                Vector3 smoothNormal = SmoothedNormal(hit);
                //Point now contains the point on the terrain that we want to place our object.  
                //We should now get the rotation to plce our object with, that is, that normals of the area of the terrain.
                Quaternion rot = Quaternion.LookRotation(smoothNormal, Vector3.up);

                transform.position = point;
                transform.rotation = rot;
				transform.Rotate (initialEulerRotation);
				transform.RotateAround (transform.position, smoothNormal, rotAroundForward += rotationSpeed * Time.deltaTime);
				transform.Translate (positionOffset);
                onTerrain = true;
            }   
        }

        if (!onTerrain)
        {
            Vector3 point = ray.GetPoint(defaultDist);
            transform.position = point;
        }

        //Debug.Log("Can place here: " + CanPlace());

    }

    public Vector3 SmoothedNormal(RaycastHit hit)
    {

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return hit.normal;
        }

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];
        Vector3 baryCenter = hit.barycentricCoordinate;
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        interpolatedNormal = interpolatedNormal.normalized;
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

		if ((interpolatedNormal - hit.normal).magnitude > 0.3f) {

			return hit.normal;

		}
			

        return interpolatedNormal;
    }

    void FixedUpdate()
    {

    }

    /**
        Locks/Unlocks the object at it's current position.  When unlocked,
        the object will follow the mouse position  (on the terrain if mouse is over it,
        or in 3D space if the mouse is not over the terrain)
    */
    public bool SetLocked(bool locked)
    {

        if (locked)
        {
            bool canPlace = CanPlace();
            if (canPlace)
            {
                this.locked = true;
                return true;
            } else
            {
                return false;
            }
        }

        this.locked = locked;
        return true;
    }

    /**

        Checks whether the object could be placed where it currently sits; whether or not
        the object is over terrain, with no coral in the way.

        This method only checks collisions with other objects with the tag 'Coral'

        Will always return true if the object is already locked.

        This method assumes the object has an attached Collider

    */
    public bool CanPlace()
    {

        if (!onTerrain)
        {
            return false;
        }

        if (locked)
        {
            return true;
        }

        Collider col = GetComponent<Collider>();
   
        bool anotherCoral = false;

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Coral");

        foreach(GameObject ob in objs)
        {
            if (ob != gameObject && ob.GetComponent<Collider>().bounds.Intersects(col.bounds))
            {
                anotherCoral = true;
            }
        }
       
        return !anotherCoral;
    }

}
