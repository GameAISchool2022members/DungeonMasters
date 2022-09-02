using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class GenerateCave : MonoBehaviour
{

    //Game objects to instantiate
    public GameObject tile;
    public GameObject navTile;

    public GameObject goal;

    public GameObject enemyObj;
    public GameObject[] enemies; //Prefabs

    public GameObject playerPrefab;

    public GameObject painting;

    private List<CaveRoom> rooms;
    public NavMeshSurface nav;

    public Sprite tempImage;
    public int tempNum;

    public int[] currentlySpawning; //which enemies to spawn for this level

    //Instance pools

    public GameObject floorInstancePool;
    public GameObject enemyInstancePool;
    public GameObject otherPool;

    //Variables

    private GameObject enemy;

    public int width;
    public int height;

    public int minRoom;
    public int minWall;

    public string seed;

    public int smoothIterations;
    [Range(0, 9)]
    public int smoothLevel;
    [Range(0, 7)]
    public int smoothGap;
    [Range(0, 100)]
    public int randomFillPercent;

    private int[,] map;
    private int[,] objects;
    private List<List<Coord>> roomRegions;
    private List<List<Coord>> wallRegions;
    private List<Coord> edges;


    private bool fullyConnected;

    //CPPN 
    public static GameManager Instance { get; private set; }
    public List<Sprite> sprites;

    public List<Collectables> generatedCollectibles;

    void Start()
    {        
        //Setting up level
        seed = "1";
        currentlySpawning = new int[3];
        for (int i = 0; i < 3; i++) //Choose 3 enemies to spawn (may be repeated)
        {
            currentlySpawning[i] = UnityEngine.Random.RandomRange(0, 4);
        }

        generatedCollectibles = new List<Collectables>();
        generatedCollectibles.Add(new Collectables());
        generatedCollectibles.Add(new Collectables());
        generatedCollectibles.Add(new Collectables());
        generatedCollectibles.Add(new Collectables());


        //Population arbitrarily defined
        generatedCollectibles[0].image = Resources.Load<Sprite>("4");
        generatedCollectibles[1].image = Resources.Load<Sprite>("8");
        generatedCollectibles[2].image = Resources.Load<Sprite>("16");
        generatedCollectibles[3].image = Resources.Load<Sprite>("32");

        //Set player pos again to avoid a bug
        GenerateMap();
        SetPlayer();
        SetGoal();
        //StartCoroutine(WaitInitialPop());
    }

    /*
    IEnumerator WaitInitialPop()
    {

        generatedCollectibles = new List<Collectables>();
        generatedCollectibles.Add(new Collectables());
        generatedCollectibles.Add(new Collectables());
        generatedCollectibles.Add(new Collectables());
        generatedCollectibles.Add(new Collectables());


        //Population arbitrarily defined
        generatedCollectibles[0].image = Resources.Load<Sprite>("4");
        generatedCollectibles[1].image = Resources.Load<Sprite>("8");
        generatedCollectibles[2].image = Resources.Load<Sprite>("16");
        generatedCollectibles[3].image = Resources.Load<Sprite>("32");

        generatedCollectibles[0].ID = 0;
        generatedCollectibles[1].ID = 1;
        generatedCollectibles[2].ID = 2;
        generatedCollectibles[3].ID = 3;

        //yield return DataManager.Instance.canStartGenerating;
        //yield return DataManager.Instance.UploadSelectedCollectables(generatedCollectibles); //Initialising population (with arbitary images)

        //generatedCollectibles = DataManager.Instance.GetCurrentLevelCollectables();

        //Update the sprites to what is now in the pop
        sprites.Add(generatedCollectibles[0].image);
        sprites.Add(generatedCollectibles[1].image);
        sprites.Add(generatedCollectibles[2].image);
        sprites.Add(generatedCollectibles[3].image);

        

        
    }*/

    public void NextLevel()
    {
        //Send information about the enemy prefabs
        for(int i = 0; i < enemies.Length; i++)
        {
            Debug.Log("image " + i + "  = " + enemies[i].GetComponent<EnemyType>().image);
            Debug.Log("ID " + i + "  = " + enemies[i].GetComponent<EnemyType>().ID);
            Debug.Log("Intensity array " + i + "  = " + enemies[i].GetComponent<EnemyType>().intensities);
            for(int j = 0; j < enemies[i].GetComponent<EnemyType>().intensities.Count; j++)
            {
                Debug.Log("    value " + j + "  = " + enemies[i].GetComponent<EnemyType>().intensities[j]);
            }
        }
        
        for(int i=0;i< 3; i++) //Choose 3 enemies to spawn (may be repeated)
        {
            currentlySpawning[i] = UnityEngine.Random.RandomRange(0, 4);
        }

        //Remove all objects
        foreach (Transform child in floorInstancePool.transform) //Remove tiles
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in enemyInstancePool.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int c  = 0; c < otherPool.transform.childCount; c++){
         
            if(otherPool.transform.GetChild(c).GetComponent<Player>() == null)
            {
                GameObject.Destroy(otherPool.transform.GetChild(c).gameObject);
            }            
        }
        seed = (int.Parse(seed) +1).ToString();
        GenerateMap();
        SetPlayer();
        SetGoal();
    }

    public void SetPlayer()
    {
        int leftOffset = 2;
        for(int x = 0; x < width; x++) //Set initial player position
        {
            for(int y= 0; y < height; y++) //Find valid position
            {
                if (map[x,y]==0)
                {
                    leftOffset--;
                    if(leftOffset <= 0)
                    {
                        playerPrefab.transform.position = new Vector3(x, y, 0);
                    }
                    break;                    
                }
            }
            if (leftOffset <= 0)
            {
                break;
            }
        }
    }

    public void SetGoal()
    {
        int RightOffset = 2;
        for (int x = width-1; x > 0; x--) //Set initial player position
        {
            for (int y = 0; y < height-1; y++) //Find valid position
            {               
                if (map[x, y+1] == 0)
                {
                    RightOffset--;
                    if (RightOffset <= 0)
                    {
                        GameObject goalObj = Instantiate(goal, new Vector3(x, y+1, 0), Quaternion.identity);
                        goalObj.transform.localScale = new Vector3(2, 2, 1);
                        goalObj.transform.parent = otherPool.transform;
                    }
                    break;
                }
            }
            if (RightOffset <= 0)
            {
                break;
            }
        }
    }


    public void GenerateMap() //Build the new map
    {
        objects = new int[width, height];
        map = new int[width, height]; //define map
        RandomfillMap();             //initial random map

        for (int i = 0; i < smoothIterations; i++)    //Smoothen map (make into caves)
        {
            SmoothMap();
        }

        roomRegions = GetRegions(0);
        wallRegions = GetRegions(1);

        RemoveTrivialRegions();     //Remove remaining outlier bits

        rooms = RegionsToRooms(roomRegions);

        //Ensure fully connected
        fullyConnected = false;
        while (!fullyConnected)
        {
            roomRegions = GetRegions(0);
            if(roomRegions.Count <= 1)
            {
                fullyConnected = true;
            }
            else
            {
                rooms = RegionsToRooms(roomRegions);
                ConnectClosestRooms(rooms, false);                
            }
        }
        
        BuildMap();                 //Instantiate Map
        PopulateMap();              //Place enemies
        nav.BuildNavMesh();         //Build Navmesh
    }

    void PopulateMap()
    {
        SetPlayer();
        SetGoal();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(map[x,y] == 0)
                {
                    if (!edges.Contains(new Coord(x, y)) && objects[x, y] != 1)
                    {                            
                        //Spawn new enemy (random low chance and must be 15 m away from player)
                        if (UnityEngine.Random.Range(0f, 10f) < 0.25f && Vector3.Distance(playerPrefab.transform.position, new Vector3(x - 0.5f, y + 0.5f, 0.2f)) > 15) 
                        {
                            if (UnityEngine.Random.Range(0f, 10f) < 3.3f)
                            {
                               // Debug.Log("enemies[" + currentlySpawning[0] + "] out of " + enemies.Length);
                                enemy = Instantiate(enemies[currentlySpawning[0]], new Vector3(x - 0.5f, y + 0.5f, 0.2f), Quaternion.Euler(0, 0, 0));
                            }
                            else if (UnityEngine.Random.Range(0f, 10f) < 6.3f)
                            {
                               // Debug.Log("enemies[" + currentlySpawning[1] + "] out of " + enemies.Length);
                                enemy = Instantiate(enemies[currentlySpawning[1]], new Vector3(x - 0.5f, y + 0.5f, 0.2f), Quaternion.Euler(0, 0, 0));
                            }
                            else
                            {
                               // Debug.Log("enemies[" + currentlySpawning[2] + "] out of " + enemies.Length);
                                enemy = Instantiate(enemies[currentlySpawning[2]], new Vector3(x - 0.5f, y + 0.5f, 0.2f), Quaternion.Euler(0, 0, 0));
                            }
                            

                            enemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                            enemy.transform.parent = enemyInstancePool.transform;
                            objects[x, y] = 1;
                        }                                                        
                    }                                       
                }               
            }
        }

        for (int r = 0; r < rooms.Count;r++)
        {
            //70% chance to make colelctable
            if(UnityEngine.Random.Range(0,100) > 30)
            {
                Coord paintingLoc = GetRandomTile(rooms[r]);
                
                //must be 20m fartehr from player spawn
                if (Vector3.Distance(playerPrefab.transform.position, new Vector3(paintingLoc.tileX, paintingLoc.tileY, 0)) > 20f)
                {
                    for (int h = paintingLoc.tileY; h > 0; h--) //Find first floor
                    {
                        if (map[paintingLoc.tileX, h - 1] == 1)
                        {
                            if(generatedCollectibles.Count > 0) //Ensure there are still Images to place
                            {
                                GameObject newPainting = Instantiate(painting, new Vector3(paintingLoc.tileX, h, 0), Quaternion.identity);

                                //Get a random possible collectable
                                int toPlace = UnityEngine.Random.RandomRange(0, generatedCollectibles.Count);

                                //AssetDatabase.CreateAsset(generatedCollectibles[toPlace].image, "Assets/Resources/Sprites/Sprite"+toPlace+".png");

                                newPainting.GetComponent<Collectables>().ID = generatedCollectibles[toPlace].ID;
                                newPainting.GetComponent<Collectables>().image = generatedCollectibles[toPlace].image;
                                newPainting.GetComponentInChildren<SpriteRenderer>().sprite = generatedCollectibles[toPlace].image;
                                newPainting.transform.parent = otherPool.transform;

                                //generatedCollectibles.Remove(generatedCollectibles[toPlace]); //Do not repeat, so remove from list
                                break;
                            }
                            
                        }
                    }
                }
            } 
        }
        
    }

    void RandomfillMap()
    {
        System.Random rand = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) //Is a wall (edge)
                {
                    map[x, y] = 1;
                }
                else//May or may not be a wall
                {
                    map[x, y] = (rand.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void BuildMap()
    {
        edges = new List<Coord>();
        string seed = Time.time.ToString();
        GameObject caveTile;
        for (int x = 1; x < width; x++)
        {
            //Debug.Log("building map");
            for (int y = 1; y < height; y++)
            {
                //Debug.Log("x= " + x + ", y= " + y + " = [" + map[x,y] + "]");
                if (map[x, y] == 1)
                {                    
                        caveTile = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                        caveTile.transform.parent = floorInstancePool.transform;                                        
                }
                else if (map[x, y] == 0)
                {
                    caveTile = Instantiate(navTile, new Vector3(x, y, 1.2f), Quaternion.identity);
                    caveTile.transform.parent = floorInstancePool.transform;
                    
                }
                if (map[x - 1, y] != map[x, y])
                {
                    edges.Add(new Coord(x, y));
                }
                if (map[x, y - 1] != map[x, y])
                {
                    edges.Add(new Coord(x, y));
                }
            }
        }

        //Force borders (with extra padding)
        for (int x = -11; x < width+11; x++)
        {
            for(int y= -6; y <= 0; y++)
            {
                caveTile = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                caveTile.transform.parent = floorInstancePool.transform;
            }
            for (int y = height; y < height + 6; y++)
            {
                caveTile = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                caveTile.transform.parent = floorInstancePool.transform;
            }

        }
        for (int y = -6; y < height+6; y++)
        {
            for (int x = -11; x <= 0; x++)
            {
                caveTile = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                caveTile.transform.parent = floorInstancePool.transform;
            }

            for (int x = width; x < width + 11; x++)
            {
                caveTile = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity);
                caveTile.transform.parent = floorInstancePool.transform;
            }
            
            
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int wallCount = GetWallCount(x, y);

                if (wallCount > smoothLevel)
                {
                    map[x, y] = 1;
                }
                else if (wallCount <= smoothLevel - smoothGap)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetWallCount(int xpos, int ypos) //check how many sorrounding tiles are walls
    {
        int walls = 0;
        for (int x = xpos - 1; x <= (xpos + 1); x++)
        {
            for (int y = ypos - 1; y <= (ypos + 1); y++)
            {
                if ((x >= 0 && x < width) && (y >= 0 && y < height))
                {
                    if (x != xpos || y != ypos) //Not the actual centre tile
                    {
                        walls += map[x, y];
                    }
                }
                else
                {
                    walls++;
                }
            }
        }
        return walls;
    }

 
    //Get regions (flood method)
    List<Coord> GetRegionTiles(int startX, int startY) //Flood the region the same as starting tile
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == map[startX, startY])
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    List<List<Coord>> GetRegions(int tileType) //Exclude flooded tiles and find non flooded
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    void RemoveTrivialRegions()
    {
        List<Coord> region = new List<Coord>();
        for (int regionIndex = 0; regionIndex < roomRegions.Count; regionIndex++) //Remove Rooms that are too small
        {
            region = roomRegions[regionIndex];
            if (region.Count < minRoom)
            {
                foreach (Coord tile in region)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
                roomRegions.Remove(region); //Remove no longer existing region
            }
        }
        for (int regionIndex = 0; regionIndex < wallRegions.Count; regionIndex++) //Remove walls that are too small
        {
            region = wallRegions[regionIndex];
            if (region.Count < minWall)
            {
                foreach (Coord tile in region)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }
        //Reset surviving rooms
        roomRegions = GetRegions(0);
        wallRegions = GetRegions(1);
    }

    List<CaveRoom> RegionsToRooms(List<List<Coord>> regions)
    {
        List<CaveRoom> rooms = new List<CaveRoom>();
        foreach (List<Coord> region in regions)
        {
            rooms.Add(new CaveRoom(region, map,width,height));       //Make a Room Equivalent of the regions
        }
        if(rooms.Count > 0)
        {
            rooms.Sort();
            rooms[0].isMain = true;
            rooms[0].startAccessable = true;
        }        

        return rooms;
    }
        
    Coord GetRandomTile(CaveRoom room)
    {
        CaveRoom checkRoom = room;
        int randomTile;
        int counter = 0;

        while (true)
        {
            randomTile = UnityEngine.Random.Range(0, checkRoom.tiles.Count);
            Coord potentialStart = checkRoom.tiles[randomTile];

            if (
                !room.edgeTiles.Contains(potentialStart)&&                  //not too close to edge
                objects[potentialStart.tileX, potentialStart.tileY] != 1)   //not already taken
            {
                //stopPos = checkRoom.tiles[randomTile];
                objects[potentialStart.tileX, potentialStart.tileY] = 1;
                return potentialStart;
            }
            else
            {                    
                counter++;
                if(counter >= room.tiles.Count) //No more tiles to check
                {
                    //stopPos = checkRoom.tiles[randomTile];
                    objects[potentialStart.tileX, potentialStart.tileY] = 1;
                    return potentialStart;
                }
                checkRoom.tiles.Remove(potentialStart); //Remove from search pool
            }
        }
    }

    void ConnectClosestRooms(List<CaveRoom> rooms, bool startAccess)
    {
        
        List<CaveRoom> roomsA = new List<CaveRoom>();
        List<CaveRoom> roomsB = new List<CaveRoom>();

        if (startAccess)
        {
            foreach (CaveRoom room in rooms)
            {
                if (room.startAccessable) //Accessable rooms
                {
                    roomsB.Add(room);
                }
                else    //Non Accessable rooms
                {
                    roomsA.Add(room);                    
                }
            }
        }
        else
        {
            roomsA = rooms;
            roomsB = rooms;
        }
                
        CaveRoom bestRoomA = new CaveRoom();
        CaveRoom bestRoomB = new CaveRoom();
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        float shortest = float.MaxValue;
        bool hasConnection = false;

        foreach (CaveRoom roomA in roomsA) //For all Rooms A
        {           
            if (!startAccess)
            {
                hasConnection = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }
            shortest = float.MaxValue;
            foreach (CaveRoom roomB in roomsB) //Find closest RoomB to RoomA
            {
                if (roomA == roomB || roomA.IsConnected(roomB)) //Is same room or already connected
                {
                    continue;
                }

                //Compare all edge tiles of all rooms to find the closest connections
                foreach (Coord tileA in roomA.edgeTiles)
                {
                    foreach (Coord tileB in roomB.edgeTiles)
                    {
                        float distance = Mathf.Pow(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2), 0.5f);
                        if (distance < shortest || !hasConnection)
                        {
                            hasConnection = true;
                            shortest = distance;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                            bestTileA = tileA;
                            bestTileB = tileB;
                        }                        
                    }
                }                
            }
            if (hasConnection && !startAccess)
            {
                Createpassage(roomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        if (hasConnection && startAccess)
        {
            Createpassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(rooms, true);
        }
        if (!startAccess)
        {
            ConnectClosestRooms(rooms, true);
        }
    }

    void Createpassage(CaveRoom roomA, CaveRoom roomB, Coord tileA, Coord tileB)
    {
        CaveRoom.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord tile in line)
        {
            FillPassage(tile, 2);
        }
    }

    void FillPassage(Coord tile, int radius)
    {
        for(int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if((x*x) + (y*y) <= (radius * radius))
                {
                    int xTile = tile.tileX + x;
                    int yTile = tile.tileY + y;
                    if (xTile < width && yTile < height
                        && xTile > 0 && yTile > 0)
                    {
                        map[xTile, yTile] = 0;
                    }                    
                }
            }
        }
    }

    List<Coord> GetLine(Coord pos1, Coord pos2){
        List<Coord> line = new List<Coord>();
        int x = pos1.tileX;
        int y = pos1.tileY;

        int dx = pos2.tileX - pos1.tileX;
        int dy = pos2.tileY - pos1.tileY;

        bool inverted = false;
        float step = Math.Sign(dx);
        float gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign (dy);
            gradientStep = Math.Sign(dx);            
        }

        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += (int)step;
            }
            else
            {
                x += (int)step;
            }
            gradientAccumulation += shortest;
            if( gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += (int)gradientStep;
                }
                else
                {
                    y += (int)gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }
        return line;
        }
    
    struct Coord
    {
        public int tileX;
        public int tileY;
        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }
    class CaveRoom :IComparable<CaveRoom>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<CaveRoom> connectedRooms;
        public bool startAccessable;
        public bool isMain;
        public int roomSize;

        public CaveRoom()
        {
            isMain = false;
        }

        public CaveRoom(List<Coord> roomTiles, int[,] map, int maxWidth,int maxHeight)
        {
            tiles = roomTiles;
            isMain = false;
            roomSize = tiles.Count;
            connectedRooms = new List<CaveRoom>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX-1; x <= tile.tileX+1; x++)
                {
                    for (int y = tile.tileY-1; y <= tile.tileY+1; y++)
                    {
                        
                        if (x > 0 && y > 0 && x < maxWidth && y < maxHeight) { 
                            if (x == tile.tileX || y == tile.tileY)
                            {
                                if (map[x, y] == 1)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AccessableFromStart()
        {
            if (!startAccessable)
            {
                startAccessable = true;
                foreach(CaveRoom room in connectedRooms)
                {
                    room.AccessableFromStart();
                }
            }
        }

        public static void ConnectRooms(CaveRoom roomA, CaveRoom roomB)
        {
            if (roomA.startAccessable)
            {
                roomB.AccessableFromStart();
            }
            else
            {
                roomA.AccessableFromStart();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(CaveRoom otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }
        public int CompareTo(CaveRoom otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}

