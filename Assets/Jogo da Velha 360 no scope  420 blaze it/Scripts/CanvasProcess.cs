using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasProcess : MonoBehaviour {

    public bool taf = false;
    public Animator frame = null;
    public Animator text = null;
    public AudioSource music = null;

    public delegate void ThatsAllFolks(bool value);
    public ThatsAllFolks thatsAllFolks;
    // Start is called before the first frame update
    void Start()
    {
        thatsAllFolks += SetTaf;
    }

    // Update is called once per frame
    void Update()
    {
        if (taf == true) {
            playClips();
        }
    }

    public void SetTaf(bool value) {
        taf = value;
    }

    public void playClips() {
        taf = false;
        frame.Play("TAF2");
        text.Play("TAF");
        music.Play();
    }
}
