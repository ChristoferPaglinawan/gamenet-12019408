using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    [SerializeField]

    Image healthBar;


    private float startHealth = 100;
    public float health;
    
    private void Start() 
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;    
    }

    public enum RaiseEventsCode
    {
        WhoIsDeadEventCode = 1,
    }

    private int deathOrder = 0;

    private void OnEnable() 
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable() 
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoIsDeadEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfDeadPlayer = (string)data[0];
            deathOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nickNameOfDeadPlayer + " " + deathOrder);

            GameObject orderUIText = RacingGameManager.instance.deathTextUI[deathOrder - 1];
            orderUIText.SetActive(true);

            if(viewId != photonView.ViewID) 
            {
                //orderUIText.GetComponent<Text>().text = deathOrder + " " + nickNameOfDeadPlayer + "(YOU)";
                //orderUIText.GetComponent<Text>().color = Color.red;
                orderUIText.GetComponent<Text>().text = deathOrder + " " + nickNameOfDeadPlayer + " is dead.";
            }
        }
    }

    private void Update() 
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            GameObject winnningPlayerUIText = RacingGameManager.instance.drWinningTextUI;
            winnningPlayerUIText.SetActive(true);
            winnningPlayerUIText.GetComponent<Text>().text = "You win!";
        }    
    }


    private void Die()
    {
        if(photonView.IsMine)
        {
            RacingGameManager.instance.LeaveRoom();
            PhotonNetwork.LoadLevel("LobbyScene");
            
        }

        deathOrder++;
        string nickName = photonView.Owner.NickName;

        int viewId = photonView.ViewID;

        // event data
        object[] data = new object[] { nickName, deathOrder, viewId};

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoIsDeadEventCode, data, raiseEventOptions, sendOption);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);

        healthBar.fillAmount = health / startHealth;

        if(health <= 0)
        {
            Die();
        }
    }
}
