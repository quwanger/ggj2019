using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioClip[] pushes;
    public AudioClip[] pulls;
    public AudioClip[] explosions;
    public AudioClip[] missiles;
    public AudioClip[] gets;
    public AudioClip[] magnets;

    public AudioSource[] sources;

    AudioSource sfx;
    AudioSource soundtrack;

	// Use this for initialization
	void Start () {
        sources = GetComponents<AudioSource>();
        sfx = sources[0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(string code)
    {
        switch (code)
        {
            case "pushes":
                sfx.PlayOneShot(pushes[Random.Range(0, pushes.Length)]);
                break;
            case "pulls":
                sfx.PlayOneShot(pulls[Random.Range(0, pulls.Length)]);
                break;
            case "explosions":
                sfx.PlayOneShot(explosions[Random.Range(0, explosions.Length)]);
                break;
            case "missiles":
                sfx.PlayOneShot(missiles[Random.Range(0, missiles.Length)]);
                break;
            case "gets":
                sfx.PlayOneShot(gets[Random.Range(0, gets.Length)]);
                break;
        }
    }
}
