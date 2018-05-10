using System.Collections.Generic;
using UnityEngine;

//The main class for each ship - has the graph data for the individual nodes.
public class Ship : MonoBehaviour {

    public float unitSize = 1.0f;

    private ShipModule root;

    private Rigidbody2D rb;

    Dictionary<Vector2, Vertex> shipModules = new Dictionary<Vector2, Vertex>();//Holds all references to ships

    public Vector2 centerOfMass;//Where the camera should center on

    public float moveSpeed = 1.0f;

    public float rotationTorque = 2.0f;

    public int maxBoost;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CalculateMass();
        UpdateMovementAndTorque();
        StaggerGuns();
        if (gameObject.tag == "Player")
        {
            GetComponent<PlayerMovement>().UpdateStats();
        }
    }

    public void StaggerGuns()
    {
        List<Gun> guns = new List<Gun>();
        List<EnemyGun> enemyGuns = new List<EnemyGun>();
        foreach (KeyValuePair<Vector2, Vertex> kV in shipModules)
        {
            if (kV.Value.module.tag == "PlayerModule")
            {
                guns.Add(kV.Value.module.GetComponentInChildren<Gun>());
            }
            if (kV.Value.module.tag == "EnemyModule")
            {
                enemyGuns.Add(kV.Value.module.GetComponentInChildren<EnemyGun>());
            }
        }
        for(int i = 0; i < guns.Count; i++)
        {
            guns[i].staggerLag = i * guns[i].fireCooldown / guns.Count;
        }
        for (int i = 0; i < enemyGuns.Count; i++)
        {
            enemyGuns[i].staggerLag = i * enemyGuns[i].fireCooldown / enemyGuns.Count;
        }
    }

    public void UpdateMovementAndTorque()
    {
        Vector2 moveAndTorque = Vector2.zero;
        maxBoost = 0;
        foreach(KeyValuePair<Vector2, Vertex> kV in shipModules)
        {
            if(kV.Value.module.tag == "MovementModule")
            {
                MovementModule module = kV.Value.module.GetComponent<MovementModule>();
                moveAndTorque += new Vector2(module.movement, module.torque);
            }
            if (kV.Value.module.tag == "BoostModule")
            {
                maxBoost += 200;
            }
        }
        moveSpeed = moveAndTorque.x;
        rotationTorque = moveAndTorque.y;
    }

    void CalculateMass()//Also calculates boost
    {
        Vector2 centerOfMass = Vector2.zero;
        int weight = 0;
        foreach (KeyValuePair<Vector2, Vertex> v in shipModules)
        {
            centerOfMass += new Vector2(v.Value.module.transform.localPosition.x * v.Value.module.mass, v.Value.module.transform.localPosition.y * v.Value.module.mass);
            weight += v.Value.module.mass;
        }
        if (weight == 0) return;
        centerOfMass /= weight;
        rb.centerOfMass = centerOfMass;
        rb.mass = weight;
    }

    public void SetInvincible()
    {
        foreach (KeyValuePair<Vector2, Vertex> v in shipModules)
        {
            v.Value.module.invincible = true;
        }
    }

    public void AddModule(int x, int y, ShipModule toInstantiate)//Called to add a new node to this ship.
    {
        Vector2 vec = new Vector2(x, y);
        if (!shipModules.ContainsKey(vec))
        {
            ShipModule instance = Instantiate(toInstantiate) as ShipModule;
            instance.SetMothership(this);
            RegisterModule(instance, vec);
            SearchAndConnect(vec);
        }
    }

    void RegisterModule(ShipModule module, Vector2 vec)//Sets the appropriate position and adds to the shipModules list
    {

            float posY = vec.y - (unitSize * Mathf.Tan(Mathf.PI/6) * 0.25f * vec.y);//Math to offset y to fit hexagonal grid

            float posX = vec.x + (unitSize * 0.5f * vec.y);//Offsets the x position to create a hexagonal grid instead of xy grid

            Vector2 pos = new Vector2(posX, posY);
            module.transform.position = pos + (Vector2)gameObject.transform.position;

            List<Vertex> list = new List<Vertex>();
            Vertex v = new Vertex(false, list, module);

            shipModules.Add(vec, v);

            module.xyPos = vec;

        if(module.id == GameManager.instance.database.Cockpit.representativeModule.id)
        {
            root = module;
        }

    }

    int AddConnection(Vector2 vec1, Vector2 vec2)//Add a connection between two modules
    {
        if (shipModules.ContainsKey(vec1) && shipModules.ContainsKey(vec2)){

            Vector2 v2v1 = vec1 - vec2;
            Vertex v1;
            shipModules.TryGetValue(vec1, out v1);

            Vertex v2;
            shipModules.TryGetValue(vec2, out v2);

            for(int i = 0; i < v2.module.connectionPositions.Length; i++)
            {
                if(v2.module.connectionPositions[i] == v2v1)
                {
                    break;
                }
                else if(i == v2.module.connectionPositions.Length - 1)
                {
                    return 0;
                }
            }

            v1.adj.Add(v2);
            v2.adj.Add(v1);

            return 1;

        }
        return 0;
    }

    void SearchAndConnect(Vector2 xyPos)//Connects a new module to all appropriate adjacent modules
    {
        Vertex v;
        shipModules.TryGetValue(xyPos, out v);
        ShipModule module = v.module;

        if(module != null)
        {
            for(int i = 0; i < module.connectionPositions.Length; i++)
            {
                Vector2 offset = module.connectionPositions[i] + xyPos;
                AddConnection(xyPos, offset);
            }
        }
    }

    public void DestroyNode(Vector2 xyPos)//Destroy a node and connections and then implment effects
    {
        if (shipModules.ContainsKey(xyPos))
        {
           
            Vertex destroyedVertex;
            shipModules.TryGetValue(xyPos, out destroyedVertex);

            for (int i = 0; i < destroyedVertex.adj.Count; i++)//Remove connections
            {
                Vertex adjVertex = destroyedVertex.adj[i];
                adjVertex.adj.Remove(destroyedVertex);
                destroyedVertex.adj.Remove(adjVertex);
            }
            
            if (root == destroyedVertex.module) Destroy(gameObject);//Handles when the root is destroyed

            shipModules.Remove(destroyedVertex.module.xyPos);

            Destroy(destroyedVertex.module.gameObject);

        }

        List<Vertex> nodesToDestroy = GetUnconnectedModules();

        for (int i = 0; i < nodesToDestroy.Count; i++)//Destroys nodes that were isolated when the node was destroyed
        {
            Vertex nodeToDestroy = nodesToDestroy[i];
            if (nodeToDestroy != null && nodeToDestroy.module)
            {
                for (int j = 0; j < nodeToDestroy.adj.Count; j++)
                {
                    nodeToDestroy.adj[j].adj.Remove(nodeToDestroy);
                }
                shipModules.Remove(nodeToDestroy.module.xyPos);
                Destroy(nodeToDestroy.module.gameObject);
            }
        }

        UpdateMovementAndTorque();//Update Movement
        CalculateMass();
        StaggerGuns();
        if(gameObject.tag == "Player")
        {
            GetComponent<PlayerMovement>().UpdateStats();
        }
    }

    List<Vertex> GetUnconnectedModules()//Determinse whether the root at position is connected to the root
    {
        foreach(KeyValuePair<Vector2, Vertex> v in shipModules)
        {
            v.Value.setVisited(false);
        }

        List<Vertex> notVisited = new List<Vertex>();
        Vertex origin;
        if(!shipModules.TryGetValue(root.xyPos, out origin)) return notVisited;

        Queue<Vertex> q = new Queue<Vertex>();
            origin.visited = true;
            q.Enqueue(origin);

            while (q.Count != 0)
            {

                Vertex current = q.Dequeue();

                for (int i = 0; i < current.adj.Count; i++)
                {
                    Vertex adj = current.adj[i];
                    if (!adj.visited)
                    {
                        current.adj[i].setVisited(true);
                        q.Enqueue(current.adj[i]);
                    }
                }
            }

        foreach (KeyValuePair<Vector2, Vertex> v in shipModules)
        {
            if (!v.Value.visited)
            {
                notVisited.Add(v.Value);
            }
        }

        return notVisited;

    }

    class Vertex
    {
        public bool visited;
        public ShipModule module;
        public List<Vertex> adj;

        public void setVisited(bool v)
        {
            visited = v;
        }

        public Vertex(bool V, List<Vertex> A, ShipModule M)
        {
            visited = V;
            adj = A;
            module = M;
        }
    }

}
