using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviourPunCallbacks
{
    public Transform nozzle;

    public GameObject bulletPrefab1;

    void Start()
    {
        GetComponent<BulletController>().enabled = photonView.IsMine;
    }

    void Update()
    {
        float destroySecond = 5;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab1.name, nozzle.transform.position, transform.rotation);

            Destroy(bullet, destroySecond);
        }
    }
}
