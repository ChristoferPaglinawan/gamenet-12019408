using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;

    [SerializeField]
    TextMeshProUGUI playerNameText;

    private void Start() 
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();

        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovementScript>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovementScript>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            
            camera.enabled = photonView.IsMine;
        }

        playerNameText.text = photonView.Owner.NickName;
    }
}
