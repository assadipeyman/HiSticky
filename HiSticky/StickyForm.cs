using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiSticky
{
    public partial class StickyForm : Form
    {
        NoteManager noteManager;
        NoteFileManager nfManager;
        int lastLineCount = 0;
        Color rowNumberingColor = Color.FromArgb(160,160,160);
        const string splitter = "&&&***&&&";
        public StickyForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            SetStyle(
             ControlStyles.AllPaintingInWmPaint |
                //ControlStyles.Opaque | //better performance if disabled
             ControlStyles.UserPaint |
             ControlStyles.ResizeRedraw |
             ControlStyles.OptimizedDoubleBuffer//|
                //ControlStyles.CacheText
             , true);
            txt_note.TextChanged += txt_note_TextChanged;
        }

        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            SolidBrush b = new SolidBrush(Color.FromArgb(248, 247, 182));
            e.Graphics.FillRectangle(b, rc);
            this.BackColor = Color.FromArgb(252, 252, 182);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);

        }
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                
                return cp;
            }
        }

        public void onNavigate()
        {
            saveOnFile();
            txt_note.Text = noteManager.getNote(getCurrentNoteIndex());
            lastLineCount = txt_note.Lines.Count();
        }

        public void saveOnFile()
        {
            nfManager.writeFile(NoteHelper.serialize(noteManager));
        }

        void txt_note_TextChanged(object sender, EventArgs e)
        {
            noteManager.updateNote(getCurrentNoteIndex(), txt_note.Text);
            if (lastLineCount != txt_note.Lines.Count())
            {
                lastLineCount = txt_note.Lines.Count();
                updateAllRowNumberingRequest();
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            showNext();
        }

        private void showNext()
        {
            int page = getCurrentNoteIndex() + 1;
            if (!noteManager.isNoteExist(page)) return;
            if (page > 0)
                lbl_number.Text = page.ToString();
            onNavigate();
        }

        int getCurrentNoteIndex()
        {
            return int.Parse(lbl_number.Text);
        }

        private void btn_prev_Click(object sender, EventArgs e)
        {
            showPrev();
        }
        private void showPrev()
        {
            int page = getCurrentNoteIndex() - 1;
            if (!noteManager.isNoteExist(page)) return;
            if (page > 0)
                lbl_number.Text = page.ToString();
            onNavigate();
        }


        private void StickyForm_Load(object sender, EventArgs e)
        {
            nfManager = new NoteFileManager(Application.StartupPath + "/", "notes.data");
            List<string> notes = NoteHelper.deserialize(nfManager.getData(), splitter);
            if (notes.Count > 0)
                noteManager = new NoteManager(splitter, notes);
            else
            {
                noteManager = new NoteManager(splitter);
                noteManager.addNote("");
            }
            onNavigate();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            createNew();
        }
        private void createNew()
        {
            noteManager.addNote("");
            showNext();
        }


        private void StickyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveOnFile();
        }

        private void panel2_Resize(object sender, EventArgs e)
        {

        }
        int maxLC = 1;
        private void txt_note_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void updateAllRowNumberingRequest()
        {
            shouldUpdateNumbering = false;
            timer2.Enabled = true;
        }
        private void updateAllRowNumbering()
        {         
            int lineCount = txt_note.Lines.Count();
            Graphics g = CreateGraphics();
            int width =(int) g.MeasureString(lineCount.ToString(), new Font("arial", 10)).Width+2;
            Bitmap b = new Bitmap(width, txt_note.Height);            
            pictureBox1.Width = width;           
            g = Graphics.FromImage(b);
            g.Clear(pictureBox1.BackColor);
            for (int i = 0; i < lineCount; i++)
            {
                int charIndex = txt_note.GetFirstCharIndexFromLine(i);
                Point p = txt_note.GetPositionFromCharIndex(charIndex);
                p.X = 5;
                p.Y += 5;
                g.DrawString((i + 1).ToString(), new Font("arial", 10), new SolidBrush(rowNumberingColor), p);                
            }
            pictureBox1.Image = b;            
        }

        private void txt_note_VScroll(object sender, EventArgs e)
        {
            updateAllRowNumberingRequest();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txt_note.SelectionStart = txt_note.TextLength;
            txt_note.Focus();
            txt_note.ScrollToCaret();
            updateAllRowNumberingRequest();
            timer1.Enabled = false;
        }
        bool shouldUpdateNumbering = false;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (shouldUpdateNumbering)
            {
                timer2.Enabled = false;
                updateAllRowNumbering();
            }
            else
                shouldUpdateNumbering = true;
        }

        private void StickyForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool captured = false;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    captured = true;
                    if (e.Control)
                        Width += 50;
                    else
                        showNext();
                    break;
                case Keys.Left:
                    captured = true;
                    if (e.Control)
                        Width -= 50;
                    else
                        showPrev();
                    break;
                case Keys.N:
                    if (e.Control)
                    {
                        captured = true;
                        createNew();
                    }
                    break;
                case Keys.Up:
                    if (e.Control)
                    {
                        captured = true;
                        Height -= 50;
                    }
                    break;
                case Keys.Down:
                    if (e.Control)
                    {
                        captured = true;
                        Height += 50;
                    }
                    break;
                case Keys.Oemplus:
                    if (e.Control)
                    {
                        captured = true;
                        if (Opacity < 1)
                            Opacity += .1;
                    }
                    break;
                case Keys.OemMinus:
                    if (e.Control)
                    {
                        captured = true;
                        if (Opacity > .6)
                            Opacity -= .1;
                    }
                    break;
            }
            e.SuppressKeyPress = captured;
        }

        private void StickyForm_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
    }
}