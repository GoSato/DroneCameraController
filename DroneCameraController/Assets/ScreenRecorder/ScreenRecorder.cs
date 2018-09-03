/* 
*   NatCorder
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatCorderU.Examples
{

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Core;
    using Core.Recorders;

    public class ScreenRecorder : MonoBehaviour
    {

        /**
        * ReplayCam Example
        * -----------------
        * This example records the screen using a `CameraRecorder`.
        * When we want mic audio, we play the mic to an AudioSource and record the audio source using an `AudioRecorder`
        * -----------------
        * Note that UI canvases in Overlay mode cannot be recorded, so we use a different mode (this is a Unity issue)
        */

        [SerializeField]
        private int _width = 1920;
        [SerializeField]
        private int _height = 1080;

        [Header("Recording")]
        public Container container = Container.MP4;

        [Header("Microphone")]
        public bool recordAudio;
        public AudioSource audioSource;

        private CameraRecorder videoRecorder;
        private AudioRecorder audioRecorder;

        public void StartRecording()
        {
            // First make sure recording microphone is only on MP4
            recordAudio &= container == Container.MP4;
            // Create recording configurations // Clamp video width to 720
            var width = _width;
            var height = _height;
            var framerate = container == Container.GIF ? 10 : 30;
            var videoFormat = new VideoFormat(width, (int)height, framerate);
            var audioFormat = recordAudio ? AudioFormat.Unity : AudioFormat.None;
            // Start recording
            NatCorder.StartRecording(container, videoFormat, audioFormat, OnReplay);
            videoRecorder = CameraRecorder.Create(Camera.main);
            // If recording GIF, skip a few frames to give a real GIF look
            if (container == Container.GIF)
                videoRecorder.recordEveryNthFrame = 5;
            // Start microphone and create audio recorder
            if (recordAudio)
            {
                audioRecorder = AudioRecorder.Create(audioSource, false);
            }
        }

        public void StopRecording()
        {
            // Stop the microphone if we used it for recording
            if (recordAudio)
            {
                audioRecorder.Dispose();
            }

            // Stop the recording
            videoRecorder.Dispose();
            NatCorder.StopRecording();
        }

        void OnReplay(string path)
        {
            Debug.Log("Saved recording to: " + path);
            // Playback the video
#if UNITY_IOS
            Handheld.PlayFullScreenMovie("file://" + path);
#elif UNITY_ANDROID
            Handheld.PlayFullScreenMovie(path);
#endif
        }
    }
}