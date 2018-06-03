using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using SocketIO;

public class ConnectionManager : MonoBehaviour {

	public LoginPanelController loginPanelController;
	public JoyStickController joystickController;
	public SocketIOComponent socket;
	public Player playGameObj;

	void Start () {
		StartCoroutine (ConnectToServer ());
		socket.On ("USER_CONNECTED", OnUserConnected);
		socket.On ("PLAY", OnUserPlay);
		socket.On ("MOVE", OnUserMove);
		socket.On ("USER_DISCONNECTED", OnUserDisconnected);
		joystickController.gameObject.SetActive (false);
		loginPanelController.playBtn.onClick.AddListener (OnClickPlayBtn);
		joystickController.OnCommandMove += OnCommandMove;
	}

	void OnCommandMove (Vector3 vec3){
		Dictionary<string, string> data = new Dictionary<string, string> ();
		Vector3 position = new Vector3 (vec3.x, vec3.y, vec3.z);
		data["position"] = position.x + "," + position.y + "," + position.z;
		socket.Emit ("MOVE", new JSONObject (data));
	}

	void OnUserMove (SocketIOEvent obj){
		GameObject player = GameObject.Find (JsonToString (obj.data.GetField ("name").ToString (), "\"")) as GameObject;
		player.transform.position = JsonToVector3 (JsonToString (obj.data.GetField ("position").ToString (), "\""));
	}

	string JsonToString(string target, string s){
		string[] newString = Regex.Split (target, s);
		return newString [1];
	}

	Vector3 JsonToVector3(string target){
		Vector3 newVector;
		string[] newString = Regex.Split (target, ",");
		newVector = new Vector3 (float.Parse (newString[0]), float.Parse(newString[1]), float.Parse(newString[2]));

		return newVector;
	}

	void OnUserDisconnected(SocketIOEvent obj){
		Destroy (GameObject.Find (JsonToString (obj.data.GetField ("name").ToString (), "\"")));
	}

	void OnClickPlayBtn(){
		if (loginPanelController.inputField.text != "") {
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["name"] = loginPanelController.inputField.text;
			Vector3 position = new Vector3 (0, 0, 0);
			data ["position"] = position.x + "," + position.y + "," + position.z;
			socket.Emit ("PLAY", new JSONObject (data));
		} else {
			loginPanelController.inputField.text = "Please enter your name again !";
		}
	}

	IEnumerator ConnectToServer(){

		yield return new WaitForSeconds (.5f);
		socket.Emit ("USER_CONNECT");

	}

	private void OnUserConnected(SocketIOEvent evt){
		Debug.Log ("Message from server is : " + evt.data + "OnUserConnected");
		GameObject otherPlayer = GameObject.Instantiate (playGameObj.gameObject, playGameObj.position, Quaternion.identity) as GameObject;
		Player otherPlayerCon = otherPlayer.GetComponent<Player> ();
		otherPlayerCon.playerName = JsonToString (evt.data.GetField ("name").ToString (), "\"");
		otherPlayer.transform.position = JsonToVector3 (JsonToString (evt.data.GetField ("position").ToString (), "\""));
		otherPlayerCon.id = JsonToString (evt.data.GetField ("id").ToString (), "\"");
	}

	private void OnUserPlay(SocketIOEvent evt){
		Debug.Log ("Message from server is : " + evt.data + "OnUserPlay");
		loginPanelController.gameObject.SetActive (false);
		joystickController.gameObject.SetActive (true);
		joystickController.ActionJoyStick ();

		GameObject player = GameObject.Instantiate (playGameObj.gameObject, playGameObj.position, Quaternion.identity) as GameObject;
		Player playerCon = player.GetComponent<Player> ();

		playerCon.playerName = JsonToString (evt.data.GetField ("name").ToString (), "\"");
		playerCon.transform.position = JsonToVector3 (JsonToString (evt.data.GetField ("position").ToString (), "\""));
		playerCon.id = JsonToString (evt.data.GetField ("id").ToString (), "\"");
		joystickController.playerObj = player;
	}

}
