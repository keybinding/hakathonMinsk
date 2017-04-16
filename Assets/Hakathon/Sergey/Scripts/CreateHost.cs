using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateHost : MonoBehaviour {

	void Start()
    {
        GetComponent<NetworkManager>().StartClient();
    }
}
