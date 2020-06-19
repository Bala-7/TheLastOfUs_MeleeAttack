using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator _ac;
    private int health = 3;
    public Transform lookAtNode;


    public ParticleSystem bloodFX;

    private void Awake()
    {
        _ac = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bloodFX.isPlaying) bloodFX.Stop();
    }


    public void Hit() {
        health--;
        if (health <= 0)
        {
            _ac.Play("Faint");
        }
        else _ac.SetTrigger("Hit");

        bloodFX.Play();
    }

    public Transform GetLookAt() { return lookAtNode; }
}
