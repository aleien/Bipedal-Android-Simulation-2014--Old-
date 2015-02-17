using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkingScript : MonoBehaviour {
	
	private List<GameObject> forcePoints;
	private List<GameObject> forceRenderers;
	private string walkingButtonText;
	private string throwSpheresText;
	private bool moving;
	private bool throwingSpheres;
	private bool balancing;
	private Vector3 spheresPosition;
	private float lastThrowingTime;
	
	public RightFootStep rightStep;	
	public LeftFootStep leftStep;	
	public GameObject shell;

	void Start () {		
		forcePoints = new List<GameObject>();
		forceRenderers = new List<GameObject>();
		walkingButtonText = "Walk";
		throwSpheresText = "Throw Spheres";
		moving = false;
		throwingSpheres = false;
		balancing = true;
		spheresPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
		
		lastThrowingTime = 0;
		//Time.timeScale = 0.5f;			  		
	}
	
	void Update () {	
		spheresPosition.x = transform.position.x;
		
		
		/*if (transform.position.y < 0.5f) {
			balancing = false;
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("Point")) {
				foreach(Transform t in g.GetComponentsInChildren<Transform>()) {
					Destroy(t.gameObject);
				}
			}
			
		}*/
		
		if (throwingSpheres == true && lastThrowingTime + 0.1f < Time.time) {
		
			Instantiate(shell, spheresPosition + new Vector3 (Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0f), new Quaternion(0f, 0f, 0f, 0f));
			lastThrowingTime = Time.time;
		}
	}
	

	
	void OnGUI () {
		// Make a background box
		//GUI.Box(new Rect(10,30,100,20), "Time: " + Mathf.Floor(Time.time*100)/100);
		if (GUI.Button (new Rect (10,50,100,50), walkingButtonText)) {
			
			if (moving == false) {
				rightStep.state = 1;
				leftStep.state = 0;
				walkingButtonText = "Stop";
				moving = true;
			} else {
				rightStep.state = 0;
				leftStep.state = 0;
				walkingButtonText = "Walk";
				moving = false;
			}
		}
		if (GUI.Button (new Rect (10,110,100,50), throwSpheresText)) {
			if (throwingSpheres == false) {
				throwingSpheres = true;
				lastThrowingTime = Time.time;
				print (lastThrowingTime);
			} else {
				throwingSpheres = false;
			}
		}	
		
		GUI.Box(new Rect(Screen.width - 300, 50 , 280, 300), " ");
		GUI.Label(new Rect(Screen.width - 290, 50 , 280, 300), "X: " + rightStep.transform.position.x + " Y: " + rightStep.transform.position.y + " Z: " + rightStep.transform.position.z);
		GUI.Label (new Rect(Screen.width - 290, 65 , 280, 300), "Right foot");
		GUI.Label (new Rect(Screen.width - 290, 80 , 280, 300), "X: " + rightStep.rightFootPosition.x + " Y: " + rightStep.rightFootPosition.y + " Z: " + rightStep.rightFootPosition.z);
		GUI.Label (new Rect(Screen.width - 290, 95 , 280, 300), "State: " + rightStep.state);
		GUI.Label (new Rect(Screen.width - 290, 110 , 280, 300), "Previous State: " + rightStep.previousState);
		GUI.Label (new Rect(Screen.width - 290, 125 , 280, 300), "Desired: " + rightStep.desired);
		
		GUI.Label (new Rect(Screen.width - 290, 160 , 280, 300), "Left foot");
		GUI.Label (new Rect(Screen.width - 290, 175 , 280, 300), "X: " + rightStep.leftFootPosition.x + " Y: " + rightStep.leftFootPosition.y + " Z: " + rightStep.leftFootPosition.z);
		GUI.Label (new Rect(Screen.width - 290, 190 , 280, 300), "State: " + leftStep.state);
		GUI.Label (new Rect(Screen.width - 290, 205 , 280, 300), "Previous State: " + leftStep.previousState);
		GUI.Label (new Rect(Screen.width - 290, 220 , 280, 300), "Desired: " + leftStep.desired);
		
		GUI.Label (new Rect(Screen.width - 290, 240 , 280, 300), "Center of mass: " + rigidbody.worldCenterOfMass);
		//GUI.TextArea(new Rect(Screen.width - 300, 50 , 280, 300), "X: " + rightStep.transform.position.x + " Y: " + rightStep.transform.position.y + " Z: " + rightStep.transform.position.z);
		
		
		
	}
}
