using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PracticalWork3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            VolumeSlider.Minimum = 0;
            VolumeSlider.Maximum = 1;
            VolumeSlider.Value = 1;
            songList.DisplayMemberPath = "songName";
        }

        int fileIndex = 0;
        Random rand = new Random();
        List<Song> fileList = new List<Song>();
        List<Song> randomList = new List<Song>();
        bool pause = false;
        bool loop = false;
        bool random = false;

        private async void FolderOpen_Button_Click(object sender, RoutedEventArgs e)
        {
            

            if (random) Random_Button_Click(sender, e);
            if (pause) Pause_Button_Click(sender, e);
            mediaElement.Stop();

            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            dialog.Title = "Выберите папку с музыкой";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(dialog.FileName);

                fileList.Clear();
                foreach (string file in files)
                {
                    if (file.EndsWith(".mp3") | file.EndsWith(".waw") | file.EndsWith(".m4a"))
                    {
                        fileList.Add(new Song(file));
                    }
                }

                songList.ItemsSource = fileList;
                songList.SelectedIndex = 0;

                Play_Button_Click(sender, e);
            }
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.Count != 0)
            {
                mediaElement.Stop();
                SongPositionSlider.Value = 0;

                if (!random)
                {
                    CurrentSongName.Text = fileList[songList.SelectedIndex].songName;
                    mediaElement.Source = new Uri(fileList[songList.SelectedIndex].path);
                }
                else
                {
                    CurrentSongName.Text = randomList[songList.SelectedIndex].songName;
                    mediaElement.Source = new Uri(randomList[songList.SelectedIndex].path);
                }
                mediaElement.Play();
            }
        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!pause)
            {
                mediaElement.Pause();
                Pause_Button.Foreground = Brushes.Orange;
                pause = true;
            }
            else
            {
                mediaElement.Play();
                Pause_Button.Foreground = Brushes.White;
                pause = false;
            }
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
        }

        private void songList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Play_Button_Click(sender, e);
        }

        private void Loop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!loop)
            {
                Loop_Button.Foreground = Brushes.Orange;
                loop = true;
            }
            else
            {
                Loop_Button.Foreground = Brushes.White;
                loop = false;
            }
        }

        private void Random_Button_Click(object sender, RoutedEventArgs e)
        {
            RandomList();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = (double)VolumeSlider.Value;
        }

        private void SongPositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Position = new TimeSpan(Convert.ToInt64(SongPositionSlider.Value));

        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            timer.Start();
            SongPositionSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.Ticks;
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {

            if (!loop) NextSong_Button_Click(sender, e);
            else Play_Button_Click(sender, e);
        }

        private void PreviousSong_Button_Click(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedIndex == 0 | songList.SelectedIndex == -1)
            {
                songList.SelectedIndex = songList.Items.Count - 1;
            }
            else
            {
                songList.SelectedIndex--;
            }
        }

        private void NextSong_Button_Click(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedIndex + 1 == songList.Items.Count)
            {
                songList.SelectedIndex = 0;
            }
            else
            {
                songList.SelectedIndex++;
            }
        }

        private void RandomList()
        {
            if (!random)
            {
                Random_Button.Foreground = Brushes.Orange;
                random = true;

                fileIndex = songList.SelectedIndex;

                foreach (Song song in fileList)
                {
                    randomList.Add(song);
                }

                for (int i = randomList.Count - 1; i >= 1; i--)
                {
                    int j = rand.Next(i + 1);

                    var temp = randomList[j];
                    randomList[j] = randomList[i];
                    randomList[i] = temp;
                }

                songList.ItemsSource = randomList;
                songList.SelectedIndex = 0;

            }
            else
            {
                Random_Button.Foreground = Brushes.White;
                random = false;

                songList.ItemsSource = fileList;
                songList.SelectedIndex = fileIndex;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SongPositionSlider.Value = mediaElement.Position.Ticks;

            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                if (mediaElement.Position.Seconds < 10)
                {
                    NowTime.Text = mediaElement.Position.Minutes.ToString() + ":0" + mediaElement.Position.Seconds.ToString();
                    RemainingTime.Text = (mediaElement.NaturalDuration.TimeSpan.Minutes - mediaElement.Position.Minutes).ToString() + ":" + (mediaElement.NaturalDuration.TimeSpan.Seconds - mediaElement.Position.Seconds).ToString();
                }
                else if (mediaElement.NaturalDuration.TimeSpan.Seconds - mediaElement.Position.Seconds < 10)
                {
                    NowTime.Text = mediaElement.Position.Minutes.ToString() + ":" + mediaElement.Position.Seconds.ToString();
                    RemainingTime.Text = (mediaElement.NaturalDuration.TimeSpan.Minutes - mediaElement.Position.Minutes).ToString() + ":" + (mediaElement.NaturalDuration.TimeSpan.Seconds - mediaElement.Position.Seconds).ToString();
                }
                else
                {
                    NowTime.Text = mediaElement.Position.Minutes.ToString() + ":" + mediaElement.Position.Seconds.ToString();
                    RemainingTime.Text = (mediaElement.NaturalDuration.TimeSpan.Minutes - mediaElement.Position.Minutes).ToString() + ":" + (mediaElement.NaturalDuration.TimeSpan.Seconds - mediaElement.Position.Seconds).ToString();
                }
            }
        }
    }
}
