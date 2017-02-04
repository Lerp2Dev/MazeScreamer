using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Grid : MonoBehaviour
{

    public Vector3 GridSize;
    public float Buffer;
    public GameObject player;
    public bool notResolved;

    private Transform CellPrefab;
    private List<Transform> Set = new List<Transform>(), CompletedSet = new List<Transform>();
    private List<Vector3> finalPoints = new List<Vector3>();
    private Transform[,] GridArr;
    private Texture2D pbBackground, pbBar;
    private Dictionary<string, Transform> CellSets = new Dictionary<string, Transform>();
    private Vector3 finalPosition;
    private bool generating; //This will be an enum
    private float time;

    void Start()
    {
        //Textures
        pbBackground = new Texture2D(1, 1);
        pbBackground.SetPixel(0, 0, Color.black);
        pbBackground.Apply();

        pbBar = new Texture2D(1, 1);
        pbBar.SetPixel(0, 0, Color.red);
        pbBar.Apply();

        GenerateMaze();

    }

    void Update()
    {
        if (!generating && notResolved)
            time += Time.deltaTime;
    }

    void OnGUI()
    {
        if (generating)
        {
            GUI.DrawTexture(new Rect(Screen.width / 2 - 255, Screen.height / 2 - 15, 510, 30), pbBackground);
            GUI.DrawTexture(new Rect(Screen.width / 2 - 252.5f, Screen.height / 2 - 12.5f, 505 * (CompletedSet.Count / (GridSize.x * GridSize.z)), 25), pbBar);
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 12.5f, 300, 25), "Generating Maze "+GridSize.x+"x"+GridSize.z+": "+(CompletedSet.Count * 100 / (GridSize.x * GridSize.z)) + "%", new GUIStyle("label") { fontSize = 14, alignment = TextAnchor.MiddleCenter });
        }
        if (!notResolved)
            GUI.Window(0, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 45, 400, 90), FinishWindow, "Game finished!");
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(finalPosition+Vector3.up*Buffer, 1);
    }

}
