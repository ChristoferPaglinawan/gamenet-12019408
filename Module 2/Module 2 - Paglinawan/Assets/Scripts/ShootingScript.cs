using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShootingScript : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;
    
    private Animator animator;

    public int deathCount;
    
    private void Start() 
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        deathCount = 0;

        animator = this.GetComponent<Animator>();
    }

    private void Update() 
    {
        
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f,0.5f));

        if(Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects",RpcTarget.All, hit.point);

            if(hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        GameObject killText = GameObject.Find("KillText");

        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if(health <= 0)
        {
            if(photonView.IsMine)
            {
                killText.GetComponent<Text>().text = "You are killed by " + info.Sender.NickName;
            }
            
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if(photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        GameObject killText = GameObject.Find("KillText");

        float respawnTime = 5.0f;

        while(respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";
        killText.GetComponent<Text>().text = "";

        int randomPointX = Random.Range(-20,20);
        int randomPointZ = Random.Range(-20,20);

        this.transform.position = new Vector3(randomPointX, 0 ,randomPointZ);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        photonView.RPC("IncrementDeathCount", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void IncrementDeathCount()
    {
        GameObject lostText = GameObject.Find("LostText");
        deathCount++;
        Debug.Log("Death Count: " + deathCount);

        if(deathCount >= 10)
        {
            if(photonView.IsMine)
            {
                lostText.GetComponent<Text>().text = "You lose.";
            }
        }
    }
}
