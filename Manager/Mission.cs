using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mission : MonoBehaviour {

	public List<GameObject> Children;
	private int missionNum = 0;
	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
		{
			if (child.tag == "RoomDynamicAsset")
			{
				Children.Add(child.gameObject);
				Debug.Log(child.gameObject.name);
			}
		}
		missionNum = Children.Count;
	}

	public int MissionNum {
		get {
			return missionNum;
		}
		set {
			if(missionNum > Mathf.Abs(value)){
				Debug.Log("Init mission Target counting");
			}
			missionNum = value;
			if (missionNum <= 0) {
				Debug.Log("Got this far -- Mision Clear --");
				Application.LoadLevel(MainMenu.Main_Menu_Scene);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.X)) {
			MissionNum--;
		}
	}
}
