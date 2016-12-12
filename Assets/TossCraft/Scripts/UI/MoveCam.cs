using UnityEngine;
using System.Collections;

///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MoveCam : MonoBehaviour {
	
	public float sensitivityX = 5F;
	public float sensitivityY = 5F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	void Update () {
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Horizontal") * sensitivityX;

		rotationY += Input.GetAxis("Vertical") * sensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

	}
		
	void Start () {
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
}