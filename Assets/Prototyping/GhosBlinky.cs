using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhosBlinky : MonoBehaviour
{

    private Rigidbody2D rb;
    private Direction currentDirection;
    private TileController currentTile;                     //tile ghost is currently over
    private GhostState ghostState;
    private Vector3 chaseTile, scatterTile;                 //tile ghost is navigating to in chase or scatter state
    private float snapTile;
    private Vector3 turnTile;
    private bool newTile;

    public float GhostSpeed = 1.8f;
    bool[] tileAvailableDirs;
    float[] targetDistances;
    public GameObject PacMan, ScatterTarget;                               //pac man player object that this ghost will chase

    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    enum GhostState
    {
        Chase,
        Scatter,
        Frightened
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //initialize ghost set to move right
        rb.velocity = new Vector2(GhostSpeed, 0f);
        currentDirection = Direction.Right;
        ghostState = GhostState.Chase;
        //grap open gate info of current tile (tile ghost is currently over)
        tileAvailableDirs = new bool[4];
        targetDistances = new float[4];
        snapTile = 0.05f;
        turnTile = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        newTile = false;

        //TESTING
        //set target tiles for chase and scatter
       // chaseTile = new Vector3(0f,0f, 0f);
        chaseTile = PacMan.transform.position;
        scatterTile = ScatterTarget.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        //State machine for ghost
        switch ((int) ghostState)
        {
            case 0:
                //if at center of tile and current tile has more than 2 gates, pick the one that has lowest Euclidian distance to target tile
                if (Vector3.Distance(rb.transform.position, currentTile.transform.position) <= snapTile && newTile)
                {
                    chaseTile = PacMan.transform.position;
                    //remove opposite direction from available gates
                    //int oppDir = (int)OppositeDirection(currentDirection);
                    //tileAvailableDirs[oppDir] = false;
                    if (currentDirection == Direction.Up)
                        tileAvailableDirs[1] = false;
                    if (currentDirection == Direction.Down)
                        tileAvailableDirs[0] = false;
                    if (currentDirection == Direction.Left)
                        tileAvailableDirs[3] = false;
                    if (currentDirection == Direction.Right)
                        tileAvailableDirs[2] = false;

                    //count available gates on current tile
                    int gateCount = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (tileAvailableDirs[i] == true)
                            gateCount++;
                    }

                    if (gateCount > 1)
                    {
                        //decide which turn to make based on Euclidian distance to target tile. Cannot reverse direction

                        //get distances to target tile from all four gates
                        targetDistances[0] = Vector3.Distance(chaseTile, new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 1, 0f));
                        targetDistances[1] = Vector3.Distance(chaseTile, new Vector3(currentTile.transform.position.x, currentTile.transform.position.y - 1, 0f));
                        targetDistances[2] = Vector3.Distance(chaseTile, new Vector3(currentTile.transform.position.x - 1, currentTile.transform.position.y, 0f));
                        targetDistances[3] = Vector3.Distance(chaseTile, new Vector3(currentTile.transform.position.x + 1, currentTile.transform.position.y, 0f));



                        float minDist = float.MaxValue;
                        int minDirection = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            if (tileAvailableDirs[i] == false)
                                targetDistances[i] = 0f;

                            if (targetDistances[i] < minDist && targetDistances[i] > 0f)
                            {
                                minDist = targetDistances[i];
                                minDirection = i;
                            }
                                
                        }

                        ////center on tile then change direction
                        if (turnTile != rb.transform.position)
                        {
                            turnTile = rb.transform.position;
                            rb.transform.position = currentTile.transform.position;
                            ChangeDirection((Direction)minDirection);
                        }
                        
                        //newTile = false;
                    }
                    else
                    {
                        //if only 2 gates, exit the opposite one you came in
                        for (int j = 0; j < 4; j++)
                        {
                            if (tileAvailableDirs[j] == true)
                            {
                                rb.transform.position = currentTile.transform.position;
                                ChangeDirection(DirectionConvert(j));
                                Debug.Log("turning: " + j.ToString());
                            }
                        }
                    }
                    newTile = false;
                }
                    break;
            case 1:
                //
                break;
            case 2:
                //
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tile_tag")
        {
            currentTile = collision.GetComponent<TileController>();
            tileAvailableDirs[0] = currentTile.Up;
            tileAvailableDirs[1] = currentTile.Down;
            tileAvailableDirs[2] = currentTile.Left;
            tileAvailableDirs[3] = currentTile.Right;
            newTile = true;

            //switch to new tile for calcs
            //if(tileName != collision.name)
            //{
            //    tileName = collision.name;
            //    newTile = true;
            //}
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    //newTile = true;
    //}

    private Direction DirectionConvert(int i)
    {
        Direction d = Direction.Up;
        if(i == 1)
            d = Direction.Down;
        if (i == 2)
            d =  Direction.Left;
        if (i == 3)
            d =  Direction.Right;
        return d;
    }

    private void ChangeDirection(Direction d)
    {
        switch ((int)d)
        {
            case (int)Direction.Up:
                //go up
                rb.velocity = new Vector2(0f, GhostSpeed);
                break;
            case (int)Direction.Down:
                //
                rb.velocity = new Vector2(0f, -GhostSpeed);
                break;
            case (int)Direction.Left:
                //
                rb.velocity = new Vector2(-GhostSpeed, 0f);
                break;
            case (int)Direction.Right:
                //
                rb.velocity = new Vector2(GhostSpeed, 0f);
                break;
            default:
                break;
        }

        currentDirection = d;
    }

    private Direction OppositeDirection(Direction d)
    {
        //function returns the opposite passed to it
        int intDir = (int)d;
        if (intDir % 2 > 0)
            return (Direction)(intDir - 1);
        else
            return (Direction)(intDir + 1);
    }
}
