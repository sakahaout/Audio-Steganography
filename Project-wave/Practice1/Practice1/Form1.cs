﻿using System;

using System.Text;

using System.Windows.Forms;
using System.IO;
namespace Practice1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                string cover_audio_file = @"F:\DIU\ThesisProject\Project-wave\Project-wave\Audio\StegoP1.wav";
                string screct_message = "ABCDEFGHIJKLMONPQRSTUVWXYZ";
                Char[] screct_message_binary = messageBinaryFormat(screct_message);

                WaveAudio waveCoverAudio = new WaveAudio(new FileStream(cover_audio_file, FileMode.Open, FileAccess.Read));
                var leftstream = waveCoverAudio.GetLeftStream();
                var rightstream = waveCoverAudio.GetRightStream();


                int messageLengthTrack = 0;
                for (int i = 0; i < screct_message_binary.Length; i++)
                {
                    int leftStreamValue = leftstream[i];
                    int test = leftStreamValue;
                    int evenodd = 1;
                    if (leftStreamValue < 0)
                    {
                        evenodd = -1;
                        leftStreamValue = leftStreamValue * evenodd; //if value is odd then we convert to even so that we can convert into binary easily
                    }
                    string binaryValueLeftStreamValue = Convert.ToString(leftStreamValue, 2).PadLeft(16, '0');
                    int newLeftStreamValue = Convert.ToInt32((binaryValueLeftStreamValue.Substring(0, binaryValueLeftStreamValue.Length - 1) + screct_message_binary[messageLengthTrack]), 2) * evenodd;
                    Console.WriteLine(test + "_" + newLeftStreamValue);
                    //leftStream.Insert(i, (short)newLeftStreamValue); //Replace audio data bit with message bit.
                    leftstream[i] = (short)newLeftStreamValue;
                    messageLengthTrack++;
                }

                waveCoverAudio.WriteFile(@"F:\DIU\ThesisProject\Project-wave\Project-wave\Audio\output.wav");
                MessageBox.Show("Sucessful");


            }
            catch (Exception ex)
            { 
                MessageBox.Show("Fail");
            }
            
        }
        public string messageBinary = "";
        public char[] messageBinaryFormat(string message)
        {
            /*
            * Here we are creating messages bit to binary form
            */
            //message is converted to 8bit binary
            StringBuilder sb = new StringBuilder();
            foreach (char c in message.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            messageBinary = sb.ToString();

            //to maintain error from pass length we have to add (extra 0 or 00)
            //if (((messageBinary.Length) % 3) == 2)
            //{
            //    messageBinary += "0";
            //}
            //else if (((messageBinary.Length) % 3) == 1)
            //{
            //    messageBinary += "00";
            //}

            return messageBinary.ToCharArray(); //all binary bit has been converted into array.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string stego_audio = @"F:\DIU\ThesisProject\Project-wave\Project-wave\Audio\output.wav";
            WaveAudio waveStegoAudio = new WaveAudio(new FileStream(stego_audio, FileMode.Open, FileAccess.Read));

            var leftstream = waveStegoAudio.GetLeftStream();
            var rightstream = waveStegoAudio.GetRightStream();
            string binaryMessagelengthChar = "";
            for (int i = 0; i < 208; i++)
            {
                int leftStreamValue = leftstream[i];
                Console.WriteLine(leftStreamValue);
                int evenodd = 1;
                if (leftStreamValue < 0)
                {
                    evenodd = -1;
                    leftStreamValue = leftStreamValue * evenodd; //if value is odd then we convert to even so that we can convert into binary easily
                }
                string binaryValueLeftStreamValue = Convert.ToString(leftStreamValue, 2).PadLeft(16, '0');
                //binaryMessagelengthChars.Add(binaryValueLeftStreamValue[15]);
                binaryMessagelengthChar += binaryValueLeftStreamValue[15].ToString();
            }

            string totalSecretMessageBitCoreString = binaryMessagelengthChar;
            int initialFrame = 0;
            int perFrame = 8;
            string secret_message_final = "";
            for (int i = 0; i< 26; i++)
                {
                initialFrame = perFrame * i;
                string temp = totalSecretMessageBitCoreString.Substring(initialFrame, 8);
                secret_message_final += (char)Convert.ToInt32(temp, 2);

            }
            MessageBox.Show(secret_message_final.ToString());
        }
    }
}
