using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{

    Canvas canvas;
    int imageOrder; //1 = background 2 = playtime
    public GameObject[] gameG;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (imageOrder == 1 )
        {

        }
        canvas.planeDistance = 1;
        
    }
}
