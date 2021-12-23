using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManController : MonoBehaviour
{

    private Rigidbody2D rb;
    private TileController currentTile;                     //tile player is currently over
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float SnapTile;        //distance threshold for player snapping to center of tile 

    enum Direction
    {
        Up,
        Down,
        Left,
        Right, 
        None
    }

    private Direction currentDirection, nextDirection, pressedDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDirection = Direction.None;
        nextDirection = Direction.None;
        PlayerSpeed = 3f;
        SnapTile = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        //process player movement
        Movement();
    }

    private void Movement()
    {
        //handle player movement

        //grap open gate info of current tile (tile player is currently over)
        bool[] tileAvailableDirs = new bool[4];
        tileAvailableDirs[0] = currentTile.Up;
        tileAvailableDirs[1] = currentTile.Down;
        tileAvailableDirs[2] = currentTile.Left;
        tileAvailableDirs[3] = currentTile.Right;
       // tileAvailableDirs[4] = false;

        //collect pressed input
        pressedDirection = Direction.None;
        if (Input.GetAxis("Horizontal") < -0.3f)
            pressedDirection = Direction.Left;
        if (Input.GetAxis("Horizontal") > 0.3f)
            pressedDirection = Direction.Right;
        if (Input.GetAxis("Vertical") < -0.3f)
            pressedDirection = Direction.Down;
        if (Input.GetAxis("Vertical") > 0.3f)
            pressedDirection = Direction.Up;

        //set nextDirection
        if (pressedDirection != currentDirection)
            nextDirection = pressedDirection;


        if (currentDirection != Direction.None)
        {
            //if moving

            if (Vector3.Distance(rb.transform.position, currentTile.transform.position) <= SnapTile)
            {
                //if center of tile

                if (!tileAvailableDirs[(int)currentDirection])
                {
                    //if running into wall
                    StopPlayer();
                    //snap to tile center
                    rb.transform.position = currentTile.transform.position;

                    //if (tileAvailableDirs[(int)nextDirection])
                    //    ChangeDirection(nextDirection);
                }
                else
                {
                    if (nextDirection != Direction.None)
                    {
                        if (tileAvailableDirs[(int)nextDirection])
                            ChangeDirection(pressedDirection);
                    }
                }
            }
            else
            {
                //moving but not center of tile. only need to process reversal of direction
                if ((currentDirection == Direction.Up && nextDirection == Direction.Down) ||
                    (currentDirection == Direction.Down && nextDirection == Direction.Up) ||
                    (currentDirection == Direction.Right && nextDirection == Direction.Left) ||
                    (currentDirection == Direction.Left && nextDirection == Direction.Right))
                {
                    ChangeDirection(nextDirection);
                }
            }
        }
        else
        {
            //if stopped
            //if player not blocked in pressed direction by another player, and if direction is free on current tile, start moving in pressed direction
            if (pressedDirection != Direction.None)
            {
                if (tileAvailableDirs[(int)pressedDirection])
                    ChangeDirection(pressedDirection);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tile_tag")
            currentTile = collision.GetComponent<TileController>();
    }

    private void StopPlayer()
    {
        currentDirection = Direction.None;
        nextDirection = Direction.None;
        rb.velocity = new Vector2(0f,0f);
    }

    private void ChangeDirection(Direction d)
    {
        switch ((int) d)
        {
            case (int) Direction.Up:
                //go up
                rb.velocity = new Vector2(0f, PlayerSpeed);
                rb.rotation = 90f;
                break;
            case (int) Direction.Down:
                //
                rb.velocity = new Vector2(0f, -PlayerSpeed);
                rb.rotation = 270f;
                break;
            case (int) Direction.Left:
                //
                rb.velocity = new Vector2(-PlayerSpeed, 0f);
                rb.rotation = 180f;
                break;
            case (int) Direction.Right:
                //
                rb.velocity = new Vector2(PlayerSpeed, 0f);
                rb.rotation = 0f;
                break;
            default:
                break;
        }

        currentDirection = d;
        nextDirection = d;
    }
}
