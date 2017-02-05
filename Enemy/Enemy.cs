using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	public enum ENEMY_STATE { PATROL = 0, CHASE = 1, ATTACK = 2, Alert = 3, DEAD = 4, DEBUG=5 , IDLE_ATTACK=6};
	public ENEMY_STATE ActiveState = ENEMY_STATE.PATROL;

	protected Transform ThisTransform = null;
	public Transform PlayerTransform = null;
	public List<Transform> Players_transform_list = null;
	protected NavMeshAgent Agent = null;
	protected Animator anim;

	public float TimeOut = 8;
	public float distance2hitTarget = 0;
	public float distance2Player = 0;
	public float PatrolDistance = 40;
	public float AttackDistance = 100;
	public float CloseRange = 40;
	public float InsightDistance = 100;
	public float fieldOfView = 90;
	public float RecoveryDelay = 1.5f;

	public Transform[] EnemiesTransform;
	public int enemiesNum = 0;

	public LayerMask inSightMask ;

	public bool playerInSight;

	public Vector3 D_hit;
	public Vector3 D_thisPos;
	public Vector3 randomPosition ;
	public int navMask = 1;

	public WeaponEnemy testWeapon;


	// Use this for initialization
	void Start () {
		ThisTransform = this.transform;
		D_thisPos = ThisTransform.position;
		PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

		Players_transform_list = new List<Transform> ();
		var ptl = GameObject.FindGameObjectsWithTag ("Player");
		foreach(GameObject pt in ptl){
			Players_transform_list.Add (pt.transform);
		}

		Agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator> ();

		fieldOfView = 90;
		InsightDistance = 1800;
		AttackDistance = 100;
		CloseRange = 40;
		//  GET LIST OF ENEMIES AND SAVE TRANSFORM COMPONENT FOR QUICK ACCES
		var enemies = GameObject.FindGameObjectsWithTag ("enemy");
		EnemiesTransform = new Transform[enemies.Length-1];
		foreach (GameObject e in enemies) {
			if (!e.name.Equals (this.gameObject.name)) {
				EnemiesTransform [enemiesNum] = e.transform;
				enemiesNum++;
			}
		}
		//-------------------------------------------------
		navMask =  NavMesh.GetNavMeshLayerFromName("EnvironmentStealth");

		//-------------------------------------------------
		ChangeState(ActiveState);
	}


	public void ChangeState(ENEMY_STATE State)
	{
		Agent.Stop ();
		StopAllCoroutines();
		Debug.Log ("Changing state from " + ActiveState + "  to  " + State + "   Time: " + Time.time);
		ActiveState = State;

		switch (ActiveState)
		{
		case ENEMY_STATE.ATTACK:
			StartCoroutine(AI_Attack());
			return;

		case ENEMY_STATE.CHASE:
			StartCoroutine(AI_Chase());
			return;

		case ENEMY_STATE.PATROL:
			StartCoroutine (AI_Patrol ());
			return;

		case ENEMY_STATE.Alert:
			StartCoroutine(AI_Alert());
			return;
//		case ENEMY_STATE.DEAD:
//			StartCoroutine(AI_Dead());
//			return;
		case ENEMY_STATE.IDLE_ATTACK:
			StartCoroutine(AI_IdleAttack());
			return;
		case ENEMY_STATE.DEBUG:
			StartCoroutine(AI_DEBUG());
			return;
		case ENEMY_STATE.DEAD:
			StartCoroutine(AI_DEAD());
			return;
		}
	}

	IEnumerator AI_DEBUG()
	{
		//Agent.Resume ();
	//	Agent.SetDestination(PlayerTransform.position);

		while (true) {
			D_thisPos = ThisTransform.position;
			distance2Player =  (ThisTransform.position - PlayerTransform.position).sqrMagnitude;
		//	Debug.Log ("DEBUGGING AI");
			if ((playerInSight=HaveLineSightToPlayer (PlayerTransform))) {
				Debug.Log (ENEMY_STATE.Alert);
			}

			yield return new WaitForEndOfFrame ();
		}
	}


	IEnumerator AI_DEAD()
	{
		//Agent.Resume ();
		//	Agent.SetDestination(PlayerTransform.position);
		Agent.Stop();
		anim.Play("PistolDeathC");
		yield return null;
	}

	IEnumerator AI_Patrol(){

			// Agent.Stop();
		Agent.speed = 1.0f;

//		if (Agent.hasPath) {
//			//Sanim.SetInteger("enemyAnimState", 0);
//		}
		Agent.Resume ();
		while (ActiveState == ENEMY_STATE.PATROL){
				anim.Play("RelaxedWalk");
	//			//Get random destination on map
			NavMeshHit hit;
			randomPosition = Random.insideUnitSphere * PatrolDistance;
			NavMesh.SamplePosition(randomPosition, out hit,  Mathf.Infinity, 1 << NavMesh.GetNavMeshLayerFromName("Walkable"));
			randomPosition += ThisTransform.position;
	
				//Set destination
			Agent.SetDestination(hit.position);
			D_hit = hit.position;
	//			//Set distance range between object and destination to classify as 'arrived'
			float ArrivalDistance = 2.0f;
			
			TimeOut = 10;
	
				//Wait until enemy reaches destination or times-out, and then get new position
			while ( (ThisTransform.position - hit.position).sqrMagnitude > ArrivalDistance && TimeOut > 0){
				
					distance2hitTarget = (ThisTransform.position - hit.position).sqrMagnitude; //##################
					D_thisPos = ThisTransform.position;
					TimeOut -= 0.6f;
					distance2Player = (ThisTransform.position - PlayerTransform.position).sqrMagnitude;
					foreach (Transform pt in Players_transform_list) {
						if ((ThisTransform.position - pt.position).sqrMagnitude < InsightDistance) {
							if ((playerInSight = HaveLineSightToPlayer (pt))) {
								PlayerTransform = pt;
								Alarm (); // ALARMING THE OTHER DRONES
								ChangeState (ENEMY_STATE.CHASE);
								yield break;	
							}
						}
					}

	
				yield return new WaitForSeconds(0.6f);

			}
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForEndOfFrame();
	}



	IEnumerator AI_Chase()
	{	
		Agent.Resume ();
		Agent.speed = 3.5f;
		anim.Play("PistolCombatRunF");

		while (ActiveState == ENEMY_STATE.CHASE) {
			D_thisPos = ThisTransform.position;
			if ((playerInSight = HaveLineSightToPlayer (PlayerTransform))) {
				distance2Player = (ThisTransform.position - PlayerTransform.position).sqrMagnitude;
				D_hit = PlayerTransform.position;
				Agent.SetDestination (D_hit);
				if (distance2Player < AttackDistance) {
					ChangeState (ENEMY_STATE.ATTACK);
					yield break;
				}

			} else{
				ChangeState (ENEMY_STATE.Alert);
				yield break;
				
			}
			
			yield return new WaitForEndOfFrame ();
		}
	}




	IEnumerator AI_Attack()
	{
		Debug.Log ("Entering Ai attack " + Time.time.ToString ());
		Agent.Resume ();
		Agent.speed = 1;
		float ElapsedTime = RecoveryDelay;
		while (ActiveState == ENEMY_STATE.ATTACK) {

			ElapsedTime -= Time.deltaTime;
			D_thisPos = ThisTransform.position;
			D_hit = PlayerTransform.position;
			distance2Player =  (ThisTransform.position - PlayerTransform.position).sqrMagnitude;

			if ((playerInSight=HaveLineSightToPlayer (PlayerTransform)) && distance2Player < AttackDistance) {

				if (distance2Player < 10) {
					Agent.SetDestination (transform.position);
				} else {
					anim.Play("MIL2_M3_W1_Walk_Aim_F_Loop");
					Agent.SetDestination (PlayerTransform.position);
				}
				Vector3 lookPos = PlayerTransform.position - ThisTransform.position;
				Quaternion rot = Quaternion.LookRotation (lookPos);
				transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime*10);
//				Debug.Log ("PlayerInsight " + Time.time.ToString ());
				//Make strike
				if (ElapsedTime <= 0) {
					//Reset elapsed time
					ElapsedTime = RecoveryDelay;
					//anim.Play("PistolAttack");
//					Debug.Log ("ATTACK do damage now");
//					anim.CrossFade("PistolAttack",1);
//					shooting = true;

					Debug.Log ("Exiting Ai attack " + Time.time.ToString ());
					ChangeState(ENEMY_STATE.IDLE_ATTACK);
					yield break;
				} 
			}else{
				ChangeState(ENEMY_STATE.CHASE);
				yield break;
			}

			yield return new WaitForEndOfFrame ();
		}
	}


	IEnumerator AI_IdleAttack(){
		Agent.Resume ();
		Debug.Log ("Entering Ai idleAttack " + Time.time.ToString ());
		Agent.speed = 0.1f;
		while (ActiveState == ENEMY_STATE.IDLE_ATTACK) {
			float ElapsedTime = RecoveryDelay;
			bool shot = false;

			D_thisPos = ThisTransform.position;
	
			distance2Player =  (ThisTransform.position - PlayerTransform.position).sqrMagnitude;

			while (ElapsedTime > 0) {
				ElapsedTime -= Time.deltaTime;


				Vector3 lookPos = PlayerTransform.position - ThisTransform.position;
				Quaternion rot = Quaternion.LookRotation (lookPos);
				transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime*10);

				if(!shot && ElapsedTime < (RecoveryDelay/2)){
					testWeapon.shoot ();
					shot = true;
					Debug.Log ("AI shooting");
				}

				anim.Play ("PistolAttack");

				if (!HaveLineSightToPlayer (PlayerTransform)) {
					Debug.Log ("Exiting Ai idleAttack attack " + Time.time.ToString ());
					ChangeState (ENEMY_STATE.CHASE);
					yield break;
				} 

				if (distance2Player > CloseRange && ElapsedTime <= 0) {
					Debug.Log ("Exiting Ai idleAttack attack " + Time.time.ToString ());
					D_hit = PlayerTransform.position;
					Agent.SetDestination (D_hit);
					ChangeState (ENEMY_STATE.ATTACK);
					yield break;
				} else if (distance2Player < CloseRange) {
					Agent.SetDestination (transform.position);
				}

				yield return new WaitForEndOfFrame ();
			}
			anim.Play("PistolCombatIdle");
		}
//		Agent.Stop ();
		yield return null;
	}



	IEnumerator AI_Alert()
	{
		anim.Play("PistolCombatIdle");
		//    anim.SetInteger("enemyAnimState", 1);
		int turnDir = Random.Range(1,5);
		turnDir = turnDir > 2 ? -turnDir : turnDir;
		//Loop forever while in chase state
		while (ActiveState == ENEMY_STATE.Alert)
		{
			float alertTime = 6.3f;
			//If within attack range, then change to attack state
			while (alertTime  > 0){
				alertTime -= Time.deltaTime;
				transform.Rotate (0, Time.deltaTime*45*turnDir, 0, Space.World);
				foreach (Transform pt in Players_transform_list) {
					if ((ThisTransform.position - pt.position).sqrMagnitude < InsightDistance) {
						if ((playerInSight = HaveLineSightToPlayer (pt))) {
							PlayerTransform = pt;
							Alarm (); // ALARMING THE OTHER DRONES
							ChangeState (ENEMY_STATE.CHASE);
							yield break;	
						}
					}
				}
				yield return new WaitForEndOfFrame();
			}
			ChangeState(ENEMY_STATE.PATROL);
			yield break;

		}

		//Wait until next frame
		yield return null;

	}




	public void Alarm(){
		for (int i = 0; i < EnemiesTransform.Length; i++) {
			Enemy enemyScript = EnemiesTransform [i].GetComponent<Enemy> ();
			if (enemyScript.ActiveState == ENEMY_STATE.PATROL) {
				enemyScript.Agent.SetDestination (PlayerTransform.position);
				enemyScript.ChangeState (ENEMY_STATE.CHASE);
			}
		}
	}



	private bool HaveLineSightToPlayer(Transform Player)
	{
		//Get angle between enemy sight and player
		float Angle = Mathf.Abs(Vector3.Angle(ThisTransform.forward,
			(Player.position - ThisTransform.position).normalized));
		//		Debug.Log ("Angle " + Angle);
		//If angle is greater than field of view, we cannot see player
		if (Angle > fieldOfView/2) return false;
		//Check with raycast- make sure player is not on other side of wall
		RaycastHit hit;
		if (Physics.Linecast(ThisTransform.position, Player.position, out hit, inSightMask))
		{ // COULD ALSO ADD A LAYER MASK FOR MORE EFFICENCY
			if (hit.transform.tag.Equals ("Player")) {
				playerInSight = true;
				return playerInSight ;
			}
			Debug.Log("Something is interfering with the line cast\n Tag: " + hit.transform.tag + "   Name: " +
				hit.transform.gameObject.name + "   Layer: " + inSightMask.value);
			playerInSight = false;
			return playerInSight ;
		}
		//We can see player
		//TODO rotate transform toward player

		return true;
	}


}
