using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuiController : MonoBehaviour {


	public Texture2D MainFocusTexture;
	public Texture2D DefaultTexture;
	public Text	MainFocusText;
	public Text Up_1_Text;
	public Text Up_2_Text;
	public Text Down_1_Text;
	public Text Down_2_Text;

	public GameObject MainFocusTextureRef;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.X)) {
			MainFocusTextureRef.GetComponent<RawImage>().texture = MainFocusTexture;	
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			MainFocusTextureRef.GetComponent<RawImage>().texture = DefaultTexture;	
		}
	}
}
