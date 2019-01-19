using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class CustomIK : MonoBehaviour {

	#region public transforms
	public Transform rootBone;
	public Transform elbowBone;
	public Transform endBone;

	public Transform targetIK;
	public Transform elbowIK;
    #endregion

    #region public settings
    public bool debugLines;
	public bool handRotation;
	public bool isEnabled;
	#endregion

	void Update () {
		if (isEnabled) {
            if (rootBone == null || elbowBone == null || endBone == null || targetIK == null || elbowIK == null)
            {
                return;
            }
			CalculateIK ();
		}
	}

	void CalculateIK(){
		#region declaring and calculating
		float upperArmLength = Vector3.Distance(rootBone.position, elbowBone.position);
		float forearmLength = Vector3.Distance(elbowBone.position, endBone.position);
		float armLength = upperArmLength + forearmLength;
		float minimumLength = upperArmLength - forearmLength;
		float targetDistance = Vector3.Distance(rootBone.position, targetIK.position);
		float triangleHeight = CalculateTriangleHeight (upperArmLength, forearmLength, targetDistance);
		float adjacentLength = Mathf.Sqrt (Mathf.Pow (upperArmLength, 2) - Mathf.Pow (triangleHeight, 2));
		Vector3 adjacent = (targetIK.position - rootBone.position).normalized * adjacentLength + rootBone.position;
		Vector3 elbowsAdjacent = CalculateElbowAdjacent ();
		Vector3 elbowHeight = Vector3.Cross((targetIK.position - rootBone.position).normalized, (elbowIK.position - elbowsAdjacent).normalized) * -1;
		Vector3 jointPosition = adjacent + Vector3.Cross ((targetIK.position - rootBone.position).normalized, elbowHeight)*triangleHeight;
		float angleElbow = Vector3.Angle(rootBone.position - jointPosition, targetIK.position - jointPosition);
		float angleUpperArm = Vector3.Angle(jointPosition - rootBone.position, targetIK.position - rootBone.position);
		float targetAngleV; 
			if(rootBone.position.y >= targetIK.position.y){
			targetAngleV = Vector3.Angle(Vector3.forward, targetIK.position - rootBone.position);
			}else{
			targetAngleV = -Vector3.Angle(Vector3.forward, targetIK.position - rootBone.position);
		}
		float targetAngleH;

		#endregion
		
		#region assign parameters
		if (targetDistance < armLength && targetDistance > minimumLength) {
            rootBone.localEulerAngles = new Vector3(- angleUpperArm, 0, 0);
            elbowBone.localEulerAngles = new Vector3(180f - angleElbow, 0, 0);
			this.transform.LookAt(targetIK.position, elbowIK.position - elbowsAdjacent);
		}else if(targetDistance < minimumLength){
            rootBone.localEulerAngles = Vector3.zero;
			this.transform.LookAt(targetIK.position, elbowIK.position - elbowsAdjacent);
            elbowBone.localEulerAngles = new Vector3(180f, 0, 0);
		}else{
            rootBone.localEulerAngles = Vector3.zero;
			this.transform.LookAt(targetIK.position, elbowIK.position - elbowsAdjacent);
            elbowBone.localEulerAngles = new Vector3(0, 0, 0);
		}
		#endregion

		#region hand rotation
		if(handRotation){
            endBone.rotation = targetIK.rotation;
		}
		#endregion

		#region debug draw
		if (debugLines) {
			Debug.DrawLine (rootBone.position, targetIK.position, Color.yellow);
			Debug.DrawLine (elbowBone.position, elbowIK.position, Color.blue);
		}
		#endregion
	}

	#region calculate elbow height
	Vector3 CalculateElbowAdjacent(){
		float upperArmToElbowLength = Vector3.Distance (rootBone.position, elbowIK.position);
		float elbowToTargetLength = Vector3.Distance (elbowIK.position, endBone.position);
		float upperArmToTargetLength = Vector3.Distance (rootBone.position, endBone.position);
		float triangleHeight = CalculateTriangleHeight (upperArmToElbowLength, elbowToTargetLength, upperArmToTargetLength);
		float adjacentLength = Mathf.Sqrt (Mathf.Pow (upperArmToElbowLength, 2) - Mathf.Pow (triangleHeight, 2));
		int isObtuse = 1;
			if(Mathf.Pow(upperArmToElbowLength,2f) + Mathf.Pow(upperArmToTargetLength,2f) < Mathf.Pow(elbowToTargetLength,2f)){
			isObtuse = -1;
			}
		Vector3 adjacent = (targetIK.position - rootBone.position).normalized * adjacentLength * isObtuse + rootBone.position;

		if (debugLines) {
			Debug.DrawLine (rootBone.position, elbowIK.position, Color.grey);
			Debug.DrawLine (targetIK.position, elbowIK.position, Color.grey);
			Debug.DrawLine (adjacent, elbowIK.position, Color.grey);
		}


		return adjacent;
	#endregion
	}

	#region calculate triangle height
	float CalculateTriangleHeight(float side1, float side2, float side3){
		float halfPerimeter = (side1 + side2 + side3) / 2;
		float triangleArea = Mathf.Sqrt (halfPerimeter * (halfPerimeter - side1) * (halfPerimeter - side2) * (halfPerimeter - side3));
		float triangleHeightToSide3 = (2f * triangleArea) / side3;
		return triangleHeightToSide3;
	}
	#endregion
}
















