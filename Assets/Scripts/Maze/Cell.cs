using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
    public List<Transform> Adjacents = new List<Transform>();
    public List<Transform> AllAdjacents = new List<Transform>();
    public Vector3 Position;
}
