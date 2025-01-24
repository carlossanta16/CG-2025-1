using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    public static float[] _samples = new float[512];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        /*
         *
         * 22050 / 512 = 43 Hertz Per Sample
         * 
         *20-60 Hz
         *60-250 Hz
         *250-500 Hz
         *2000-4000 Hz
         *4000-6000 Hz
         *6000-20000 Hz
         *
         * 0 - 2 = 86 Hz 
         * 1 - 4 = 172 Hz - 87-258
         * 2 - 8 = 344 Hz - 259-602
         * 3 - 16 = 688 Hz - 603-1290
         * 4 - 32 = 1376 Hz - 1291-2666
         * 5 - 64 = 2752 Hz - 2667-5418
         * 6 - 128 = 5504 Hz - 5419-10922
         * 7 - 256 = 11008 Hz - 10923-21930
         * 510
         */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            
        }


    }
    
}
