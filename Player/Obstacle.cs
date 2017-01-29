using UnityEngine;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;

public class Obstacle : MonoBehaviour
{
    [Range(0.01f, 0.1f)]
    public float Damage = 0.05f;

    public float TimeToLive;
    public ParticleSystem Explosion;
	// Use this for initialization
	void Start ()
	{
	    TimeToLive = Random.Range(30, 120)*1 + EnvironmentManager.Instance.FlowManager.FlowLevel;
	    Explosion = GetComponent<ParticleSystem>();
        Invoke("SelfDestruction", TimeToLive);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SelfDestruction()
    {
        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision player)
    {
        if (player.gameObject.tag != "Player") return;
        Explosion.Play();
        player.gameObject.GetComponent<AntEater>().GotHit(Damage);
        Destroy(gameObject, 1);
    }

}
