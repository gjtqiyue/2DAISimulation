using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Path planner
 * A* pathfinding
 */
public class PathPlanner : MonoBehaviour {

    public static PathPlanner instance = null;

    public class Path
    {
        public int g;   //score for the next tile
        public int h;   //estimate cost to the target throw this tile
        public Path parent;
        public int x;   //x-coordinate
        public int y;   //y-coordinate

        public Path(int g, int h, Path parent, int x, int y)
        {
            this.g = g;
            this.h = h;
            this.parent = parent;
            this.x = x;
            this.y = y;
        }

        public int f()
        {
            return g + h;     // f = g + h
        }
    }

    private List<Vector3Int> happyPath = null;
    private Transform target;
    private Path nextMove;
    private Path currentTile;
    private Path previousPath;
    public Vector3 destination;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public List<Vector3Int> PlanPath(Transform from, Vector3 dest)
    {
        target = from;
        destination = dest;
        happyPath = Route(Vector3Int.RoundToInt(from.position), destination);
        //StartCoroutine(ExecutePath());
        return happyPath;
    }

    /*
     * chanse AI code
     * only activate when enter chase state
     *
     */
    private List<Path> ValidAdjTiles (Path p)
    {
        List<Path> adj = new List<Path> ();

        int x = p.x;
        int y = p.y;

        for (int i=-1; i<=1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int _x = x + i;
                int _y = y + j;

                // skip self and diagonal
                if ((i == 0 && j == 0) || (i != 0 && j != 0)){
                    continue;
                }
                
                // if there is no wall ahead or nothing blocking the path, add it to the open list
                else {
                    if (!WallAhead(new Vector2(x + 0.5f, y + 0.5f), new Vector2(_x + 0.5f + j*0.4f, _y + 0.5f + i*0.4f)))
                        if (!WallAhead(new Vector2(x + 0.5f, y + 0.5f), new Vector2(_x + 0.5f - j * 0.4f, _y + 0.5f - i * 0.4f)))
                            if (!WallAhead(new Vector2(x + 0.5f, y + 0.5f), new Vector2(_x + 0.5f, _y + 0.5f)))
                                adj.Add(new Path((p.g + 1), DistanceToTarget(_x, _y, destination), p, _x, _y));
                }       
            }
        }

        return adj;
    }

    private bool WallAhead (Vector2 start, Vector2 end)
    {
        //Debug.DrawLine(start, end, Color.green);
        RaycastHit2D hit;
        target.GetComponent<BoxCollider2D> ().enabled = false;
        hit = Physics2D.Raycast(start, end - start, 1f);
        target.GetComponent<BoxCollider2D>().enabled = true;

        if (hit.transform != null && (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.name == "Wall" || hit.transform.gameObject.tag == "Enemy" || hit.transform.gameObject.tag == "Player" ))
        {      
            return true;
        }
        else
            return false;
    }

    private int DistanceToTarget(int x, int y, Vector3 destination)
    {
        int estimateDistance = (int)(Mathf.Abs(destination.x - x) + Mathf.Abs(destination.y - y));

        return estimateDistance;
    }

    
    private Path TileWithLowestF (List<Path> list)
    {
        Path choice;
        
        int minF = int.MaxValue;
        int index = 0;
        for (int i=0; i<list.Count; i++)
        {
            
            if (list[i].f() <= minF)
            {
                minF = list[i].f();
                index = i;
            }
        }
        choice = list[index];

        return choice;
    }


    private bool Contain (List<Path> list, Path p)
    {
        for (int i=0; i<list.Count; i++)
        {
            if (list[i].x == p.x && list[i].y == p.y)
            return true;
        }
        return false;
    }
    
    private List<Vector3Int> Route(Vector3Int self, Vector3 dest)
    {
        int time = 0;
        Path startTile = new Path(0, DistanceToTarget(self.x, self.y, dest), null, (int)self.x, (int)self.y);
        nextMove = startTile;
        
        List<Path> openList = new List<Path>();             //tile that we can go 
        List<Path> closedList = new List<Path>();           //tile that we've passed
        openList.Add(startTile);

        while (openList.Count >= 1)
        {       
            currentTile = nextMove;
           
            //remove current tile from the openlist and add to the closed list
            closedList.Add(currentTile);
            openList.Remove(currentTile);

            //if closed list contains the destination tile then we arrived at the destination
            if (currentTile.x == destination.x && currentTile.y == destination.y)
            {
                return BuildPath();
            }
            
            List<Path> adjList = ValidAdjTiles(currentTile);

            // for all the adjancent tile in the list, if it's in the closed list (the path we come from) then we ignore it
            for (int i=0; i<adjList.Count; i++)
            {
                if (Contain(closedList, adjList[i]))
                    continue;

                if (!Contain(openList, adjList[i]))
                {
                    //if the openlist doesn't have this adjacent tile, we add it to the list
                    openList.Add(adjList[i]);
                }
                
            }
           

            if (openList.Count > 0)
                nextMove = TileWithLowestF(openList);
            else
                return null; //failed to find a path

            time++;
           
        }
        return null;

    }

    private List<Vector3Int> BuildPath()
    {
        
        List<Vector3Int> shortestPath = new List<Vector3Int>();
        Path current = currentTile;
        shortestPath.Insert(0, new Vector3Int(current.x, current.y, 0));
        currentTile = null;

        while (current.parent != null)
        {
            current = current.parent;
            shortestPath.Insert(0, new Vector3Int(current.x, current.y, 0));
            //Debug.Log(new Vector3(current.x, current.y));
            
        } 

        //shortestPath.Insert(0, new Vector3Int((int)target.position.x, (int)target.position.y, 0));
        Debug.Log(shortestPath.Count);
        return shortestPath;
    }

    //IEnumerator ExecutePath()
    //{
    //    if (happyPath != null)
    //    {
    //        index = 0;
    //        while (index != happyPath.Count)
    //        {
    //            Debug.DrawLine(transform.position+new Vector3(0.5f,0.5f), new Vector3(happyPath[index].x, happyPath[index].y) + new Vector3(0.5f, 0.5f), Color.black);
    //            transform.position = new Vector3(happyPath[index].x, happyPath[index].y);
    //            index++;
    //            yield return new WaitForSeconds(0.1f);
    //        }
    //    }
    //}
}
