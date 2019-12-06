using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace VictimProgram
{
    public partial class Form1 : Form
    {


        [DllImport("user32.dll")]
        public static extern int MessageBox(
            IntPtr hWnd,
            String lpText,
            String lpCaption,
            UInt32 uType0
        );

        public Form1()
        {
            InitializeComponent();
        }



   

    

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String lpText = textBox1.Text;
            String lpCaption = textBox2.Text;
            UInt32 uType = Convert.ToUInt32(textBox3.Text);


            MessageBox(IntPtr.Zero, lpText, lpCaption, uType);


        }
    }
}
