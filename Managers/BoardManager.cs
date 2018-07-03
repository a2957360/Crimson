//using UnityEngine;
//using System;
//using System.Collections.Generic; 		//Allows us to use Lists.
//using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

//public class BoardManager : MonoBehaviour
//{
//    // Using Serializable allows us to embed a class with sub properties in the inspector.
//    [Serializable]
//    public class Count
//    {
//        public int minimum;             //Minimum value for our Count class.
//        public int maximum;             //Maximum value for our Count class.

//        //Assignment constructor.
//        public Count(int min, int max)
//        {
//            minimum = min;
//            maximum = max;
//        }
//    }

//    public int columns = 0;                                         //Number of columns in our game board.
//    public int rows = 0;                                            //Number of rows in our game board.
//    public Count obstableCount = new Count(5, 9);
//    //public Count chestCount = new Count(1, 5);                       //Lower and upper limit for our random number of food items per level.
//    //public GameObject exit;                                         //Prefab to spawn for exit.
//    public GameObject pathTile;                                     //Tile used display path availability
//    public GameObject debugTile;                                    //Tile used to display terrain type 
//    public GameObject[] floorTiles;                                 //Array of floor prefabs.
//    public GameObject[] obstableTiles;                                  //Array of wall prefabs.
//    //public GameObject[] chestTiles;
//    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
//    public GameObject[] heroTiles;
//    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.

//    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
//    private Transform debugHolder;
//    private Transform heroHolder;
//    private Transform enemyHolder;
//    private Transform obstacleHolder;
//    private List<Vector3> gridPositions = new List<Vector3>();  //A list of possible locations to place tiles.


//    //Clears our list gridPositions and prepares it to generate a new board.
//    void InitialiseList()
//    {
//        //Clear our list grid
//        GameManager.Instance.ClearShadowBoard();

//        gridPositions.Clear();

//        //Loop through x axis (columns).
//        for (int x = 1; x < columns - 1; x++)
//        {
//            //Within each column, loop through y axis (rows).
//            for (int y = 1; y < rows - 1; y++)
//            {
//                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
//                gridPositions.Add(new Vector3(x, y, 0f));
//            }
//        }
//    }

//    //Sets up the outer walls and floor (background) of the game board.
//    void BoardSetup()
//    {
//        //Instantiate Board and set boardHolder to its transform.
//        boardHolder = new GameObject("Board").transform;
//        debugHolder = new GameObject("Debug").transform;
//        heroHolder = new GameObject("Hero").transform;
//        enemyHolder = new GameObject("Enemy").transform;
//        obstacleHolder = new GameObject("Obstacles").transform;

//        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
//        for (int x = -1; x < columns + 1; x++)
//        {
//            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
//            for (int y = -1; y < rows + 1; y++)
//            {
//                int cost = 1;
//                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
//                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

//                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
//                if (x == -1 || x == columns || y == -1 || y == rows)
//                {
//                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
//                    cost = 11;
//                }
//                else
//                {
//                    if (GameManager.Instance._debugMode)
//                    {
//                        if (pathTile != null)
//                        {
//                            GameObject curPathTile =
//                        Instantiate(pathTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
//                            PathStatusLayer script = curPathTile.GetComponent<PathStatusLayer>();
//                            script._isSet = true;
//                            script._X = x;
//                            script._Y = y;
//                            curPathTile.transform.SetParent(debugHolder);

//                            GameObject curDebugTile =
//                        Instantiate(debugTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
//                            TextureMeshLayer debugScript = curDebugTile.GetComponent<TextureMeshLayer>();
//                            debugScript._isSet = true;
//                            debugScript._X = x;
//                            debugScript._Y = y;
//                            curDebugTile.transform.SetParent(debugHolder);
//                        }
//                    }
//                }

//                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
//                GameObject instance =
//                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

//                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
//                instance.transform.SetParent(boardHolder);

//                Cell newCell = new Cell(x, y, cost);
//                GameManager.Instance._cells.Add(new Vector2(x, y), newCell);
//            }
//        }
//    }


//    //RandomPosition returns a random position from our list gridPositions.
//    Vector3 RandomPosition()
//    {
//        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
//        int randomIndex = Random.Range(0, gridPositions.Count);

//        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
//        Vector3 randomPosition = gridPositions[randomIndex];

//        //Remove the entry at randomIndex from the list so that it can't be re-used.
//        gridPositions.RemoveAt(randomIndex);

//        //Return the randomly selected Vector3 position.
//        return randomPosition;
//    }

//    /*
//     * Type:
//     * 0: Obstacle
//     * 1: Hero
//     * 2: Enemy
//     * */
//    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
//    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum, int type)
//    {
//        //Choose a random number of objects to instantiate within the minimum and maximum limits
//        int objectCount = Random.Range(minimum, maximum + 1);

//        //Instantiate objects until the randomly chosen limit objectCount is reached
//        for (int i = 0; i < objectCount; i++)
//        {
//            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
//            Vector3 randomPosition = RandomPosition();

//            //Choose a random tile from tileArray and assign it to tileChoice
//            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

//            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
//            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);

//            switch (type)
//            {
//                case 0:
//                    {
//                        instance.transform.SetParent(obstacleHolder);
//                    }
//                    break;
//                case 1:
//                    {
//                        instance.transform.SetParent(heroHolder);
//                        ChrController chr = instance.GetComponent<ChrController>();
//                        if (chr != null)
//                        {
//                            chr._MovementRange = Random.Range(3, 4);
//                            GameManager.Instance._heros.Add(chr);
//                        }
//                        RandomSpriteColor(instance, true);
//                    }
//                    break;
//                case 2:
//                    {
//                        instance.transform.SetParent(enemyHolder);
//                        ChrController chr = instance.GetComponent<ChrController>();
//                        if (chr != null)
//                        {
//                            chr._MovementRange = Random.Range(3, 4);
//                            GameManager.Instance._enemies.Add(chr);
//                        }
//                        RandomSpriteColor(instance, false);
//                    }
//                    break;
//            }
//            GameManager.Instance.ChangeCellCost((int)randomPosition.x, (int)randomPosition.y, type, 10);
//        }
//    }
//    //SetupScene initializes our level and calls the previous functions to lay out the game board
//    public void SetupScene(int level)
//    {
//        //Reset our list of gridpositions.
//        InitialiseList();

//        //Creates the outer walls and floor.
//        BoardSetup();

//        //Instantiate a random number of obstacle tiles based on minimum and maximum, at randomized positions.
//        LayoutObjectAtRandom(obstableTiles, obstableCount.minimum, obstableCount.maximum, 0);

//        //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
//        //LayoutObjectAtRandom(chestTiles, chestCount.minimum, chestCount.maximum);

//        //Determine number of enemies based on current level number, based on a logarithmic progression
//        //int enemyCount = GameManager.Instance._numEnemies;

//        //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
//        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount, 2);

//        //int heroCount = GameManager.Instance._numHeroes;

//        LayoutObjectAtRandom(heroTiles, heroCount, heroCount, 1);

//        //Instantiate the exit tile in the upper right hand corner of our game board
//        //Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
//    }

//    public void RandomSpriteColor(GameObject target, bool player)
//    {
//        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
//        if (sr != null)
//        {
//            if (player)
//                sr.color = new Color(Random.Range(0.0f, 1.0f), 0, Random.Range(0.0f, 1.0f));
//            else
//                sr.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0);
//        }
//    }
//}
