using System;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging.ComplexFilters;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ColorMap = SciColorMaps.ColorMap;
using System.IO;
using Image = AForge.Imaging.Image;
using Label = System.Windows.Forms.Label;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using iTextSharp.text.pdf;
using Rectangle = System.Drawing.Rectangle;
using NAudio.Lame;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Net.Http;



namespace MultiMedia_Project1
{
    internal static class Program
    {
    
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



          
            Application.Run(new FormSelection());
          

        }
    }


    public partial class Form3 : Form
    {
        private Bitmap img1, img2;

        public Form3()
        {
           

            
            this.Width = 1500;
            this.Height = 1000;
            Button loadImg1Button = new Button { Text = "تحميل الصورة 1", Left = 50, Width = 100, Top = 50, BackColor = Color.Aqua };
            Button loadImg2Button = new Button { Text = "تحميل الصورة 2", Left = 50, Width = 100, Top = 100, BackColor = Color.Aqua };
            Button compareButton = new Button { Text = "مقارنة الصورتين", Left = 50, Width = 100, Top = 150, BackColor = Color.Gold };

            
            PictureBox img1Box = new PictureBox { Left = 375, Top = 50, Width = 500, Height = 500, SizeMode = PictureBoxSizeMode.Zoom };
            PictureBox img2Box = new PictureBox { Left = 1000, Top = 50, Width = 500, Height = 500, SizeMode = PictureBoxSizeMode.Zoom };

           
            RichTextBox resultBox = new RichTextBox { Left = 50, Top = 200, Width = 300, Height = 100 };



            
            this.Controls.Add(loadImg1Button);
            this.Controls.Add(loadImg2Button);
            this.Controls.Add(img1Box);
            this.Controls.Add(img2Box);
            this.Controls.Add(compareButton);
            this.Controls.Add(resultBox);

           
            loadImg1Button.Click += (sender, e) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    img1 = new Bitmap(dialog.FileName);
                    img1Box.Image = img1;
                }
            };

            loadImg2Button.Click += (sender, e) =>
            {

                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    img2 = new Bitmap(dialog.FileName);
                    img2Box.Image = img2;
                }
            };

            compareButton.Click += (sender, e) =>
            {
                
                if (img1 != null && img2 != null)
                {
                    resultBox.ForeColor = Color.Green;
                    resultBox.Text = "نتيجة المقارنة: " + GetAbsoluteDifference(img1, img2);
                }
                else
                {
                    resultBox.ForeColor = Color.Red;
                    resultBox.Text = "الرجاء تحميل الصورتين قبل المقارنة.";
                }
            };
        }

        public static string GetAbsoluteDifference(Bitmap img1, Bitmap img2)
        {
            if (img1.Size != img2.Size)
            {
                return ("الصور يجب أن تكون بنفس الحجم.");
            }
            int totalDifference = 0;
           
            for (int y = 0; y < img1.Height; ++y)
            {
                for (int x = 0; x < img1.Width; ++x)
                {
                    Color pixel1 = img1.GetPixel(x, y);
                    Color pixel2 = img2.GetPixel(x, y);

                    int diffR = Math.Abs(pixel1.R - pixel2.R);
                    int diffG = Math.Abs(pixel1.G - pixel2.G);
                    int diffB = Math.Abs(pixel1.B - pixel2.B);

                  
                    totalDifference += diffR + diffG + diffB;
                }
            }

            int threshold = 10000;

            if (totalDifference > threshold)
            {

                return " الحالة غير مستقرة";
            }
            else
            {

                return "الحالة مستقرة  ";
            }


        }

    }

    //part 1



    public partial class Form2 : Form
    {
        PictureBox pictureBox;
        Button saveButton;
        Button cropButton;
        Button colorMapButton;
        Button colorButton;
        Button uploadButton;
        ComboBox shapeSelector;
        ComboBox colorMapSelector;
        bool color = false;
        bool colorMap = false;
        bool crop = false;
        string currentShape = "Rectangle"; 

        Bitmap bmp;

        Point? selectionStart = null;
        Rectangle selectionRectangle = Rectangle.Empty;
        Point selectionCenter;
        int selectionRadius = 0;
        List<Point> linePoints = new List<Point>();

        TextBox commentTextBox; 
        Button addCommentButton;
        Button compression;


      

   




    public Form2()
        {
            pictureBox = new PictureBox
            {
                Left = 50,
                Top = 20,
                Width = 800,
                Height = 800,
              

            };

       


        TrackBar compressionTrackBar = new TrackBar {
                Left = 1040,
                Top = 80,
                Width = 100,
                Minimum = 0,
                Maximum = 100,
                 BackColor = Color.Green,
                 Height = 20,
            };

           


            Label compressionValueLabel = new Label { Left = 1020, Top = 80 ,Width=25, BackColor = Color.Green,Height=45 };
            this.Controls.Add(compressionValueLabel);

            compressionTrackBar.ValueChanged += (s, e) => {
                compressionValueLabel.Text = compressionTrackBar.Value.ToString();
            };

            Button exportPdfButton = new Button { Text = "Export PDF", Left = 1050, Top = 300, Width = 110 };
            Button writeReportButton = new Button { Text = "Write Report", Left = 1150, Top = 360, Width = 110 };

            Button recordAudioButton = new Button { Text = "Record Audio", Left = 1150, Top = 320, Width = 110 };
            compression = new Button { Text = "compression image", Left = 1020, Top = 50, Enabled = false,Width=120 };
            
            addCommentButton = new Button {Width=110 , Text = "Add Comment", Left = 1150, Top = 250 };
            cropButton = new Button { Text = "cut", Left = 1020, Top = 20, Enabled = false, Width = 120 };
            saveButton = new Button { Text = "save", Left = 1150, Top = 50, Enabled = false };
            colorMapButton = new Button { Text = "ColorMap", Left = 1150, Top = 110, Enabled = true };
            colorButton = new Button { Text = "color", Left = 1150, Top = 80, Enabled = true };
            uploadButton = new Button { Text = "upload ", Left = 1150, Top = 20, Enabled = true };

            commentTextBox = new TextBox { Left = 1150, Top = 280, Width = 200 };

            shapeSelector = new ComboBox { Left = 1150, Top = 170 };
            shapeSelector.Items.Add("Rectangle");
            shapeSelector.Items.Add("Circle");
            shapeSelector.Items.Add("Line");
            shapeSelector.SelectedIndex = 0;
            shapeSelector.SelectedIndexChanged += (s, e) => { currentShape = shapeSelector.SelectedItem.ToString(); };

            colorMapSelector = new ComboBox { Left = 1150, Top = 200 };
            colorMapSelector.Items.Add("jet");
            colorMapSelector.Items.Add("spring");
            colorMapSelector.Items.Add("winter");
            colorMapSelector.Items.Add("cool");
            colorMapSelector.Items.Add("hot");
            colorMapSelector.Items.Add("summer");
            colorMapSelector.Items.Add("autumn");
            colorMapSelector.Items.Add("bone");
            colorMapSelector.Items.Add("copper");
            colorMapSelector.Items.Add("Rainbow");
            colorMapSelector.SelectedIndex = 0;

            this.Controls.Add(pictureBox);
            this.Controls.Add(cropButton);
            this.Controls.Add(saveButton);
            this.Controls.Add(colorMapButton);
            this.Controls.Add(colorButton);
            this.Controls.Add(uploadButton);
            this.Controls.Add(shapeSelector);
            this.Controls.Add(colorMapSelector);
            this.Controls.Add(compression);
            this.Controls.Add(compressionTrackBar);

            cropButton.Click += (s, e) => { crop = true; color = false; colorMap = false; };
            colorMapButton.Click += (s, e) => { colorMap = true; color = false; crop = false; };
            colorButton.Click += (s, e) => { color = true; colorMap = false; crop = false; };

            compression.Click += (s, e) => {


               long qualty = compressionTrackBar.Value;

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Image",
                    Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CompressImage(bmp, qualty).Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                }
            };

           



            uploadButton.Click += uploadImage;
            saveButton.Click += SaveButton_Click;

            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.Paint += PictureBox_Paint;

            recordAudioButton.Click += (s, e) =>
            {
              
                RecordAudioButton_Click();
            };



            this.Controls.Add(recordAudioButton);

          
            this.Height = 800;
            this.Width = 1500;
            this.Text = "Picture Xray Select";


           
            this.Controls.Add(commentTextBox);

           
          
            addCommentButton.Click += AddCommentButton_Click;
            this.Controls.Add(addCommentButton);

          
            writeReportButton.Click += WriteReportButton_Click;
            this.Controls.Add(writeReportButton);


        }



        private void WriteReportButton_Click(object sender, EventArgs e)
        {
            using (Form reportForm = new Form())
            {
                reportForm.Text = "Write Medical Report";
                reportForm.Width = 400;
                reportForm.Height = 300;

                TextBox reportTextBox = new TextBox
                {
                    Multiline = true,
                    Dock = DockStyle.Fill,
                    Font = new System.Drawing.Font("Arial", 12)
                };

                Button saveReportButton = new Button
                {
                    Text = "Save Report as TXT",
                    Dock = DockStyle.Bottom
                };

                Button saveAsPdfButton = new Button
                {
                    Text = "Save Report as PDF",
                    Dock = DockStyle.Bottom
                };

                saveReportButton.Click += (s, args) =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save Report",
                        Filter = "Text Files(*.TXT)|*.TXT|All files (*.*)|*.*"
                    };

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, reportTextBox.Text);
                        MessageBox.Show("Report saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                //
                Button saveCompressedPdfButton = new Button
                {
                    Text = "Save Compressed Report as PDF",
                    Dock = DockStyle.Bottom
                };

                saveCompressedPdfButton.Click += (s, args) =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save Report",
                        Filter = "PDF Files(*.PDF)|*.PDF|All files (*.*)|*.*"
                    };

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportTextToPdf(reportTextBox.Text, saveFileDialog.FileName, compress: true);
                        MessageBox.Show("Report saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                reportForm.Controls.Add(saveCompressedPdfButton);


              
                saveAsPdfButton.Click += (s, args) =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save Report",
                        Filter = "PDF Files(*.PDF)|*.PDF|All files (*.*)|*.*"
                    };

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportTextToPdf(reportTextBox.Text, saveFileDialog.FileName, compress: false);
                        MessageBox.Show("Report saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                reportForm.Controls.Add(reportTextBox);
                reportForm.Controls.Add(saveReportButton);
                reportForm.Controls.Add(saveAsPdfButton);

                reportForm.ShowDialog();
            }
        }

        public void ExportTextToPdf(string text, string filePath, bool compress)
        {
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

            if (compress)
            {
                writer.SetFullCompression();
            }

            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph(text));
            doc.Close();
        }




        private void RecordAudioButton_Click()
        {
            using (var waveIn = new WaveInEvent())
            {
                waveIn.WaveFormat = new WaveFormat(44100, 1); 
                WaveFileWriter waveFileWriter = null;
                string outputAudioFile = null;

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Audio",
                    Filter = "MP3 Files(*.mp3)|*.mp3|All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputAudioFile = saveFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Please select folder and Enter name file");
                    return;
                }

                MessageBox.Show("Click OK to start recording...");

                waveIn.DataAvailable += (sender, e) =>
                {
                   
                    if (waveFileWriter == null)
                    {
                        waveFileWriter = new WaveFileWriter("temp.wav", waveIn.WaveFormat);
                    }
                  
                    waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                };

                waveIn.StartRecording();
                MessageBox.Show("Click OK to stop recording.... Audio saved to: " + outputAudioFile);

                waveIn.StopRecording();

                   waveFileWriter?.Dispose();

              
                bool compress = MessageBox.Show("Do you want to compress the audio?", "Compress Audio", MessageBoxButtons.YesNo) == DialogResult.Yes;

               
                if (compress)
                {
                    using (var reader = new AudioFileReader("temp.wav"))
                    using (var writer = new LameMP3FileWriter(outputAudioFile, reader.WaveFormat, LAMEPreset.ABR_128))
                    {
                        reader.CopyTo(writer);
                    }
                   
                    System.IO.File.Delete("temp.wav");
                }
                else
                {
                    
                    System.IO.File.Move("temp.wav", outputAudioFile);
                }
            }
        }




        public Bitmap CompressImage(Bitmap image, long quality)
        {
            
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageDecoders().First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

           
            MemoryStream compressedStream = new MemoryStream();
            image.Save(compressedStream, jpegCodec, encoderParams);

            
            Bitmap compressedImage = new Bitmap(compressedStream);

            return compressedImage;
        }







        private void AddCommentButton_Click(object sender, EventArgs e)
        {
            
            string comment = commentTextBox.Text;

           
            if (!string.IsNullOrWhiteSpace(comment))
            {
               
                using (Graphics g = Graphics.FromImage(pictureBox.Image))
                {
                    
                    using (System.Drawing.Font font = new System.Drawing.Font("Arial", 15 , FontStyle.Bold))
                    {
                        using (SolidBrush brush = new SolidBrush(Color.Red))
                        {
                            
                            g.DrawString(comment, font, brush, new PointF(10, 10));
                        }
                    }
                }

               
                pictureBox.Refresh();
            }
        }


            private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            selectionStart = e.Location;
            if (currentShape == "Circle")
            {
                selectionCenter = e.Location;
            }
            else if (currentShape == "Line")
            {
                linePoints.Clear();
                linePoints.Add(e.Location);
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectionStart.HasValue)
            {
                if (currentShape == "Rectangle")
                {
                    selectionRectangle = new Rectangle(
                        Math.Min(e.X, selectionStart.Value.X),
                        Math.Min(e.Y, selectionStart.Value.Y),
                        Math.Abs(e.X - selectionStart.Value.X),
                        Math.Abs(e.Y - selectionStart.Value.Y));
                }
                else if (currentShape == "Circle")
                {
                    selectionRadius = (int)Math.Sqrt(Math.Pow(e.X - selectionCenter.X, 2) + Math.Pow(e.Y - selectionCenter.Y, 2));
                }
                else if (currentShape == "Line")
                {
                    linePoints.Add(e.Location);
                }

                pictureBox.Invalidate();
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            selectionStart = null;
            if (currentShape == "Line" && linePoints.Count > 2 && IsPointNear(linePoints[0], e.Location))
            {
                linePoints.Add(linePoints[0]); 
            }
            ApplyChangesToSelectedArea();
            pictureBox.Invalidate();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (selectionStart.HasValue)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    if (currentShape == "Rectangle")
                    {
                        e.Graphics.DrawRectangle(pen, selectionRectangle);
                    }
                    else if (currentShape == "Circle")
                    {
                        e.Graphics.DrawEllipse(pen, selectionCenter.X - selectionRadius, selectionCenter.Y - selectionRadius, selectionRadius * 2, selectionRadius * 2);
                    }
                    else if (currentShape == "Line")
                    {
                        if (linePoints.Count > 1)
                        {
                            e.Graphics.DrawLines(pen, linePoints.ToArray());
                        }
                    }
                }
            }
        }

        private void ApplyChangesToSelectedArea()
        {
            if (bmp == null) return;

            Bitmap newBmp = (Bitmap)bmp.Clone();
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (IsWithinSelection(i, j))
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        int lightness = (int)((pixel.R + pixel.G + pixel.B) / 3.0);
                        if (colorMap)
                        {
                          
                            switch (colorMapSelector.SelectedIndex)
                            {

                                case 0:
                                    var colormap0 = new ColorMap("jet", 0.02, 1);

                                    newBmp.SetPixel(i, j, colormap0.GetColor(lightness / 255.0));
                                    break;
                                case 1:
                                    var colormap1 = new ColorMap("spring", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap1.GetColor(lightness / 255.0));
                                    break;
                                case 2:
                                    var colormap2 = new ColorMap("winter", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap2.GetColor(lightness / 255.0));
                                    break;
                                case 3:
                                    var colormap3 = new ColorMap("cool", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap3.GetColor(lightness / 255.0));
                                    break;
                                case 4:
                                    var colormap4 = new ColorMap("hot", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap4.GetColor(lightness / 255.0));
                                    break;
                                case 5:
                                    var colormap5 = new ColorMap("summer", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap5.GetColor(lightness / 255.0));
                                    break;
                                case 6:
                                    var colormap6 = new ColorMap("autumn", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap6.GetColor(lightness / 255.0));
                                    break;
                                case 7:
                                    var colormap7 = new ColorMap("bone", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap7.GetColor(lightness / 255.0));
                                    break;
                                case 8:
                                    var colormap8 = new ColorMap("copper", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap8.GetColor(lightness / 255.0));
                                    break;
                                case 9:
                                    var colormap9 = new ColorMap("Rainbow", 0.02, 1);
                                    newBmp.SetPixel(i, j, colormap9.GetColor(lightness / 255.0));
                                    break;

                            }
                        }
                        if (color)
                        {
                            ApplyColor(newBmp, i, j, lightness);
                        }
                    }
                }
            }

            if (crop && currentShape == "Rectangle")
            {
                newBmp = CropImage(newBmp, selectionRectangle);
            }

            bmp.Dispose();
            bmp = newBmp;

            pictureBox.Image = bmp;
            saveButton.Enabled = true;
        }

        private Bitmap CropImage(Bitmap sourceImage, Rectangle cropRectangle)
        {
            Bitmap croppedImage = new Bitmap(cropRectangle.Width, cropRectangle.Height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(sourceImage, 0, 0, cropRectangle, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }




        private bool IsWithinSelection(int x, int y)
        {
            if (currentShape == "Rectangle")
            {
                return selectionRectangle.Contains(x, y);
            }
            else if (currentShape == "Circle")
            {
                int dx = x - selectionCenter.X;
                int dy = y - selectionCenter.Y;
                return dx * dx + dy * dy <= selectionRadius * selectionRadius;
            }
            else if (currentShape == "Line")
            {
                return IsPointInPolygon(linePoints, new Point(x, y));
            }
            return false;
        }

        private bool IsPointNear(Point p1, Point p2, int tolerance = 5)
        {
            return Math.Abs(p1.X - p2.X) <= tolerance && Math.Abs(p1.Y - p2.Y) <= tolerance;
        }

        private bool IsPointInPolygon(List<Point> polygon, Point point)
        {
            int polygonLength = polygon.Count, i = 0;
            bool inside = false;
           
            float pointX = point.X, pointY = point.Y;
            
            float startX, startY, endX, endY;
            Point endPoint = polygon[polygonLength - 1];
            endX = endPoint.X;
            endY = endPoint.Y;
            while (i < polygonLength)
            {
                startX = endX;
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.X;
                endY = endPoint.Y;

                inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }




        private void ApplyColor(Bitmap bmp, int x, int y, int lightness)
        {
            if (lightness < 32)
            {
                bmp.SetPixel(x, y, Color.FromArgb(0, 0, 128)); 
            }
            else if (lightness < 64)
            {
                bmp.SetPixel(x, y, Color.Blue);
            }
            else if (lightness < 96)
            {
                bmp.SetPixel(x, y, Color.FromArgb(0, 128, 128));
            }
            else if (lightness < 128)
            {
                bmp.SetPixel(x, y, Color.Green);
            }
            else if (lightness < 160)
            {
                bmp.SetPixel(x, y, Color.Yellow);
            }
            else if (lightness < 192)
            {
                bmp.SetPixel(x, y, Color.Orange);
            }
            else if (lightness < 224)
            {
                bmp.SetPixel(x, y, Color.Red);
            }
            else
            {
                bmp.SetPixel(x, y, Color.Maroon);
            }
        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Image",
                Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image.Save(saveFileDialog.FileName);
            }
        }



        private void uploadImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bmp = new Bitmap(openFileDialog.FileName);
                bmp = ConvertToGrayscale(bmp);

                Grayscale filter1 = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayImage = filter1.Apply(bmp);

                pictureBox.Image = grayImage;
                cropButton.Enabled = true;
                compression.Enabled = true;
                
            }
        }



        private Bitmap ConvertToGrayscale(Bitmap bmp)
        {
            Bitmap grayBmp = new Bitmap(bmp.Width, bmp.Height);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    int grayValue = (int)((pixel.R + pixel.G + pixel.B) / 3.0);
                    Color grayPixel = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayBmp.SetPixel(i, j, grayPixel);
                }
            }
            return grayBmp;
        }
    }







    public partial class FormSelection : Form
    {



  

        
        public FormSelection()
        {
            this.Width = 400;
            this.Height = 850;
            this.Text = "اختر نوع العملية المطلوبة";

            Button button1 = new Button { Text = "تقييم حالة المريض", Left = 50, Width = 150, Height = 70, Top = 50, DialogResult = DialogResult.Yes, BackColor = Color.GreenYellow };
            Button button2 = new Button { Text = "تحديد المنطقة المصابة", Left = 50, Width = 150, Height = 70, Top = 200, DialogResult = DialogResult.No, BackColor = Color.Orange };
            Button button3 = new Button { Text = "البحث عن صورة", Left = 50, Width = 150, Height = 70, Top = 350, BackColor = Color.Aqua };
            Button button4 = new Button { Text = "تحسين جودة الصورة", Left = 50, Width = 150, Height = 70, Top = 500, BackColor = Color.Red };
            Button button5 = new Button { Text = "تصنيف شدة الحالة", Left = 50, Width = 150, Height = 70, Top = 650, BackColor = Color.Purple };
            Button button6 = new Button { Text = "المشاركة على WhatsApp", Left = 210, Width = 100, Height = 35, Top = 350, BackColor = Color.GreenYellow };
            Button button7 = new Button { Text = "المشاركة على Telegram", Left = 210, Width = 100, Height = 35, Top = 385, BackColor = Color.GreenYellow };

            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);
            this.Controls.Add(button4);
            this.Controls.Add(button5);
            this.Controls.Add(button6);
            this.Controls.Add(button7);

            button1.Click += (sender, e) =>
            {
                Form3 form3 = new Form3();
                form3.Show();
            };
            button2.Click += (sender, e) =>
            {
                Form2 form2 = new Form2();
                form2.Show();
            };
            button3.Click += (sender, e) =>
            {
                FormSearch formSearch = new FormSearch();
                formSearch.Show();
            };

            button4.Click += (sender, e) =>
            {
                ForrierTransfrom fft = new ForrierTransfrom();
                fft.Show();
            };

            button5.Click += (sender, e) =>
            {
                Form4 form4 = new Form4();
                form4.Show();
            };

            button6.Click += async (s, args) =>
            {
                string filePath = SelectFile();
                if (!string.IsNullOrEmpty(filePath))
                {
                    string url = "https://api.whatsapp.com/send?text=" + Uri.EscapeDataString(filePath);
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            };

            button7.Click += async (s, args) =>
            {
                string filePath = SelectFile();
                if (!string.IsNullOrEmpty(filePath))
                {
                    SendFileToTelegram(filePath);
                }
            };
        }

        private string SelectFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }
            return null;
        }

        private async void SendFileToTelegram(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
        string botToken = "7286654551:AAFpzK9SoR2XLLjj3evQMJjU8iQLsG6CWXc";
            string chatId = "1234519037";
            string apiUrl = $"https://api.telegram.org/bot{botToken}/sendDocument";

            try
            {
                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StringContent(chatId), "chat_id");
                    form.Add(new StreamContent(fileStream), "document", Path.GetFileName(filePath));

                    HttpResponseMessage response = await client.PostAsync(apiUrl, form);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($" file sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to send  file. Response: {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while sending file. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        public partial class FormSearch : Form
        {
            private TextBox sizeTextBox;
            private TextBox dateTextBox;
            private Button searchSizeButton;
            private Button searchDateButton;
            private PictureBox pictureBox;
            private ListBox resultsListBox;

            private Dictionary<string, string> fileMap; 

            public FormSearch()
            {
                this.Width = 1000;
                this.Height = 1200;

                sizeTextBox = new TextBox { Left = 100, Top = 220, Width = 100 };
                dateTextBox = new TextBox { Left = 100, Top = 250, Width = 100 };
                searchSizeButton = new Button { Text = "بحث", Left = 10, Top = 220 };
                searchDateButton = new Button { Text = "بحث", Left = 10, Top = 250 };
                pictureBox = new PictureBox { Left = 350, Top = 20, Width = 600, Height = 900, SizeMode = PictureBoxSizeMode.Zoom };
                resultsListBox = new ListBox { Left = 10, Top = 300, Width = 300, Height = 400 };

                fileMap = new Dictionary<string, string>(); // تهيئة الخريطة

                Label dateLabel = new Label
                {
                    Location = new Point(245, 220),
                    Size = new Size(100, 20),
                    Text = "إدخال الحجم"
                };

                Label dateLabel1 = new Label
                {
                    Location = new Point(245, 253),
                    Size = new Size(100, 20),
                    Text = "إدخال التاريخ"
                };

                this.Controls.Add(dateLabel);
                this.Controls.Add(dateLabel1);

                searchDateButton.Click += (s, e) =>
                {
                    string directoryPath = @"C:\Users\admin\Desktop\projects multimedia\X-Ray-project\images";
                    string format = "M-d-yyyy";
                    DateTime date;
                    try
                    {
                        date = DateTime.ParseExact(dateTextBox.Text, format, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("الرجاء إدخال تاريخ صحيح بالتنسيق (M-d-yyyy).");
                        return;
                    }

                    List<string> imagePaths = FindImagesByDate(date, directoryPath);
                    if (imagePaths.Count > 0)
                    {
                        resultsListBox.Items.Clear();
                        fileMap.Clear();
                        foreach (var path in imagePaths)
                        {
                            string fileName = Path.GetFileName(path);
                            resultsListBox.Items.Add(fileName);
                            fileMap[fileName] = path;
                        }
                        pictureBox.Image = Image.FromFile(imagePaths[0]);
                    }
                    else
                    {
                        MessageBox.Show("عذرا .. لا يوجد صورة بهذا التاريخ");
                    }
                };

                searchSizeButton.Click += (s, e) =>
                {
                    string directoryPath = @"C:\Users\admin\Desktop\projects multimedia\X-Ray-project\images";
                    double size;
                    if (!double.TryParse(sizeTextBox.Text, out size))
                    {
                        MessageBox.Show("الرجاء إدخال حجم صحيح.");
                        return;
                    }

                    List<string> imagePaths = FindImagesBySize(size, directoryPath);
                    if (imagePaths.Count > 0)
                    {
                        resultsListBox.Items.Clear();
                        fileMap.Clear();
                        foreach (var path in imagePaths)
                        {
                            string fileName = Path.GetFileName(path);
                            resultsListBox.Items.Add(fileName);
                            fileMap[fileName] = path;
                        }
                        pictureBox.Image = Image.FromFile(imagePaths[0]);
                    }
                    else
                    {
                        MessageBox.Show("عذرا .. لا يوجد صورة بهذا الحجم");
                    }
                };

                resultsListBox.SelectedIndexChanged += (s, e) =>
                {
                    if (resultsListBox.SelectedIndex >= 0)
                    {
                        string selectedFileName = resultsListBox.SelectedItem.ToString();
                        if (fileMap.ContainsKey(selectedFileName))
                        {
                            string selectedImagePath = fileMap[selectedFileName];
                            pictureBox.Image = Image.FromFile(selectedImagePath);
                        }
                    }
                };

                this.Controls.Add(sizeTextBox);
                this.Controls.Add(dateTextBox);
                this.Controls.Add(searchSizeButton);
                this.Controls.Add(searchDateButton);
                this.Controls.Add(pictureBox);
                this.Controls.Add(resultsListBox);
            }

            public List<string> FindImagesByDate(DateTime targetDate, string directoryPath)
            {
                try
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        throw new DirectoryNotFoundException("المسار المعطى غير موجود.");
                    }

                    string[] imageFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                    List<string> matchingImages = new List<string>();

                    foreach (string imagePath in imageFiles)
                    {
                        FileInfo fileInfo = new FileInfo(imagePath);
                        DateTime fileDate = fileInfo.LastWriteTime;

                        if (fileDate.Date == targetDate.Date)
                        {
                            matchingImages.Add(imagePath);
                        }
                    }

                    return matchingImages;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("خطأ: " + ex.Message);
                    return new List<string>();
                }
            }

            public List<string> FindImagesBySize(double targetSize, string directoryPath)
            {
                try
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        throw new DirectoryNotFoundException("المسار المعطى غير موجود.");
                    }

                    string[] imageFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                    List<string> matchingImages = new List<string>();

                    foreach (string imagePath in imageFiles)
                    {
                        FileInfo fileInfo = new FileInfo(imagePath);
                        double fileSizeInBytes = fileInfo.Length;

                        if (Math.Abs(fileSizeInBytes - targetSize) < double.Epsilon)
                        {
                            matchingImages.Add(imagePath);
                        }
                    }

                    return matchingImages;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("خطأ: " + ex.Message);
                    return new List<string>();
                }
            }
        }



        public partial class ForrierTransfrom : Form
                    {
                        PictureBox pictureBox;
                        Button saveButton;
                        public ForrierTransfrom()
                        {
                            this.Width = 1000;
                            this.Height = 1200;
                            Button FFTtransform = new Button { Text = "upload ", Left = 500, Top = 10 };
                            saveButton = new Button { Text = "save", Left = 700, Top = 10, Enabled = false };
                            FFTtransform.Size = new Size(100, 50);
                            saveButton.Size = new Size(100, 50);
                            saveButton.BackColor = Color.YellowGreen;
                            FFTtransform.BackColor = Color.Goldenrod;
                            FFTtransform.Click += forrier_transform;
                            this.Controls.Add(FFTtransform);
                            saveButton.Click += SaveButton_Click;
                            this.Controls.Add(saveButton);

                        }

                        private void forrier_transform(object sender, EventArgs e)
                        {
                            Bitmap bmp;

                            
                            OpenFileDialog openFileDialog = new OpenFileDialog();

                           
                            openFileDialog.Title = "Select an image";
                            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";

                            
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                              
                                bmp = new Bitmap(openFileDialog.FileName);


                               
                                Grayscale filter1 = new Grayscale(0.2, 0.7, 0.2);

                               
                                Bitmap grayImage = filter1.Apply(bmp);
                                
                                pictureBox = new PictureBox { Left = 50, Top = 50, Width = 200, Height = 200 };

                                pictureBox.Size = new Size(1500, 1500);
                                this.Controls.Add(pictureBox);


                                int newWidth = (int)Math.Pow(2, Math.Ceiling(Math.Log(grayImage.Width, 2)));
                                int newHeight = (int)Math.Pow(2, Math.Ceiling(Math.Log(grayImage.Height, 2)));

                               
                                ResizeBilinear resizeFilter = new ResizeBilinear(newWidth, newHeight);

                               
                                Bitmap resizedImage = resizeFilter.Apply(grayImage);
                                
                                ComplexImage complexImage = ComplexImage.FromBitmap(resizedImage);
                               
                                complexImage.ForwardFourierTransform();


                               
                                FrequencyFilter filter = new FrequencyFilter(new AForge.IntRange(5, 35)); 

                                
                                filter.Apply(complexImage);

                                
                                complexImage.BackwardFourierTransform();

                               
                                Bitmap filteredImage = complexImage.ToBitmap();

                               
                                pictureBox.Image = filteredImage;

                                saveButton.Enabled = true;

                            }
                        }

                        public void SaveButton_Click(object sender, EventArgs e)
                        {
                            // إنشاء مربع حوار حفظ الملف
                            SaveFileDialog saveFileDialog = new SaveFileDialog();

                            // تعيين خصائص مربع الحوار
                            saveFileDialog.Title = "حفظ الصورة";
                            saveFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";

                            // عرض مربع الحوار
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                // حفظ الصورة في المسار الذي اختاره المستخدم
                                pictureBox.Image.Save(saveFileDialog.FileName);
                            }
                        }

                    }


        public partial class Form4 : Form
        {
            private Bitmap img;
            private PictureBox pictureBox;
            private RichTextBox resultBox;
            private Button loadImgButton, classifyButton;
            private Rectangle selectionRectangle;
            private bool isSelecting;

            public Form4()
            {
               
                this.Width = 800;
                this.Height = 600;

              
                loadImgButton = new Button { Text = "تحميل الصورة", Left = 50, Width = 100, Top = 50, BackColor = Color.Aqua };
                classifyButton = new Button { Text = "تصنيف الحالة", Left = 50, Width = 100, Top = 100, BackColor = Color.Gold };
                pictureBox = new PictureBox { Left = 200, Top = 50, Width = 500, Height = 500, SizeMode = PictureBoxSizeMode.Zoom };
                resultBox = new RichTextBox { Left = 50, Top = 150, Width = 100, Height = 100 };

               
                this.Controls.Add(loadImgButton);
                this.Controls.Add(classifyButton);
                this.Controls.Add(pictureBox);
                this.Controls.Add(resultBox);

              
                loadImgButton.Click += LoadImgButton_Click;
                classifyButton.Click += ClassifyButton_Click;
                pictureBox.MouseDown += PictureBox_MouseDown;
                pictureBox.MouseMove += PictureBox_MouseMove;
                pictureBox.MouseUp += PictureBox_MouseUp;
                pictureBox.Paint += PictureBox_Paint;
            }

            private void LoadImgButton_Click(object sender, EventArgs e)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    img = new Bitmap(dialog.FileName);
                    pictureBox.Image = img;
                }
            }

            private void PictureBox_MouseDown(object sender, MouseEventArgs e)
            {
                if (img == null) return;
                isSelecting = true;
                selectionRectangle = new Rectangle(e.X, e.Y, 0, 0);
            }

            private void PictureBox_MouseMove(object sender, MouseEventArgs e)
            {
                if (isSelecting)
                {
                    selectionRectangle.Width = e.X - selectionRectangle.Left;
                    selectionRectangle.Height = e.Y - selectionRectangle.Top;
                    pictureBox.Invalidate();
                }
            }

            private void PictureBox_MouseUp(object sender, MouseEventArgs e)
            {
                isSelecting = false;
            }

            private void PictureBox_Paint(object sender, PaintEventArgs e)
            {
                if (selectionRectangle != null && selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
                {
                    e.Graphics.DrawRectangle(Pens.Red, selectionRectangle);
                }
            }

            private void ClassifyButton_Click(object sender, EventArgs e)
            {
                if (img != null && selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
                {
                   
                    float scaleX = (float)img.Width / pictureBox.Width;
                    float scaleY = (float)img.Height / pictureBox.Height;

                    Rectangle actualRect = new Rectangle(
                        (int)(selectionRectangle.Left * scaleX),
                        (int)(selectionRectangle.Top * scaleY),
                        (int)(selectionRectangle.Width * scaleX),
                        (int)(selectionRectangle.Height * scaleY)
                    );

                    resultBox.ForeColor = Color.Green;
                    resultBox.Text = "شدة الحالة المرضية: " + ClassifyDiseaseSeverity(img, actualRect);
                }
                else
                {
                    resultBox.ForeColor = Color.Red;
                    resultBox.Text = "الرجاء تحميل الصورة وتحديد جزء منها أولاً.";
                }
            }

            private string ClassifyDiseaseSeverity(Bitmap img, Rectangle rect)
            {
                int whitePixelCount = 0;
                int pixelCount = rect.Width * rect.Height;

                for (int y = rect.Top; y < rect.Top + rect.Height; ++y)
                {
                    for (int x = rect.Left; x < rect.Left + rect.Width; ++x)
                    {
                        Color pixel = img.GetPixel(x, y);
                        int hu = GetHounsfieldUnit(pixel);

                        if (hu >= 1000) 
                        {
                            whitePixelCount++;
                        }
                    }
                }

                float whitePercentage = (whitePixelCount / (float)pixelCount) * 100;

                if (whitePercentage > 85)
                {
                    return "شديدة";
                }
                else if (whitePercentage >= 45 && whitePercentage <= 85)
                {
                    return "متوسطة";
                }
                else
                {
                    return "خفيفة";
                }
            }

            private int GetHounsfieldUnit(Color pixel)
            {
                
                int intensity = (pixel.R + pixel.G + pixel.B) / 3;
                int hu = (int)((intensity / 255.0) * 4000 - 1000);
                return hu;
            }



        }






    }


  



}








