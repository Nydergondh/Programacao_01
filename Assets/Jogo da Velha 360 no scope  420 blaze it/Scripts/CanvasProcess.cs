using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasProcess : MonoBehaviour {

    public bool taf = false;

    private Canvas canvas;

    public GameObject endingButton;
    public Animator frame = null;
    public Animator text = null;
    public AudioSource music = null;

    //delegate to play end scene
    public delegate void ThatsAllFolks(bool value);
    public ThatsAllFolks thatsAllFolks;

    // Start is called before the first frame update
    void Awake() {
        canvas = GetComponent<Canvas>();    
    }

    void Start()
    {
        thatsAllFolks += SetTaf;
    }

    // Update is called once per frame
    void Update()
    {
        if (taf == true) {
            playClips();
            StartCoroutine(ActiveEndButton());
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

    private IEnumerator ActiveEndButton() {

        yield return new WaitForSeconds(5f);
        endingButton.SetActive(true);
        
    }

    public void QuitGame() {
        Debug.Log("I Quited the application");
        Application.Quit();
    }

    public void OnChangeScreen(int change) {

        canvas.planeDistance = change;
    }

    public void SetMenuBool(bool value) {
        frame.SetBool("Menu",value);
        text.SetBool("Menu", value);
    }
}
