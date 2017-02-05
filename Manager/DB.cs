using System; 
using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
class DB : MonoBehaviour
{   
	private GAO_LOGS glogs;
	void Start(){
		glogs = this.gameObject.GetComponent<GAO_LOGS> ();
	}

	public WWW GET(string url)
	{

		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www));
		return www; 
	}

	public WWW POST(string url, Dictionary<string,string> post)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<String,String> post_arg in post)
		{
			form.AddField(post_arg.Key, post_arg.Value);
		}
		WWW www = new WWW(url, form);

		StartCoroutine(WaitForRequest(www));
		return www; 
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
		//	glogs.serverResult = www.text;//
			//Debug.Log("WWW Ok!: " + www.text);//####################### uncomment to debug
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}
}