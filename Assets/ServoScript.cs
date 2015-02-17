using UnityEngine;
using System.Collections;

public class ServoScript : MonoBehaviour {
private string insertedArmSpeed = "0";
private int temp = 0;
private HingeJoint[] hingeJoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		GUI.Box(new Rect (10,10,180,150), "Insert speed");
		insertedArmSpeed = GUI.TextField(new Rect(13, 30, 80, 20), insertedArmSpeed, 25);
		if (GUI.Button (new Rect (100,30,80,20), "Apply left")) {
			hingeJoints = GetComponents<HingeJoint>();
			int.TryParse(insertedArmSpeed, out temp);
			HingeJoint joint = hingeJoints[0];
			if (temp >= -100 && temp <= 100) {
				JointMotor m = joint.motor;
				m.targetVelocity = temp;
				joint.motor = m;
			}
			else if (temp < -100) {
				JointMotor m = joint.motor;
				m.targetVelocity = -100;
				joint.motor = m;
			}
			else if (temp > 100) {
				JointMotor m = joint.motor;
				m.targetVelocity = 100;
				joint.motor = m;
			}
		}
		if (GUI.Button (new Rect (100, 55, 80, 20), "Apply right")) {
			hingeJoints = GetComponents<HingeJoint>();
			int.TryParse(insertedArmSpeed, out temp);
			HingeJoint joint = hingeJoints[1];
			if (temp >= -100 && temp <= 100) {
				JointMotor m = joint.motor;
				m.targetVelocity = temp;
				joint.motor = m;
			}
			else if (temp < -100) {
				JointMotor m = joint.motor;
				m.targetVelocity = -100;
				joint.motor = m;
			}
			else if (temp > 100) {
				JointMotor m = joint.motor;
				m.targetVelocity = 100;
				joint.motor = m;
			}
		}
		
	}
}
