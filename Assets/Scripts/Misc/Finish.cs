using UnityEngine;
using System.Collections;

public class Finish : MonoBehaviour {

    private Grid grid;
    private Transform player;

	// Use this for initialization
	void Start () {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        player = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if ((player.transform.position - transform.position).magnitude < 1)
            grid.notResolved = false;
	}

}
