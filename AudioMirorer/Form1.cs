using System;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Drawing;

namespace AudioMirorer
{
    public partial class Form1 : Form
    {
        private AudioMirror mirror = new AudioMirror();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // List all output devices (Render endpoints)
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var device in devices)
            {
                comboOutput.Items.Add(device.FriendlyName);
            }

            if (comboOutput.Items.Count > 0)
                comboOutput.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (comboOutput.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an output device.");
                return;
            }
            bool success = mirror.Start(comboOutput.SelectedIndex);

            if (success)
            {
                this.BackColor = Color.LightGreen;
            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            mirror.Stop();
            this.BackColor = SystemColors.ActiveCaption;
            System.Media.SystemSounds.Beep.Play();
        }

        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            mirror.SetVolume(trackVolume.Value / 100f);
        }
    }
}
