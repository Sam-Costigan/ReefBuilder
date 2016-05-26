using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	// Only one object can be dragged at a time,
	// so this attribute is static for that reason.
	public static GameObject draggedObject;

	public GameObject terrain;
	public Transform prefab;

	public Vector3 startPosition;

	public Text costText;

	private Image image;
	private Transform coral;
	private GameScript gameScript;
	private CoralScript coralScript;

	void Start () {
		gameScript = GameObject.Find ("GameController").GetComponent<GameScript> ();
		coralScript = prefab.GetComponent<CoralScript> ();
		image = GetComponent<Image> ();
		image.sprite = coralScript.icon;
		costText.text = "" + coralScript.getCost();
	}

	public void Update() {
		if (gameScript.getPolyps () < coralScript.getCost ()) {
			image.color = Color.black;
		} else {
			image.color = Color.white;
		}
	}

	public void OnBeginDrag (PointerEventData eventData) {
		// While in deletion mode, preventing placing objects
		if (gameScript.getDeleteMode () || (gameScript.getPolyps() < coralScript.getCost())) {
			return;
		}

		draggedObject = gameObject;
		startPosition = transform.position;

		coral = (Transform)Instantiate(prefab, Vector3.zero, Quaternion.identity);
		coral.GetComponent<SnapToTerrain>().terrain = terrain;

		Camera.main.GetComponent<MouseOrbit>().lockCamera(true);
	}

	public void OnDrag (PointerEventData eventData) {
		// This needs to be here to maintain the drag, otherwise it
		// ends immediately.
	}

	public void OnEndDrag (PointerEventData eventData) {
		// While in deletion mode, preventing placing objects
		if (gameScript.getDeleteMode () || (gameScript.getPolyps() < coralScript.getCost())) {
			return;
		}

		draggedObject = null;
		transform.position = startPosition;

		bool placed = coral.GetComponent<SnapToTerrain>().SetLocked(true);

		// If the component was placed, then remove currency equal to the objects
		// cost. If it wasn't, destroy the created object.
		if (placed) {
			gameScript.removePolyps (coralScript.getCost());
		} else {
			Destroy (coral.gameObject);
		}

		coral = null;
		Camera.main.GetComponent<MouseOrbit>().lockCamera(false);
	}
}