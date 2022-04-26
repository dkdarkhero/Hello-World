using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public bool forward = true;
        public int TurnValue = 450;
        public int currentTUrn = 0;
        public float spawnTimer;
        public GameObject player;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Move();
            }
        }

        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            if (NetworkManager.Singleton.IsServer)
            {

                spawnTimer += Time.deltaTime;

                if (spawnTimer > 5)
                {
                    spawnTimer = 0;
                  GameObject go = Instantiate(player, new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), gameObject.transform.rotation);
                    go.GetComponent<NetworkObject>().Spawn();
                }

                Debug.Log("Server");

                if ( forward == true)
                {
                    Position.Value += new Vector3(0, 0.01f , 0);
                    currentTUrn++;
                }

                if (forward == false)
                {
                    Position.Value -= new Vector3(0, 0.01f, 0);
                    currentTUrn--;
                }


                if (currentTUrn > TurnValue)
                {
                    forward = false;
                }

                if (currentTUrn < 0)
                {
                    forward = true;
                }
               

            }
            transform.position = Position.Value;
        }
    }
}
