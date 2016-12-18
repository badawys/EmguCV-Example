using System;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Image
{
    public partial class Form1 : Form
    {
        Image<Bgr, Byte> My_Image;
        Image<Gray, Byte> gray_image;
        Image<Bgr, Byte> My_image_copy;
        Image<Bgr, Byte> image_2;
        Image<Bgr, Byte> image_3;
        Image<Bgr, Byte> image_4;
        Image<Bgr, Byte> image_5;
        Image<Bgr, Byte> image_6;

        OpenFileDialog open = new OpenFileDialog();    

        bool gray_in_use = false;

        public Form1()
        {
            InitializeComponent();
            open.Filter = "Image Files (*.tif; *.dcm; *.jpg; *.jpeg; *.bmp; *.gif)|*.tif; *.dcm; *.jpg; *.jpeg; *.bmp; *.gif";
        }

        private void Load_BTN_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog() == DialogResult.OK)
            {
                //Load the Image
                My_Image = new Image<Bgr, Byte>(open.FileName);

                //Display the Image
                image_PCBX.Image = My_Image.ToBitmap();

                My_image_copy = My_Image.Copy();

                //Set sepctrum choice
                Red_Spectrum_CHCK.Checked = true;
                Red_Spectrum_CHCK.Enabled = true;
                Green_Spectrum_CHCK.Checked = true;
                Green_Spectrum_CHCK.Enabled = true;
                Blue_Spectrum_CHCK.Checked = true;
                Blue_Spectrum_CHCK.Enabled = true;

                // Enable compine and substact buttons
                combine_button.Enabled = true;
                substract_button.Enabled = true;

                //Display the histogram
                addHistogram(My_Image);
            }
        }

        private void image_PCBX_MouseMove(object sender, MouseEventArgs e)
        {
            if (image_PCBX.Image != null)
            {
                X_pos_LBL.Text = "X: " + e.X.ToString();
                Y_pos_LBL.Text = "Y: " + e.Y.ToString();

                if (gray_in_use)
                {
                    Val_LBL.Text = "Value: " + gray_image[e.Y, e.X].ToString();
                }
                else
                {
                    Val_LBL.Text = "Value: " + My_Image[e.Y, e.X].ToString();
                }
            }
        }

        private void Convert_btn_Click(object sender, EventArgs e)
        {
            if (My_Image != null)
            {
                if (gray_in_use)
                {
                    gray_in_use = false;
                    image_PCBX.Image = My_Image.ToBitmap();
                    Convert_btn.Text = "Convert to Gray";

                    Red_Spectrum_CHCK.Checked = true;
                    Red_Spectrum_CHCK.Enabled = true;
                    Green_Spectrum_CHCK.Checked = true;
                    Green_Spectrum_CHCK.Enabled = true;
                    Blue_Spectrum_CHCK.Checked = true;
                    Blue_Spectrum_CHCK.Enabled = true;
                }
                else
                {
                    gray_image = My_Image.Convert<Gray, Byte>();
                    gray_in_use = true;
                    image_PCBX.Image = gray_image.ToBitmap();
                    Convert_btn.Text = "Convert to Colour";

                    Red_Spectrum_CHCK.Enabled = false;
                    Green_Spectrum_CHCK.Enabled = false;
                    Blue_Spectrum_CHCK.Enabled = false;
                }
            }

        }

        private void Red_Spectrum_CHCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!Red_Spectrum_CHCK.Checked)
            {
                //Remove Red Spectrum programatically
                Suppress(2);
            }
            else
            {
                //Add Red Spectrum programatically
                Un_Suppress(2);
            }
            image_PCBX.Image = My_image_copy.ToBitmap();


        }

        private void Green_Spectrum_CHCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!Green_Spectrum_CHCK.Checked)
            {
                //Remove Green Spectrum programatically
                Suppress(1);
            }
            else
            {
                //Add Green Spectrum programatically
                Un_Suppress(1);
            }
            image_PCBX.Image = My_image_copy.ToBitmap();
        }

        private void Blue_Spectrum_CHCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!Blue_Spectrum_CHCK.Checked)
            {
                //Remove Blue Spectrum programatically
                Suppress(0);
            }
            else
            {
                //Add Blue Spectrum programatically
                Un_Suppress(0);
            }
            image_PCBX.Image = My_image_copy.ToBitmap();
        }

        private void Suppress(int spectrum)
        {

            for (int i = 0; i < My_Image.Height; i++)
            {
                for (int j = 0; j < My_Image.Width; j++)
                {
                    My_image_copy.Data[i, j, spectrum] = 0;
                }
            }
        }

        private void Un_Suppress(int spectrum)
        {
            for (int i = 0; i < My_Image.Height; i++)
            {
                for (int j = 0; j < My_Image.Width; j++)
                {
                    My_image_copy.Data[i, j, spectrum] = My_Image.Data[i, j, spectrum];
                }
            }
        }

        private void addHistogram (Image<Bgr, Byte> img)
        {
            histogramBox1.ClearHistogram();
            histogramBox1.GenerateHistograms(My_Image, 256);
            histogramBox1.Refresh();
        }

        private void combine_button_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog() == DialogResult.OK)
            {

                image_2 = new Image<Bgr, Byte>(open.FileName);
                if (open.ShowDialog() == DialogResult.OK)
                {
                    image_3 = new Image<Bgr, Byte>(open.FileName);
                    if (image_2.Size != image_3.Size || My_Image.Size != image_2.Size || My_Image.Size != image_3.Size)
                    {
                        int maxheight = 0, minheight = 0, maxwidth = 0, minwidth = 0;

                        if (image_2.Width > image_3.Width)
                        {
                            maxwidth = image_2.Width;
                            minwidth = image_3.Width;
                        }
                        else
                        {
                            maxwidth = image_3.Width;
                            minwidth = image_2.Width;
                        }

                        if (image_2.Height > image_3.Height)
                        {
                            maxheight = image_2.Height;
                            minheight = image_3.Height;
                        }
                        else
                        {
                            maxheight = image_3.Height;
                            minheight = image_2.Height;
                        }

                        if (My_Image.Width > maxwidth)
                        {
                            maxwidth = My_Image.Width;
                            minwidth = maxwidth;
                        }
                        else
                        {
                            minwidth = My_Image.Width;
                        }

                        if (My_Image.Height > maxheight)
                        {
                            maxheight = My_Image.Height;
                            minheight = maxheight;
                        }
                        else
                        {
                            minheight = My_Image.Height;
                        }

                        
                        image_4 = new Image<Bgr, byte>(maxwidth, maxheight);
                        image_4.ROI = new Rectangle(0, 0, My_Image.Width, My_Image.Height);
                        CvInvoke.cvCopy(My_Image, image_4, IntPtr.Zero);
                        image_4.ROI = new Rectangle(0, 0, minwidth, minheight);

                        image_5 = new Image<Bgr, byte>(maxwidth, maxheight);
                        image_5.ROI = new Rectangle(0, 0, image_2.Width, image_2.Height);
                        CvInvoke.cvCopy(image_2, image_5, IntPtr.Zero);
                        image_5.ROI = new Rectangle(0, 0, minwidth, minheight);

                        image_6 = new Image<Bgr, byte>(maxwidth, maxheight);
                        image_6.ROI = new Rectangle(0, 0, image_3.Width, image_3.Height);
                        CvInvoke.cvCopy(image_3, image_6, IntPtr.Zero);
                        image_6.ROI = new Rectangle(0, 0, minwidth, minheight);


                        image_PCBX.Image = image_4.Add(image_5).Add(image_6).Bitmap;
                    }
                    else {
                        image_PCBX.Image = My_Image.Add(image_2).Add(image_3).ToBitmap();
                    }
                }
            }
        }

        private void substract_button_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog() == DialogResult.OK)
            {
                image_2 = new Image<Bgr, Byte>(open.FileName);

                if (image_2.Size != My_Image.Size)
                {
                    int maxheight = 0, minheight = 0, maxwidth = 0, minwidth = 0;

                    if (image_2.Width > My_Image.Width)
                    {
                        maxwidth = image_2.Width;
                        minwidth = My_Image.Width;
                    }
                    else
                    {
                        maxwidth = My_Image.Width;
                        minwidth = image_2.Width;
                    }

                    if (image_2.Height > My_Image.Height)
                    {
                        maxheight = image_2.Height;
                        minheight = My_Image.Height;
                    }
                    else
                    {
                        maxheight = My_Image.Height;
                        minheight = image_2.Height;
                    }

                    image_4 = new Image<Bgr, byte>(maxwidth, maxheight);
                    image_4.ROI = new Rectangle(0, 0, My_Image.Width, My_Image.Height);
                    CvInvoke.cvCopy(My_Image, image_4, IntPtr.Zero);
                    image_4.ROI = new Rectangle(0, 0, minwidth, minheight);

                    image_5 = new Image<Bgr, byte>(maxwidth, maxheight);
                    image_5.ROI = new Rectangle(0, 0, image_2.Width, image_2.Height);
                    CvInvoke.cvCopy(image_2, image_5, IntPtr.Zero);
                    image_5.ROI = new Rectangle(0, 0, minwidth, minheight);


                    image_PCBX.Image = image_4.Sub(image_5).Bitmap;
                }
                else {
                    image_PCBX.Image = My_Image.Sub(image_2).ToBitmap();
                }
            }
        }
    }
}