using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Sanford.Multimedia;
using Sanford.Multimedia.Midi;

namespace TestForm
{
    public enum KeyOf
    {
        Eb, // 3 flats
        Bb, // 2 flats
        F,  // 1 flat
        C,
        G,  // 1 sharp
        D,  // 2 sharps
    }

    public struct MidiNote
    {
        public const int
            C0 = 24,
            C1 = 36,

            A2 = 45,
            B2 = 47,
            C2 = 48,
            D2 = 50,
            E2 = 52,
            F2 = 53,
            G2 = 55,

            A3 = 57,
            B3 = 59,
            C3 = 60,

            A4 = 69,
            B4 = 71,
            C4 = 72,
            D4 = 74,
            E4 = 76,
            F4 = 77,
            G4 = 79,

            C5 = 84;
    }

    public struct Sign
    {
        public const int
            None = -2,
            Flat = -1,
            Natural = 0,
            Sharp = 1;
    }

    public partial class Form1 : Form
    {
        private const int SysExBufferSize = 4096;
        private KeyOf curKey = KeyOf.C;
        private Pen blackPen = new Pen(Color.Black, 1);
        private Bitmap bmpNote = null;
        private Bitmap bmpSharp = null;
        private Bitmap bmpFlat = null;
        private Bitmap bmpNatural = null;
        readonly string[] notes = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        private readonly string[] fiths = { "C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F" };
        readonly List<int> showNotes = new List<int>();
        private readonly int bY;
        private readonly int sY = 3;
        DateTime startTime = DateTime.Now;


        private InputDevice inDevice = null;

        private SynchronizationContext context;


        public Form1()
        {
            InitializeComponent();

            bmpNote = Properties.Resources.Note;
            bmpNote.MakeTransparent(Color.White);
            bmpSharp = Properties.Resources.Sharp;
            bmpSharp.MakeTransparent(Color.White);
            bmpFlat = Properties.Resources.Flat;
            bmpFlat.MakeTransparent(Color.White);
            bmpNatural = Properties.Resources.Natural;
            bmpNatural.MakeTransparent(Color.White);

            foreach (KeyOf k in Enum.GetValues(typeof(KeyOf)))
            {
                switch (k)
                {
                    case KeyOf.Eb:
                        KeyCombo.Items.Add("Eb : 3 Flats");
                        break;
                    case KeyOf.Bb:
                        KeyCombo.Items.Add("Bb : 2 Flats");
                        break;
                    case KeyOf.F:
                        KeyCombo.Items.Add("F  : 1 Flat");
                        break;
                    case KeyOf.C:
                        KeyCombo.Items.Add("C");
                        break;
                    case KeyOf.G:
                        KeyCombo.Items.Add("G  : 1 Sharp");
                        break;
                    case KeyOf.D:
                        KeyCombo.Items.Add("D  : 2 Sharps");
                        break;
                }
            }
            KeyCombo.SelectedIndex = (int)curKey;

            bY = (StafPanel.Height - Properties.Resources.Clef.Height) / 2 + 139;

        }

        protected override void OnLoad(EventArgs e)
        {
            if (InputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI input devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }
            else
            {
                try
                {
                    context = SynchronizationContext.Current;

                    inDevice = new InputDevice(0);
                    inDevice.ChannelMessageReceived += HandleChannelMessageReceived;
                    inDevice.Error += new EventHandler<ErrorEventArgs>(inDevice_Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Close();
                }
            }

            try
            {
                inDevice.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (inDevice != null)
            {
                try
                {
                    inDevice.StopRecording();
                    inDevice.Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                inDevice.Close();
            }

            base.OnClosed(e);
        }

        private void inDevice_Error(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Error.Message, "Error!",
                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {

                if (e.Message.Command == ChannelCommand.NoteOn && e.Message.Data2 != 0)
                {
                    if (startTime.AddMilliseconds(100) <= DateTime.Now)
                    {
                        ClearStaff();
                    }
                    startTime = DateTime.Now;
                    int n = e.Message.Data1;
                    showNotes.Add(n);
                    NoteLable.Text = string.Format("{0} {1}", notes[n % 12], n);
                    Refresh();
                }
            }, null);
        }

        private void ClearClef()
        {
            //System.Drawing.Graphics graphics = StafPanel.CreateGraphics();
            //graphics.DrawImageUnscaled(Properties.Resources.Note, 0, 0);
        }

        private void ClearStaff()
        {
            showNotes.Clear();
            ClearClef();
        }

        // GetNotePos
        // - returns the position of note n on the staff
        private int GetNotePos(KeyOf k, int n)
        {
            // Cakewalk
            //double [] posBb = {                                     0, 1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 7};
            //double [] posCn = {                               0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 6, 6};
            //double [] posDn = {                         0, 0, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6};
            //double [] posEb = {                  -1, 0, 1, 1, 2, 3, 3, 4, 4, 5, 5, 6  7};
            //double [] posFn = {                0, 0, 1, 2, 2, 3, 4, 4, 5, 5, 6, 6};
            //double [] posGn = {          0, 0, 1, 1, 2, 3, 3, 4, 5, 5, 6, 6};
            //double [] posAn = {    0, 0, 1, 1, 2, 2, 3, 4, 4, 5, 6, 6};
            //double [] posAb = {    0, 1, 1, 2, 2, 3, 4, 4, 4, 5, 6, 6};
            // Cubase
            // 0- s0110101011001101010110
            // 1# 01011010110
            // 2# 01011010101
            // 3# 01010110101
            // 4# 11010110101
            double[] posF = { 0, 0, 1, 2, 2, 3, 3, 4, 5, 5, 6, 6 };
            double[] posC = { 0, 0, 1, 2, 2, 3, 3, 4, 4, 5, 6, 6 };
            double[] posS = { 0, 0, 1, 1, 2, 3, 3, 4, 4, 5, 6, 6 };

            double[] pos = posC;
            switch (k)
            {
                case KeyOf.Eb:
                case KeyOf.Bb:
                case KeyOf.F:
                    pos = posF;
                    break;
                case KeyOf.D:
                case KeyOf.G:
                    pos = posS;
                    break;
            }
            return (int)(pos[n % 12]) + (n / 12) * 7;
        }

        private int GetNotePosY(KeyOf k, int n)
        {
            return bY - GetNotePos(k, n) * sY;
        }

        private void DrawSign(Graphics g, int x, int y, int m)
        {
            switch (m)
            {
                case Sign.Flat:
                    g.DrawImageUnscaled(bmpFlat, x - 10, y - 7);
                    break;
                case Sign.Natural:
                    g.DrawImageUnscaled(bmpNatural, x - 10, y - 5);
                    break;
                case Sign.Sharp:
                    g.DrawImageUnscaled(bmpSharp, x - 10, y - 5);
                    break;
            }
        }

        private int GetNoteSign(KeyOf k, int n)
        {
            n %= 12;
            switch (k)
            {
                case KeyOf.Eb:
                    switch (n)
                    {
                        case 0: return Sign.None;
                        case 1: return Sign.Flat;
                        case 2: return Sign.None;
                        case 3: return Sign.None;
                        case 4: return Sign.Natural;
                        case 5: return Sign.None;
                        case 6: return Sign.Flat;
                        case 7: return Sign.None;
                        case 8: return Sign.None;
                        case 9: return Sign.Natural;
                        case 10: return Sign.None;
                        case 11: return Sign.Natural;
                    }
                    break;
                case KeyOf.Bb:
                    switch (n)
                    {
                        // Natural
                        case 4:
                        case 11:
                            return Sign.Natural;
                        case 1:
                            return Sign.Sharp;
                        case 6:
                        case 8:
                            return Sign.Flat;
                    }
                    break;
                case KeyOf.F:
                    switch (n)
                    {
                        // Natural
                        case 11:
                            return Sign.Natural;
                        // Flat
                        case 1:
                        case 3:
                        case 6:
                        case 8:
                            return Sign.Flat;
                    }
                    break;
                case KeyOf.C:
                    switch (n)
                    {
                        // Sharp
                        case 1:
                        case 3:
                        case 6:
                            return Sign.Sharp;
                        case 8:
                        case 10:
                            return Sign.Flat;
                    }
                    break;
                case KeyOf.G:
                    switch (n)
                    {
                        // Natural
                        case 5:
                            return Sign.Natural;
                        // Sharp
                        case 1:
                        case 3:
                            return Sign.Sharp;
                        case 8:
                        case 10:
                            return Sign.Flat;
                    }
                    break;
                case KeyOf.D:
                    switch (n)
                    {
                        // Natural
                        case 0:
                        case 5:
                            return Sign.Natural;
                        // Sharp
                        case 3:
                        case 8:
                        case 10:
                            return Sign.Sharp;
                    }
                    break;
            }
            return Sign.None;
        }

        private int GetKeySign(KeyOf k, int n)
        {
            n = n % 12;
            switch (k)
            {
                case KeyOf.Eb:
                    switch (n)
                    {
                        case 3: return Sign.Flat;
                        case 8: return Sign.Flat;
                        case 10: return Sign.Flat;
                    }
                    break;
                case KeyOf.Bb:
                    switch (n)
                    {
                        case 3: return Sign.Flat;
                        case 8: return Sign.Flat;
                    }
                    break;
                case KeyOf.F:
                    switch (n)
                    {
                        case 3: return Sign.Flat;
                    }
                    break;
                case KeyOf.C:
                    break;
                case KeyOf.G:
                    switch (n)
                    {
                        case 5: return Sign.Sharp;
                    }
                    break;
                case KeyOf.D:
                    switch (n)
                    {
                        case 0: return Sign.Sharp;
                        case 5: return Sign.Sharp;
                    }
                    break;
            }
            return Sign.None;
        }

        private void ShowSign(Graphics g, int x, int y, int n)
        {
            DrawSign(g, x, y, GetNoteSign(curKey, n));
        }

        private void ShowKey(Graphics g)
        {
            switch (curKey)
            {
                case KeyOf.Eb:
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.B4), Sign.Flat);
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.B2), Sign.Flat);
                    DrawSign(g, 47 + 8, GetNotePosY(KeyOf.C, MidiNote.E4), Sign.Flat);
                    DrawSign(g, 47 + 8, GetNotePosY(KeyOf.C, MidiNote.E2), Sign.Flat);
                    DrawSign(g, 47 + 16, GetNotePosY(KeyOf.C, MidiNote.A4), Sign.Flat);
                    DrawSign(g, 47 + 16, GetNotePosY(KeyOf.C, MidiNote.A2), Sign.Flat);
                    break;
                case KeyOf.Bb:
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.B4), Sign.Flat);
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.B2), Sign.Flat);
                    DrawSign(g, 47 + 8, GetNotePosY(KeyOf.C, MidiNote.E4), Sign.Flat);
                    DrawSign(g, 47 + 8, GetNotePosY(KeyOf.C, MidiNote.E2), Sign.Flat);
                    break;
                case KeyOf.F:
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.B4), Sign.Flat);
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.B2), Sign.Flat);
                    break;
                case KeyOf.C:
                    break;
                case KeyOf.G:
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.F4), Sign.Sharp);
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.F2), Sign.Sharp);
                    break;
                case KeyOf.D:
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.F4), Sign.Sharp);
                    DrawSign(g, 47 + 8, GetNotePosY(KeyOf.C, MidiNote.C4), Sign.Sharp);
                    DrawSign(g, 47, GetNotePosY(KeyOf.C, MidiNote.F2), Sign.Sharp);
                    DrawSign(g, 47 + 8, GetNotePosY(KeyOf.C, MidiNote.C2), Sign.Sharp);
                    break;
            }
        }

        private void ShowNotes()
        {
            System.Drawing.Graphics g = StafPanel.CreateGraphics();
            ShowNotes(g);
        }

        private void ShowNotes(System.Drawing.Graphics g)
        {
            showNotes.Sort();
            showNotes.Reverse();

            g.DrawImageUnscaled(Properties.Resources.Clef, 10, (StafPanel.Height - Properties.Resources.Clef.Height) / 2);
            ShowKey(g);

            // p = 25 G2, 35 C3, 45 F4
            int a = 0;
            int pMax = 45;
            int pMin = 25;
            int cx = 0;
            foreach (int n in showNotes)
            {
                int p = GetNotePos(curKey, n);
                int y = bY - p * sY;
                int x = 100;

                ShowSign(g, x, y, n);

                if (p == a - 1)
                {
                    x += 7;
                    a = 0;
                    if (p == 35)
                    {
                        cx = 8;
                    }
                }
                else
                {
                    a = p;
                }

                g.DrawImageUnscaled(bmpNote, x, y);

                pMin = Math.Min(pMin, p);
                pMax = Math.Max(pMax, p);

            }

            // Draw staff lines
            for (int p = pMin; p <= pMax; p++)
            {
                if (p % 2 == 0)
                    continue;

                int y = bY - p * sY + sY;
                int x1 = 100 - 5;
                int x2 = 100 + 8 + 5;
                Point point1 = new Point(x1, y);
                Point point2 = new Point(x2, y);
                g.DrawLine(blackPen, point1, point2);
            }
        }

        private void StafPanel_Paint(object sender, PaintEventArgs e)
        {
            ShowNotes(e.Graphics);
        }

        private void KeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            curKey = (KeyOf)KeyCombo.SelectedIndex;
            Refresh();
        }

    }
}
