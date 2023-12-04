using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace tiff
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string selected_folder = SelectWorkingFolder();
            if (selected_folder != null)
            {
                CreateMonochromeMultiTiff(selected_folder);
            }

        }

        public static string SelectWorkingFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
                else return null;
            }
        }

        public static void CreateMonochromeMultiTiff(string source_folder)
        {
            string[] files = System.IO.Directory.GetFiles(source_folder, "*.jpg");
            if (files != null)
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
                ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/tiff");

                TiffBitmapEncoder encoder = new TiffBitmapEncoder() { Compression = TiffCompressOption.Ccitt4 };
                using MemoryStream ms = new MemoryStream();

                foreach (string file in files)
                {
                    using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(file))
                    {
                        bitmap.Save(ms, imageCodecInfo, encoderParameters);
                        BitmapFrame bitmapFrame = BitmapFrame.Create(ms);
                        encoder.Frames.Add(bitmapFrame);
                    }
                }

                using(FileStream fileStream = new FileStream(source_folder +"\\multi.tiff", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    encoder.Save(fileStream);
                }

            }
        }

        public static void CreateMultiTiff(string source_folder)
        {
            string[] image_files = System.IO.Directory.GetFiles(source_folder, "*.jpg");
            string save_path = source_folder + "\\multi.tiff";

            var encoder = new TiffBitmapEncoder()
            {
                Compression = TiffCompressOption.Ccitt4
            };

            foreach (string file_name in image_files)
            {
                BitmapFrame bmpFrame = BitmapFrame.Create(new Uri(file_name, UriKind.RelativeOrAbsolute));
                encoder.Frames.Add(bmpFrame);
            }

            var outputFileStrm = new FileStream(save_path, FileMode.Create, FileAccess.Write, FileShare.None);
            encoder.Save(outputFileStrm);
            outputFileStrm.Close();
        }

        public static void ConvertToMonochromeTiff(string source_folder)
        {
            string[] files = System.IO.Directory.GetFiles(source_folder, "*.jpg");
            if (files != null)
            {
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionCCITT4);

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");
                string destination_folder = source_folder + "\\bmp\\";
                Directory.CreateDirectory(destination_folder);

                foreach (string file_name in files)
                {
                    using (System.Drawing.Bitmap sourceBitmap = new System.Drawing.Bitmap(file_name))
                    {
                        sourceBitmap.Save(destination_folder + Path.GetFileName(file_name), myImageCodecInfo, encoderParams);
                    }
                }

            }

        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }

}
