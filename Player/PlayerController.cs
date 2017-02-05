using UnityEngine;
using System.Collections;



public enum ACTION_STATE{
	IDLE = 0,
	WALK = 1,
	RUN = 2,
	AIM = 3,
	SHOOT = 4,
	DIE = 5,
	AIM_RUN = 6,
	CROUCHWALK = 7,
	CROUCH = 8,
	HIT = 9,
	WIN = 10,
};

public class PlayerController : Photon.MonoBehaviour {
	
	CharacterController cc;
	public bool isControllable = false;// ======================================================================
	// The action_state.
	// </summary>
	public ACTION_STATE action_state = ACTION_STATE.IDLE;
	private ACTION_STATE Last_action_state = ACTION_STATE.IDLE;
	Animator anim;

	public bool shoot = false; 
	public bool isSniping = false;
	public bool isCrouching = false;
	public float speed = 1.0f;
	public float default_speed = 1.0f;
	public float verticalVelocity = 0.0f;
	public float jumpSpeed = 6.0f;
	public float shootingRecoveryTime = 0.0f;
	public Vector3 direction = Vector3.zero;


	public float[] accelZ_smooth;
	public int smoothArray = 40;
	bool moving = false;
	bool jump = false;
	public float snipeAnimNom = 0;

	public Camera vr_rightCam =  null;
	public Camera vr_leftCam = null;
	public Camera aimCam = null;
	public Transform loop = null;

	public Weapon testweapon;
	public Transform rightEye = null;
	private AudioSource SFX = null;
	public AudioClip changeAudio, fireAudio;

	// Use this for initialization
	int debugPressedBt = 0;

	void Start () {
		cc = GetComponent<CharacterController> ();
	//	Physics.IgnoreCollision(target.transform.parent.GetComponent<Collider>(),transform.GetComponent<Collider>());
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource> ();
		speed = MainEventManager.StateManager.GameState.Avatar.speed;
		default_speed = MainEventManager.StateManager.GameState.Avatar.speed;
		accelZ_smooth = new float[smoothArray];

		if (!rightEye) {
			Debug.Log ("Set aiming pivot");
		} 

		anim.speed = speed * 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		if (action_state == ACTION_STATE.DIE || action_state == ACTION_STATE.WIN) {
			anim.Play ("PistolDeathC");
			isControllable = false;
//			if (photonView.isMine) {
//				if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_option2)) {
//					Debug.Log ("TODO: leaving the room, dont know why");
//					PhotonNetwork.LeaveRoom ();
//				}
//			}

			return;
		}

		if (isControllable) {
			//#########################################
			#if UNITY_EDITOR
			int _x = 0, _z = 0;
			if (Input.GetButton (MainEventManager.StateManager.GameState.myJoyStick.m_forward)) {
				_z = 1;
			}
			if (Input.GetButton (MainEventManager.StateManager.GameState.myJoyStick.m_backward)) {
				_z = -1;
			}
			if (Input.GetButton (MainEventManager.StateManager.GameState.myJoyStick.m_right)) {
				_x = 1;
			}
			if (Input.GetButton (MainEventManager.StateManager.GameState.myJoyStick.m_left)) {
				_x = -1;
			}
			//direction = transform.rotation * (new Vector3 (Input.GetAxis ("Horizontal"), 0, -Input.GetAxis ("Vertical")));
			direction = transform.rotation * (new Vector3 (_x, 0, _z));
			transform.Rotate (0, Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_left) * 2.5f, 0);
			transform.Rotate (0, Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_right) * -2.5f, 0);
			#elif UNITY_ANDROID

			direction = transform.rotation *(new Vector3 (Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.m_right), 0, -Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.m_forward)));
			//direction = transform.rotation *(new Vector3 (0,0,0));
			snipeAnimNom = accelUpdate ();

			#else 
			direction = transform.rotation *(  new Vector3 (Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.m_forward), 0, -Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.m_right)));
			transform.Rotate(0,Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_right),0);
			#endif

			shootingRecoveryTime -= Time.deltaTime;

			if (direction.magnitude > 1f) {
				direction = direction.normalized;
			}


//			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_option1) ||
//			    Input.acceleration.z <= -0.6f) {
//				isCrouching = true;
//			}
//			if (Input.GetButtonUp (MainEventManager.StateManager.GameState.myJoyStick.bt_option1) || 
//			   Input.acceleration.z >= 0.1f ) {
//				isCrouching = false;
//			}
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter)) {
				debugPressedBt++;
			}
			
			
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_sniping)) {
				isSniping = true;
				if (MainEventManager.StateManager.GameState.ActualWeapon.weapon_type != 0) {
					Debug.Log ("AAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
					setSnipingCam (true);
				} else {
					setSnipingCam (false);
				}
			}
			if (Input.GetButtonUp (MainEventManager.StateManager.GameState.myJoyStick.bt_sniping)) {
				isSniping = false;
				setSnipingCam(false);
			}

//			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_aim)) {
//				isAiming = true;
//			}
//		
//			if (Input.GetButtonUp (MainEventManager.StateManager.GameState.myJoyStick.bt_aim)) {
//				isAiming = false;
//			}

			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_shoot) &&
			     shootingRecoveryTime <= 0) {
				shoot = true;
				shootingRecoveryTime = MainEventManager.StateManager.GameState.ActualWeapon.reloadSpeed;
			}


			
			if (shoot) {
				shoot = false;
				Debug.Log ("player shooting");
				testweapon.shoot ();
			}

			if (direction.magnitude <= 0.05f) {
				if (isSniping) {
					setSnipingCam (true);
	//					if (action_state != ACTION_STATE.AIM) {
	//						anim.CrossFade ("StandingAim",0.5f);
	//					}  
					action_state = ACTION_STATE.AIM;

				} else if (isCrouching) {
	//					if (action_state != ACTION_STATE.CROUCH) {
	//						anim.CrossFade ("CrouchIdle",0.5f);
	//					}
					action_state = ACTION_STATE.CROUCH;
				} else if (action_state != ACTION_STATE.IDLE) {
	//					if(Last_action_state != action_state){
	//						anim.CrossFade ("Idle",1f);
	//					}
					action_state = ACTION_STATE.IDLE;
				}
			} else if (direction.magnitude > 0.1f) {
				if (isSniping) {
					setSnipingCam (false);
					action_state = ACTION_STATE.AIM_RUN;
				} else if (isCrouching) {
					action_state = ACTION_STATE.CROUCHWALK;
					isCrouching = false;
				} else {
					action_state = ACTION_STATE.WALK;
				}
				

				//anim.speed = direction.magnitude;
			}			

	

		} 
		// Player is no longer in controller of player controller



		switch(action_state){
		case ACTION_STATE.IDLE:
				anim.Play("Idle");
				break;
		case ACTION_STATE.AIM:
				if (MainEventManager.StateManager.GameState.ActualWeapon.weapon_type == 0) {
					anim.Play ("PistolAttack",0,0);
				} else {
					if (snipeAnimNom > 0) {
						anim.Play ("StandingAimUp", 0, snipeAnimNom);
					} else {
						anim.Play ("StandingAimDown", 0, -snipeAnimNom);
					}
				}
				break;
		case ACTION_STATE.CROUCH:
				anim.Play("CrouchIdle");
				break;
		case ACTION_STATE.CROUCHWALK:
				anim.Play("CrouchWalk");
				break;
		case ACTION_STATE.WALK:
			speed = default_speed;
				anim.Play("RelaxedWalk");
				break;
		case ACTION_STATE.AIM_RUN:
			speed = default_speed * 1.5f;
			if (MainEventManager.StateManager.GameState.ActualWeapon.weapon_type == 0) {
				anim.Play ("MIL2_M3_W1_Jog_Aim_F_Loop");
			} else {
				anim.Play ("RunAim");
			}
			break;
		case ACTION_STATE.WIN:
		
			break;
			
		}




		Last_action_state = action_state;
		
	}
	
			void setSnipingCam(bool ss){
		Debug.Log ("Setting sniping " + ss.ToString ());
			vr_rightCam.enabled = !ss;
			vr_leftCam.enabled = !ss;
			aimCam.enabled = ss;
	}
	
	// Once per physic update
	void FixedUpdate(){
 		Vector3 dist = direction * speed * Time.deltaTime;
		if (cc.isGrounded && verticalVelocity < 0) {
			//Set animation here jump false
			verticalVelocity = Physics.gravity.y * Time.deltaTime;
			jump = false;
			
		} else {
			if(Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f){  
				// set jump animaton true
			}
			verticalVelocity += Physics.gravity.y * Time.deltaTime;
		}
		dist.y = verticalVelocity*Time.deltaTime;
		cc.Move (dist);
	}

	//CUSTOM LEARP ACCEL_Z
	float accelUpdate(){
		float tmp = 0;
		for (int i=0; i<smoothArray-1; i++) {
			accelZ_smooth[i] = accelZ_smooth[i+1];
			tmp += accelZ_smooth[i];
		}
		accelZ_smooth [smoothArray - 1] = Input.acceleration.z;
		
		return tmp / smoothArray;
	}


}
