using NAudio.Wave;
using System;

public class SineWaveProvider32 : WaveProvider32
{
    private int sample;
    private float frequency;
    private float amplitude;

    public SineWaveProvider32(float frequency = 440f, float amplitude = 0.25f)
    {
        this.SetWaveFormat(44100, 1);
        this.frequency = frequency;
        this.amplitude = amplitude;
    }

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        int sampleRate = WaveFormat.SampleRate;

        for (int n = 0; n < sampleCount; n++)
        {
            buffer[n + offset] = (float)(amplitude * Math.Sin((2 * Math.PI * sample * frequency) / sampleRate));
            sample++;
            if (sample >= sampleRate) sample = 0;
        }

        return sampleCount;
    }
}
