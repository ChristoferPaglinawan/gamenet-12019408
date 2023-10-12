using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start() 
    {
        if(PhotonNetwork.IsConnected)
        {
            if(playerPrefab != null)
            {
                StartCoroutine(DelayedPlayerSpawn());
            }
        }
    }

    private void Update() 
    {
        
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(3);

        int randomPointX = Random.Range(-10,10);
        int randomPointZ = Random.Range(-10,10);

        playerPrefab = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX, 0, randomPointZ), Quaternion.identity);
    }
}
