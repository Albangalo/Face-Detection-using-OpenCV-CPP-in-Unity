using UnityEngine;
using System;

namespace Assets.HolographicDisplay
{
    /// <summary>
    /// Script that emulates a 3D holographic display based on the viewer position.
    /// Usage:
    /// - Attach to a camera.
    /// - Update the HeadPosition each frame either in this or in an external script based on some
    ///   form of headtracking, like using the awesome Kinect plugins of Rumen Filkov.
    /// - For best effect - and if available - use a stereoscopic display and calculate the head 
    ///   position twice by simply offsetting the HeadPosition .03 to the left and to the right for
    ///   each of the views.
    /// </summary>
    class SimpleHolographicCamera : MonoBehaviour
    {
        public float ScreenWidth = 0.88f;
        public float ScreenHeight = 0.50f;
        public Vector3 HeadPosition;

        private float left = -0.2F;
        private float right = 0.2F;
        private float top = 0.2F;
        private float bottom = -0.2F;

        private PositionAtFaceScreenSpace facePosScript;

        /// <summary>
        /// Updates the projection matrix and camera position to get the correct anamorph perspective
        /// </summary>
        void LateUpdate()
        {
            facePosScript = GetComponent<PositionAtFaceScreenSpace>();
            HeadPosition = facePosScript.GetHeadPos();
            Camera cam = GetComponent<Camera>();
            left = cam.nearClipPlane * (-(ScreenWidth / 2) - HeadPosition.x) / HeadPosition.z;
            right = cam.nearClipPlane * (ScreenWidth / 2 - HeadPosition.x) / HeadPosition.z;
            bottom = cam.nearClipPlane * (-(ScreenHeight / 2) - HeadPosition.y) / HeadPosition.z;
            top = cam.nearClipPlane * (ScreenHeight / 2 - HeadPosition.y) / HeadPosition.z;
            cam.transform.position = new Vector3(-HeadPosition.x, HeadPosition.y, HeadPosition.z);
            cam.transform.LookAt(new Vector3(-HeadPosition.x, HeadPosition.y, 0));

            Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
            cam.projectionMatrix = m;
        }

        /// <summary>
        /// Calculates the camera projection matrix
        /// </summary>
        /// <returns>The off center.</returns>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <param name="bottom">Bottom.</param>
        /// <param name="top">Top.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float x = 2.0F * near / (right - left);
            float y = 2.0F * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }
    }
}
