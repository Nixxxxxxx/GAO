//Loads and Saves game state data to and from xml file
//-----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO;
//-----------------------------------------------
public class LoadSaveManager : MonoBehaviour
{
	//Save game data
	[XmlRoot("GameData")]
	public class GameStateData
	{
		public class mState{
			public bool networkStatus = true;
			public bool firstRun = true;
		}

		//-----------------------------------------------
		public struct DataTransform
		{
			public float X;
			public float Y;
			public float Z;
			public float RotX;
			public float RotY;
			public float RotZ;
			public float ScaleX;
			public float ScaleY;
			public float ScaleZ;
		}
		//-----------------------------------------------
		//Data for enemy
		public class DataEnemy
		{
			//Enemy Transform Data
			public DataTransform PosRotScale;
			//Enemy ID
			public int EnemyID;
			//Health
			public int Health;
		}
		public class DataAvatar
		{
			//Enemy Transform Data
			public string name = "xxxxxx";
			public int rank = 1;
			public int level = 100;
			public int health = 100;
			public float speed = 2f;
			public float defense = 5;
			public int points = 2;
			public int cash = 10;
			public int capacity = 5;
			public int nextLevel = 100;

			public DataAvatar(string _name, int _health, float _speed, float _defense){
				name = _name;
				health = _health;
				speed = _speed;
				defense = _defense;
			}
			public DataAvatar(){
				name = "Default";
			}
		}


		public class DataWeapon
		{
			//Enemy Transform Data
			public string name = "xxxxxx";
			public int damage = 10;
			public int weapon_type = 0;
			public float reloadSpeed = 1;
			public float weight = 1f; 
			public int ammo = 100;
			public int price = 100000;
			public bool owned = false;
			public DataWeapon(string _name, int _damage,float rs,float kg, int wt, int pr,int _ammo, bool own){
				name = _name;
				damage = _damage;
				weapon_type = wt;
				reloadSpeed = rs;
				weight = kg;
				price = pr;
				owned = own;
				ammo = _ammo;
			}

			public DataWeapon(){
				name = "Default";
			}
	
		}
		//-----------------------------------------------
		//Data for player
		public class DataPlayer
		{
			//Transform Data
			public string PlayerName;
			public string Password;
			public bool successfulRegister;
			public DataTransform PosRotScale;

			//Collected cash
			public float CollectedCash;

			//Has Collected Gun 01?
			public bool CollectedGun;
			public int kill;
			//Health
			public int Health;
		}
		//-----------------------------------------------
		public class DataLevels
		{
			public bool TrainingLevel1 = false;
			public bool TrainingLevel2 = false;
			public bool DummyChallenge = false;
			public bool Rescue_worker = false;
			public bool Stealth = false;
			public bool Hangar = false;
			public bool Dawn = false;
			public bool Snipping = false;

		}
		
		public class JoyStick
		{
	
			
			public string bt_sniping = "NOT SET";
			public string bt_shoot = "NOT SET";
			public string bt_enter = "NOT SET";
			public string bt_return = "NOT SET";
			public string bt_aim = "NOT SET";
			public string bt_option4 = "NOT SET";
			public string bt_cycle_left = "NOT SET";
			public string bt_cycle_right = "NOT SET";
			
			public string m_forward = "NOT SET";
			public string m_backward = "NOT SET";
			public string m_left = "NOT SET";
			public string m_right = "NOT SET";
			public string t_axis1_right = "NOT SET";
			public string t_axis1_left = "NOT SET";
			public string t_axis2_up = "NOT SET";
			public string t_axis2_down = "NOT SET";
			public string set_speed = "NOT SET";
			
	
		}
		
		//-----------------------------------------------
		//Instance variables
		public List<DataEnemy> Enemies = new List<DataEnemy>();
		public List<DataAvatar> dAvatar = new List<DataAvatar>();
		public List<DataWeapon> weapons = new List<DataWeapon>();

		public DataPlayer Player = new DataPlayer();
		public DataAvatar Avatar = new DataAvatar ();
		public DataWeapon ActualWeapon = new DataWeapon ();

		public JoyStick myJoyStick = new JoyStick();
		public mState stats = new mState ();
		public DataLevels Lv = new DataLevels ();
		public List<string> LvNames = new List<string> ();

	}



	//Game data to save/load
	public GameStateData GameState = new GameStateData();

	//-----------------------------------------------
	//Saves game data to XML file
	public void Save(string FileName = "GameData.xml")
	{
		//Clear existing enemy data
		GameState.Enemies.Clear();

		//Call save start notification
		//GameManager.Notifications.PostNotification("SaveGamePrepare",0);

		//Now save game data
		XmlSerializer Serializer = new XmlSerializer(typeof(GameStateData));
		FileStream Stream = new FileStream(FileName, FileMode.Create);
		Serializer.Serialize(Stream, GameState);
		Stream.Close();

		//Call save end notification
	//	GameManager.Notifications.PostNotification("SaveGameComplete",0);
	}
	//-----------------------------------------------
	//Load game data from XML file
	public void Load(string FileName =  "GameData.xml")
	{
//		Debug.Log("load Manager");
		//Call load start notification
	//	GameManager.Notifications.PostNotification("LoadGamePrepare",0);
		
		XmlSerializer Serializer = new XmlSerializer(typeof(GameStateData));
		FileStream Stream = new FileStream(FileName, FileMode.Open);
		GameState = Serializer.Deserialize(Stream) as GameStateData;
		Stream.Close();
		//Call load end notification
//		GameManager.Notifications.PostNotification("LoadGameComplete",0);
		Debug.Log ("Loading SaveGamed Complete ");
	}
	//-----------------------------------------------

	public int getActWeapon(string gunName){
		//Debug.Log (gunName);
		for (int i = 0; i < MainEventManager.StateManager.GameState.weapons.Count ; i++) {
			if (MainEventManager.StateManager.GameState.weapons [i].name.Equals (gunName)) {
				return i;
			}
		}
		return -1;
	}

}
//-----------------------------------------------