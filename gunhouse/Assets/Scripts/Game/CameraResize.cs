using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gunhouse
{
    public class CameraResize : MonoBehaviour
    {
        Camera cameraToAdjust;

//        enum AspectRatio { SixteenByNine, FourByThree };
//        [SerializeField] AspectRatio aspectRatio;

        void Awake()
        {
            cameraToAdjust = GetComponent<Camera>();
        }

        void Update()
        {
//            ResizeLetterboxing();

            /* NOTE(shane): the orthographic size is adjusted for every screen ratio.
                if a screen ratio is not 16:9 than we place some letter boxing to hide
                the edges of the screen. */
            cameraToAdjust.orthographicSize = (490.5f / Screen.width) * Screen.height;
        }

//        void ResizeLetterboxing()
//        {
//            float targetaspect = aspectRatio == AspectRatio.SixteenByNine ? 16.0f / 9.0f : 4.0f / 3.0f;
//            float windowaspect = (float)Screen.width / (float)Screen.height;
//            float scaleheight = windowaspect / targetaspect;
//
//            if (scaleheight < 1.0f) {
//                cameraToAdjust.rect = new Rect(0, (1 - scaleheight) * .5f, 1, scaleheight);
//            }
//            else {
//                float scalewidth = 1 / scaleheight;
//                cameraToAdjust.rect = new Rect((1 - scalewidth) * .5f, 0, scalewidth, 1);
//            }
//        }
    }
}