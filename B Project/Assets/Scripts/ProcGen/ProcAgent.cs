using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TileDistribution {
    exponential,
    uniform,
    geometric,
    custom,
    customCenter
}

[RequireComponent(typeof(Probability))]
public class ProcAgent : MonoBehaviour {

    [SerializeField]
    public float restProbability = 0.2f;
    public float merchantProbability = 0.2f;
    public float settlementProbability = 0.2f;
    public float treasureProbability = 0.2f;
    public float nothingProbability = 0.2f;

    public float tileProbability = 0.85f;
    public float tileSize = 1f;
    public float tilePaddingX = 0f;
    public float tilePaddingY = 0f;

    public int islandSize = 4;
    public int minAdjacency = 2;

    public int maxRests = 1;
    public int maxMerchants = 1;
    public int maxSettlements = 1;
    public int maxTreasure = 2;

    public int maxSteps = 50;

    public Vector2Int center = new Vector2Int(0, 0);
    public TileDistribution distribution = TileDistribution.exponential;
    public bool fillHoles = false;
    public bool overwriteCells = false;
    public bool connectCells = false;
    public bool customCenter = false; //Is the center automatically set to the middle of the map or defined?
    public bool debug = false;
    public AnimationCurve customProb;
    public GameObject tile;
    public GameObject parent = null; //Where the gameobjects will be instantiated under

    Probability prob;
    private UndirectedGraph<Node<ProcData>> graph;
    private Vector2Int vertex = new Vector2Int(); //Used by the agent as it traverses the graph
    private int islandDim = 0;
    private int rests = 0;
    private int merchants = 0;
    private int settlements = 0;
    private int treasure = 0;
    private int steps = 0;
    [SerializeField]
    private int tileCount = 0; //The number of tiles in the current map
    private ProcData[,] tilemap;

    /// <summary>
    /// Creates a map using the current settings.
    /// </summary>
    public void createMap() {
        generateGraph();
        InstantiateHexes();
    }

    /// <summary>
    /// Destroys the map
    /// </summary>
    public void destroyMap() {
        var children = new List<GameObject>();
        foreach (Transform child in parent.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        tileCount = 0;
    }

    public void destroyMapEdit() {
        var children = new List<GameObject>();
        foreach (Transform child in parent.transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));

        tileCount = 0;
    }

    public bool mapExists() {

        if (tileCount > 0) {
            return true;
        }
        return false;
    }

    public string tileCountString() {
        return tileCount.ToString();
    }

    /// <summary>
    /// Sets the data at position (x,y) to the given data
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="data"></param>
    public void setData(int x, int y, ProcData data) {
        if (isBounds(x, y) && tilemap[x,y] != null) {
            tilemap[x, y] = data;
        }
    }

    public bool isBounds(int x, int y)
    {
        if (x > 0 && x < islandDim && y > 0 && y < islandDim)
        {
            return true;
        }
        return false;
    }

    void Start() {
        prob = this.GetComponent<Probability>();
    }

    void OnValidate() {
        float[] arr = tileDistribution();
        prob = this.GetComponent<Probability>();
        prob.normalizeDistribution(arr);

        restProbability = arr[0];
        merchantProbability = arr[1];
        settlementProbability = arr[2];
        treasureProbability = arr[3];
        nothingProbability = arr[4];

        minAdjacency = Mathf.Clamp(minAdjacency, 0, 6);
    }

    void traverseGraph() {
        ++steps;
        List<Vector2Int> adjacent = neighbours(vertex.x, vertex.y);

        int v = Random.Range(0, adjacent.Count - 1);
        vertex = adjacent[v];
        ProcData data = tilemap[vertex.x, vertex.y];
        HexTile tile = data.obj.GetComponent<HexTile>();

        if (data.type == CellType.empty || overwriteCells) {
            int index = prob.pickIndex(tileDistribution());

            switch (index) {
                case 0:
                    if (rests < maxRests) {
                        data.type = CellType.rest;
                        tile.setMaterial();
                    }
                    break;
                case 1:
                    if (merchants < maxMerchants) {
                        data.type = CellType.merchant;
                        tile.setMaterial();
                    }
                    break;
                case 2:
                    if (settlements < maxSettlements) {
                        data.type = CellType.settlement;
                        tile.setMaterial();
                    }
                    break;
                case 3:
                    if (treasure < maxTreasure) {
                        data.type = CellType.treasure;
                        tile.setMaterial();
                    }
                    break;
                case 4:
                    break;
            }
        }

        if (steps < maxSteps) {
            Invoke("traverseGraph", 0.5f);
        }
        else {
            steps = 0;
        }
    }

    void InstantiateHexes() {
        if (parent == null) {
            parent = this.gameObject;
        }
        
        
        for (int x = 0; x < islandDim; x++) {
            for (int y = 0; y < islandDim; y++) {

                ProcData data = tilemap[x, y];
                if (data.type != CellType.nil) {
                    Vector2 pos = hexCoordinate(x, y);
                    data.obj = Instantiate(tile, new Vector3(0f, 0f, 0f), this.tile.transform.rotation, parent.transform);
                    data.obj.transform.Translate(pos.x, 0f, pos.y);
                    data.pos.x = x;
                    data.pos.y = y;
                    data.agent = this;
                    HexTile hexTile = data.obj.AddComponent<HexTile>();
                    hexTile.data = data;
                    ++tileCount;
                }
            }
        }
    }

    void generateGraph() {
        tileCount = 0;
        //Generate a hex map
        ProcData[,] map = new ProcData[2 * islandSize + 1, 2 * islandSize + 1];
        islandDim = 2 * islandSize + 1;

        //Set the vector to the custom vector if the user has specified one.
        Vector2Int vCenter = new Vector2Int();
        if (!customCenter) {
            vCenter = new Vector2Int(islandSize, islandSize);
        }
        else {
            vCenter = center;
        }
        
        for (int x = 0; x < islandDim; x++) {
            for (int y = 0; y < islandDim; y++) {
                map[x, y] = new ProcData(CellType.nil);
                int d = distance(x, y, vCenter.x, vCenter.y);
                //Find probability of the tile appearing in (x,y)
                float p = evaluateProb(distribution, d);

                float q = Random.Range(0f, 1f);

                //Should we create a cell here?
                if (q < p) {
                    map[x, y].type = CellType.empty;
                }


            }
        }

        //Place a cell in the center of the map
        map[vCenter.x, vCenter.y].type = CellType.empty;

        //Basis to be used to check cell neighbours
        int[] neighbours = basis();
        List<Vector2Int> remove = new List<Vector2Int>();

        //boolean hashmap used for constructing disjointed subgraphs
        bool[] visited = new bool[islandDim * islandDim];
        //List of subgraphs
        List<List<Vector2Int>> subgraphs = new List<List<Vector2Int>>();

        //Get rid of tiles without the minimal adjacent neighbours.
        for (int x = 0; x < islandDim; x++) {
            for (int y = 0; y < islandDim; y++) {
                int adjCount = 0;

                //Iterate over the current cell's neighbours
                for (int i = 0; i < neighbours.Length; i += 2) {
                    int xPos = x + neighbours[i];
                    int yPos = y + neighbours[i + 1];

                    if (xPos < 0 || xPos >= islandDim || yPos < 0 || yPos >= islandDim) {
                        continue;
                    }

                    if (map[xPos, yPos].type == CellType.empty) {
                        ++adjCount;
                    }
                }

                //Add cells to a buffer to be removed later, removing them as the map is searched will modify their adjacent counts
                if (adjCount < minAdjacency) {
                    remove.Add(new Vector2Int(x, y));
                }

                //Fill any holes if specified
                if (map[x, y].type == CellType.nil && fillHoles && adjCount == 6) {
                    Debug.Log("filled (" + x + "," + y + ")");
                    map[x, y].type = CellType.empty;
                }

                //Connect disjointed subgraphs if specified
                if (connectCells && map[x, y].type == CellType.empty && !visited[hash(x, y)]) {
                    subgraphs.Add(BFS(x, y, visited, map));
                }
            }
        }

        foreach (Vector2Int pos in remove) {
            map[pos.x, pos.y].type = CellType.nil;
        }

        //Connect subgraphs seperated from the center
        if (connectCells) {
            foreach(List<Vector2Int> graph in subgraphs) {
                //Check if this graph has the center, draw a line connecting the two.
                if(!graph.Contains(center)) {
                    int r = Random.Range(0, graph.Count - 1);

                    Connect(graph[r], vCenter, map);
                }
            }
        }


        tilemap = map;
    }

    /// <summary>
    /// Calculates the probability given a distribution type and the tiles distance from the map center
    /// </summary>
    /// <param name="tDistribution">The tile distribution to be used</param>
    /// <param name="d">the distance from the center</param>
    /// <returns></returns>
    public float evaluateProb(TileDistribution tDistribution, int d) {
        float p = 0;
        switch (tDistribution) {
            case TileDistribution.exponential:
                p = Mathf.Pow(tileProbability, d);
                break;
            case TileDistribution.uniform:
                p = tileProbability;
                break;
            case TileDistribution.geometric:
                p = Mathf.Pow(1 - tileProbability, d - 1) * tileProbability;
                break;
            case TileDistribution.custom:
                p = Mathf.Clamp01(customProb.Evaluate(Random.Range(tileProbability, 1f)));
                break;
            case TileDistribution.customCenter:
                p = Mathf.Clamp01(customProb.Evaluate(Mathf.Pow(tileProbability, d)));
                break;
        }

        return p;
    }

    /// <summary>
    /// Convert integer (x, y) coordinates to a Vector2 map position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2 hexCoordinate(int x, int y) {
        float w = Mathf.Sqrt(3) * tileSize;
        float h = 2 * tileSize;
        float thirdH = h * 0.75f;

        float xPos = (y % 2) == 0 ? w * x + x * tilePaddingX : w * x + w / 2 + x * tilePaddingX;
        float yPos = -y * thirdH - y * tilePaddingY;
        return new Vector2(xPos, yPos);
    }

    List<Vector2Int> neighbours(int x, int y) {
        int[] b = basis();
        List<Vector2Int> neigh = new List<Vector2Int>();

        for (int i = 0; i < b.Length; i += 2) {
            int xPos = x + b[i];
            int yPos = y + b[i + 1];

            if (xPos < 0 || xPos >= islandDim || yPos < 0 || yPos >= islandDim || tilemap[xPos, yPos].type == CellType.nil) {
                continue;
            }
            neigh.Add(new Vector2Int(xPos, yPos));
        }

        return neigh;
    }

    int hash(int x, int y) {
        return x * y + x;
    }

    List<Vector2Int> neighbours(int x, int y, List<Vector2Int> neigh, bool[] visited, ProcData[,] map) {
        int[] b = basis();
        for (int i = 0; i < b.Length; i += 2) {
            int xPos = x + b[i];
            int yPos = y + b[i + 1];

            if (!isBounds(xPos, yPos) || map[xPos, yPos].type == CellType.nil) {
                continue;
            }

            if (!visited[hash(x, y)])
            {
                continue;
            }
            else
            {
                neigh.Add(new Vector2Int(xPos, yPos));
                visited[hash(x, y)] = true;
            }
        }

        return neigh;
    }

    void Connect(int x1, int y1, int x2, int y2, ProcData[,] map) {
        Vector2Int[] line = Line(x1, y1, x2, y2);

        foreach(Vector2Int point in line) {
            ProcData data = map[point.x, point.y];

            data.type = CellType.empty;
        }
    }

    void Connect(Vector2Int a1, Vector2Int b1, ProcData[,] map) {
        Connect(a1.x, a1.y, b1.x, b1.y, map);
    }

    Vector2Int[] Line(int x1, int y1, int x2, int y2) {
        int n = distance(x1,y1,x2,y2);
        Vector2Int[] points = new Vector2Int[n];
        Vector3Int a = oddRToCube(x1, y1);
        Vector3Int b = oddRToCube(x2, y2);

        for(int i = 0; i < n; i++) {
            points[i] = cubeToOddR(cubeRound(cubeLerp(a, b , 1.0f / n * i)));
        }

        return points;
    }

    Vector2Int[] Line(Vector2Int a1, Vector2Int b1) {
        return Line(a1.x, a1.y, b1.x, b1.y);
    }

    /// <summary>
    /// Returns the 6 directional vectors used in this Hexagonal representation.
    /// </summary>
    /// <returns></returns>
    int[] basis() {
        int[] neighbours = 
        {
            -1,-1,
            -1,0,
            -1,1,
            0,-1,
            1,0,
            0,1
        };

        return neighbours;
    }

    List<Vector2Int> BFS(int x, int y, bool[] visited, ProcData[,] map) {
        
        List<Vector2Int> subgraph = new List<Vector2Int>();
        int current = 0;
        subgraph.Add(new Vector2Int(x, y));
        neighbours(x, y, subgraph, visited, map);
        current = subgraph.Count;

        for (int i = current; i < subgraph.Count; i++) {
            Debug.Log("Current Index: " + i);
            Debug.Log("Subgraph size: " + subgraph.Count);
            neighbours(subgraph[i].x, subgraph[i].y, subgraph, visited, map);
        }

        return subgraph;
    }

    CellType pickType(float s) {
        return CellType.empty;
    }

    float[] tileDistribution() {
        float[] arr = {
            restProbability,
            merchantProbability,
            settlementProbability,
            treasureProbability,
            nothingProbability
        };

        return arr;
    }

    Vector3Int cubeRound(Vector3 a) {
        int rx = Mathf.RoundToInt(a.x);
        int ry = Mathf.RoundToInt(a.y);
        int rz = Mathf.RoundToInt(a.z);

        float xDiff = Mathf.Abs(rx - a.x);
        float yDiff = Mathf.Abs(ry - a.y);
        float zDiff = Mathf.Abs(rz - a.z);

        if(xDiff > yDiff && xDiff > zDiff) {
            rx = -ry - rz;
        }
        else if (yDiff > zDiff) {
            ry = -rx - rz;
        }
        else {
            rz = -rx - ry;
        }

        return new Vector3Int(rx, ry, rz);
    }

    Vector3 cubeLerp(Vector3Int a, Vector3Int b, float t) {
        return new Vector3(
            Mathf.Lerp(a.x, b.x, t),
            Mathf.Lerp(a.y, b.y, t),
            Mathf.Lerp(a.z, b.z, t));
    }

    Vector2Int cubeToOddR(int x, int y, int z) {
        int col = x + (z - (z & 1)) / 2;
        int row = z;
        return new Vector2Int(col, row);
    }

    Vector2Int cubeToOddR(Vector3Int a) {
        return cubeToOddR(a.x, a.y, a.z);
    }

    Vector3Int oddRToCube(int a0, int a1) {
        int x = a0 - (a1 - (a1 & 1)) / 2;
        int z = a1;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    Vector3Int oddRToCube(Vector2Int a) {
        return oddRToCube(a.x, a.y);
    }

    int distance(int a0, int a1, int b0, int b1) {
        return length(oddRToCube(a0, a1) - oddRToCube(b0, b1));
    }

    int length(Vector3Int hex) {
        return ((Mathf.Abs(hex.x) + Mathf.Abs(hex.y) + Mathf.Abs(hex.z)) / 2);
    }

    
}
