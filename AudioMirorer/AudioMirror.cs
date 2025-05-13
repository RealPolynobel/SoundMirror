using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Windows.Forms;

public class AudioMirror
{
    private WasapiLoopbackCapture capture;
    private WasapiOut output;
    private BufferedWaveProvider buffer;
    private VolumeSampleProvider volumeProvider;
    private float volume = 1.0f;

    public bool Start(int outputIndex)
    {
        Stop(); // Ensure clean start

        // Setup capture
        capture = new WasapiLoopbackCapture();

        buffer = new BufferedWaveProvider(capture.WaveFormat)
        {
            DiscardOnBufferOverflow = true,
            BufferDuration = TimeSpan.FromMilliseconds(1000) // Helps prevent stuttering
        };

        // Setup output
        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        if (outputIndex >= devices.Count)
        {
            MessageBox.Show("Selected output device index is out of range.");
            return false;
        }

        MMDevice outputDevice = devices[outputIndex];
        MMDevice defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        // Check if trying to mirror to the same device being captured
        if (outputDevice.ID == defaultDevice.ID)
        {
            MessageBox.Show("Cannot mirror to the same output device that is being captured.");
            return false;
        }

        output = new WasapiOut(outputDevice, AudioClientShareMode.Shared, false, 300);

        // Use SampleProvider for smooth volume adjustment
        var sampleProvider = buffer.ToSampleProvider();
        volumeProvider = new VolumeSampleProvider(sampleProvider);
        volumeProvider.Volume = volume;

        output.Init(volumeProvider);

        capture.DataAvailable += (s, a) =>
        {
            buffer.AddSamples(a.Buffer, 0, a.BytesRecorded);
        };

        try
        {
            output.Play();
            capture.StartRecording();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error starting audio mirroring:\n" + ex.Message);
            Stop();
            return false;
        }
    }

    public void SetVolume(float vol)
    {
        volume = vol;
        if (volumeProvider != null)
            volumeProvider.Volume = volume;
    }

    public void Stop()
    {
        capture?.StopRecording();
        capture?.Dispose();
        capture = null;

        output?.Stop();
        output?.Dispose();
        output = null;
    }
}
