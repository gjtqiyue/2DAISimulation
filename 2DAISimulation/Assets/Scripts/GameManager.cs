using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField]
    private List<Vector3> EnemySpawnPoints;

    [SerializeField]
    private List<Vector3> AlcovePoints;

    private List<Vector3> SafePoints;

    public GameObject enemy;
    public GameObject agent;
    public GameObject item;
    public GameObject menu;

    private ControllableAgentScript player;     // player reference
    private AIAgentScript theAI;                // ai reference

    private List<GameObject> items = new List<GameObject>();
    private List<iTeleportable> entities = new List<iTeleportable>();

    private bool isPlayerAlive;
    private bool isAIAlive;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        // We add two points in the middle of the map as safe points
        SafePoints = AlcovePoints;
        SafePoints.Add(new Vector3(43, 8));
        SafePoints.Add(new Vector3(0, 8));

    }

    void Start () {
        // show the menu
        ShowPanel();
	}
	
	void Update () {
        // if no player or Ai agent exsit right now we do nothing
        if (player == null && theAI == null) return;

        // check if anyone dies, then remove the reference from the game manager
        if (player.GetStatus())
        {
            isPlayerAlive = false;
            entities.Remove(player);
            player.gameObject.SetActive(false);
        }
        if (theAI.GetStatus())
        {
            isAIAlive = false;
            entities.Remove(theAI);
            theAI.gameObject.SetActive(false);
        }

        // check if game wins
        WinCheck();
	}

    private void ShowPanel()
    {
        menu.SetActive(true);
    }

    private void HidePanel()
    {
        menu.SetActive(false);
    }

    // start the game by spawning items, agents and enemies
    public void StartGame()
    {
        HidePanel();
        SpawnItem();
        SpawnAgents();
        SpawnEnemy();
    }

    // return the current number of items left
    public int GetItemAmount()
    {
        return items.Count;
    }

    public void Score(Agent target, Collectable itemCollected)
    {
        items.Remove(itemCollected.gameObject);
        target.Score();
    }

    private void WinCheck()
    {
        if (!isPlayerAlive && !isAIAlive)
        {
            if (player.GetScore() > theAI.GetScore())
                WinGame("player");
            else if (player.GetScore() < theAI.GetScore())
                WinGame("AI");
            else
                WinGame("draw");
        }
        else if (GetItemAmount() == 0)
        {
            if (player.GetScore() > theAI.GetScore())
                WinGame("player");
            else if(player.GetScore() < theAI.GetScore())
                WinGame("AI");
            else
                WinGame("draw");
        }
    }

    // clear all the game instances
    private void ClearEntites()
    {
        for (int i=0; i<entities.Count; i++)
        {
            Destroy(entities[i].GetTransform().gameObject);
        }

        entities.Clear();
    }

    // Show the win message based on the winner
    private void WinGame(string name)
    {
        items.Clear();
        ClearEntites();
        ShowPanel();
        Text text = menu.transform.GetChild(0).GetComponent<Text>();
        switch (name)
        {
            case "player":
                text.text = "Player wins!";
                break;
            case "AI":
                text.text = "AI wins!";
                break;
            case "draw":
                text.text = "Game Draw!";
                break;
        }
    }

    private void SpawnItem()
    {
        if (AlcovePoints.Count != 10) Debug.Log("you need exactly 10 alcoves");

        // randomly decide a location near the alcove points
        for (int i = 0; i < 10; i++)
        {
            int x, y;
            do
            {
                x = Random.Range(-1, 2);
                y = Random.Range(-1, 2);
            }
            while (x == 0 && y == 0);

            GameObject gameObj = Instantiate(item, AlcovePoints[i] + new Vector3(x, y), Quaternion.identity);
            items.Add(gameObj);
        }
    }

    private void SpawnEnemy()
    {
        int x = Random.Range(0, 2);
        int y = Random.Range(0, 2);
        GameObject gameobj = Instantiate(enemy, EnemySpawnPoints[2*x], Quaternion.identity);
        entities.Add(gameobj.GetComponent<EnemyScript>());
        GameObject gameobj2 = Instantiate(enemy, EnemySpawnPoints[2 * y + 1], Quaternion.identity);
        entities.Add(gameobj2.GetComponent<EnemyScript>());

    }

    private void SpawnAgents()
    {

        //spawn the AI agent
        int x = Random.Range(0, 5);
        GameObject AIObj = Instantiate(agent, AlcovePoints[x*2], Quaternion.identity);
        theAI = AIObj.AddComponent<AIAgentScript>();
        entities.Add(theAI);
        isAIAlive = true;

        //spawn the controllable agent
        GameObject playerObj = Instantiate(agent, AlcovePoints[x*2+1], Quaternion.identity);
        player = playerObj.AddComponent<ControllableAgentScript>();
        player.tag = "Player";
        entities.Add(player);
        isPlayerAlive = true;
    }

    public int GetCloestItem(Vector3 agentPos)
    {
        if (items.Count == 0) return -1;

        float min = float.MaxValue;
        int indexMin = 0;
        //Debug.Log(items.Count);
        for (int i=0; i<items.Count; i++)
        {
            Vector3 pos = items[i].transform.position;
            float dist = Mathf.Abs(pos.y - agentPos.y) + Mathf.Abs(pos.x - agentPos.x);
            if (dist < min)
            {
                min = dist;
                indexMin = i;
            }
        }
        return indexMin;      
    }

    public Vector3 GetItemLocation(int index)
    {
        return items[index].transform.position;
    }

    // return the cloest safe point away form the enemy
    public Vector3 GetCloestSafePointAwayFromTarget(Transform from, Transform target)
    {
        float x;
        int minIndex = 0;
        int minX = int.MaxValue;

        if (transform.position.y > 17 || transform.position.y < 0)
        {
            return GetClosestAlcovePoint(transform);
        }
        else
        {
            for (int i = 0; i < SafePoints.Count; i++)
            {
                Vector3 pt = SafePoints[i];

                if (Mathf.Abs(pt.x - from.position.x) < Mathf.Abs(pt.x - target.position.x) && Mathf.Sign(pt.x - from.position.x) == Mathf.Sign(pt.x - target.position.x))
                {
                    if (Mathf.Abs(pt.x - from.position.x) < minX)
                    {
                        minX = (int)Mathf.Abs(pt.x - from.position.x);
                        minIndex = i;
                    }

                }
            }

            x = SafePoints[minIndex].x;

            // manually check if the agent is currently in a tunnel then we go to the tunnel safe point
            // else depends on the agent location we give him points at either upper area or lower area
            if (x == 43 || x == 0) return SafePoints[minIndex];
            else if (from.position.y < 8) return SafePoints[minIndex];
            else return SafePoints[minIndex + 1];
        }
    }

    public Vector3 GetAlcoveLocation(int index)
    {
        return AlcovePoints[index];
    }

    // go throught the entities and return the clesest teleportable near it
    public Transform GetCloestTeleportable(Transform origin)
    {
        int index = 0 ;
        float minDist = float.MaxValue;
        for (int i=0; i<entities.Count; i++)
        {
            Transform target = entities[i].GetTransform();
            if (target != origin)
            {
                float dist = (target.position - origin.position).magnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
        }

        return entities[index].GetTransform();
    }

    public Vector3 GetClosestAlcovePoint(Transform from)
    {
        int index = 0;
        int d = int.MaxValue;
        for (int i=0; i<AlcovePoints.Count; i++)
        {
            int dist = (int)Mathf.Abs(from.position.x - AlcovePoints[i].x) + (int)Mathf.Abs(from.position.y - AlcovePoints[i].y);
            if (dist < d)
            {
                d = dist;
                index = i;
            }
        }

        return AlcovePoints[index];
    }
}
