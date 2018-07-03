using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

public class LoadBoardFromXML : MonoBehaviour
{
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.

        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 0;                                         //Number of columns in our game board.
    public int rows = 0;                                            //Number of rows in our game board.
    //public Count obstableCount = new Count(5, 9);
    //public Count chestCount = new Count(1, 5);                       //Lower and upper limit for our random number of food items per level.
    //public GameObject exit;                                         //Prefab to spawn for exit.
    public GameObject pathTile;                                     //Tile used display path availability
    public GameObject debugTile;                                    //Tile used to display terrain type 
    //public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] itemTiles;                                  //Array of wall prefabs.
    //public GameObject[] chestTiles;
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] heroTiles;
    //public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
    //public int level;

    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private Transform debugHolder;
    private Transform heroHolder;
    private Transform enemyHolder;
    private Transform obstacleHolder;
    private List<Vector3> gridPositions = new List<Vector3>();  //A list of possible locations to place tiles.


    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list grid
        GameManager.Instance.ClearShadowBoard();

        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < columns - 1; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 1; y < rows - 1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;
        debugHolder = new GameObject("Debug").transform;
        heroHolder = new GameObject("Hero").transform;
        enemyHolder = new GameObject("Enemy").transform;
        obstacleHolder = new GameObject("Obstacles").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = 0; x < columns; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = 0; y < rows; y++)
            {
                int cost = 1;
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                //GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (GameManager.Instance._debugMode)
                {
                    if (pathTile != null)
                    {
                        GameObject curPathTile =
                    Instantiate(pathTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        PathStatusLayer script = curPathTile.GetComponent<PathStatusLayer>();
                        script._isSet = true;
                        script._X = x;
                        script._Y = y;
                        curPathTile.transform.SetParent(debugHolder);

                        GameObject curDebugTile =
                    Instantiate(debugTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        TextureMeshLayer debugScript = curDebugTile.GetComponent<TextureMeshLayer>();
                        debugScript._isSet = true;
                        debugScript._X = x;
                        debugScript._Y = y;
                        curDebugTile.transform.SetParent(debugHolder);
                    }
                }

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                //GameObject instance =
                //    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                //instance.transform.SetParent(boardHolder);

                Cell newCell = new Cell(x, y, cost, 0, 0);
                GameManager.Instance._cells.Add(new Vector2(x, y), newCell);
            }
        }
    }

    /*
     * Type:
     * 0: Obstacle
     * 1: Hero
     * 2: Enemy
     * */
    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObject(int num, Vector3 positon, int type)
    {
        switch (type)
        {
            case 0:
                {
                    GameObject instance = Instantiate(itemTiles[num], positon, Quaternion.identity);
                    instance.transform.SetParent(obstacleHolder);
                    GameManager.Instance.ChangeCellCost((int)positon.x, (int)positon.y, type, 20);
                }
                break;
            case 1:
                {
                    GameObject instance = Instantiate(heroTiles[num - 1], positon, Quaternion.identity);
                    instance.transform.SetParent(heroHolder);
                    ChrController chr = instance.GetComponent<ChrController>();
                    if (chr != null)
                    {
                        //chr._MovementRange = 6;
                        GameManager.Instance._heros.Add(chr);
                    }
                    GameManager.Instance.ChangeCellCost((int)positon.x, (int)positon.y, type, 10);
                }
                break;
            case 2:
                {
                    GameObject instance = Instantiate(enemyTiles[num - 1], positon, Quaternion.identity);
                    instance.transform.SetParent(enemyHolder);
                    ChrController chr = instance.GetComponent<ChrController>();
                    if (chr != null)
                    {
                        //chr._MovementRange = Random.Range(2, 5);
                        GameManager.Instance._enemies.Add(chr);
                    }
                    GameManager.Instance.ChangeCellCost((int)positon.x, (int)positon.y, type, 10);
                }
                break;
        }

    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene()
    {
        List<Unit> herolist = GameManager.Instance.loadxml.levels[0].heroPositions;
        List<Unit> enemylist = GameManager.Instance.loadxml.levels[0].enemyPositions;
        List<Tile> Obstacle = GameManager.Instance.loadxml.levels[0].Stiles;
        MissionManager.Instance._numHeroes = herolist.Count;
        MissionManager.Instance._numEnemies = enemylist.Count;
        //Reset our list of gridpositions.
        InitialiseList();

        //Creates the outer walls and floor.
        BoardSetup();

        for (int i = 0; i < Obstacle.Count; i++)
        {
            if (Obstacle[i]._item != 0)
            {
                LayoutObject(0, new Vector3(Obstacle[i]._x, Obstacle[i]._y, 0), 0);
                //GameManager.Instance._cells[new Vector2(Obstacle[i]._x, Obstacle[i]._y)]._cost = 10;
            }
            Cell newCell = GameManager.Instance.GetCell(Obstacle[i]._x, Obstacle[i]._y);
            switch (Obstacle[i]._terrain)
            {
                case 0: // Walkable
                    {
                        newCell._terrian = Cell.Terrians.Walkable;
                    }
                    break;
                case 1: // NonAccess
                    {
                        newCell._cost += 30;
                        newCell._terrian = Cell.Terrians.NonAccess;
                    }
                    break;
                case 2: // Forest
                    {
                        newCell._terrian = Cell.Terrians.Forest;
                    }
                    break;
                case 3: // Fort
                    {
                        newCell._terrian = Cell.Terrians.Fort;
                    }
                    break;
                case 4: // AttackingSpot
                    {
                        newCell._terrian = Cell.Terrians.AttackingSpot;
                    }
                    break;
                case 5: // Danger
                    {
                        newCell._terrian = Cell.Terrians.Danger;
                    }
                    break;
            }
            //if (Obstacle[i]._terrain != 0)
            //{
            //    Cell newCell = GameManager.Instance.GetCell(Obstacle[i]._x, Obstacle[i]._y);
            //    newCell._cost += 30;
            //}
        }
        for (int i = 0; i < herolist.Count; i++)
        {
            LayoutObject(herolist[i]._type, new Vector3(herolist[i]._x, herolist[i]._y, 0), 1);
        }
        for (int i = 0; i < enemylist.Count; i++)
        {
            LayoutObject(enemylist[i]._type, new Vector3(enemylist[i]._x, enemylist[i]._y, 0), 2);
        }


    }
}
