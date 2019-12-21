using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PathfindingSheep : MonoBehaviour
{
    Grid grid;
    public Transform Wolf;
    public Transform Target;
    public float wolfSpeed = 1f;
    FSM_wolf fSM_Wolf;

    float timeToNextPathUpdate = 1.0f;
    List<Node> currentPath = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        grid = GetComponent<Grid>();
        //Vector3 targetPosition = Target.position;
        //    if (SceneManager.GetActiveScene().name == "PathFindingExample")
        //    {
        //        fenceGenerator.doneGenerating = true;
        //        targetPosition = new Vector3(0, 0, 35);
        //    }
            //Debug.Log(Wolf.position);
            PathFinder(Wolf.position, Target.position);
            timeToNextPathUpdate = 1.0f;


        if (currentPath != null)
        {
            MoveAlonePath(currentPath);
        }
    }

    void PathFinder(Vector3 start, Vector3 target)
    {
        // Vector3[] waypoints = new Vector3[0];
        // bool pathSuccess = false;
        List<Node> explored = new List<Node>();
        HashSet<Node> pathlist = new HashSet<Node>();
        Node Startnode = grid.WorldToNode(start);
        Node Endnode = grid.WorldToNode(target);
        //Startnode.setFcost(0);
        //Startnode.setGcost(0);
        //Startnode.setHcost(0);
        explored.Add(Startnode);
        while (explored.Count > 0)
        {
            Node current = getSmallet(explored);
            //Debug.unityLogger.Log("Current: " + current.GetHashCode() + "," + Endnode.GetHashCode());
            explored.Remove(current);
            pathlist.Add(current);
            if (current.Equals(Endnode))
            {
                //pathSuccess = true;
                getFinalPath(Startnode, Endnode);
                break;
            }
            List<Node> neighbours = findneighbours(current);
            foreach (Node neighbour in neighbours)
            {
                if (neighbour.getIsobstacle())
                {
                    continue;
                }
                float newHcost = GetDistance(Endnode, neighbour);
                float newGcost = current.getGcost() + GetDistance2(current, neighbour);
                float newFcost = newHcost + newGcost;
                if (pathlist.Contains(neighbour) && newGcost >= neighbour.Gcost)
                {
                    continue;
                }
                if (!explored.Contains(neighbour) || newGcost < neighbour.Gcost)
                {
                    neighbour.setHcost(newHcost);
                    neighbour.setGcost(newGcost);
                    neighbour.setFcost(newHcost + newGcost);
                    neighbour.setparent(current);
                    if (!explored.Contains(neighbour))
                    {
                        explored.Add(neighbour);
                    }
                    else
                    {
                        explored.Remove(neighbour);
                        explored.Add(neighbour);
                    }
                }
            }
            //if (pathSuccess)
            //{
            //    waypoints = RetracePath(Startnode, Endnode);
            //}
            //OnPathFound(waypoints, pathSuccess);
        }
        //Debug.Log(pathlist.Count);

        List<Node> findneighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int newx = node.getx() + i;
                    int newy = node.gety() + j;
                    if (newx < grid.GridSizeX && newx >= 0 && newy < grid.GridSizeY && newy >= 0)
                    {
                        neighbours.Add(grid.grid[newx, newy]);
                    }
                }
            }
            return neighbours;
        }

        void getFinalPath(Node s, Node e)
        {
            string print = "";
            List<Node> finalpath = new List<Node>();
            Node tem = e;
            while (tem != s)
            {
                finalpath.Add(tem);
                tem = tem.getparent();
            }
            finalpath.Reverse();
            foreach (Node fn in finalpath)
            {
                print = print + fn.getx() + "," + fn.gety() + "->";
            }
            //Debug.unityLogger.Log("Final: " + print);
            currentPath = finalpath;
        }

        Node getSmallet(List<Node> tobesorted)
        {
            Node n = tobesorted[0];
            foreach (Node nn in tobesorted)
            {
                if (nn.getFcost() < n.getFcost())
                {
                    n = nn;
                }
                else if (Mathf.RoundToInt(nn.getFcost()) == Mathf.RoundToInt(n.getFcost()))
                {
                    if (nn.getHcost() < n.getHcost())
                    {
                        n = nn;
                    }
                }
            }
            return n;
        }



        //Vector3[] RetracePath(Node startNode, Node endNode)
        //{
        //    List<Node> path = new List<Node>();
        //    Node currentNode = endNode;

        //    while (currentNode != startNode)
        //    {
        //        path.Add(currentNode);
        //        currentNode = currentNode.getparent();
        //    }
        //    Vector3[] waypoints = SimplifyPath(path);
        //    Array.Reverse(waypoints);
        //    string moveprint = "";
        //    foreach (Vector3 pathvec in waypoints)
        //    {
        //        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        cube.transform.position = pathvec;
        //        moveprint = moveprint + pathvec.x + "," + pathvec.z + "->";
        //        Wolf.position = Vector3.MoveTowards(Wolf.position, pathvec, 1 * Time.deltaTime);
        //        Debug.unityLogger.Log(moveprint);
        //    }
        //    return waypoints;

        //}

        //Vector3[] SimplifyPath(List<Node> path)
        //{
        //    List<Vector3> waypoints = new List<Vector3>();
        //    Vector2 directionOld = Vector2.zero;

        //    for (int i = 1; i < path.Count; i++)
        //    {
        //        Vector2 directionNew = new Vector2(path[i - 1].getx() - path[i].getx(), path[i - 1].gety() - path[i].gety());
        //        if (directionNew != directionOld)
        //        {
        //            waypoints.Add(path[i].getWorldPosition());
        //        }
        //        directionOld = directionNew;
        //    }
        //    return waypoints.ToArray();
        //}

        //public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        //{
        //    if (pathSuccessful)
        //    {
        //        path = newPath;
        //        targetIndex = 0;
        //        StopCoroutine("FollowPath");
        //        StartCoroutine("FollowPath");
        //    }
        //}

        //IEnumerator FollowPath()
        //{
        //    Vector3 currentWaypoint = path[0];
        //    while (true)
        //    {
        //        if (transform.position == currentWaypoint)
        //        {
        //            targetIndex++;
        //            if (targetIndex >= path.Length)
        //            {
        //                yield break;
        //            }
        //            currentWaypoint = path[targetIndex];
        //        }

        //        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
        //        yield return null;

        //    }
        //}

        //internal class NodeComparer : IComparer<Node>
        //{
        //    public int Compare(Node x, Node y)
        //    {
        //        //first by age
        //        int result = x.getFcost().CompareTo(y.getFcost());

        //        return result;
        //    }
        //}

        //int GetDistance(Node nodeA, Node nodeB)
        //{
        //    int dstX = Mathf.Abs(nodeA.getx() - nodeB.gety());
        //    int dstY = Mathf.Abs(nodeA.gety() - nodeB.gety());

        //    if (dstX > dstY)
        //        return 14 * dstY + 10 * (dstX - dstY);
        //    return 14 * dstX + 10 * (dstY - dstX);
        //}

        float GetDistance2(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.getx() - nodeB.getx());
            int dstY = Mathf.Abs(nodeA.gety() - nodeB.gety());

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        float GetDistance(Node nodeA, Node nodeB)
        {
            Vector3 vescost = nodeA.getWorldPosition() - nodeB.getWorldPosition();
            return vescost.magnitude;
        }
    }

    Node getNodeWithLineOfSight(List<Node> GridPath, Vector3 position)
    {
        Node best = null;
        Vector2 pos = new Vector2(position.x, position.z);
        int c = 0;
        foreach (Node node in GridPath)
        {
            c++;
            Vector2 target = new Vector2(node.getx() + Grid.LeftBottom.x, node.gety() + Grid.LeftBottom.z);
            Vector2 widthFactor = new Vector2(0, 0);
            widthFactor.x = -(target - pos).y;
            widthFactor.y = (target - pos).x;
            widthFactor.Normalize();
            widthFactor *= .4f;
            if (flocking.canIWalkLine(pos, target) && 
                flocking.canIWalkLine(pos + widthFactor, target + widthFactor) && 
                flocking.canIWalkLine(pos - widthFactor, target - widthFactor))
            {
                best = node;
            }
        }
        return best;
    }

    void MoveAlonePath(List<Node> GridPath)
    {
        if (GridPath.Count > 1)
        {
            Node targetNode = getNodeWithLineOfSight(GridPath, Wolf.position);
            if (targetNode == null)
            {
                return;
            }
            Vector3 tempTarget = new Vector3(targetNode.getx(), 0, targetNode.gety());
            tempTarget += Grid.LeftBottom;
            tempTarget -= Wolf.position;

            float angle = Wolf.eulerAngles.y;
            angle = angle / 180 * Mathf.PI;

            float angleToTarget = Mathf.Atan2(tempTarget.x, tempTarget.z) - angle;
            while (angleToTarget > Mathf.PI)
            {
                angleToTarget -= Mathf.PI * 2;
            }
            while (angleToTarget < -Mathf.PI)
            {
                angleToTarget += Mathf.PI * 2;
            }

            Vector3 angularVelocity = new Vector3(0, 0, 0);
           
            if (angleToTarget > 0)
            {
                angularVelocity = new Vector3(0, Mathf.PI, 0);
            }
            else
            {
                angularVelocity = new Vector3(0, -Mathf.PI, 0);
            }
            if (Mathf.Abs(angleToTarget) < flocking.MAX_ANGLE_TO_TARGET_WITH_FULL_ANGULAR_SPEED)
            {
                float angleSpeedFactor = Mathf.Abs(angleToTarget) / flocking.MAX_ANGLE_TO_TARGET_WITH_FULL_ANGULAR_SPEED;
                angularVelocity *= angleSpeedFactor;
            }

            //WolfObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;


            Vector3 vel = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            vel.Normalize();
            //Debug.Log(vel);

            //Debug.Log(WolfObject.GetComponent<Rigidbody>().velocity);
            //WolfObject.GetComponent<Rigidbody>().AddForce(vel * wolfSpeed * 200);// = vel * 10;
            //Debug.Log(WolfObject.GetComponent<Rigidbody>().velocity);
        }
    }
}
