using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RacingGameManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextUI;

    public static RacingGameManager instance = null;
    
    public Text timeText;

    public List<GameObject> lapTriggers = new List<GameObject>();

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    Vector3 desiredRotation = new Vector3(0, 90, 0);
    

    private void Start() 
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            Quaternion rotation = Quaternion.Euler(desiredRotation);

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int) playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int) playerSelectionNumber].name, instantiatePosition, rotation);
            }
        }

        foreach(GameObject go in finisherTextUI)
        {
            go.SetActive(false);
        }
    }

    private void Update() 
    {
        
    }
}
