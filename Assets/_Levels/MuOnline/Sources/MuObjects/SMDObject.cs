
using UnityEngine;
using System.Collections;

public class SMDObject : MonoBehaviour {
	
	protected Transform SMD;
	protected Renderer SMDrendered;
	
	protected void InitSMD() {
		SMD = gameObject.transform.Find("SMDImport");
		if (SMD!=null)
			SMDrendered = SMD.GetComponent<Renderer>();
		else {
			Debug.LogWarning("Not found SMD");	
		}
	}
	
}
