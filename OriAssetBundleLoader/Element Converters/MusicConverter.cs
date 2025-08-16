using Il2Cpp;
using Il2CppMoon.Wwise;
using MelonLoader;
using System.Collections;
using UnityEngine;

public class MusicConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        // Silences the music
        SoundZoneTrigger silenceTrigger = PrefabCachingManager.GetPrefab("musicToSilence").GetComponent<SoundZoneTrigger>();
        silenceTrigger.TriggerOnce = false;
        silenceTrigger.Triggered();

        // Loads the music audio clip
        //AudioSource musicSource = Asset.GetComponent<AudioSource>();
        //if (musicSource != null)
        //{
        //    MelonLogger.Msg("Got audio source");

        //    if(musicSource.clip != null)
        //    {
        //        MelonLogger.Msg("Got audio clip: " + musicSource.clip.name);

        //        //musicSource.Play();
        //    }

        //    MelonCoroutines.Start(loadAndPlayMusic(musicSource));
        //}
    }

    public IEnumerator loadAndPlayMusic(AudioSource audioSource)
    {
        MelonLogger.Msg("Loading music file...");

        WWW www = new WWW("file:///test.wav");
        yield return www;

        if (www.error != null)
        {
            MelonLogger.Error(www.error);
            yield break;
        }

        MelonLogger.Msg("Loaded music file!");

        WAV wav = new WAV(www.bytes);

        MelonLogger.Msg(wav.ToString());
        MelonLogger.Msg("Left Channel Length: " + wav.LeftChannel.Length);
        MelonLogger.Msg("Right Channel Length: " + wav.RightChannel.Length);

        MelonLogger.Msg("Creating audio clip from wav bytes...");

        AudioClip audioClip = AudioClip.Create("testMusic", wav.SampleCount, 2, wav.Frequency, false);

        float[] combinedChannels = new float[wav.LeftChannel.Length + wav.RightChannel.Length];
        int pointer = 0;
        int lpointer = 0;
        int rpointer = 0;
        while (pointer < combinedChannels.Length)
        {
            combinedChannels[pointer] = wav.LeftChannel[lpointer];
            lpointer++;
            pointer++;
            combinedChannels[pointer] = wav.RightChannel[rpointer];
            rpointer++;
            pointer++;
        }
        audioClip.SetData(combinedChannels, 0);

        audioClip.GetData(combinedChannels, 0);
        MelonLogger.Msg("Commbined channels: " + combinedChannels.Length);

        MelonLogger.Msg("Playing audio clip in 3 seconds...");

        //AkSoundEngine.PostEvent("CustomLevelMusic", gameObject);

        audioSource.clip = audioClip;
        audioSource.PlayDelayed(3f);
    }
}

/* From http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html */
public class WAV
{

    // convert two bytes to one float in the range -1 to 1
    static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    static int bytesToInt(byte[] bytes, int offset = 0)
    {
        int value = 0;
        for (int i = 0; i < 4; i++)
        {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }
    // properties
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    public WAV(byte[] wav)
    {

        // Determine if mono or stereo
        ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

        // Get the frequency
        Frequency = bytesToInt(wav, 24);

        // Get past all the other sub chunks to get to the data subchunk:
        int pos = 12;   // First Subchunk ID from 12 to 16

        // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        // Pos is now positioned to start of actual sound data.
        SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
        if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

        // Allocate memory (right will be null if only mono sound)
        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        // Write to double array/s:
        int i = 0;
        while (pos < SampleCount)
        {
            LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2)
            {
                RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }
    }

    public override string ToString()
    {
        return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
    }
}