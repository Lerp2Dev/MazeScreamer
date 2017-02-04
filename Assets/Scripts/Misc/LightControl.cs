using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {

    private bool activate = true;
    private Light myLight;

	// Use this for initialization
	void Start () {
        myLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.L))
        {
            activate = !activate;
            myLight.range = ((activate) ? 15 : 0);
        }

	}

}
