using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RightFootStep : MonoBehaviour {	
	private List<GameObject> forcePoints;
	private List<GameObject> forceRenderers;
	private Dictionary<string, Component> limbs;	
	private Dictionary<string, GameObject> arms;
	private List<Vector3> desiredForcePoints;
	
	private float previousStateTime;
	public Vector3 leftFootPosition;
	public Vector3 rightFootPosition;
	public Vector3 centerOfMass;
	
	private float forceRate;
	private float halfShoulderDistance; 
	private bool once = true;
	public Vector3 desired = new Vector3();
	public int previousState;
	
	public float timer;
	public int state;
	public Transform forceRendererPrefab;
	
	public Transform centerOfMassObject;
	public Transform desiredCenterOfMassObject;

	// Use this for initialization
	void Start () {
		limbs = new Dictionary<string, Component>();
		arms = new Dictionary<string, GameObject>(); 
		desiredForcePoints = new List<Vector3>();
		forcePoints = new List<GameObject>();
		forceRenderers = new List<GameObject>();
		timer = 0;
		state = 0;	
		previousState = 0;	
		
		foreach (Component c in gameObject.GetComponentsInChildren<HingeJoint>()) {
			limbs.Add(c.name, c);
		}
		
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Arm")) {
			arms.Add (g.name.Remove (g.name.Length - 9), g);
		}
		
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Point")) {
			forcePoints.Add(g);		
		}		
		
		forcePoints.Sort(
			delegate(GameObject i1, GameObject i2)
			{
			return i1.name.CompareTo(i2.name);
		}
		);					
		
		GameObject[] robotParts = GameObject.FindGameObjectsWithTag("Robot");
		float mass = 0f;
		
		foreach (GameObject c in robotParts) {
			mass += c.rigidbody.mass;
		}
		
		forceRate = mass*mass;	
				
		leftFootPosition = limbs["left_foot"].transform.position;
		rightFootPosition = limbs["right_foot"].transform.position;
		
		desired = leftFootPosition + new Vector3(0.25f, 0, 0.07f);
		UpdateDesiredForcePosition();
		
		
		halfShoulderDistance = Mathf.Abs((forcePoints[0].transform.position.x - forcePoints[1].transform.position.x)/2);
	}
	
	public void Setup() {		
		leftFootPosition = limbs["left_foot"].transform.position;
		rightFootPosition = limbs["right_foot"].transform.position;		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		timer += Time.deltaTime;
		
		if (state <= 10) {
			centerOfMassObject.transform.position = rigidbody.worldCenterOfMass;
			desiredCenterOfMassObject.transform.position = desired;
			if (state == 1) { state = 2; desired = leftFootPosition + new Vector3(0, 0, 0.07f); }
			else if (state == 2) { desired = leftFootPosition + new Vector3(0, 0, 0.07f); } // + new Vector3(0, 0, 0.07f); }
			else if (state == 3) { desired = leftFootPosition + new Vector3(0, 0, 0.07f); bendLeg("right", 60); }
			else if (state == 4) { desired = leftFootPosition + new Vector3(0, 0, 0.3f);} 
			else if (state == 5) { desired = leftFootPosition + new Vector3(0, 0, 0.3f); bendLeg("left", -10); } 
			else if (state == 6) { desired = leftFootPosition + new Vector3(0, 0, 0.3f); bendLeg ("right", 1); } 
			else if (state == 7) { desired = rightFootPosition; } 
			else if (state == 8) { desired = rightFootPosition; bendLeg("left", 30); } 
			else if (state == 9) { desired = rightFootPosition; bendLeg("left", 1); } 
			else if (state == 10) { 
			if (CheckStateTransition()) {
				previousStateTime = timer;
			}
			if (timer - previousStateTime >= 1 && once) {
				leftFootPosition = limbs["left_foot"].transform.position;
				desired = leftFootPosition + new Vector3(0.25f, 0, 0.07f);	
				UpdateDesiredForcePosition();
				once = false;
				print ("bom");
				
			}		
			
			if ( timer - previousStateTime > 1 && Mathf.Abs(rigidbody.worldCenterOfMass.x - desired.x) <= 0.2f && Mathf.Abs(rigidbody.worldCenterOfMass.z - desired.z) <= 0.2f) {
				//state = 1;
				//once = true;
			}
			//
				//GetComponent<LeftFootStep>().Setup();
				//GetComponent<LeftFootStep>().leftStepState = 1; 									
		}			
		}
		moveWeight (desired);
		changeState(desired);
		
		
	}	
		
	void moveWeight(Vector3 desiredPosition) {			
		int i = 0;
		if (CheckStateTransition()) UpdateDesiredForcePosition();		
		foreach (GameObject f in forcePoints) {				
				Vector3 c = Vector3.zero;
				c.x = desiredForcePoints[i].x - f.transform.position.x;
				c.z = desiredForcePoints[i].z - f.transform.position.z;

				rigidbody.AddForceAtPosition(forceRate*c, f.transform.position);
				
				drawForces(f, c);
				i++;
		}
				
		
	}
	
	void UpdateDesiredForcePosition() {
		desiredForcePoints = new List<Vector3>();
		//print (state);
		foreach (GameObject g in forcePoints) {
			desiredForcePoints.Add(desired - rigidbody.worldCenterOfMass + g.transform.position);
		}				
	}
	
	void changeState(Vector3 targetPosition) {		
		//if (state == 3 || state == 5 || state == 6 || state == 7 || state == 8 || state == 9) state++;
		if (Mathf.Abs(rigidbody.worldCenterOfMass.x - targetPosition.x) <= 0.2f && Mathf.Abs(rigidbody.worldCenterOfMass.z - targetPosition.z) <= 0.2f) {
			if (state == 2) state = 3;
			else if (state == 4) state = 5;
			else if (state == 7) state = 8;
		
		}			
	}
	
	bool CheckStateTransition() {
		if (previousState != state) {
			previousState = state;
			return true;
		} else return false;		
	}
	
	void drawForces(GameObject f, Vector3 c) {	
		if (f.GetComponent<LineRenderer>() != null)	{
			LineRenderer line = f.GetComponent<LineRenderer>();
			line.SetWidth(0.02f, 0.02f);
			line.SetPosition(0, f.transform.position); 	
			line.SetPosition(1, f.transform.position + c);
		}			
	}
	
	void clearForces() {
		foreach (GameObject g in forcePoints) {
			LineRenderer l = g.GetComponent<LineRenderer>(); 
			l.SetPosition(0, Vector3.zero);
			l.SetPosition(1, Vector3.zero);							
		}
	}
	
	void bendRightLeg(float thightAnlge, float kneeAnlge, float footAnlge) {
		RotateCertainAngle(limbs["right_thight"], thightAnlge, 150);
		RotateCertainAngle(limbs["right_knee"], kneeAnlge, 150);
		RotateCertainAngle(limbs["right_foot"], footAnlge, 150);
	}	
	
	void bendLeg(string name, float angle) {
		if (name == "right" && state < 5) {
			
			RotateCertainAngle(limbs["right_thight"], angle, 150);
			RotateCertainAngle(limbs["right_knee"], -angle, 150);
			//print(limbs["right_thight"].name + angle);
			
			if (limbs["right_thight"].hingeJoint.angle > angle - 5f) {
				state = 4;
				//print ("Step State: " + rightStepState);
			}
		}
		
		else if (name == "right" && state == 6) {
			//RotateCertainAngle(angle, limbs[3]);
			RotateCertainAngle(limbs["right_knee"], -angle, 150);
			//print(limbs[3].hingeJoint.angle);
			
			if (limbs["right_knee"].hingeJoint.angle < angle + 5f) {
				state = 7;
				//print ("Step State: " + rightStepState);
				rightFootPosition = limbs["right_foot"].transform.position;
			}
		}
		
		else if (name == "left" && state == 5) {
			RotateCertainAngle(limbs["left_thight"], angle, 150);
			//print(limbs[0].hingeJoint.angle);
			
			if (limbs["left_thight"].hingeJoint.angle < -angle + 5f) {
				state = 6;
				//print ("Step State: " + rightStepState);
			}
		}
		
		else if (name == "right" && state == 7) {
			RotateCertainAngle(limbs["right_thight"], angle, 150);
			//print(limbs[3].hingeJoint.angle);
			
			if (limbs["left_thight"].hingeJoint.angle < angle + 5f) {
				state = 8;
				//print ("Step State: " + rightStepState);
			}
		}
		
		else if (name == "left" && state == 8) {
			RotateCertainAngle(limbs["left_thight"], angle, 150);
			RotateCertainAngle(limbs["left_knee"], -angle, 150);
			RotateCertainAngle(limbs["left_foot"], angle, 150);
			//print(limbs[0].hingeJoint.angle);
			
			if (limbs["left_thight"].hingeJoint.angle > angle - 5f) {
				state = 9;
				//print ("Step State: " + rightStepState);
			}					
		}
		
		else if (name == "left" && state == 9) {
			RotateCertainAngle(limbs["left_thight"], angle, 50);
			RotateCertainAngle(limbs["left_knee"], -angle, 50);
			RotateCertainAngle(limbs["left_foot"], angle, 50);
			//print(limbs[0].hingeJoint.angle);
			
			if (limbs["left_thight"].hingeJoint.angle < angle + 1f) {
				foreach (KeyValuePair<string, Component> h in limbs) {
					JointLimits l = new JointLimits();
					l.min = 0;
					l.max = 0;
					h.Value.hingeJoint.limits = l;				
				}
				leftFootPosition = limbs["left_foot"].transform.position;
				rightFootPosition = limbs["right_foot"].transform.position;
				state = 10;
				//print ("Step State: " + rightStepState);
			}
		}
		
	}
	
	void RotateCertainAngle(Component limb, float angle, int force) {
		JointLimits j = limb.hingeJoint.limits;
		JointMotor m = limb.hingeJoint.motor;
		if (angle >= 0) {
			j.max = angle;
			m.targetVelocity = 80;
		} else if (angle < 0) {
			j.min = angle;
			m.targetVelocity = -80;
		}
		
		limb.hingeJoint.limits = j;
		
		m.force = force;
		limb.hingeJoint.motor = m;
	}
}
