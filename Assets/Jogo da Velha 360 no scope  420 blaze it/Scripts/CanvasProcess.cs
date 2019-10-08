
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasProcess : MonoBehaviour {

    public bool taf = false;
    public static CanvasProcess instance = null;

    public GameObject Image;
    public GameObject MultiuplayerMenu;
    public GameObject OnlineMenu;
    public GameObject LocalMenu;

    public GameObject endingButton;
    public Animator frame = null;
    public AudioSource music = null;

    public RawImage Ending;
    public RawImage[] EndingImages;

    //delegate to play end scene
    public delegate void ThatsAllFolks(bool value);
    public ThatsAllFolks thatsAllFolks;

    // Start is called before the first frame update
    void Awake() {

        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(instance);
            instance = this;
        }
    }

    void Start() {
        thatsAllFolks += SetTaf;
    }

    // Update is called once per frame
    void Update() {
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
        Canvas canvas = instance.GetComponent<Canvas>();
        canvas.planeDistance = change;
    }

    public void SetMenuBool(bool value) {

        frame.SetBool("Menu", value);

    }

    public bool GetMultiplayerMenu() {
        return MultiuplayerMenu.activeSelf;
    }

    public void SetMultiplayerMenu(GameObject obj) {
        MultiuplayerMenu = obj;
    }

    public void StartGame() {

        SetMenuBool(false);
        MultiuplayerMenu.SetActive(false);
        Image.SetActive(false);
        instance.OnChangeScreen(10);

        BoardManager.instance.enabled = true;

    }

    public void WonScreen(int whoWon) {
        if (whoWon == 1) {
            //X ganhou
            Ending.texture = EndingImages[whoWon - 1].texture;
        }
        else if (whoWon == 2) {
            //O Ganhou
            Ending.texture = EndingImages[whoWon - 1].texture;
        }
        else if (whoWon == 3) { 
            //Deu Velha Anim
            Ending.texture = EndingImages[whoWon - 1].texture;
        }
        else{
            //NormalState
            Ending.texture = EndingImages[3].texture;
        }
    }
}