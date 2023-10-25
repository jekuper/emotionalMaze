using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using TMPro;
public class Node {
    public bool rightDoor = true;
    public bool leftDoor = true;
    public bool topDoor = true;
    public bool BottomDoor = true;
    public SpriteRenderer leftWall = null, rightWall = null, topWall = null, bottomWall = null;
    public bool hasCamp = false;
    public bool hasStar = false;
    public bool hasKey = false;
    public bool isPartOfEnemyPath = false;

    public static Vector2 getPostion(int i, int j) {
        return new Vector2(j * mazeManager.cellSize - (float)mazeManager.cellSize / 2, i * mazeManager.cellSize - (float)mazeManager.cellSize / 2);
    }
    public static Vector2Int getIndexes(Vector2 pos) {
        return new Vector2Int((int)((pos.y + (float)mazeManager.cellSize / 2) / mazeManager.cellSize), (int)((pos.x + (float)mazeManager.cellSize / 2) / mazeManager.cellSize));
    }
    public static Vector2 leftEdge(int i, int j) {
        Vector2 pos = getPostion(i, j);
        return new Vector2(pos.x - (float)mazeManager.cellSize / 2, pos.y);
    }
    public static Vector2 rightEdge(int i, int j) {
        Vector2 pos = getPostion(i, j);
        return new Vector2(pos.x + (float)mazeManager.cellSize / 2, pos.y);
    }
    public static Vector2 bottomEdge(int i, int j) {
        Vector2 pos = getPostion(i, j);
        return new Vector2(pos.x, pos.y - (float)mazeManager.cellSize / 2);
    }
    public static Vector2 topEdge(int i, int j) {
        Vector2 pos = getPostion(i, j);
        return new Vector2(pos.x, pos.y + (float)mazeManager.cellSize / 2);
    }
}
public class mazeManager : MonoBehaviour
{
    public int seed = -1;
    public Vector2Int mazeSize = new Vector2Int(50, 50); 
    public GameObject horWall, vertWall, star, camp, key, sEnemy, tEnemy;
    public TextMeshProUGUI seedText;
    public Transform mazeContainer;
    public int starCount = 5;
    public int campCount = 4;
    public int enemiesCount = 4;
    public static int cellSize = 3;
    public GameObject[] moveCenter;
    public Transform background;
    public Transform rightTop;
    public List<List<float>> mx;

    List<List<Node>> graph;
    List<List<int>> us;
    void Start()
    {
        starCount = Globals.mainSettings.starCount;
        campCount = Globals.mainSettings.campCount;
        enemiesCount = Globals.mainSettings.enemiesCount;


        horWall.GetComponent<SpriteRenderer>().size = new Vector2(cellSize, horWall.GetComponent<SpriteRenderer>().size.y);
        vertWall.GetComponent<SpriteRenderer>().size = new Vector2(vertWall.GetComponent<SpriteRenderer>().size.x, cellSize);
        key.transform.localScale = new Vector3((float)cellSize / 2, (float)cellSize / 2, 1);
        if (seed == -1) {
            seed = (int)DateTime.UtcNow.Ticks;
        }
        UnityEngine.Random.InitState(seed);
        Debug.Log("SEED = "+seed);
        seedText.text = seed.ToString();


        graph = new List<List<Node>>(new List<Node>[mazeSize.x + 1]);
        us = new List<List<int>>(new List<int>[mazeSize.x + 1]);
        for (int i = 1; i <= mazeSize.x; i++) {
            graph[i] = new List<Node>(new Node[mazeSize.y + 1]);
            us[i] = new List<int>(new int[mazeSize.y + 1]);
            for (int j = 1; j <= mazeSize.y; j++) {
                us[i][j] = 0;
                graph[i][j] = new Node();
            }
        }
        
        foreach (GameObject item in moveCenter) {
            item.transform.position = new Vector3((float)mazeSize.x / 2 * cellSize, (float)mazeSize.y / 2 * cellSize, item.transform.position.z);
        }
        transform.position = new Vector3((float)mazeSize.x / 2 * cellSize + cellSize / 2, (float)mazeSize.y / 2 * cellSize, transform.position.z);
        background.GetComponent<SpriteRenderer>().size = new Vector2(mazeSize.x * cellSize, mazeSize.y * cellSize);
        rightTop.position = new Vector3(mazeSize.x * cellSize, mazeSize.y * cellSize, rightTop.position.z);

        dfs(1, 1);
        ClearMazeArea(new Vector2Int(mazeSize.x / 2, mazeSize.y / 2), new Vector2Int(mazeSize.x / 2 + 2, mazeSize.y / 2 + 2));
        Debug.Log("maze has been built");
        placeKey();
        Debug.Log("keys have been placed");
        placeCamps(campCount, mazeSize.x);
        Debug.Log("camps have been placed");
        mx = getMaxArray(mazeSize.x / 2 + 1, mazeSize.y / 2 + 1);
        placeShootingEnemies(enemiesCount, 20);
        Debug.Log("shottings have been placed");
        placeTargetingEnemies(enemiesCount / 2);
        Debug.Log("targetings have been placed");
        placeStars(starCount, mazeSize.x);
        Debug.Log("stars have been placed");
        buildMaze();
        Debug.Log("finished");
        GetComponent<movements>().UpdateScore();
    }
    private void dfs(int i, int j) {
        if (us[i][j] == 1)
            return;
        us[i][j] = 1;
        List<int> available = new List<int>();
        if (i - 1 > 0) {
            available.Add(3);
        }
        if (j - 1 > 0) {
            available.Add(4);
        }
        if (i + 1 <= mazeSize.x) {
            available.Add(1);
        }
        if (j + 1 <= mazeSize.y) {
            available.Add(2);
        }
        available.Shuffle();
        foreach (int item in available) {
            Vector2Int to = new Vector2Int();
            if (item == 1) {
                to = new Vector2Int(i + 1, j);
            }
            if (item == 2) {
                to = new Vector2Int(i, j + 1);
            }
            if (item == 3) {
                to = new Vector2Int(i - 1, j);
            }
            if (item == 4) {
                to = new Vector2Int(i, j - 1);
            }
            if (us[to.x][to.y] == 0) {
                if (item == 1) {
                    graph[i][j].topDoor = false;
                    graph[to.x][to.y].BottomDoor = false;
                }
                if (item == 2) {
                    graph[i][j].rightDoor = false;
                    graph[to.x][to.y].leftDoor = false;
                }
                if (item == 3) {
                    graph[i][j].BottomDoor = false;
                    graph[to.x][to.y].topDoor = false;
                }
                if (item == 4) {
                    graph[i][j].leftDoor = false;
                    graph[to.x][to.y].rightDoor = false;
                }
                dfs(to.x, to.y);
            }
        }
    }
    private void buildMaze() {
        for(int i = 1; i <= mazeSize.x; i++) {
            for(int j = 1; j <= mazeSize.y; j++) {
                if (i == 1) {
                    if (j - 1 > 0 && graph[i][j - 1].BottomDoor) {
                        SpriteRenderer wall = graph[i][j - 1].bottomWall;
                        graph[i][j - 1].bottomWall = null;
                        wall.size = new Vector2(wall.size.x + cellSize, wall.size.y);
                        wall.transform.position = new Vector3(wall.transform.position.x + (float)cellSize / 2, wall.transform.position.y, wall.transform.position.z);
                        graph[i][j].bottomWall = wall;
                    }
                    else {
                        graph[i][j].bottomWall = Instantiate(horWall, Node.bottomEdge(i, j), new Quaternion(), mazeContainer).GetComponent<SpriteRenderer>();
                    }
                }
                if (j == 1) {
                    if(i - 1 > 0 && graph[i - 1][j].leftDoor) {
                        SpriteRenderer wall = graph[i - 1][j].leftWall;
                        graph[i - 1][j].leftWall = null;
                        wall.size = new Vector2(wall.size.x, wall.size.y + cellSize);
                        wall.transform.position = new Vector3(wall.transform.position.x, wall.transform.position.y + (float)cellSize / 2, wall.transform.position.z);
                        graph[i][j].leftWall = wall;
                    }
                    else {
                        graph[i][j].leftWall = Instantiate(vertWall, Node.leftEdge(i, j), new Quaternion(), mazeContainer).GetComponent<SpriteRenderer>();
                    }
                }
                if (graph[i][j].topDoor) {
                    if (j - 1 > 0 && graph[i][j - 1].topDoor) {
                        SpriteRenderer wall = graph[i][j - 1].topWall;
                        graph[i][j - 1].topWall = null;
                        wall.size = new Vector2(wall.size.x + cellSize, wall.size.y);
                        wall.transform.position = new Vector3(wall.transform.position.x + (float)cellSize / 2, wall.transform.position.y, wall.transform.position.z);
                        graph[i][j].topWall = wall;
                    }
                    else {
                        graph[i][j].topWall = Instantiate(horWall, Node.topEdge(i, j), new Quaternion(), mazeContainer).GetComponent<SpriteRenderer>();
                    }
                }
                if (graph[i][j].rightDoor) {
                    if (i - 1 > 0 && graph[i - 1][j].rightDoor) {
                        SpriteRenderer wall = graph[i - 1][j].rightWall;
                        graph[i - 1][j].rightWall = null;
                        wall.size = new Vector2(wall.size.x, wall.size.y + cellSize);
                        wall.transform.position = new Vector3(wall.transform.position.x, wall.transform.position.y + (float)cellSize / 2, wall.transform.position.z);
                        graph[i][j].rightWall = wall;
                    }
                    else {
                        graph[i][j].rightWall = Instantiate(vertWall, Node.rightEdge(i, j), new Quaternion(), mazeContainer).GetComponent<SpriteRenderer>();
                    }
                }
            }
        }
        for (int i = 1; i <= mazeSize.x; i++) {
            for (int j = 1; j <= mazeSize.y; j++) {
                graph[i][j].leftWall = null;
                graph[i][j].rightWall = null;
                graph[i][j].topWall = null;
                graph[i][j].bottomWall = null;
            }
        }
    }
    /*  
      private List<Vector2Int> getPath(int stx, int sty, int enx, int eny) {
          List<Vector2Int> result = new List<Vector2Int>();
          List<List<int>> usL;
          usL = new List<List<int>>(new List<int>[mazeSize.x + 1]);
          for (int i = 1; i <= mazeSize.x; i++) {
              usL[i] = new List<int>(new int[mazeSize.y + 1]);
              for (int j = 1; j <= mazeSize.y; j++) {
                  usL[i][j] = 0;
              }
          }
          Dictionary<Vector2Int, Vector2Int> p = new Dictionary<Vector2Int, Vector2Int>();
          Queue<Pair<Vector2Int, Vector2Int>> q = new Queue<Pair<Vector2Int, Vector2Int>>();
          p[new Vector2Int(stx, sty)] = new Vector2Int(stx, sty);
          q.Enqueue(new Pair<Vector2Int, Vector2Int>(new Vector2Int(stx, sty), new Vector2Int(stx, sty)));
          while (q.Count != 0) {
              Pair<Vector2Int, Vector2Int> e = q.Peek();
              Vector2Int v = e.First;
              Vector2Int curP = e.Second;
              q.Dequeue();
              if(usL[v.x][v.y] != 0) {
                  continue;
              }
              usL[v.x][v.y] = 1;
              p[v] = curP;
              if (v.x == enx && v.y == eny) {
  //                Debug.Log("path found");
                  break;
              }
              Vector2Int Top = new Vector2Int(v.x + 1, v.y);
              Vector2Int Bottom = new Vector2Int(v.x - 1, v.y);
              Vector2Int Left = new Vector2Int(v.x, v.y - 1);
              Vector2Int Right = new Vector2Int(v.x, v.y + 1);
              if (!graph[v.x][v.y].topDoor && Top != curP) {
                  q.Enqueue(new Pair<Vector2Int, Vector2Int>(Top, v));
              }
              if (!graph[v.x][v.y].BottomDoor && Bottom != curP) {
                  q.Enqueue(new Pair<Vector2Int, Vector2Int>(Bottom, v));
              }
              if (!graph[v.x][v.y].leftDoor && Left != curP) {
                  q.Enqueue(new Pair<Vector2Int, Vector2Int>(Left, v));
              }
              if (!graph[v.x][v.y].rightDoor && Right != curP) {
                  q.Enqueue(new Pair<Vector2Int, Vector2Int>(Right, v));
              }
          }

          for(Vector2Int i = new Vector2Int(enx, eny); i != new Vector2Int(stx, sty); i = p[i]) {
  //            Debug.Log(i);
              result.Add(i);

          }
          result.Add(new Vector2Int(stx, sty));
          result.Reverse();
          return result;
      }
      */
    private List<List<float>> getMaxArray2(int stx, int sty) {

        List<List<float>> d = new List<List<float>>(new List<float>[mazeSize.x + 1]);
        int INF = 1000000;
        for (int i = 1; i <= mazeSize.x; i++) {
            d[i] = new List<float>(new float[mazeSize.y + 1]);
            for (int j = 1; j <= mazeSize.y; j++) {
                d[i][j] = INF;
            }
        }
        d[stx][sty] = 0;

        List<Pair<float, Vector2Int>> s = new List<Pair<float, Vector2Int>>();
        s.Add(new Pair<float, Vector2Int>(0, new Vector2Int(stx, sty)));
        while(s.Count != 0)
        {
            s = s.OrderBy(o => o.First).ToList();
            Vector2Int v = new Vector2Int(s[0].Second.x, s[0].Second.y);
            s.RemoveAt(0);

            if (!graph[v.x][v.y].topDoor)
            {
                Vector2Int pos = new Vector2Int(v.x + 1, v.y);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    s.Add(new Pair<float, Vector2Int>(d[pos.x][pos.y], pos));
                }
            }
            if (!graph[v.x][v.y].BottomDoor)
            {
                Vector2Int pos = new Vector2Int(v.x - 1, v.y);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    s.Add(new Pair<float, Vector2Int>(d[pos.x][pos.y], pos));
                }
            }

            if (!graph[v.x][v.y].leftDoor)
            {
                Vector2Int pos = new Vector2Int(v.x, v.y - 1);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    s.Add(new Pair<float, Vector2Int>(d[pos.x][pos.y], pos));
                }
            }
            if (!graph[v.x][v.y].rightDoor)
            {
                
                Vector2Int pos = new Vector2Int(v.x, v.y + 1);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    s.Add(new Pair<float, Vector2Int>(d[pos.x][pos.y], pos));
                }
            }
        }

        
        return d;
    }
    private List<List<float>> getMaxArray(int stx, int sty)
    {

        List<List<float>> d = new List<List<float>>(new List<float>[mazeSize.x + 1]);
        List<List<Vector2Int>> p = new List<List<Vector2Int>>(new List<Vector2Int>[mazeSize.x + 1]);
        float INF = 100000000;
        List<List<int>> u = new List<List<int>>(new List<int>[mazeSize.x + 1]);
        for (int i = 1; i <= mazeSize.x; i++)
        {
            d[i] = new List<float>(new float[mazeSize.y + 1]);
            p[i] = new List<Vector2Int>(new Vector2Int[mazeSize.y + 1]);
            u[i] = new List<int>(new int[mazeSize.y + 1]);
            for (int j = 1; j <= mazeSize.y; j++)
            {
                d[i][j] = INF;
                u[i][j] = 0;
            }
        }
        d[stx][sty] = 0;
        for (int i = 1; i < mazeSize.x * mazeSize.y; i++)
        {
            Vector2Int v = new Vector2Int(-1, -1);
            for (int j = 1; j <= mazeSize.x; j++)
            {
                for (int k = 1; k <= mazeSize.y; k++)
                {
                    if (u[j][k] == 0 && (v.x == -1 || d[j][k] < d[v.x][v.y]))
                    {
                        v = new Vector2Int(j, k);
                    }
                }
            }

            if (d[v.x][v.y] == INF)
            {
                break;
            }
            u[v.x][v.y] = 1;

            if (!graph[v.x][v.y].topDoor)
            {
                Vector2Int pos = new Vector2Int(v.x + 1, v.y);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    p[pos.x][pos.y] = v;
                }
            }
            if (!graph[v.x][v.y].BottomDoor)
            {
                Vector2Int pos = new Vector2Int(v.x - 1, v.y);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    p[pos.x][pos.y] = v;
                }
            }
            if (!graph[v.x][v.y].leftDoor)
            {
                Vector2Int pos = new Vector2Int(v.x, v.y - 1);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    p[pos.x][pos.y] = v;
                }
            }
            if (!graph[v.x][v.y].rightDoor)
            {
                Vector2Int pos = new Vector2Int(v.x, v.y + 1);
                float dist = Vector2.Distance(Node.getPostion(stx, sty), Node.getPostion(pos.x, pos.y));
                if (d[pos.x][pos.y] > Mathf.Max(d[v.x][v.y], dist))
                {
                    d[pos.x][pos.y] = Mathf.Max(d[v.x][v.y], dist);
                    p[pos.x][pos.y] = v;
                }
            }
        }

        return d;
    }
    private void placeStars(int cnt, int checkDist) {
        float l = 3.5f, r = GetComponent<DistanceJoint2D>().distance, dif = ((float)mazeSize.x * cellSize / 2 - r) / (float)(cnt - 1);
        int checkDistInit = checkDist;
        Globals.distIncrement = ((float)Mathf.Ceil(Mathf.Sqrt(2) * mazeSize.x / 2f * cellSize) - GetComponent<DistanceJoint2D>().distance) / starCount;
        for (int i = 0; i < cnt; i++) {
            int cntt = 0;
            float rd = r;
            checkDist = checkDistInit;
            while (cntt == 0) {
                List<Vector2Int> av = new List<Vector2Int>();
                for(int q = 1; q <= mazeSize.x; q++) {
                    for(int w = 1; w <= mazeSize.y; w++) {
                        if(mx[q][w] > l && mx[q][w] <= r) {
                            av.Add(new Vector2Int(q, w));
                        }
                    }
                }
    //            Debug.Log(av.Count);
                av.Shuffle();
                for(int ind = 0; ind < av.Count; ind++) {
                    bool f = false;
                    for (int g = -checkDist; g <= checkDist; g++) {
                        for(int t = -checkDist; t <= checkDist; t++) {
                            if (av[ind].x + g < 1 || av[ind].x + g > mazeSize.x ||
                                av[ind].y + t < 1 || av[ind].y + t > mazeSize.y ||
                                Mathf.Abs(g) + Mathf.Abs(t) > checkDist)
                                continue;
                            if (graph[av[ind].x + g][av[ind].y + t].hasStar || graph[av[ind].x][av[ind].y].hasCamp || graph[av[ind].x][av[ind].y].hasKey) {
                                f = true;
                                break;
                            }
                        }
                        if (f)
                            break;
                    }
                    if (!f) {
 //                       Debug.Log(av[ind].x + " " + av[ind].y + " " + mx[av[ind].x][av[ind].y]);
                        graph[av[ind].x][av[ind].y].hasStar = true;
                        cntt = 1;
                        GameObject starClone = Instantiate(star, Node.getPostion(av[ind].x, av[ind].y), new Quaternion(), mazeContainer);
                        starClone.transform.localScale = new Vector3(cellSize, cellSize, starClone.transform.localScale.z);
                        break;
                    }
                }
                if (cntt == 0) {
                    if (checkDist > 0) {
                        checkDist--;
                    }
                    else {
                        Debug.LogWarning("infinite loop star spawn: skipping. Camp ind = " + i);
                        //                       Application.Quit();
                        starCount--;
                        break;
                    }
                }
            }
//            Debug.Log(string.Format("l = {0}, r = {1}, dif = {2}", l, r, dif));
            l = rd;
            r = rd + dif;
        }
    }
    private void placeCamps(int cnt, int checkDist) {
        mx = getMaxArray(mazeSize.x / 2 + 1, mazeSize.y / 2 + 1);
        float l = 7f, dif = Mathf.Ceil(((float)mazeSize.x * cellSize / 2 - l) / (float)(cnt)), r = l + dif;
        int checDistInit = checkDist;
        
        for (int i = 0; i < cnt; i++) {
            //           Debug.Log(string.Format("l = {0}, r = {1}, dif = {2}", l, r, dif));
            float rd = r;
            checkDist = checDistInit;
            int cntt = 0;
            List<Vector2Int> av = new List<Vector2Int>();
            for (int q = 1; q <= mazeSize.x; q++) {
                for (int w = 1; w <= mazeSize.y; w++) {
                    if (mx[q][w] > l && mx[q][w] <= r) {
                        av.Add(new Vector2Int(q, w));
                    }
                }
            }
            av.Shuffle();
            //Debug.Log("l = "+l+" r = "+r+" av count = "+av.Count);
            while(cntt == 0) {
            //    Debug.Log("camp ind = "+i+" checkDist = "+checkDist);
                for (int ind = 0; ind < av.Count; ind++) {
                    bool f = false;
                    for (int g = -checkDist; g <= checkDist; g++) {
                        for (int t = -checkDist; t <= checkDist; t++) {
                            if (av[ind].x + g < 1 || av[ind].x + g > mazeSize.x ||
                                av[ind].y + t < 1 || av[ind].y + t > mazeSize.y)
                                continue;
                            if (graph[av[ind].x + g][av[ind].y + t].hasCamp || graph[av[ind].x][av[ind].y].hasKey) {
                                f = true;
                                break;
                            }
                        }
                        if (f)
                            break;
                    }
                    if (!f) {
                        graph[av[ind].x][av[ind].y].hasCamp = true;
                        cntt = 1;
                        ClearMazeArea(new Vector2Int(av[ind].x - 1, av[ind].y - 1), new Vector2Int(av[ind].x + 1, av[ind].y + 1));
                        spawnCamp(Node.getPostion(av[ind].x, av[ind].y));
                        break;
                    }
                }
                if (cntt == 0) {
                    if (checkDist > 0) {
                        checkDist--;
                    }
                    else {
                        Debug.LogWarning("infinite loop camp spawn: skipping. Camp ind = " + i);
                        //                       Application.Quit();
                        break;
                    }
                }
                //if (cntt == 0) {
                //    Debug.LogError("failed to create camp: not enough space");
                //}
            }
            l = rd;
            r = rd + dif;
        }
    }
    private void placeShootingEnemies(int cnt, int maxPathLen) {
        for(int i = 0; i < cnt; i++) {
            Vector2Int v = new Vector2Int(UnityEngine.Random.Range(1, mazeSize.x + 1), UnityEngine.Random.Range(1, mazeSize.y + 1));
            while(v == new Vector2Int(mazeSize.x / 2 + 1, mazeSize.y / 2 + 1) ||
                  graph[v.x][v.y].hasCamp == true || graph[v.x][v.y].isPartOfEnemyPath) {
                v = new Vector2Int(UnityEngine.Random.Range(1, mazeSize.x + 1), UnityEngine.Random.Range(1, mazeSize.y + 1));
            }
            GameObject enemyClone = Instantiate(sEnemy, Node.getPostion(v.x, v.y), new Quaternion());
            enemyClone.transform.localScale = new Vector3(enemyClone.transform.localScale.x * cellSize, enemyClone.transform.localScale.y * cellSize, enemyClone.transform.localScale.z);
            graph[v.x][v.y].isPartOfEnemyPath = true;

            int len = UnityEngine.Random.Range(1, maxPathLen);
            shootingEnemy se = enemyClone.GetComponent<shootingEnemy>();
            se.positions.Clear();
            se.positions.Add(Node.getPostion(v.x, v.y));
            se.movingSpeed = UnityEngine.Random.Range(2, 3);
            se.timeoutInit = UnityEngine.Random.Range(1, 3);

            Vector2Int p = v;
            for (int j = 0; j < len; j++) {
                List<Vector2Int> available = new List<Vector2Int>();
                if (v.x - 1 > 0 && graph[v.x][v.y].BottomDoor == false) {
                    available.Add(new Vector2Int(v.x - 1, v.y));
                }
                if (v.x + 1 > 0 && graph[v.x][v.y].topDoor == false) {
                    available.Add(new Vector2Int(v.x + 1, v.y));
                }
                if (v.y - 1 > 0 && graph[v.x][v.y].leftDoor == false) {
                    available.Add(new Vector2Int(v.x, v.y - 1));
                }
                if (v.y + 1 > 0 && graph[v.x][v.y].rightDoor == false) {
                    available.Add(new Vector2Int(v.x, v.y + 1));
                }
                available.Shuffle();
                bool placed = false;
                for (int z = 0; z < available.Count; z++) {
                    if (available[z] == p || graph[available[z].x][available[z].y].hasCamp || available[z] == new Vector2Int(mazeSize.x / 2 + 1, mazeSize.y / 2 + 1)
                        || graph[available[z].x][available[z].y].isPartOfEnemyPath)
                        continue;
                    placed = true;
                    p = v;
                    v = available[z];
                    graph[v.x][v.y].isPartOfEnemyPath = true;
                    se.positions.Add(Node.getPostion(v.x, v.y));
                    break;
                }
                if (!placed) {
                    break;
                }
            }
        }
    }
    public GameObject SpawnTargetingEnemy(Vector2 position, int stage)
    {
        GameObject enemyClone = Instantiate(tEnemy, position, new Quaternion());
        enemyClone.transform.localScale = new Vector3(enemyClone.transform.localScale.x * cellSize, enemyClone.transform.localScale.y * cellSize, enemyClone.transform.localScale.z);
        targetingEnemy te = enemyClone.GetComponent<targetingEnemy>();
        te.timeoutInit = UnityEngine.Random.Range(1, 3);
        te.target = transform;
        te.stage = stage;
        return enemyClone;
    }
    private void placeTargetingEnemies(int cnt) {
        for (int i = 0; i < cnt; i++) {
            Vector2Int v = new Vector2Int(UnityEngine.Random.Range(1, mazeSize.x + 1), UnityEngine.Random.Range(1, mazeSize.y + 1));
            while (v == new Vector2Int(mazeSize.x / 2 + 1, mazeSize.y / 2 + 1) ||
                  graph[v.x][v.y].hasCamp == true || graph[v.x][v.y].isPartOfEnemyPath) {
                v = new Vector2Int(UnityEngine.Random.Range(1, mazeSize.x + 1), UnityEngine.Random.Range(1, mazeSize.y + 1));
            }
            GameObject enemyClone = SpawnTargetingEnemy(Node.getPostion(v.x, v.y), 1);
            if (!graph[v.x][v.y].rightDoor) {
                enemyClone.transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            if (!graph[v.x][v.y].leftDoor) {
                enemyClone.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            if (!graph[v.x][v.y].topDoor) {
                enemyClone.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (!graph[v.x][v.y].BottomDoor) {
                enemyClone.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            graph[v.x][v.y].isPartOfEnemyPath = true;
        }
    }
    private void placeKey() {
        List<Vector2Int> pos = new List<Vector2Int>();
        pos.Add(new Vector2Int(1, 1));
        pos.Add(new Vector2Int(1, mazeSize.y));
        pos.Add(new Vector2Int(mazeSize.x, 1));
        pos.Add(new Vector2Int(mazeSize.x, mazeSize.y));
        pos.Shuffle();
        graph[pos[0].x][pos[0].y].hasKey = true;
        GameObject keyClone = Instantiate(key, Node.getPostion(pos[0].x, pos[0].y), new Quaternion());
        keyClone.SetActive(false);
        GetComponent<movements>().key = keyClone;
    }
    private void ClearMazeArea(Vector2Int leftBottom, Vector2Int rightTop) {
        leftBottom = new Vector2Int(Mathf.Clamp(leftBottom.x, 1, mazeSize.x), Mathf.Clamp(leftBottom.y, 1, mazeSize.y));
        rightTop = new Vector2Int(Mathf.Clamp(rightTop.x, 1, mazeSize.x), Mathf.Clamp(rightTop.y, 1, mazeSize.y));
//        Debug.Log(leftBottom);
//        Debug.Log(rightTop);

        for (int i = leftBottom.x; i < rightTop.x; i++) {
            for(int j = leftBottom.y; j < rightTop.y; j++) {
                graph[i][j].topDoor = false;
                graph[i][j].rightDoor = false;
                graph[i + 1][j].BottomDoor = false;
                graph[i][j + 1].leftDoor = false;
            }
        }
        for (int i = leftBottom.x; i < rightTop.x; i++) {
            graph[i][rightTop.y].topDoor = false;
            graph[i + 1][rightTop.y].BottomDoor = false;
        }
        for (int j = leftBottom.y; j < rightTop.y; j++) {
            graph[rightTop.x][j].rightDoor = false;
            graph[rightTop.x][j + 1].leftDoor = false;
        }
    }
    public void spawnCamp(Vector2 pos) {
        GameObject campClone = Instantiate(camp, pos, new Quaternion(), mazeContainer);
        campClone.transform.localScale = new Vector3((float)cellSize / 2, (float)cellSize / 2, campClone.transform.localScale.z);
        campClone.GetComponent<CircleCollider2D>().radius = cellSize;
        campClone.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>(true).pointLightOuterRadius = (float)cellSize * (float)cellSize / 2;
    }
}
