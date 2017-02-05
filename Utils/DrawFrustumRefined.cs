using UnityEngine;
using System.Collections;
//-------------------------------------------------------
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
//-------------------------------------------------------
public class DrawFrustumRefined : MonoBehaviour 
{
	//-------------------------------------------------------
	private Camera Cam = null;
	public bool ShowCamGizmo = true;
	//-------------------------------------------------------
	void Awake()
	{
		Cam = GetComponent<Camera>();
	}
	//-------------------------------------------------------
	void OnDrawGizmos()
	{
		//Should we show gizmo?
		if(!ShowCamGizmo) return;

		Vector2 v = DrawFrustumRefined.GetGameViewSize(); //Get size (dimensions) of Game Tab
		float GameAspect = v.x/v.y; //Calculate tab aspect ratio
		float FinalAspect = GameAspect / Cam.aspect; //Divide by cam aspect ratio

		Matrix4x4 LocalToWorld = transform.localToWorldMatrix;
		Matrix4x4 ScaleMatrix = Matrix4x4.Scale(new Vector3(Cam.aspect * (Cam.rect.width / Cam.rect.height), FinalAspect,1)); //Build scaling matrix for drawing camera gizmo
		Gizmos.matrix = LocalToWorld * ScaleMatrix;
		Gizmos.DrawFrustum(transform.position, Cam.fieldOfView, Cam.nearClipPlane, Cam.farClipPlane, FinalAspect); //Draw camera frustum
		Gizmos.matrix = Matrix4x4.identity; //Reset gizmo matrix
	}
	//-------------------------------------------------------
	//Function to get dimensions of game tab
	public static Vector2 GetGameViewSize()
	{
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		return (Vector2) GetSizeOfMainGameView.Invoke(null,null);
	}
	//-------------------------------------------------------
}
//-------------------------------------------------------