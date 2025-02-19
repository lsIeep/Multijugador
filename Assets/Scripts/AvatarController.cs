using System.Collections;
using System.Collections.Generic;
using ED.SC;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class AvatarController : NetworkBehaviour {
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private GameObject doorPrefab;
    private readonly List<Color> m_Colors = new List<Color>() { Color.blue , Color.green, Color.red};
    private SpriteRenderer m_Renderer;
    private static NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>(Vector3.zero, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Color> m_Color = new NetworkVariable<Color>(Color.black, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        SmartConsole.Log($"ClientId {OwnerClientId}");
        if (OwnerClientId > 2)
        {
            NetworkManager.Singleton.DisconnectClient(OwnerClientId);
            Destroy(gameObject);
            return;
        }
        // player id defines player position
        transform.position += Vector3.right * 2 * OwnerClientId;
        // get component for color change
        m_Renderer = GetComponent<SpriteRenderer>();
        // assign initial color to network variable
        if(IsOwner) m_Color.Value = m_Colors[(int) OwnerClientId];
        //ChangeColorServerRpc(m_Colors[(int) OwnerClientId]);
    }

    // Update is called once per frame
    void Update() {
        //SmartConsole.Log(OwnerClientId + ";   randomNumber; " + randomNumber.Value);

        // sets color to network variable value
        if (m_Renderer.color != m_Color.Value) m_Renderer.color = m_Color.Value;
        
        if (!IsOwner) return;

        /*if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = Random.Range(0, 100);
        }*/
        
        // movement
        if (Input.GetKey(KeyCode.A)) LerpPosition(Vector3.left, movementSpeed);
        if (Input.GetKey(KeyCode.D)) LerpPosition(Vector3.right, movementSpeed);
        if (Input.GetKey(KeyCode.W)) LerpPosition(Vector3.up, movementSpeed);
        if (Input.GetKey(KeyCode.S)) LerpPosition(Vector3.down, movementSpeed);
        
        // color change
        // if you need to change something not in transform, it's needed RPC calling
        // RPC calling executes the method in several clients (specified by RpcTarget)
        // if (Input.GetKeyDown(KeyCode.RightControl)) m_Color.Value = m_Colors[2];
        if (Input.GetKeyDown(KeyCode.RightControl)) SpawnDoorServerRpc();

        if(Input.GetKeyDown(KeyCode.T)) TestClientRpc();
    }

    void LerpPosition(Vector3 offset, float speed) {
        Vector3 positionNow = transform.position;
        transform.position = Vector3.Lerp(positionNow, positionNow +  offset, Time.deltaTime * speed);
    }

    //[ServerRpc(RequireOwnership = false)]
    [Rpc(SendTo.Server)]
    void SpawnDoorServerRpc()
    {
        SmartConsole.Log("spawn door");
        GameObject door = Instantiate(doorPrefab, m_Position.Value, Quaternion.identity);
        m_Position.Value += Vector3.right + Vector3.up;
        door.GetComponent<NetworkObject>().Spawn();
    }

    [Rpc(SendTo.Server)]
    void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        SmartConsole.Log("TestServerRpc " + OwnerClientId + "," + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void TestClientRpc()
    {
        SmartConsole.Log("TestClientRpc");
    }
    
    // [ServerRpc(RequireOwnership = false)]
    // void ChangeColorServerRpc(Color newColor)
    // {
    //     ChangeColorClientRpc(newColor);
    // }
    //
    // [ClientRpc]
    // void ChangeColorClientRpc(Color newColor)
    // {
    //     SmartConsole.Log($"New color {newColor}");
    //     if(m_Renderer!=null) m_Renderer.color = newColor;
    // }
}
