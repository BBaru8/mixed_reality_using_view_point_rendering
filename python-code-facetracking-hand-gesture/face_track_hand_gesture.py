"""
This is the python code for Face recognition and tracking and Hand Gesture Recogtion. Both of these implementations are based on haar-like features. 
We have made this code from two githyub repo: Face: https://github.com/alex-lechner/Face-Eyes-Smile-Detection and 
hand: https://github.com/srvindian/handgesture_fingers_count  
"""
import os
import cv2
import socket
import time
# Get the path of the this script
CURRENT_FILE_PATH = os.path.dirname(__file__)

# Load the haar-like features
# face,eye and smile 
FACE_CASCADE = cv2.CascadeClassifier(os.path.join(
    CURRENT_FILE_PATH, 'haarcascade_frontalface_default.xml'))
EYE_CASCADE = cv2.CascadeClassifier(
    os.path.join(CURRENT_FILE_PATH, 'haarcascade_eye.xml'))
SMILE_CASCADE = cv2.CascadeClassifier(
    os.path.join(CURRENT_FILE_PATH, 'haarcascade_smile.xml'))
#fingertip 
cascade=cv2.CascadeClassifier('fingertip_cascade.xml')


def face_detection(bw_img, orig_img):
    """Takes the black-white version of an image and the original image and
    performs face, eye and smile detection on the black-white image. The detected features
    are drawn with rectangles on the original image.

    :param bw_img: black and white image from original image
    :param orig_img: original image
    :type bw_img: <class 'numpy.ndarray'>
    :type orig_img: <class 'numpy.ndarray'>
    :return: returns orig_img with rectangles on regions of interest
    :rtype: <class 'numpy.ndarray'>
    """

    faces = FACE_CASCADE.detectMultiScale(bw_img, 1.3, 5)

    for fx, fy, fw, fh in faces:
        cv2.rectangle(orig_img, (fx, fy), (fx+fw, fy+fh), (255, 0, 0), 2)
        region_of_interest_bw = bw_img[fy:fy+fh, fx:fx+fw]
        region_of_interest_color = orig_img[fy:fy+fh, fx:fx+fw]
        eyes = EYE_CASCADE.detectMultiScale(region_of_interest_bw, 1.1, 22)
        for ex, ey, ew, eh in eyes:
            cv2.rectangle(region_of_interest_color, (ex, ey),
                          (ex+ew, ey+eh), (0, 255, 0), 2)
        smiles = SMILE_CASCADE.detectMultiScale(region_of_interest_bw, 1.7, 22)
        for sx, sy, sw, sh in smiles:
            cv2.rectangle(region_of_interest_color, (sx, sy),
                          (sx+sw, sy+sh), (0, 0, 255), 2)
        sock.sendto( (str(fx)+"_"+str(fy)+"_"+str(fw)+"_"+str(fh)+"_"+str(fh)).encode(), (UDP_IP, UDP_PORT_1) )
    bw_img_1	=	cv2.resize(bw_img, None, fx=scaling_factor, fy=scaling_factor, interpolation=cv2.INTER_AREA)
    gray = bw_img_1
    n=0	
    n1=0
    try:
        rects=cascade.detectMultiScale(gray,1.2,20)
        for(x,y,w,h)	in	rects:
            n1+=1
        sock.sendto((str(n1)).encode(), (UDP_IP, UDP_PORT_2))		
        for(x,y,w,h)	in	rects:					
            cv2.rectangle(orig_img,(x,y),(x+w,y+h),(0,255,0),2)
            n+=1
            cv2.putText(orig_img, str(n), (x+np.uint8(w/2)-8,y+np.uint8(h/2)+5), cv2.FONT_HERSHEY_SIMPLEX,.8, (255,0,0), 2)
            cv2.circle(orig_img,(x+np.uint8(w/2),y+np.uint8(h/2)),2,(0,0,255),-1)

    except:
        pass
    orig_img=cv2.flip(orig_img,2)			
    cv2.putText(orig_img, str(n1), (10,450), cv2.FONT_HERSHEY_SIMPLEX,5, (200,255,120), 2)
    return orig_img


def make_screenshot(img, counter):
    """Takes a screenshot and saves the frame/image

    :param img: colored image of the video capture process
    :param counter: increased counter so the previously saved images aren't overwritten
    :type img: <class 'numpy.ndarray'>
    :type counter: int
    """
    img_path = os.path.join(CURRENT_FILE_PATH, 'imgs')
    if not os.path.exists(img_path):
        os.makedirs(img_path)
    cv2.imwrite(os.path.join(
        img_path, 'screenshot-{0}.jpeg'.format(counter)), img)


def start_video_capturing(video_capture):
    """Starts the webcam and captures a video stream.
    Press 's' to take a screenshot of the video stream.
    Press 'Esc' to close and exit the video stream.

    :param video_capture: video stream object of the webcam
    :type video_capture: <class 'cv2.VideoCapture'>
    """
    screenshot_counter = 0
    while True:
        _, img = video_capture.read()
        bw_img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        canvas = face_detection(bw_img, img)
        cv2.imshow('Video', canvas)
        k = cv2.waitKey(1)
        if k == ord('s'):
            make_screenshot(img, screenshot_counter)
            screenshot_counter += 1
        elif k == 27:
            break

    video_capture.release()
    cv2.destroyAllWindows()


if __name__ == '__main__':
    UDP_IP = "127.0.0.1"
    UDP_PORT_1 = 5065 #Face recognition and tracking
    UDP_PORT_2 = 5061 #Hand Gesture recognition
    scaling_factor=1
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    VIDEO_CAPTURE = cv2.VideoCapture(0) # 0 = built-in webcam, 1 = external webcam
    start_video_capturing(VIDEO_CAPTURE)
