using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
// eye tracker = convergence = Tobii
// AR/MR = Hololens
//MIRASCPE
// R/C image
//L R
// red channel/ left image
// green/blue channel/right image
//SBS image (Side by side)m
namespace UnityTemplateProjects
{
    public class CurrentState
    {
        //constructor
        public CurrentState()
        {
        }
        //method
        public void CurrentMovement(int parameter1, int parameter2)
        {
        // Console.WriteLine("First Parameter {0}, second parameter {1}",parameter1, parameter2);

        }
        //property
        public int Counter_x { get; set; }
        public int Counter_y { get; set; }
        public int Counter_z { get; set; }

    }
    public class SimpleCameraController : MonoBehaviour
    {
        CurrentState currents1 = new CurrentState ();

        Thread receiveThread;
        UdpClient client;
        int port;
        int change,value_fing=0;
        float input = 0.0f;
        float prev_w = 0.0f, prev_x = 0.0f, prev_y = 0.0f,diff_x=0.0f, diff_y = 0.0f, diff_z = 0.0f;
        int flag = 0;
        int[] array1 = { 0, 0, 0, 0 };
        public float movespeed;
        public int connectionPort = 8888;
        bool running, flag_update = false, flag_w = false;
        Vector3 direction_1 = new Vector3();
        bool flag_xmov = false, flag_ymov = false, flag_wmov = false;
        bool usemax_x = false, usemax_y = false, usemax_z = false;
        int ratio_x = 100, ratio_y = 100, ratio_z = 100;
        void Start()
        {
            port = 5065;   //Connected to the python code

            movespeed = 1f;
            InitUDP(); //4

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

                    while (text.Length > 0)
                    {
                        int index1 = text.IndexOf('_');
                        string prev = text.Substring(0, index1);
                        string sub = text.Substring(index1 + 1);
                        text = sub;
                        array1[cnt] = Convert.ToInt32(prev);
                        cnt = cnt + 1;
                    }

                    print(array1);
                    Int32.TryParse(text, out change);
                }
                catch (Exception e)
                {
                    print(e.ToString()); //7
                }
            }
        }
        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;
            public Vector3 CurCamState;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }
            public Vector3 GetCurrentCamera()
            {
                print("Current CameraState: ");
                CurCamState = new Vector3(x, y, z);
                return CurCamState;
            }

            public void SetCurrentCamera(float xval, float yval, float zval){
                x = x + xval;
                y = y - yval;
                z = z - zval;

            }
            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }

        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            return direction;
        }

        void Update()
        {
            print("update");
            // Exit Sample  
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            // Hide and lock cursor when right mouse button pressed
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (Input.GetMouseButton(1))
            {
                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            var translation = GetInputTranslationDirection() * Time.deltaTime;
            Vector3 camcur = m_TargetCameraState.GetCurrentCamera();
            if (flag_w == false)
            {
                if (flag_update != false && array1[0] != 0)
                {
                    prev_x = array1[0];
                    prev_y = array1[1];
                    prev_w = array1[2];
                    flag_w = true; 
                }
                else
                {
                    flag_update = true;
                }
            }
            // Translation
            if (flag_w == true)
            {
                
                diff_x = (array1[0] - prev_x);
                diff_y = (array1[1] - prev_y);
                diff_z = (array1[2] - prev_w);
                //save the current position
                prev_x = array1[0];
                prev_y = array1[1];
                prev_w = array1[2];
                usemax_x = Math.Abs(diff_x) > Math.Abs(diff_y) ? (Math.Abs(diff_x) > Math.Abs(diff_z) ? true : false) : false;
                usemax_y = Math.Abs(diff_y) > Math.Abs(diff_x) ? (Math.Abs(diff_y) > Math.Abs(diff_z) ? true : false) : false;
                usemax_z = Math.Abs(diff_z) > Math.Abs(diff_x) ? (Math.Abs(diff_z) > Math.Abs(diff_y) ? true : false) : false;
                if (usemax_x)
                {
                    print("x Max");
                    ratio_x = 100; ratio_y = 500; ratio_z = 500;
                }
                else if (usemax_y)
                {
                    print("y Max");
                    ratio_x = 500; ratio_y = 100; ratio_z = 500;
                }
                else if (usemax_z) {
                    print("z Max");
                    ratio_x = 500; ratio_y = 500; ratio_z = 100;
                }
                if (Math.Abs(diff_x )> 10 || Math.Abs(diff_y) > 10 || Math.Abs(diff_z) > 10)
                {
                    m_TargetCameraState.SetCurrentCamera(diff_x / ratio_x, diff_y / ratio_y, diff_z / ratio_z/**/);
                }
            }

            // Speed up movement when shift key held
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= 10.0f;
            }

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            boost += Input.mouseScrollDelta.y * 0.2f;
            //translation *= Mathf.Pow(2.0f, boost);
            translation = translation / 5000;
            m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }

        private void print(int v1, int v2, int v3)
        {
            throw new NotImplementedException();
        }
    }

}