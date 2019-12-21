using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //Set a nodesize (diameter)
    public float nodesize;
    public LayerMask Obstacle;
    public float GroundSizeX;
    public float GroundSizeY;
    public int GridSizeX, GridSizeY;
    public static Vector3 LeftBottom;
    GameObject ground;
    public Node[,] grid;
    public bool gridexists;

    // Start is called before the first frame update
    void Start()
    {
        //ground = GameObject.Find("Ground");
        //GroundSizeX = ground.GetComponent<Transform>().localScale.x * 10;
        //GroundSizeY = ground.GetComponent<Transform>().localScale.z * 10;
        ////Ground size devides node size to get the grid size
        //GridSizeX = Mathf.RoundToInt(GroundSizeX / nodesize);
        //GridSizeY = Mathf.RoundToInt(GroundSizeY / nodesize);
        ////When the grid start from (0,0), the start posision of the ground should be the left-bottom-most position
        ////To calculate left-bottom position, find the center position of the ground first,
        ////then move left for have size of the ground.x, and move down for half size of the ground.y
        //LeftBottom = ground.GetComponent<Transform>().position + Vector3.left * (GroundSizeX / 2) + Vector3.back * (GroundSizeY / 2);
        ////Debug.unityLogger.Log("LeftX:" + LeftBottom.x + "LeftY" + LeftBottom.z);
        ////After calculate grid size, initialize grid
        //InitGrid();
    }

    // Update is called once per frame
    float t = 5.0f;
    void Update()
    {
        t -= Time.deltaTime;
        if (fenceGenerator.doneGenerating && !gridexists && t < 0)
        {
            ground = GameObject.Find("Ground");
            GroundSizeX = ground.GetComponent<Transform>().localScale.x * 10;
            GroundSizeY = ground.GetComponent<Transform>().localScale.z * 10;
            //Ground size devides node size to get the grid size
            GridSizeX = Mathf.RoundToInt(GroundSizeX / nodesize);
            GridSizeY = Mathf.RoundToInt(GroundSizeY / nodesize);
            //When the grid start from (0,0), the start posision of the ground should be the left-bottom-most position
            //To calculate left-bottom position, find the center position of the ground first,
            //then move left for have size of the ground.x, and move down for half size of the ground.y
            LeftBottom = ground.GetComponent<Transform>().position - Vector3.right * (GroundSizeX / 2) - Vector3.forward * (GroundSizeY / 2);
            //After calculate grid size, initialize grid
            InitGrid();
        }
    }

    //Method to convert grid position to ground position
    //public Vector3 NodeToWorld(Node node)
    //{
    //    float WorldX = node.getx() * nodesize+LeftBottom.x;
    //    float WorldY = node.gety() * nodesize+LeftBottom.y;
    //    Vector3 World = LeftBottom + Vector3.right * (WorldX * nodesize) + Vector3.forward * (WorldY * nodesize);
    //    return World;
    //}

    //Method to convert ground positoin to grid position
    public Node WorldToNode(Vector3 world)
    {
        float VectorXMove, VectorYMove;
        VectorXMove = world.x - LeftBottom.x;
        VectorYMove = world.z - LeftBottom.z;
        int GridX = Mathf.RoundToInt(VectorXMove/nodesize);
        int GridY= Mathf.RoundToInt(VectorYMove / nodesize);
        //Debug.unityLogger.Log("Gridx:" + GridX + "GridY" + GridY);
        return grid[GridX, GridY];
    }

    void InitGrid()
    {
        //Using 2d array to implement grid
        grid = new Node[GridSizeX, GridSizeY];
        //The autual position on the ground will moving with the increasing of both x and y of the grid
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                Vector3 ActuralPosition = LeftBottom + Vector3.right * (i * nodesize) + Vector3.forward * (j * nodesize);
                bool isobstacle = Physics.CheckSphere(ActuralPosition, nodesize * 1.3f, Obstacle);
                Node cell = new Node(i, j,ActuralPosition,isobstacle);
                grid[i, j] = cell;
            }
        }
        gridexists = true;
    }
}
