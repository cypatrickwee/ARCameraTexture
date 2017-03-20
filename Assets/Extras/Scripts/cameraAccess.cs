using UnityEngine;
using System.Collections;
using Vuforia;

public class cameraAccess : MonoBehaviour
{
    private bool mAccessCameraImage = true;
    //private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGB888;

    private bool m_RegisteredFormat = false;
    private bool m_CanCapture = false;

    public GameObject mPlane; // plane to get texcoords
    private Texture2D mCameraTexture; // camera texture

    // The desired camera image pixel format
    private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGB888;// or RGBA8888, RGB888, RGB565, YUV
    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;
    void Start()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        //VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }
    /// <summary>
    /// Called when Vuforia is started
    /// </summary>
    private void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + m_PixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register pixel format " + m_PixelFormat.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }
    }
    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    /*private void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }*/
    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    /*private void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
                if (image != null)
                {
                    string imageInfo = mPixelFormat + " image: \n";
                    imageInfo += " size: " + image.Width + " x " + image.Height + "\n";
                    imageInfo += " bufferSize: " + image.BufferWidth + " x " + image.BufferHeight + "\n";
                    imageInfo += " stride: " + image.Stride;
                    Debug.Log(imageInfo);
                    byte[] pixels = image.Pixels;
                    if (pixels != null && pixels.Length > 0)
                    {
                        Debug.Log("Image pixels: " + pixels[0] + "," + pixels[1] + "," + pixels[2] + ",...");
                    }
                }
            }
        }
    }
    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }*/


    public void OnTrackablesUpdated()
    {

            if (!m_RegisteredFormat)
            {
                CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);
                m_RegisteredFormat = true;
            }
            if (m_CanCapture)
            {
                CameraDevice cam = CameraDevice.Instance;
                Image image = cam.GetCameraImage(m_PixelFormat);
                if (image == null)
                {
                    // not on mobile devices
                    Debug.Log(m_PixelFormat + " image is not available yet");
                }
                else
                {

                    // get MVP matrix
                    Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
                    Matrix4x4 V = Camera.main.worldToCameraMatrix;

                    // use plane to get texcoords for target
                    Matrix4x4 M = mPlane.GetComponent<Renderer>().localToWorldMatrix;
                    Matrix4x4 MVP = P * V * M;

                    // screen to camera image scale
                    float r1 = image.Height * 1.0f / image.Width;
                    float r2 = Screen.width * 1.0f / Screen.height;
                    if (r1 > r2)
                    {
                        GetComponent<Renderer>().material.SetFloat("_xScale", 1.0f);
                        GetComponent<Renderer>().material.SetFloat("_yScale", r2 / r1);
                    }
                    else
                    {
                        GetComponent<Renderer>().material.SetFloat("_xScale", r1 / r2);
                        GetComponent<Renderer>().material.SetFloat("_yScale", 1.0f);
                    }

                    GetComponent<Renderer>().material.SetMatrix("_MATRIX_MVP", MVP);

                    mCameraTexture = new Texture2D(128, 128, TextureFormat.RGB24, false);
                    image.CopyToTexture(mCameraTexture);
                    mCameraTexture.Apply(); // very important

                    // set camera texture
                    GetComponent<Renderer>().material.mainTexture = mCameraTexture;

                    m_CanCapture = false;
                }
            }
        }

    void OnGUI()
    {
        // button for test
        if (GUI.Button(new Rect(0, 0, 100, 100), mCameraTexture))
        {
            m_CanCapture = true;
        }
    }
}