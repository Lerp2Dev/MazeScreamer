using UnityEngine;
using System.Collections;

public partial class Grid {

    public void GenerateMaze()
    {
        //Maze Start
        notResolved = true;
        generating = true;
        GameObject cp = (GameObject)Instantiate(Resources.Load<GameObject>("Objects/Cell"), new Vector3(0, 1000, 0), Quaternion.identity);
        CellPrefab = cp.transform;
        CellPrefab.localScale = Vector3.Scale(CellPrefab.localScale, new Vector3(Buffer, 1, Buffer));
        CreateGrid();
        SetAdjacents();
        SetStart(0, 0);
        StartCoroutine("Generate"); 
    }

    public void CreateGrid()
    {
        int x = (int)GridSize.x;
        int z = (int)GridSize.z;
        //Camera.main.transform.position = new Vector3(Mathf.Max(x, z) / 2f, Mathf.Max(x, z), Mathf.Max(x, z) / 8f);
        GridArr = new Transform[x, z];
        Transform newCell;
        for (int ix = 0; ix < x; ix++)
        {
            for (int iz = 0; iz < z; iz++)
            {
                newCell = (Transform)Instantiate(CellPrefab, new Vector3(ix, 0, iz) * Buffer, Quaternion.identity);
                newCell.name = string.Format("({0},0,{1})", ix, iz);
                newCell.parent = transform;
                newCell.GetComponent<Cell>().Position = new Vector3(ix, 0, iz);
                GridArr[ix, iz] = newCell;
            }
        }
    }

    void SetStart(int x, int z)
    {
        //GridArr[x, z].GetComponent<Renderer>().material.color = Color.black;
        Set.Insert(0, GridArr[x, z]);
        foreach (Transform adjToNext in Set[0].GetComponent<Cell>().Adjacents)
            adjToNext.GetComponent<Cell>().Adjacents.Remove(Set[0]);
    }

    void SetAdjacents()
    {
        for (int ix = 0; ix < GridSize.x; ++ix)
        {
            for (int iz = 0; iz < GridSize.z; ++iz)
            {
                if (!GridArr[ix, iz])
                    continue;

                int seed = Random.Range(0, 4);
                for (int i = 0; i < 4; ++i)
                {
                    SetAdjacentByID((seed + i) % 4, ix, iz);
                }
            }
        }
    }

    void SetAdjacentByID(int ID, int ix, int iz)
    {
        Transform cell;
        cell = GridArr[ix, iz];
        Cell cScript = cell.GetComponent<Cell>();
        switch (ID)
        {
            case 0:
                if (ix - 1 >= 0)
                    if (GridArr[ix - 1, iz])
                    {
                        cScript.Adjacents.Add(GridArr[ix - 1, iz]);
                        cScript.AllAdjacents.Add(GridArr[ix - 1, iz]);
                    }
                break;
            case 1:
                if (ix + 1 < GridSize.x)
                    if (GridArr[ix + 1, iz])
                    {
                        cScript.Adjacents.Add(GridArr[ix + 1, iz]);
                        cScript.AllAdjacents.Add(GridArr[ix + 1, iz]);
                    }
                break;
            case 2:
                if (iz - 1 >= 0)
                    if (GridArr[ix, iz - 1])
                    {
                        cScript.Adjacents.Add(GridArr[ix, iz - 1]);
                        cScript.AllAdjacents.Add(GridArr[ix, iz - 1]);
                    }
                break;
            case 3:
                if (iz + 1 < GridSize.z)
                    if (GridArr[ix, iz + 1])
                    {
                        cScript.Adjacents.Add(GridArr[ix, iz + 1]);
                        cScript.AllAdjacents.Add(GridArr[ix, iz + 1]);
                    }
                break;
        }
    }

    void FindNext()
    {

        if (Set.Count == 0)
            return;

        Transform previous = Set[0];
        Cell pScript = Set[0].GetComponent<Cell>();

        if (pScript.Adjacents.Count == 0)
            AddToCompletedSet(previous);

        Transform next;
        Cell nScript;
        for (int i = 0; i < pScript.Adjacents.Count; ++i)
        {
            next = pScript.Adjacents[0];
            nScript = next.GetComponent<Cell>();
            //DebugLine.DrawLine(previous.position, next.position, Color.red, 1);

            RaycastHit[] hitInfo;
            hitInfo = Physics.RaycastAll(previous.position + Vector3.up, next.position - previous.position, Buffer);
            foreach (RaycastHit hit in hitInfo)
                Destroy(hit.transform.gameObject);

            //next.GetComponent<Renderer>().material.color = Color.black;

            Set.Insert(0, next);

            foreach (Transform adjToNext in nScript.AllAdjacents)
                adjToNext.GetComponent<Cell>().Adjacents.Remove(next);

            nScript.Adjacents.Remove(previous);

            if (nScript.Adjacents.Count == 0)
                AddToCompletedSet(next);
        }

    }

    void AddToCompletedSet(Transform toAdd)
    {
        Set.Remove(toAdd);
        CompletedSet.Add(toAdd);
        Cell taScript = toAdd.GetComponent<Cell>();
        for (int i = taScript.AllAdjacents.Count - 1; i >= 0; --i)
        {
            Transform adjToNext = taScript.AllAdjacents[i];
            Cell atScript = adjToNext.GetComponent<Cell>();

            atScript.Adjacents.Remove(toAdd);
            taScript.Adjacents.Remove(adjToNext);
        }
    }

    IEnumerator Generate()
    {
        while (CompletedSet.Count < GridSize.x * GridSize.z)
        {
            FindNext();
            yield return null;
        }
        //Instantiate player, check all the cells for one-way paths and destroy example cell
        foreach (Transform t in GameObject.Find("Grid").GetComponentsInChildren<Transform>())
            if (t.childCount == 3 && !CellSets.ContainsKey(t.name))
                CellSets.Add(t.name, t);
        float maxDist = Mathf.Sqrt(Mathf.Pow(GridSize.x, 2) + Mathf.Pow(GridSize.z, 2));
        foreach (Transform t in CellSets.Values)
        {
            string[] coords = t.name.Replace("(", "").Replace(")", "").Split(',');
            if (Random.value < Mathf.Sqrt(Mathf.Pow(float.Parse(coords[0]), 2) + Mathf.Pow(float.Parse(coords[2]), 2)) / maxDist)
            {
                finalPosition = t.transform.position;
                break;
            }
        }
        if (player != null)
        {
            GameObject p = (GameObject)Instantiate(player, Vector3.up, Quaternion.identity);
            p.name = "Player";
        }
        GameObject f = new GameObject("Floors");
        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.parent = f.transform;
        roof.name = "Roof";
        roof.transform.position = new Vector3(Buffer * GridSize.x / 2 - 1, CellPrefab.localScale.y * (GridSize.y + 1) * 2, Buffer * GridSize.z / 2 - 1);
        roof.transform.localScale = new Vector3(Buffer * GridSize.x, .1f, Buffer * GridSize.z);
        Destroy(CellPrefab.gameObject);
        GameObject fc = (GameObject)Instantiate(Resources.Load<GameObject>("Objects/Finish"), finalPosition + Vector3.up * .5f, Quaternion.identity);
        fc.name = "Finish";
        generating = false;
    }

    void FinishWindow(int id)
    {
        GUI.Label(new Rect(10, 20, 380, 40), "You finished the maze in " + time.ToString("F2") + " secs!\nDo you want to reset it?", new GUIStyle("label") { alignment = TextAnchor.MiddleCenter });
        if (GUI.Button(new Rect(120, 60, 75, 20), "Yes"))
        {
            Destroy(GameObject.Find("Player"));
            Destroy(GameObject.Find("Floors"));
            Destroy(GameObject.Find("Finish"));
            GameObject g = (GameObject)Instantiate(Resources.Load<GameObject>("Objects/Grid"));
            g.name = "Grid";
            Destroy(gameObject);
        }
        if (GUI.Button(new Rect(205, 60, 75, 20), "No"))
            notResolved = true;
    }

}
