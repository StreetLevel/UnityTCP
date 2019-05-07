using UnityEngine;
using System.Collections;

public class ModelCamera : MonoBehaviour {

	/* Robert Gates, Maximilian Bittens, MIT License, 2019 */


	public float rotSens = 0.25f; //How sensitive it with mouse
	public float panSens = 0.00125f;

	public Vector3 lookAt = Vector3.zero;
	public Vector2 sphereCoordinates = Vector2.zero; // First is rotation around Vector3.right, Second is rotation around Vector3.up
	public float distance = 100.0f;
	public float viewAxisRotation = 0.0f;
	public float zoomRate = 1.0f;

	private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
	private Vector3 lastMouse2 = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
	private Vector3 lookAtDelta = Vector3.zero;
	private Vector3 distanceVector = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector2 _sphereCoordinates = Vector2.zero;
	private Vector3 _lookAt = Vector3.zero;
	private float _viewAxisRotation = 0.0f;

	void Awake() {
		Debug.Log ("Camera initialized."); // nop?
		distanceVector.z = -distance;
		ComputeAngles(distanceVector);
		transform.position = _lookAt + distanceVector;
	}

	public void SetView(Vector3 newLookAt, Vector2 newSphereCoordinates, float newViewAxisRotation, float newDistance){
		Debug.Log ("Setting camera."); // nop?
		lookAt = newLookAt;
		sphereCoordinates = newSphereCoordinates;
		viewAxisRotation = newViewAxisRotation;
		distance = newDistance;
	}

	void ProcessAngles() {
		var eps = 1.0f-0.99999f;
		sphereCoordinates.x = Mathf.Clamp(sphereCoordinates.x, eps, 180.0f-eps);
		sphereCoordinates.y = sphereCoordinates.y % 360.0f;
	}

	void ComputeAngles(Vector3 distanceVector) {
		sphereCoordinates.x = 180.0f - Mathf.Acos(distanceVector.z / distance) * Mathf.Rad2Deg;
		sphereCoordinates.y = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
		if (sphereCoordinates.y<0.0f) {
			sphereCoordinates.y = sphereCoordinates.y + 360.0f;
		}
		_sphereCoordinates.x = sphereCoordinates.x;
		_sphereCoordinates.y = sphereCoordinates.y;
	}


	void Update () {
		ProcessAngles();

		// Set mouse down positions
		if (Input.GetMouseButtonDown(1))
		{
			lastMouse = Input.mousePosition; // $CTK reset when we begin
		}

		if (Input.GetMouseButtonDown(0))
		{
			lastMouse2 = Input.mousePosition; // $CTK reset when we begin
		}

		// Set distance
		distance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * distance*(Mathf.Log(distance)+1.0f);
		distance = Mathf.Clamp(distance, 1.0f, 1000000.0f);



		// Update functions
		if (_sphereCoordinates.x != sphereCoordinates.x || _sphereCoordinates.y != sphereCoordinates.y)
		{
			distanceVector.x = distance * Mathf.Sin((180.0f-sphereCoordinates.x) * Mathf.Deg2Rad) * Mathf.Cos(sphereCoordinates.y * Mathf.Deg2Rad);
			distanceVector.y = distance * Mathf.Sin((180.0f-sphereCoordinates.x) * Mathf.Deg2Rad) * Mathf.Sin(sphereCoordinates.y * Mathf.Deg2Rad);
			distanceVector.z = distance * Mathf.Cos((180.0f-sphereCoordinates.x) * Mathf.Deg2Rad);
			ComputeAngles(distanceVector);
			transform.position = _lookAt + distanceVector;
			transform.LookAt(_lookAt, Vector3.up);
		}



		if ((_lookAt.x != lookAt.x) || (_lookAt.y != lookAt.y) || (_lookAt.z != lookAt.z))
		{
			transform.Translate(lookAt-_lookAt, Space.World);
			_lookAt = lookAt;
		}

		var currentDistanceSqr = (distanceVector.x*distanceVector.x + distanceVector.y*distanceVector.y + distanceVector.z*distanceVector.z);
		if (currentDistanceSqr != distance*distance)
		{
			var fac = distance/Mathf.Sqrt(currentDistanceSqr);
			Vector3 scaleDistVec = distanceVector * fac;
			transform.Translate(scaleDistVec-distanceVector, Space.World);
			distanceVector = scaleDistVec;
		}

		if (viewAxisRotation != _viewAxisRotation)
		{
			transform.RotateAround(lookAt, transform.forward, viewAxisRotation-_viewAxisRotation);
			_viewAxisRotation = viewAxisRotation;
		}

		if (Input.GetMouseButton(1))
		{
			lastMouse = Input.mousePosition - lastMouse ;
			lastMouse = new Vector3(-lastMouse.y * rotSens, lastMouse.x * rotSens, 0 );
			transform.RotateAround(lookAt, transform.forward, -_viewAxisRotation);
			transform.RotateAround(lookAt, Vector3.right, lastMouse.x);
			transform.RotateAround(lookAt, transform.up, lastMouse.y);
			transform.RotateAround(lookAt, transform.forward, _viewAxisRotation);
			distanceVector = transform.position-_lookAt;
			ComputeAngles(distanceVector);
			lastMouse =  Input.mousePosition;
		}

		if (Input.GetMouseButton(0))
		{
			lastMouse2 = Input.mousePosition - lastMouse2;
			lastMouse2.x = Mathf.Clamp(lastMouse2.x, -1000000.0f, 1000000.0f);
			lastMouse2.y = Mathf.Clamp(lastMouse2.y, -1000000.0f, 1000000.0f);
			var facx = Mathf.Log(Mathf.Abs(lastMouse2.x)+1.0f)+1.0f;
			var facy = Mathf.Log(Mathf.Abs(lastMouse2.y)+1.0f)+1.0f;
			lookAtDelta = -lastMouse2.x  * facx * transform.right + -lastMouse2.y  * facy * transform.up;
			lookAt = lookAt + lookAtDelta * panSens;
			lastMouse2 =  Input.mousePosition;
		}


	}

}
