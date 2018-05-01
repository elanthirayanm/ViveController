using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllMe : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj;
	// 2
	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	private GameObject collidingObject; 
	// 2
	private GameObject objectInHand; 

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	// Use this for initialization
	void Start () {
		
	}
	private void SetCollidingObject(Collider col)
	{
		// 1
		if (collidingObject || !col.GetComponent<Rigidbody>())
		{
			return;
		}
		// 2
		collidingObject = col.gameObject;
	}

	// 1
	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}

	// 2
	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject(other);
		Controller.TriggerHapticPulse(2000);
	}

	// 3
	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject)
		{
			return;
		}

		collidingObject = null;
	}

	private void GrabObject()
	{
		// 1
		objectInHand = collidingObject;
		collidingObject = null;
		// 2
		var joint = AddFixedJoint();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
	}

	// 3
	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint>();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}


	private void ReleaseObject()
	{
		// 1
		if (GetComponent<FixedJoint>())
		{
			// 2
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy(GetComponent<FixedJoint>());
			// 3
			objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
		}
		// 4
		objectInHand = null;
	}
	// Update is called once per frame
	void Update () {
		

		// 1
		if (Controller.GetAxis() != Vector2.zero)
		{
			Debug.Log(gameObject.name + Controller.GetAxis());
		}

		// 2
		if (Controller.GetHairTriggerDown())
		{
			Debug.Log(gameObject.name + " Trigger Press");
		}

		// 3
		if (Controller.GetHairTriggerUp())
		{
			Debug.Log(gameObject.name + " Trigger Release");
		}

		// 4
		if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
			
			Debug.Log(gameObject.name + " Grip Press");
		}

		// 5
		if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{
			Debug.Log(gameObject.name + " Grip Release");
		}
		// 1
		if (Controller.GetHairTriggerDown())
		{
			if (collidingObject)
			{
				GrabObject();
			}
		}

		// 2
		if (Controller.GetHairTriggerUp())
		{
			if (objectInHand)
			{
				ReleaseObject();
			}
		}

	}
}
