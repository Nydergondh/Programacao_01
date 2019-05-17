using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Movement : MonoBehaviour
{
    [Range(-10f, 10f)]
    [SerializeField] private float xSpeed = 5;

    [SerializeField] Transform trans = null;
    Vector3 pos;

    bool smoothSlow;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        smoothSlow = true;
    }

    // Update is called once per frame
    void Update()
    {
        pos.x += xSpeed * Time.deltaTime;
        transform.position = pos;
        ex10();
    }

    void ex1() {
        if (pos.x > 4.5f) {
            pos.x = -4.5f;
        }
        else if(pos.x < -4.5f) {
            pos.x = -4.5f;
        }
    }

    void ex2() {
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
    }

    void ex3() {
        float WaveLength = 5f;
        float WaveAmplitude = 5f;
        pos.z = Mathf.Sin(pos.x * 2 * Mathf.PI / WaveLength) * WaveAmplitude / 2;
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
    }

    void ex6() {
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
    }

    void ex7and8() {
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
        print(Mathf.Abs(trans.position.x - transform.position.x));
    }
    void ex9() {
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
        if (Mathf.Abs(trans.position.x - transform.position.x) < 2f) {
            Time.timeScale = 0.25f;
        }
        else {
            Time.timeScale = 1f;
        }
    }

    void ex10() {
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
        if (Mathf.Abs(trans.position.x - transform.position.x) < 4f && Time.timeScale > 0.25 && smoothSlow) {
            Time.timeScale -= 0.1f;
        }
        else if (Mathf.Abs(trans.position.x - transform.position.x) < 2f) {
            Time.timeScale = 0.25f;
            smoothSlow = false;
        }
        else if (Mathf.Abs(trans.position.x - transform.position.x) < 4f && Time.timeScale < 0.9 && !smoothSlow) {
            Time.timeScale += 0.1f;
            smoothSlow = false;
        }
        else {
            Time.timeScale = 1;
            smoothSlow = true;
        }
    }

    void ex11() {
        if (pos.x > 4.5f || pos.x < -4.5f) {
            xSpeed = -xSpeed;
        }
        if (Mathf.Abs(trans.position.x - transform.position.x) < 2f) {
            Time.timeScale = 0.25f;
        }
        else {
            Time.timeScale = 1f;
        }
    }

}
