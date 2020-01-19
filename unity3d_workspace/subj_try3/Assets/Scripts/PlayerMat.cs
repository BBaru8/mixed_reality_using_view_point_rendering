using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PlayerMat : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public float movespeed;
    public float xAngle, yAngle, zAngle;
    public Color altColor = Color.black;
    public Renderer rend;
    int port; int change;int value_fing = 0;


    void Example()
    {
        altColor.g = 0f;
        altColor.r = 0f;
        altColor.b = 0f;
        altColor.a = 0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        port = 5061; //Connected to the python code

        //Call Example to set all color values to zero.
        Example();
        movespeed = 2f;
        //Get the renderer of the object so we can access the color
        rend = GetComponent<Renderer>();
        //Set the initial color (0f,0f,0f,0f)
        rend.material.color = altColor;
        InitUDP();
        print("Start");

    }
    private void InitUDP()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData)); //1 
        receiveThread.IsBackground = true; //2
        receiveThread.Start(); //3
        print("InitUDP");

    }
    

    private void ReceiveData()
    {
        print("ReceiveData");

        client = new UdpClient(port); //1
        while (true) //2
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //3
                byte[] data = client.Receive(ref anyIP); //4

                string text = Encoding.UTF8.GetString(data); //5
                int cnt = 0;

                value_fing = Convert.ToInt32(text);
                print(">>: " + value_fing);
                Int32.TryParse(text, out change);
            }
            catch (Exception e)
            {
                print(e.ToString()); //7
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        float xAngle = 0.0f, yAngle = 0.0f;
        transform.Translate(movespeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, movespeed * Input.GetAxis("Vertical") * Time.deltaTime);
        if (value_fing == 1)
        {
             xAngle = movespeed;//* Input.GetAxis("Mouse X");

        }
        if (value_fing == 3)
        {
             yAngle = movespeed; //* Input.GetAxis("Mouse Y");
        }
        print(xAngle.ToString());
        print(yAngle.ToString());

        Vector3 lookhere = new Vector3(yAngle, xAngle, 0);
        transform.Rotate(lookhere);
        if (value_fing == 2)
        {
                //Alter the color          
                altColor.g += 0.1f;
                //Assign the changed color to the material.
                rend.material.color = altColor;
        }
        if (value_fing == 4)
        {
                //Alter the color          
                altColor.r += 0.1f;
                //Assign the changed color to the material.
                rend.material.color = altColor;
        }
        if (value_fing == 6)
        {
                //Alter the color            
                altColor.b += 0.1f;
                //Assign the changed color to the material.
                rend.material.color = altColor;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //Alter the color          
            altColor.a += 0.1f;
            //Assign the changed color to the material.
            rend.material.color = altColor;
        }



    }
}