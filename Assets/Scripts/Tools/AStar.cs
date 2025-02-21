using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;

    public int x;
    public int y;
    public Vector3 pos;
    public int hCost;//当前点到终点的距离
    public int gCost;//起始点到当前点的距离
    public Node parent;

    public Node(int x, int y, bool walkable, Vector3 pos)
    {
        this.walkable = walkable;
        this.x = x;
        this.y = y;
        this.pos = pos;
    }

    public int fCost { get { return gCost + hCost; } }

}

public class Grid : MonoBehaviour
{
    public Transform player, target;
    Node playerNode, targetNode;
    public List<Node> path;
    public LayerMask unwalkbaleMesk;
    public Vector2 gridSizze;//当前网格尺寸
    public float nodeRadius;//节点的半径
    float nodeDiameter;//节点直径
    public int nodeNumX;//节点数量
    public int nodeNumY;//节点数量
    public Node[,] grid; //按节点定义的网格
    // Start is called before the first frame update
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        nodeNumX = Mathf.RoundToInt(gridSizze.x / nodeDiameter);
        nodeNumY = Mathf.RoundToInt(gridSizze.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[nodeNumX, nodeNumY];
        Vector3 startPos = transform.position - new Vector3(gridSizze.x / 2, 0, gridSizze.y / 2);
        for (int x = 0; x < nodeNumX; x++)
        {
            for (int y = 0; y < nodeNumY; y++)
            {
                Vector3 currentPos = startPos + new Vector3(x * nodeDiameter + nodeRadius, 0, y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(currentPos, nodeRadius, unwalkbaleMesk);//检查小圆球范围内是否有物体并且层是否为mask

                // print(walkable);
                grid[x, y] = new Node(x, y, walkable, currentPos);
            }
        }
    }
    /// <summary>
    /// 获取点所在的Node
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Node GetNodeFromPosition(Vector3 pos)
    {
        float percentX = (pos.x + gridSizze.x / 2) / gridSizze.x;
        float percentY = (pos.z + gridSizze.y / 2) / gridSizze.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt(percentX * (nodeNumX - 1));
        int y = Mathf.RoundToInt(percentY * (nodeNumY - 1));
        return grid[x, y];
    }

    /// <summary>
    /// 画格
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(gridSizze.x, 1, gridSizze.y));
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n.walkable)
                {
                    Gizmos.color = Color.grey;
                    if (GetNodeFromPosition(player.position) == n)
                    {
                        Gizmos.color = Color.red;
                    }
                    if (GetNodeFromPosition(target.position) == n)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    if (path != null && path.Contains(n))
                    {

                        Gizmos.color = Color.blue;

                    }
                    Gizmos.DrawCube(n.pos, new Vector3(nodeDiameter * 0.9f, 1, nodeDiameter * 0.9f));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}


public class PathFinding : MonoBehaviour
{
    Grid myGrid;

    Node startNode, targetNode;
    private void Awake()
    {
        myGrid = GetComponent<Grid>();
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        startNode = myGrid.GetNodeFromPosition(startPos);
        targetNode = myGrid.GetNodeFromPosition(targetPos);
        List<Node> openSet = new List<Node>();//开放节点   需要被评估的节点
        HashSet<Node> closeSet = new HashSet<Node>();//闭合节点   已经评估的节点
        openSet.Add(startNode);

        //如果小于0证明遍历完了  没有找到路径
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);//移除当前节点
            closeSet.Add(currentNode);//添加到闭合节点里
            if (currentNode == targetNode)
            {
                RetrievePath(currentNode);
                return;
            }
            foreach (Node n in GetNeighbors(currentNode))
            {
                if (!n.walkable || closeSet.Contains(n))
                    continue;
                int newgCost = currentNode.fCost + GetDistance(currentNode, n);
                bool inOpenSet = openSet.Contains(n);
                if (newgCost < n.gCost || !inOpenSet)
                {
                    n.gCost = newgCost;
                    n.hCost = GetDistance(n, targetNode);
                    n.parent = currentNode;
                    if (!inOpenSet)
                    {
                        openSet.Add(n);
                    }
                }
            }
        }
    }

    private void RetrievePath(Node n)
    {
        List<Node> p = new List<Node>();
        while (n != startNode)
        {
            p.Add(n);
            n = n.parent;
        }
        // p.Reverse();
        myGrid.path = p;
    }

    int GetDistance(Node n1, Node n2)
    {
        int distanceX = (int)Mathf.Abs(n1.x - n2.x);
        int distanceY = (int)Mathf.Abs(n1.y - n2.y);
        if (distanceX > distanceY)
        {
            return 14 * (distanceY) + 10 * (distanceX - distanceY);
        }
        else
        {
            return 14 * (distanceX) + 10 * (distanceY - distanceX);
        }
    }
    /// <summary>
    /// 找邻居节点
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private List<Node> GetNeighbors(Node n)
    {
        List<Node> neighbors = new List<Node>();
        int xx = n.x, yy = n.y;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (xx + x >= 0 && xx + x < myGrid.nodeNumX && yy + y >= 0 && yy + y < myGrid.nodeNumY)
                {
                    neighbors.Add(myGrid.grid[xx + x, yy + y]);
                }
            }
        }

        return neighbors;
    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FindPath(myGrid.player.position, myGrid.target.position);
    }
}


