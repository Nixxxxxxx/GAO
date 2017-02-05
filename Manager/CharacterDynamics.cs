/********
REQUIRES TO BE LAST IN PARENT HIERACY

*/



using UnityEngine;
using System.Collections;

public class CharacterDynamics : MonoBehaviour {
	CharacterController cc;
	AudioSource SFX = null;
	Animator anim;



	public Vector3 direction = Vector3.zero;
	public float speed = 1.0f;
	public float verticalVelocity = 0.0f;
	public float jumpSpeed = 6.0f;

	// The action_state.
	public bool isControllable = false;// ======================================================================
	bool moving = false;
	bool jump = false;

	public int hp = 100;
	// </summary>
	public ACTION_STATE action_state = ACTION_STATE.IDLE;
	private ACTION_STATE Last_action_state = ACTION_STATE.IDLE;


	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController> ();
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource> ();
		//	Physics.IgnoreCollision(target.transform.parent.GetComponent<Collider>(),transform.GetComponent<Collider>());
	}
	
	// Update is called once per frame
	void Update () {
		
		switch(action_state){
		case ACTION_STATE.IDLE:
			anim.Play("Idle");
			break;
		case ACTION_STATE.AIM:
//			aimAnimNom = accelUpdate ();
//			if (aimAnimNom > 0) {
//				anim.Play ("StandingAimUp", 0, aimAnimNom);
//			} else {
//				anim.Play ("StandingAimDown", 0, -aimAnimNom);
//			}
			break;
		case ACTION_STATE.CROUCH:
			anim.Play("CrouchIdle");
			break;
		case ACTION_STATE.CROUCHWALK:
			anim.Play("CrouchWalk");
			break;
		case ACTION_STATE.WALK:
			anim.Play("RelaxedWalk");
			break;
		case ACTION_STATE.DIE:
			anim.Play("SSDeathB");
			break;
		}
		
		
		
		
		Last_action_state = action_state;
	}

	// Once per physic update
	void FixedUpdate(){
//		Vector3 dist = direction * speed * Time.deltaTime;
//		if (cc.isGrounded && verticalVelocity < 0) {
//			//Set animation here jump false
//			verticalVelocity = Physics.gravity.y * Time.deltaTime;
//			jump = false;
//			
//		} else {
//			if(Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f){  
//				// set jump animaton true
//			}
//			verticalVelocity += Physics.gravity.y * Time.deltaTime;		}
//		dist.y = verticalVelocity*Time.deltaTime;
//		cc.Move (dist);
	}



	
	[PunRPC]
	public void Damage2Player(int dmg, string tag = "Default") {
		hp -= dmg;
		//		if (resistant > 0) {
		//			if(tag.Equals("Player")){
		//	
		//			}
		//		}
		//		
		if (hp <= 0) {
			Debug.Log("TAG-: " + tag);
			action_state = ACTION_STATE.DIE;
		}
	}
	
	
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

}
