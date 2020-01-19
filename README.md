# Mixed Reality with view point rendering using user interaction.
This is a class project implemented by Bithi Barua, Jaideep Bommidi,  Kazi Ahmed Asif Fuad and Rupayan Mallick for the course Augmented and Virtual Reality. 
This project consists of applications of Computer Graphics and Computer Vision. It is implemented using Unity3D(Version 2018.4.10f1), OpenCV(Version 4.0.0) and Python 3.6.
[//]: # (References)
[left1]: ./images/left_1.png
[left2]: ./images/left_2.png
[right1]: ./images/right_1.png
[right2]: ./images/right_2.png
[red_color_1]: ./images/red_color_1.png
[red_color_2]: ./images/red_color_2.png
[rotate_x_1]: ./images/rotate_x_1.png
[rotate_x_2]: ./images/rotate_x_2.png

[haar-like-features]: https://www.quora.com/How-can-I-understand-Haar-like-feature-for-face-detection
[face_detection]: https://github.com/alex-lechner/Face-Eyes-Smile-Detection
[hand_gesture_recognition]: https://github.com/alex-lechner/Face-Eyes-Smile-Detection

To Run this project, at you will have to open both Unity3D (unity3d_workspace) and Python workspace(python-code-facetracking-hand-gesture). 
First, run the python code for Face Detection and Hand Gesture Recognition and then play the gaming mode in the Unity3D workspace immediately.
 
 Face Movement      |  View point rendering in Unity3d
:---------------------------------:|:---------------------------------:
![left1][left1]                    | ![left2][left2]
![right1][right1]                  | ![right2][right2]

 Hand Gesture      |  View point rendering in Unity3d
:---------------------------------:|:---------------------------------:
![red_color_1][red_color_1]        | ![red_color_2][red_color_2]
![rotate_x_1][rotate_x_1]          | ![rotate_x_2][rotate_x_2]
  
## View Point Rendering using Unity 3D
To visualise a scenario we used view point rendering in Unity3D. You can open the workspace using SampleScene.unity (located in unity3d_workspace\subj_try3\Assets\Scenes) 

## Installation of Unity3D
Unity3D is free for personal use. You can download it from 	https://unity3d.com/get-unity/download/ . Just download Download Unity Hub and install the latest Unity3D and take license using you credential.  
You will need Visual Studio as well for C# which you can download from https://visualstudio.microsoft.com/downloads/ . Community version is free of use.

## Face Detection and Hand Gesture Recognition
For Face Detection and Hand Gesture Recognition, project uses [Haar-like features][haar-like-features]. It is done in Python 3.6 and uses the open source computer vision library [OpenCV][opencv]. To install OpenCV for Python please follow the [installation process below][installation].
To do this two gits have been merged. They are avaibale at [face detection][face_detection] and [hand gesture recognition][hand_gesture_recognition]
By executing the script the built-in webcam gets started and draws blue rectangles around faces, green rectangles around eyes and red rectangles around smiling mouths in the webcam video stream.
Using hand gesture, we are counting fingers to take action.
To start the Face Detection and Hand Gesture Recognition clone this repository, open a terminal/command window and execute the following line in the root folder of this project:

```sh
python face_track_hand_gesture.py
```

If you don't have an internal webcam and you want to use an external webcam simply change the parameter of the `cv2.VideoCapture()` function on line 90 in `face_detection.py` from `0` to `1` like so:

```python
# 0 = internal webcam, 1 = external webcam
VIDEO_CAPTURE = cv2.VideoCapture(1)
```

To close and exit the webcam video stream press the **`Esc`**-key on your keyboard*.

*while the window of your webcam video stream is active

## Installation of OpenCV
To install the OpenCV library for Python execute the following line in a terminal/command window:
```sh
pip install opencv-python
```

If you are on Windows and the line above does not work then download the OpenCV wheel from the [Unofficial Windows Binaries for Python Extension Packages Website][win-open-cv-wheel].

Because this project was done in Python 3.6 you need to download either `opencv_python‑3.4.3‑cp36‑cp36m‑win32.whl` for a 32-bit operating system or `opencv_python‑3.4.3‑cp36‑cp36m‑win_amd64.whl` for a 64-bit operating system. 

Note: `cp36` stands for the Python version 3.6 so if you are using Python 3.7 you will need to look for `cp37` in the filename.

After you have downloaded the proper file you need to navigate to the location where this file was downloaded (probably your Downloads-folder) and open the command window in this folder. Then execute the following line:
```sh
# Python 3.6 for 32-bit OS
pip install opencv_python‑3.4.3‑cp36‑cp36m‑win32.whl

# Python 3.6 for 64-bit OS
pip install opencv_python‑3.4.3‑cp36‑cp36m‑win_amd64.whl
```



