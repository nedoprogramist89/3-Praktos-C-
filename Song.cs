using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TagLib;
namespace PracticalWork3
{
    internal class Song
    {
        public string path { get; set; }
        public TagLib.File file { get; set; }
        public string songName { get; set; }
        public BitmapImage songImage { get; set; }

        public Song(string path)
        {
            this.path = path;
            file = TagLib.File.Create(path);
            songName = file.Tag.Title ?? Path.GetFileNameWithoutExtension(path);

            BitmapImage bitmap = new BitmapImage();

            if (file.Tag.Pictures != null && file.Tag.Pictures.Length != 0)
            {
                IPicture SongCover = file.Tag.Pictures[0];
                using (MemoryStream ms = new MemoryStream(SongCover.Data.Data))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    songImage = bitmap;
                }
            }
            else
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("someImage.jpg", UriKind.RelativeOrAbsolute);
                bitmap.EndInit();
                songImage = bitmap;
            }
        }
    }
}
