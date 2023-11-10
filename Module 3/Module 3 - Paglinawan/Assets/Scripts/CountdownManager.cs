using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class CountdownManager : MonoBehaviourPunCallbacks
{
    public Text timerText;

    public float timeToStartRace = 5.0f;

    private void Start() 
    {
        timerText = RacingGameManager.instance.timeText;
    }

    private void Update() 
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if(timeToStartRace < 0)
            {
                photonView.RPC("StartRace",RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if(time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovementScript>().isControlEnabled = true;
        this.enabled = false;
    }
}
