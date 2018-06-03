using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickController : MonoBehaviour {

	public delegate void OnMove(Vector3 vec3);
	public event OnMove OnCommandMove;

	public ButtonManager Left;
	public ButtonManager Right;
	public ButtonManager Backward;
	public ButtonManager Forward;

	[HideInInspector]
	public GameObject playerObj;

	public bool leftMove;
	public bool rightMove;
	public bool backMove;
	public bool frontMove;

	void Start(){
		playerObj = new GameObject ();
	}

	public void ActionJoyStick(){
		Left.OnPress += OnPress;
		Right.OnPress += OnPress;
		Backward.OnPress += OnPress;
		Forward.OnPress += OnPress;
	}

	void OnPress (GameObject unit, bool state){
		switch (unit.name) {
			case "Left":
				LeftMove (state);
			break;
			case "Right":
				RightMove (state);
			break;
			case "Backward":
				BackMove (state);
			break;
			case "Forward":
				FrontMove (state);
			break;
		}

		Debug.Log (unit.name);
	}

	private void LeftMove(bool state){
		leftMove = state;
	}
	private void RightMove(bool state){
		rightMove = state;
	}
	private void BackMove(bool state){
		backMove = state;
	}
	private void FrontMove(bool state){
		frontMove = state;
	}

	void Update () {
		Transform transf = playerObj.transform;

		if (leftMove) {
			playerObj.transform.position = new Vector3 (transf.position.x - (2f * Time.deltaTime), transf.position.y, transf.position.z);
			if (OnCommandMove != null) {
				OnCommandMove (playerObj.transform.position);
			}
		}

		if (rightMove) {
			playerObj.transform.position = new Vector3 (transf.position.x + (2f * Time.deltaTime), transf.position.y, transf.position.z);
			if (OnCommandMove != null) {
				OnCommandMove (playerObj.transform.position);
			}
		}

		if (backMove) {
			playerObj.transform.position = new Vector3 (transf.position.x, transf.position.y, transf.position.z - (2f * Time.deltaTime));
			if (OnCommandMove != null) {
				OnCommandMove (playerObj.transform.position);
			}
		}

		if (frontMove) {
			playerObj.transform.position = new Vector3 (transf.position.x, transf.position.y, transf.position.z + (2f * Time.deltaTime));
			if (OnCommandMove != null) {
				OnCommandMove (playerObj.transform.position);
			}
		}
	}
}
