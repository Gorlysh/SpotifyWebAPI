using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using WMPLib;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace SpotifyProgramForms
{
    public partial class Form1 : Form
    {

        private readonly SpotifyTrack spotify_api;
        private List<string> track_id_page1;//id треков в первой вкладке
        private List<string> track_id_page2;//id треков во второй вкладке
        private List<string> track_id_page3;//id треков в третьей вкладке
        private List<string> track_id_page4;//id треков в четвертой вкладке
        private List<string> track_id_recomendation;//id рекомендуемых треков 
        private List<string> track_id_encountered;//id треков знакомых пользователю 
        private List<string> playlist_id;//id плейлистов в третьей вкладке
        private string file_name;//имя файла для сохранения
        private int num_page; //номер вкладки
        private MLModel1.ModelInput TrackData; //структура хранящая данные о треке
        private MLModel1 Model1;
        private WindowsMediaPlayer player;
        private PlotView pwDance;
        private List<double> mass_of_specification;
        private List<double> mass_of_specification_other_user;

        public Form1()
        {

            InitializeComponent();
            spotify_api = new SpotifyTrack();
            track_id_page1 = new List<string>();
            track_id_page2 = new List<string>();
            track_id_page3 = new List<string>();
            track_id_page4 = new List<string>();
            track_id_recomendation = new List<string>();
            track_id_encountered = new List<string>();
            playlist_id = new List<string>();
            file_name = null;
            TrackData = new MLModel1.ModelInput();
            Model1 = new MLModel1();
            player = new WindowsMediaPlayer();
            pwDance = new PlotView();
            mass_of_specification = new List<double>();
            mass_of_specification_other_user = new List<double>();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pwDance.Location = new System.Drawing.Point(1000, 400);
            pwDance.Size = new System.Drawing.Size(500, 500);
            this.Controls.Add(pwDance);
            pwDance.Visible = false;
            // var point = new OxyPlot.Series.DataPointSeries();
            //   pw.Model.Series.Add(new OxyPlot.Series.DataPointSeries(points));


        }

        private void button12_Click(object sender, EventArgs e) //кнопка выбор файла для сохранения
        {

            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                file_name = saveFileDialog1.FileName; //сохраняем имя выбранного файла
            }
        }

        private void View_Info(Label label, TextBox textBox, string track_id) //вывод информации о треке
        {
            label.Visible = true;
            textBox.Visible = true;
            string full_artist = null;
            var track = spotify_api.GetTrack(track_id).Result;//получаем информацию о треке
            for (int j = 0; j < track.artists.Count; j++) // формируем полный список исполнителей
            {
                if (j != track.artists.Count - 1)
                {
                    full_artist = full_artist + track.artists[j].name + ", ";
                }
                else full_artist = full_artist + track.artists[j].name;
            }
            var track_info = spotify_api.GetAudioFeatures(track.id).Result;//получаем музыкальные характеристики трека
            textBox.Text = "Название трека: " + track.name + Environment.NewLine + "Исполнитель: " //выводим всю информацию
                + full_artist + Environment.NewLine + "Альбом: " + track.album.name + Environment.NewLine + Environment.NewLine
                    + "Наличие акустических инструментов (мера уверенности): " + track_info.Acousticness + Environment.NewLine
                        + "Танцевальность: " + track_info.Danceability + Environment.NewLine
                            + "Продолжительность: " + track_info.Duration_Ms + Environment.NewLine
                                + "Энергичность: " + track_info.Energy + Environment.NewLine
                                    + "Инструментальность: " + track_info.Instrumentalness + Environment.NewLine
                                        + "Тональность: " + track_info.Key + Environment.NewLine
                                            + "Концертная запись: " + track_info.Liveness + Environment.NewLine
                                                + "Наличие помех: " + track_info.Loudness + Environment.NewLine
                                                    + "Лад: " + track_info.Mode + Environment.NewLine
                                                        + "Наличие вокала: " + track_info.Speechiness + Environment.NewLine
                                                            + "Темп: " + track_info.Tempo + Environment.NewLine
                                                                + "Размерность трека: " + track_info.Time_Signature + Environment.NewLine
                                                                    + "Позитивность: " + track_info.Valence;

            TrackData.Acousticness = Convert.ToSingle(track_info.Acousticness);          //Сохраняем данные о треке для анализа
            TrackData.Danceability = Convert.ToSingle(track_info.Danceability);
            TrackData.Duration_Ms = Convert.ToSingle(track_info.Duration_Ms);
            TrackData.Energy = Convert.ToSingle(track_info.Energy);
            TrackData.Instrumentalness = Convert.ToSingle(track_info.Instrumentalness);
            TrackData.Key = Convert.ToSingle(track_info.Key);
            TrackData.Liveness = Convert.ToSingle(track_info.Liveness);
            TrackData.Loudness = Convert.ToSingle(track_info.Loudness);
            TrackData.Mode = Convert.ToSingle(track_info.Mode);
            TrackData.Speechiness = Convert.ToSingle(track_info.Speechiness);
            TrackData.Tempo = Convert.ToSingle(track_info.Tempo);
            TrackData.Time_Signature = Convert.ToSingle(track_info.Time_Signature);
            TrackData.Valence = Convert.ToSingle(track_info.Valence);

            if (track.preview_url != null && track.preview_url != player.URL) //воспроизведение отрывка
            {
                player.URL = track.preview_url;
                player.controls.play();
            }
            else
            {
                player.controls.stop();
                player.URL = null;
            }
        }

        private void View_Info(Label label, TextBox textBox, Root_Track track) //вывод информации о треке
        {
            label.Visible = true;
            textBox.Visible = true;
            string full_artist = null;
            // var track = spotify_api.GetTrack(track_id).Result;//получаем информацию о треке
            for (int j = 0; j < track.artists.Count; j++) // формируем полный список исполнителей
            {
                if (j != track.artists.Count - 1)
                {
                    full_artist = full_artist + track.artists[j].name + ", ";
                }
                else full_artist = full_artist + track.artists[j].name;
            }
            var track_info = spotify_api.GetAudioFeatures(track.id).Result;//получаем музыкальные характеристики трека
            textBox.Text = "Название трека: " + track.name + Environment.NewLine + "Исполнитель: " //выводим всю информацию
                + full_artist + Environment.NewLine + "Альбом: " + track.album.name + Environment.NewLine + Environment.NewLine
                    + "Наличие акустических инструментов (мера уверенности): " + track_info.Acousticness + Environment.NewLine
                        + "Танцевальность: " + track_info.Danceability + Environment.NewLine
                            + "Продолжительность: " + track_info.Duration_Ms + Environment.NewLine
                                + "Энергичность: " + track_info.Energy + Environment.NewLine
                                    + "Инструментальность: " + track_info.Instrumentalness + Environment.NewLine
                                        + "Тональность: " + track_info.Key + Environment.NewLine
                                            + "Концертная запись: " + track_info.Liveness + Environment.NewLine
                                                + "Наличие помех: " + track_info.Loudness + Environment.NewLine
                                                    + "Лад: " + track_info.Mode + Environment.NewLine
                                                        + "Наличие вокала: " + track_info.Speechiness + Environment.NewLine
                                                            + "Темп: " + track_info.Tempo + Environment.NewLine
                                                                + "Размерность трека: " + track_info.Time_Signature + Environment.NewLine
                                                                    + "Позитивность: " + track_info.Valence;

            TrackData.Acousticness = Convert.ToSingle(track_info.Acousticness);          //Сохраняем данные о треке для анализа
            TrackData.Danceability = Convert.ToSingle(track_info.Danceability);
            TrackData.Duration_Ms = Convert.ToSingle(track_info.Duration_Ms);
            TrackData.Energy = Convert.ToSingle(track_info.Energy);
            TrackData.Instrumentalness = Convert.ToSingle(track_info.Instrumentalness);
            TrackData.Key = Convert.ToSingle(track_info.Key);
            TrackData.Liveness = Convert.ToSingle(track_info.Liveness);
            TrackData.Loudness = Convert.ToSingle(track_info.Loudness);
            TrackData.Mode = Convert.ToSingle(track_info.Mode);
            TrackData.Speechiness = Convert.ToSingle(track_info.Speechiness);
            TrackData.Tempo = Convert.ToSingle(track_info.Tempo);
            TrackData.Time_Signature = Convert.ToSingle(track_info.Time_Signature);
            TrackData.Valence = Convert.ToSingle(track_info.Valence);

            if (track.preview_url != null && track.preview_url != player.URL) //воспроизведение отрывка
            {
                player.URL = track.preview_url;
                player.controls.play();
            }
            else
            {
                player.controls.stop();
                player.URL = null;
            }
        }

        private void View_Track(CheckBox checkBox, Label artist, Label album, Root track_search, PictureBox pictureBox, int cnt) //Показ трека с обложкой и плеером (на первой вкладке)
        {
            checkBox.Visible = true;
            artist.Visible = true;
            album.Visible = true;
            checkBox.Text = track_search.tracks.items[cnt].name;
            if (checkBox.Text.Length > 40) //название трека
            {
                checkBox.Text = checkBox.Text.Remove(40);
                checkBox.Text += "...";
            }
            track_id_page1.Add(track_search.tracks.items[cnt].id); //сохраняю в лист id трека
            artist.Text = null;
            for (int j = 0; j < track_search.tracks.items[cnt].artists.Count; j++) //имя исполнителей
            {
                if ((j == track_search.tracks.items[cnt].artists.Count - 1) || (j == 3))
                {
                    if (j != track_search.tracks.items[cnt].artists.Count - 1)
                    {
                        artist.Text = artist.Text + track_search.tracks.items[cnt].artists[j].name + ", ...";
                    }
                    else artist.Text = artist.Text + track_search.tracks.items[cnt].artists[j].name;
                }
                else artist.Text = artist.Text + track_search.tracks.items[cnt].artists[j].name + ", ";

                if (j == 3) { j = track_search.tracks.items[cnt].artists.Count - 1; }
            }

            if (track_search.tracks.items[cnt].album.name.Length > 25)
            {
                album.Text = "Альбом: " + track_search.tracks.items[cnt].album.name.Substring(0, 25) + "..."; //название альбома
            }
            else album.Text = "Альбом: " + track_search.tracks.items[cnt].album.name; //название альбома

            if (track_search.tracks.items[cnt].album.images.Count == 3) //показ обложки
            {
                pictureBox.Visible = true;
                pictureBox.Enabled = true;
                pictureBox.ImageLocation = track_search.tracks.items[cnt].album.images[2].url;
            }

        }

        private void View_Track(Label title, Label artist, Label album, Root_Track track, PictureBox pictureBox)//Показ трека с обложкой и плеером (на второй и третьей вкладке)
        {
            title.Visible = true;
            artist.Visible = true;
            album.Visible = true;
            title.Text = track.name;
            if (title.Text.Length > 40)  //название трека
            {
                title.Text = title.Text.Remove(40);
                title.Text += "...";
            }
            artist.Text = null;

            for (int j = 0; j < track.artists.Count; j++) //список исполнителей
            {
                if ((j == track.artists.Count - 1) || (j == 3))
                {
                    if (j != track.artists.Count - 1)
                    {
                        artist.Text = artist.Text + track.artists[j].name + ", ...";
                    }
                    else artist.Text = artist.Text + track.artists[j].name;
                }
                else artist.Text = artist.Text + track.artists[j].name + ", ";

                if (j == 3) { j = track.artists.Count - 1; }
            }

            if (track.album.name.Length > 25)
            {
                album.Text = "Альбом: " + track.album.name.Substring(0, 25) + "..."; //название альбома
            }
            else album.Text = "Альбом: " + track.album.name; //название альбома

            if (track.album.images.Count == 3) //обложка трека
            {
                pictureBox.Visible = true;
                pictureBox.ImageLocation = track.album.images[2].url;
            }

        }

        private void List_of_Playlist(CheckedListBox checkedListBox, Root_List_of_PlayList_Item playlist, List<string> track_id, int offset) //вывод списка треков в плейлисте (вторая и третья вкладка)
        {
            checkedListBox.Visible = true;
            if (offset == 0) //очищаем массив от старых id треков
            {
                track_id.Clear();
            }

            string str_artict = null;
            for (int i = 0; i < playlist.items.Count; i++) //вывод треков
            {
                str_artict = null;
                for (int j = 0; j < playlist.items[i].track.artists.Count; j++) //формируем список исполнителей
                {
                    if ((j == playlist.items[i].track.artists.Count - 1) || (j == 3))
                    {
                        if (j != playlist.items[i].track.artists.Count - 1)
                        {
                            str_artict = str_artict + playlist.items[i].track.artists[j].name + ", ...";
                        }
                        else str_artict = str_artict + playlist.items[i].track.artists[j].name;
                    }
                    else str_artict = str_artict + playlist.items[i].track.artists[j].name + ", ";

                    if (j == 3) { j = playlist.items[i].track.artists.Count - 1; }
                }
                string num = "#" + (i + 1 + offset); //номер трека в списке
                string str_title = num.PadRight(5).Insert(5, playlist.items[i].track.name);
                if (str_title.Length > 40) //название трека
                {
                    str_title = str_title.Remove(40);
                    str_title += "...";
                }
                string str_res = str_title.PadRight(50).Insert(50, "Исполнитель: " + str_artict);
                checkedListBox.Items.Add(str_res); //добавляем строку трека в чеклист
                track_id.Add(playlist.items[i].track.id); //добавляем id трека в массив
            }
        }

        private void List_of_Playlist(CheckedListBox checkedListBox, Root_SavedTracks_Rootobject playlist, List<string> track_id, int offset) //вывод списка треков в плейлисте (вторая и третья вкладка)
        {
            checkedListBox.Visible = true;
            if (offset == 0) //очищаем массив от старых id треков
            {
                track_id.Clear();
            }

            string str_artict = null;
            for (int i = 0; i < playlist.items.Length; i++) //вывод треков
            {
                str_artict = null;
                for (int j = 0; j < playlist.items[i].track.artists.Length; j++) //формируем список исполнителей
                {
                    if ((j == playlist.items[i].track.artists.Length - 1) || (j == 3))
                    {
                        if (j != playlist.items[i].track.artists.Length - 1)
                        {
                            str_artict = str_artict + playlist.items[i].track.artists[j].name + ", ...";
                        }
                        else str_artict = str_artict + playlist.items[i].track.artists[j].name;
                    }
                    else str_artict = str_artict + playlist.items[i].track.artists[j].name + ", ";

                    if (j == 3) { j = playlist.items[i].track.artists.Length - 1; }
                }
                string num = "#" + (i + 1 + offset); //номер трека в списке
                string str_title = num.PadRight(5).Insert(5, playlist.items[i].track.name);
                if (str_title.Length > 40) //название трека
                {
                    str_title = str_title.Remove(40);
                    str_title += "...";
                }
                string str_res = str_title.PadRight(50).Insert(50, "Исполнитель: " + str_artict);
                checkedListBox.Items.Add(str_res); //добавляем строку трека в чеклист
                track_id.Add(playlist.items[i].track.id); //добавляем id трека в массив
            }
        }
        private void Write_to_File(CheckedListBox checkedListBox, List<string> track_id, string user_id) //запись данных о треках в файл авторизованного пользователя
        {
            if (file_name != null)
            {
                try
                {


                    StreamWriter csv = new StreamWriter(file_name, true, Encoding.UTF8); //создаем или открываем файл
                    csv.Close();//закрываем поток

                    bool flag = false;
                    StreamReader csvread = new StreamReader(file_name);
                    string first_str = csvread.ReadLine();
                    if (first_str == "USER_ID;TRACK_ID;NAME;ARTIST;Acousticness;Danceability;Duration_Ms;Energy;Instrumentalness;Key;Liveness;Loudness;Mode;Speechiness;Tempo;Time_Signature;Valence;Like")
                    {
                        flag = true;
                    }
                    csvread.Close();

                    csv = new StreamWriter(file_name, true, Encoding.UTF8); ;


                    if (flag == false)
                    {
                        csv.WriteLine("USER_ID" + ';' + "TRACK_ID" + ';' + "NAME" + ';' + "ARTIST" + ';' + "Acousticness"
                                              + ';' + "Danceability" + ';' + "Duration_Ms"
                                                + ';' + "Energy" + ';' + "Instrumentalness"
                                                    + ';' + "Key" + ';' + "Liveness"
                                                        + ';' + "Loudness" + ';' + "Mode"
                                                           + ';' + "Speechiness" + ';' + "Tempo"
                                                               + ';' + "Time_Signature" + ';' + "Valence" + ';' + "Like", Encoding.UTF8);
                    }



                    var track = spotify_api.GetSeveralTrack(track_id).Result; //получаем треки в виде двумерного массива (по 50 в одном векторе)
                    var track_info = spotify_api.GetSeveralAudioFeatures(track_id).Result; //получаем музыкальные характеристики треков в виде двумерного массива (по 100 в одном векторе)
                    int cnt = 0;
                    int t = 0; //счётчик вектора треков
                    int h = 0;//счётчик вектора музыкальных характеристик
                    for (int i = 0; i < checkedListBox.Items.Count; i++)
                    {
                        t = i / 50; //первый вектор - 0, второй вектор - 1, третий вектор - 3 и т.д.
                        h = i / 100;//первый вектор - 0, второй вектор - 1, третий вектор - 3 и т.д.
                        if (checkedListBox.GetItemChecked(i) == true) //если трек выбран, то записываем
                        {
                            csv.WriteLine(user_id + ';' + track_id[i] + ';' + track[t].tracks[i - 50 * t].name + ';' + track[t].tracks[i - 50 * t].artists[0].name + ';' + track_info[h].Audio_Features[i - 100 * h].Acousticness
                                               + ';' + track_info[h].Audio_Features[i - 100 * h].Danceability + ';' + track_info[h].Audio_Features[i - 100 * h].Duration_Ms
                                                 + ';' + track_info[h].Audio_Features[i - 100 * h].Energy + ';' + track_info[h].Audio_Features[i - 100 * h].Instrumentalness
                                                     + ';' + track_info[h].Audio_Features[i - 100 * h].Key + ';' + track_info[h].Audio_Features[i - 100 * h].Liveness
                                                         + ';' + track_info[h].Audio_Features[i - 100 * h].Loudness + ';' + track_info[h].Audio_Features[i - 100 * h].Mode
                                                            + ';' + track_info[h].Audio_Features[i - 100 * h].Speechiness + ';' + track_info[h].Audio_Features[i - 100 * h].Tempo
                                                                + ';' + track_info[h].Audio_Features[i - 100 * h].Time_Signature + ';' + track_info[h].Audio_Features[i - 100 * h].Valence + ';' + 1, Encoding.UTF8);
                            cnt++;

                        }

                    }
                    for (int i = 0; i < checkedListBox.Items.Count; i++) checkedListBox.SetItemChecked(i, false); //весь чеклист в false
                    MessageBox.Show(String.Format("Количестов записанных треков - {0}.", cnt));

                    csv.Close();//закрываем поток
                }
                catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex); }
            }
            else MessageBox.Show("Файл для записи не выбран");
        }



        private void Write_to_File(CheckedListBox checkedListBox, List<string> track_id) //запись данных о треках в файл не авторпизованного пользователя
        {
            if (file_name != null)
            {
                try
                {

                    StreamWriter csv = new StreamWriter(file_name, true, Encoding.UTF8); //создаем или открываем файл
                    csv.Close();//закрываем поток

                    bool flag = false;
                    StreamReader csvread = new StreamReader(file_name);
                    string first_str = csvread.ReadLine();
                    if (first_str == "USER_ID;TRACK_ID;NAME;ARTIST;Acousticness;Danceability;Duration_Ms;Energy;Instrumentalness;Key;Liveness;Loudness;Mode;Speechiness;Tempo;Time_Signature;Valence;Like")
                    {
                        flag = true;
                    }
                    csvread.Close();

                    csv = new StreamWriter(file_name, true, Encoding.UTF8); ;

                    if (flag == false)
                    {
                        csv.WriteLine("USER_ID" + ';' + "TRACK_ID" + ';' + "NAME" + ';' + "ARTIST" + ';' + "Acousticness"
                                              + ';' + "Danceability" + ';' + "Duration_Ms"
                                                + ';' + "Energy" + ';' + "Instrumentalness"
                                                    + ';' + "Key" + ';' + "Liveness"
                                                        + ';' + "Loudness" + ';' + "Mode"
                                                           + ';' + "Speechiness" + ';' + "Tempo"
                                                               + ';' + "Time_Signature" + ';' + "Valence" + ';' + "Like", Encoding.UTF8);
                    }


                    string user_id;

                    if (num_page == 3)
                    {
                        user_id = textBox4.Text;
                    }
                    else
                    {
                        if (label44.Visible == true) //проверка, что пользователь авторизован
                        {
                            user_id = spotify_api.GetUserProfile().Result.id;
                        }
                        else { user_id = "No_Autorization"; }

                    }


                    var track = spotify_api.GetSeveralTrack(track_id).Result; //получаем треки в виде двумерного массива (по 50 в одном векторе)
                    var track_info = spotify_api.GetSeveralAudioFeatures(track_id).Result; //получаем музыкальные характеристики треков в виде двумерного массива (по 100 в одном векторе)
                    int cnt = 0;
                    int t = 0; //счётчик вектора треков
                    int h = 0;//счётчик вектора музыкальных характеристик
                    for (int i = 0; i < checkedListBox.Items.Count; i++)
                    {
                        t = i / 50; //первый вектор - 0, второй вектор - 1, третий вектор - 3 и т.д.
                        h = i / 100;//первый вектор - 0, второй вектор - 1, третий вектор - 3 и т.д.
                        if (checkedListBox.GetItemChecked(i) == true) //если трек выбран, то like=1 
                        {
                            csv.WriteLine(user_id + ';' + track_id[i] + ';' + track[t].tracks[i - 50 * t].name + ';' + track[t].tracks[i - 50 * t].artists[0].name + ';' + track_info[h].Audio_Features[i - 100 * h].Acousticness
                                               + ';' + track_info[h].Audio_Features[i - 100 * h].Danceability + ';' + track_info[h].Audio_Features[i - 100 * h].Duration_Ms
                                                 + ';' + track_info[h].Audio_Features[i - 100 * h].Energy + ';' + track_info[h].Audio_Features[i - 100 * h].Instrumentalness
                                                     + ';' + track_info[h].Audio_Features[i - 100 * h].Key + ';' + track_info[h].Audio_Features[i - 100 * h].Liveness
                                                         + ';' + track_info[h].Audio_Features[i - 100 * h].Loudness + ';' + track_info[h].Audio_Features[i - 100 * h].Mode
                                                            + ';' + track_info[h].Audio_Features[i - 100 * h].Speechiness + ';' + track_info[h].Audio_Features[i - 100 * h].Tempo
                                                                + ';' + track_info[h].Audio_Features[i - 100 * h].Time_Signature + ';' + track_info[h].Audio_Features[i - 100 * h].Valence + ';' + 1, Encoding.UTF8);

                        }
                        else //иначе like=0
                        {
                            csv.WriteLine(user_id + ';' + track_id[i] + ';' + track[t].tracks[i - 50 * t].name + ';' + track[t].tracks[i - 50 * t].artists[0].name + ';' + track_info[h].Audio_Features[i - 100 * h].Acousticness
                                               + ';' + track_info[h].Audio_Features[i - 100 * h].Danceability + ';' + track_info[h].Audio_Features[i - 100 * h].Duration_Ms
                                                 + ';' + track_info[h].Audio_Features[i - 100 * h].Energy + ';' + track_info[h].Audio_Features[i - 100 * h].Instrumentalness
                                                     + ';' + track_info[h].Audio_Features[i - 100 * h].Key + ';' + track_info[h].Audio_Features[i - 100 * h].Liveness
                                                         + ';' + track_info[h].Audio_Features[i - 100 * h].Loudness + ';' + track_info[h].Audio_Features[i - 100 * h].Mode
                                                            + ';' + track_info[h].Audio_Features[i - 100 * h].Speechiness + ';' + track_info[h].Audio_Features[i - 100 * h].Tempo
                                                                + ';' + track_info[h].Audio_Features[i - 100 * h].Time_Signature + ';' + track_info[h].Audio_Features[i - 100 * h].Valence + ';' + 0, Encoding.UTF8);

                        }
                        cnt++;
                    }
                    for (int i = 0; i < checkedListBox.Items.Count; i++) checkedListBox.SetItemChecked(i, false); //весь чеклист в false
                    MessageBox.Show(String.Format("Количестов записанных треков - {0}.", cnt));

                    csv.Close();//закрываем поток
                }
                catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex); }
            }
            else MessageBox.Show("Файл для записи не выбран");
        }




        //Первая вкладка_______________________________________________________________________________________
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //Поиск трека (первая вкладка)
        {
            label34.Visible = false;
            textBox6.Visible = false;
            track_id_page1.Clear(); //очищаем массив от старых id треков

            CheckBox[] checkBoxes = new CheckBox[10];
            checkBoxes[0] = checkBox1;
            checkBoxes[1] = checkBox2;
            checkBoxes[2] = checkBox3;
            checkBoxes[3] = checkBox4;
            checkBoxes[4] = checkBox5;
            checkBoxes[5] = checkBox6;
            checkBoxes[6] = checkBox7;
            checkBoxes[7] = checkBox8;
            checkBoxes[8] = checkBox9;
            checkBoxes[9] = checkBox10;
            Label[] label_artist = new Label[10];
            label_artist[0] = label14;
            label_artist[1] = label16;
            label_artist[2] = label18;
            label_artist[3] = label20;
            label_artist[4] = label22;
            label_artist[5] = label24;
            label_artist[6] = label26;
            label_artist[7] = label28;
            label_artist[8] = label30;
            label_artist[9] = label32;
            Label[] label_album = new Label[10];
            label_album[0] = label13;
            label_album[1] = label15;
            label_album[2] = label17;
            label_album[3] = label19;
            label_album[4] = label21;
            label_album[5] = label23;
            label_album[6] = label25;
            label_album[7] = label27;
            label_album[8] = label29;
            label_album[9] = label31;
            PictureBox[] pictureBoxes = new PictureBox[10];
            pictureBoxes[0] = pictureBox1;
            pictureBoxes[1] = pictureBox2;
            pictureBoxes[2] = pictureBox3;
            pictureBoxes[3] = pictureBox4;
            pictureBoxes[4] = pictureBox5;
            pictureBoxes[5] = pictureBox6;
            pictureBoxes[6] = pictureBox7;
            pictureBoxes[7] = pictureBox8;
            pictureBoxes[8] = pictureBox9;
            pictureBoxes[9] = pictureBox10;

            for (int i = 0; i < 10; i++) //скрываем все элементы
            {
                checkBoxes[i].Visible = false;
                label_artist[i].Visible = false;
                label_album[i].Visible = false;
                checkBoxes[i].Checked = false;
                pictureBoxes[i].Visible = false;
                pictureBoxes[i].Enabled = false;
                player.controls.stop();
            }

            try
            {
                if (textBox1.Text != "" || textBox2.Text != "")
                {
                    var track_search = spotify_api.SearchTrack(textBox1.Text, textBox2.Text).Result; //выполняем поиск


                    if (track_search.tracks.items.Count != 0)
                    {
                        button2.Enabled = true;
                        button3.Enabled = true;
                        button2.Visible = true;
                        button3.Visible = true;
                    }
                    else
                    {
                        button2.Enabled = false;
                        button3.Enabled = false;
                        button2.Visible = false;
                        button3.Visible = false;
                        MessageBox.Show("Поиск не дал результатов!");
                    }

                    for (int i = 0; i < track_search.tracks.items.Count; i++) //выводим 10 найденных треков
                    {
                        if (i > 9) { }
                        else
                        {
                            View_Track(checkBoxes[i], label_artist[i], label_album[i], track_search, pictureBoxes[i], i);
                        }

                    }
                }
                else MessageBox.Show("Поиск не дал результатов!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e) //при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[0]); //вывод информации о треке
        }

        private void pictureBox2_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[1]);//вывод информации о треке
        }

        private void pictureBox3_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[2]);//вывод информации о треке
        }

        private void pictureBox4_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[3]);//вывод информации о треке
        }

        private void pictureBox5_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[4]);//вывод информации о треке
        }

        private void pictureBox6_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[5]);//вывод информации о треке
        }

        private void pictureBox7_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[6]);//вывод информации о треке
        }

        private void pictureBox8_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[7]);//вывод информации о треке
        }

        private void pictureBox9_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[8]);//вывод информации о треке
        }

        private void pictureBox10_Click(object sender, EventArgs e)//при нажатии на обложку трека, показать информацию о  треке (первая вкладка)
        {
            View_Info(label34, textBox6, track_id_page1[9]);//вывод информации о треке
        }


        private void button3_Click(object sender, EventArgs e) //кнопка выбрать все (первая вкладка)
        {
            CheckBox[] checkBoxes = new CheckBox[10];
            checkBoxes[0] = checkBox1;
            checkBoxes[1] = checkBox2;
            checkBoxes[2] = checkBox3;
            checkBoxes[3] = checkBox4;
            checkBoxes[4] = checkBox5;
            checkBoxes[5] = checkBox6;
            checkBoxes[6] = checkBox7;
            checkBoxes[7] = checkBox8;
            checkBoxes[8] = checkBox9;
            checkBoxes[9] = checkBox10;

            for (int i = 0; i < 10; i++) //делаем все чекбоксы true
            {
                if (checkBoxes[i].Visible == true)
                {
                    checkBoxes[i].Checked = true;
                }
                else i = 9;
            }
        }

        private void button2_Click(object sender, EventArgs e) //Кнопка записать в файл (первая вкладка)
        {
            CheckBox[] checkBoxes = new CheckBox[10];
            checkBoxes[0] = checkBox1;
            checkBoxes[1] = checkBox2;
            checkBoxes[2] = checkBox3;
            checkBoxes[3] = checkBox4;
            checkBoxes[4] = checkBox5;
            checkBoxes[5] = checkBox6;
            checkBoxes[6] = checkBox7;
            checkBoxes[7] = checkBox8;
            checkBoxes[8] = checkBox9;
            checkBoxes[9] = checkBox10;

            try
            {
                if (file_name != null)
                {

                    bool flag = false;
                    StreamReader csvread = new StreamReader(file_name);
                    string first_str = csvread.ReadLine();
                    if (first_str == "USER_ID;TRACK_ID;NAME;ARTIST;Acousticness;Danceability;Duration_Ms;Energy;Instrumentalness;Key;Liveness;Loudness;Mode;Speechiness;Tempo;Time_Signature;Valence;Like")
                    {
                        flag = true;
                    }
                    csvread.Close();


                    StreamWriter csv = new StreamWriter(file_name, true, Encoding.UTF8); //создаем или открываем файл

                    if (flag == false)
                    {
                        csv.WriteLine("USER_ID" + ';' + "TRACK_ID" + ';' + "NAME" + ';' + "ARTIST" + ';' + "Acousticness"
                                              + ';' + "Danceability" + ';' + "Duration_Ms"
                                                + ';' + "Energy" + ';' + "Instrumentalness"
                                                    + ';' + "Key" + ';' + "Liveness"
                                                        + ';' + "Loudness" + ';' + "Mode"
                                                           + ';' + "Speechiness" + ';' + "Tempo"
                                                               + ';' + "Time_Signature" + ';' + "Valence" + ';' + "Like", Encoding.UTF8);
                    }



                    string user_id;
                    if (label44.Visible == true) //проверка, что пользователь авторизован
                    {
                        user_id = spotify_api.GetUserProfile().Result.id;
                    }
                    else { user_id = "No_Autorization"; }


                    int cnt = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        if (checkBoxes[i].Checked == true) //если чекбокс выбран, то записываем трек в файл
                        {
                            var track = spotify_api.GetTrack(track_id_page1[i]).Result;
                            var track_info = spotify_api.GetAudioFeatures(track_id_page1[i]).Result;
                            csv.WriteLine(user_id + ';' + track_id_page1[i] + ';' + track.name + ';' + track.artists[0].name + ';' + track_info.Acousticness
                                               + ';' + track_info.Danceability + ';' + track_info.Duration_Ms
                                                 + ';' + track_info.Energy + ';' + track_info.Instrumentalness
                                                     + ';' + track_info.Key + ';' + track_info.Liveness
                                                         + ';' + track_info.Loudness + ';' + track_info.Mode
                                                            + ';' + track_info.Speechiness + ';' + track_info.Tempo
                                                                + ';' + track_info.Time_Signature + ';' + track_info.Valence + ';' + 1, Encoding.UTF8);
                            cnt++;
                        }
                    }
                    for (int i = 0; i < 10; i++) checkBoxes[i].Checked = false; //устанавливаем все чекбоксы false
                    MessageBox.Show(String.Format("Количестов записанных треков - {0}.", cnt));

                    csv.Close();//закрываем поток
                }
                else MessageBox.Show("Файл для записи не выбран");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }
        }


        //Вторая вкладка_______________________________________________________________________________________
        private void button4_Click(object sender, EventArgs e) //кнопка поиска плейлиста (вторая вкладка)
        {
            player.controls.stop();
            pictureBox13.Visible = false;
            label5.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            checkedListBox1.Visible = false;
            checkedListBox1.Items.Clear();
            button5.Enabled = false;
            button6.Enabled = false;
            button10.Enabled = false;
            button5.Visible = false;
            button6.Visible = false;
            button10.Visible = false;
            label33.Visible = false;
            textBox5.Visible = false;

            try
            {
                pictureBox11.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp"; //дефолтная обложка плейлиста

                if (textBox3.Text.Length != 0)
                {
                    var playlist_info = spotify_api.GetPlaylist(textBox3.Text).Result; //получаем плейлист
                    if (playlist_info.id != null)
                    {
                        if (playlist_info.name != null) //название плейлиста
                        {
                            label5.Visible = true;
                            label5.Text = playlist_info.name;
                        }
                        if (playlist_info.images.Count != 0) //обложка плейлиста
                        {
                            pictureBox11.ImageLocation = playlist_info.images[0].url;
                        }
                    }
                    else
                    {
                        textBox3.Text = null;
                        MessageBox.Show("Плейлиста с таким id не существует!");
                    }


                    var playlist = spotify_api.GetPlaylistItem(textBox3.Text, 0).Result; //получаем список треков в плейлисте
                    int total = playlist.total; //общее кол-во треков
                    int offset = 0; //смещение для обработанных треков
                    do
                    {
                        playlist = spotify_api.GetPlaylistItem(textBox3.Text, offset).Result; //получаем список треков в плейлисте со смещением
                        List_of_Playlist(checkedListBox1, playlist, track_id_page2, offset); //выводим список полученных треков (за раз выводится максимум 100 треков)
                        offset = offset + 100; //увеличиваем смещение

                    } while (total > offset);
                    button5.Enabled = true;
                    button6.Enabled = true;
                    button10.Enabled = true;
                    button5.Visible = true;
                    button6.Visible = true;
                    button10.Visible = true;
                }
                else { MessageBox.Show("Плейлиста с таким id не существует!"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e) //действие происходящее при выборе трека в чеклисте (вторая вкладка)
        {
            var num = checkedListBox1.SelectedIndex; //получаем номер выбранного трека
            var track = spotify_api.GetTrack(track_id_page2[num]).Result; //получаем выбранный трек, через массив id
            player.controls.stop();
            //  if (checkedListBox1.)
            //   { 
            try
            {
                View_Track(label10, label11, label12, track, pictureBox13); //показываем трек с обложкой

                View_Info(label33, textBox5, track);// показываем информацию о треке

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }
            //   }
        }

        private void button10_Click(object sender, EventArgs e) //кнопка отменить выделение (вторая вкладка)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false); //устанавливаем весь чеклис в false
            }
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)//кнопка выбрать все (вторая вкладка)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);//устанавливаем весь чеклис в true
            }

        }

        private void button6_Click(object sender, EventArgs e) //запись в файл (вторая вкладка)
        {
            num_page = 2;
            Write_to_File(checkedListBox1, track_id_page2); //записываем выбранные треки в файл

        }

        //третья вкладка_______________________________________________________________________________________

        private void button7_Click(object sender, EventArgs e) //кнопка получить плейлисты (третья вкладка)
        {
            button8.Visible = false;
            button9.Visible = false;
            button11.Visible = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button11.Enabled = false;
            player.controls.stop();
            label9.Visible = false;
            checkedListBox2.Visible = false;
            checkedListBox2.Items.Clear();
            label36.Visible = false;
            label37.Visible = false;
            label38.Visible = false;
            label35.Visible = false;
            textBox7.Visible = false;
            pictureBox14.Visible = false;

            try
            {
                pictureBox12.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp"; //дефолтная обложка плейлиста
                listBox1.Items.Clear();
                var list_playlists = spotify_api.GetPlayListsUser(textBox4.Text).Result; //получаем список плейлистов пользователя

                if (list_playlists.href != null)
                {
                    if (list_playlists.items.Count != 0)
                    {

                        playlist_id.Clear();

                        for (int i = 0; i < list_playlists.items.Count; i++) //выводим список плейлистов и записываем их id в массив
                        {
                            listBox1.Items.Add(list_playlists.items[i].name);
                            playlist_id.Add(list_playlists.items[i].id);
                        }
                    }
                    else { MessageBox.Show("На этом аккаунте нет сохраненных плейлистов!"); textBox4.Text = null; }
                }
                else { MessageBox.Show("Id пользователя имело неверный формат!"); textBox4.Text = null; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //действие происходящее при нажати на один из плейлистов ()третья вкладка
        {
            button8.Visible = false;
            button9.Visible = false;
            button11.Visible = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button11.Enabled = false;
            player.controls.stop();
            label9.Visible = false;
            checkedListBox2.Visible = false;
            checkedListBox2.Items.Clear();
            label36.Visible = false;
            label37.Visible = false;
            label38.Visible = false;
            label35.Visible = false;
            textBox7.Visible = false;
            pictureBox14.Visible = false;

            try
            {
                int num = listBox1.SelectedIndex;
                var playlist_info = spotify_api.GetPlaylist(playlist_id[num]).Result; //получаем плейлист
                if (playlist_info.tracks.items.Count != 0)
                {
                    pictureBox12.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp"; //дефолтная обложка

                    if (playlist_info.name != null) //выводим название плейлиста под картинкой
                    {
                        label9.Visible = true;
                        label9.Text = playlist_info.name;
                    }
                    if (playlist_info.images.Count != 0) //выводим обложку плейлиста
                    {
                        pictureBox12.ImageLocation = playlist_info.images[0].url;
                    }

                }
                else { MessageBox.Show("Этот плейлист пуст!"); }

                var playlist = spotify_api.GetPlaylistItem(playlist_id[num], 0).Result; //получаем список плейлиста
                int total = playlist.total; //общее кол-во треков в плейлисте
                int offset = 0;//смещение
                do
                {
                    playlist = spotify_api.GetPlaylistItem(playlist_id[num], offset).Result; //получаем список плейлистов со смещением

                    List_of_Playlist(checkedListBox2, playlist, track_id_page3, offset); //выводим список треков (максимум 100 за один раз)
                    offset = offset + 100; //увеличиваем смещение на 100


                } while (total > offset);
                button8.Visible = true;
                button9.Visible = true;
                button11.Visible = true;
                button8.Enabled = true;
                button9.Enabled = true;
                button11.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e) //кнопка выбрать все (третья вкладка)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)//устанавливаем весь чеклист в true
            {
                checkedListBox2.SetItemChecked(i, true);
            }
        }

        private void button11_Click(object sender, EventArgs e)//кнопка отменить выделение (третья вкладка)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)//устанавливаем весь чеклист в false
            {
                checkedListBox2.SetItemChecked(i, false);
            }
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e) //выбор одного из треков из списка (третья вкладка)
        {
            var num = checkedListBox2.SelectedIndex;
            var track = spotify_api.GetTrack(track_id_page3[num]).Result;
            player.controls.stop();
            try
            {
                View_Track(label38, label36, label37, track, pictureBox14); //выводим обложку название и плеер

                View_Info(label35, textBox7, track); //выводим информацию  о треке
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }


        private void button8_Click(object sender, EventArgs e)//запись в файл (третья вкладка)
        {
            num_page = 3;
            Write_to_File(checkedListBox2, track_id_page3);//записываем выбранные треки в файл

        }



        //четвертая вкладка_______________________________________________________________________________________
        private void button13_Click(object sender, EventArgs e)
        {
            spotify_api.Autorization();
            var user = spotify_api.GetUserProfile().Result;
            if (user != null)
            {
                label44.Visible = true;
                pictureBox17.Visible = true;
                button18.Enabled = true;
                button18.Visible = true;

                label44.Text = user.display_name;
                if (user.images.Length == 0)
                {
                    pictureBox17.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/img_474642.png";
                }
                else
                {
                    pictureBox17.ImageLocation = user.images[0].url;
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {

            button15.Visible = false;
            button16.Visible = false;
            button17.Visible = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;
            player.controls.stop();
            pictureBox16.Visible = false;
            label5.Visible = false;
            label41.Visible = false;
            label42.Visible = false;
            label43.Visible = false;
            checkedListBox3.Visible = false;
            checkedListBox3.Items.Clear();
            label40.Visible = false;
            textBox8.Visible = false;

            try
            {
                pictureBox15.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp"; //дефолтная обложка плейлиста
                pictureBox18.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp";
                pictureBox19.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp";
                pictureBox20.ImageLocation = "file:///C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/i.webp";

                var list_of_saved_track = spotify_api.GetSavedTracks(0).Result; //получаем список сохраненных треков
                if (list_of_saved_track != null)
                {
                    if (list_of_saved_track.total > 3)
                    {
                        var rand = new Random();
                        pictureBox15.ImageLocation = list_of_saved_track.items[rand.Next(0, list_of_saved_track.items.Length)].track.album.images[0].url;
                        pictureBox18.ImageLocation = list_of_saved_track.items[rand.Next(0, list_of_saved_track.items.Length)].track.album.images[0].url;
                        pictureBox19.ImageLocation = list_of_saved_track.items[rand.Next(0, list_of_saved_track.items.Length)].track.album.images[0].url;
                        pictureBox20.ImageLocation = list_of_saved_track.items[rand.Next(0, list_of_saved_track.items.Length)].track.album.images[0].url;
                    }
                    List_of_Playlist(checkedListBox3, list_of_saved_track, track_id_page4, 0); //выводим (первые 50) список полученных треков (за раз выводится максимум 50 треков)
                    int total = list_of_saved_track.total; //общее кол-во треков
                    int offset = 50; //смещение для обработанных треков
                    if (offset < total)
                    {
                        do
                        {
                            list_of_saved_track = spotify_api.GetSavedTracks(offset).Result; //получаем список сохраненных треков со смещением
                            List_of_Playlist(checkedListBox3, list_of_saved_track, track_id_page4, offset); //выводим список полученных треков (за раз выводится максимум 50 треков)
                            offset = offset + 50; //увеличиваем смещение

                        } while (total > offset);
                    }
                    button14.Enabled = true;
                    button14.Visible = true;
                    button15.Visible = true;
                    button16.Visible = true;
                    button17.Visible = true;
                    button15.Enabled = true;
                    button16.Enabled = true;
                    button17.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var num = checkedListBox3.SelectedIndex;
            var track = spotify_api.GetTrack(track_id_page4[num]).Result;
            player.controls.stop();
            try
            {
                View_Track(label43, label42, label41, track, pictureBox16); //выводим обложку название и плеер

                View_Info(label40, textBox8, track); //выводим информацию  о треке
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex);
            }

        }

        private void button17_Click(object sender, EventArgs e)//кнопка выбрать все (четвертая вкладка)
        {
            for (int i = 0; i < checkedListBox3.Items.Count; i++)//устанавливаем весь чеклист в true
            {
                checkedListBox3.SetItemChecked(i, true);
            }
        }

        private void button15_Click(object sender, EventArgs e) //кнопка отменить выделение (четвертая вкладка)
        {
            for (int i = 0; i < checkedListBox3.Items.Count; i++)
            {
                checkedListBox3.SetItemChecked(i, false); //устанавливаем весь чеклис в false
            }
        }

        private void button16_Click(object sender, EventArgs e) //запись в файл (четвертая вкладка)
        {
            num_page = 4;
            string user_id = spotify_api.GetUserProfile().Result.id;
            Write_to_File(checkedListBox3, track_id_page4, user_id);//записываем выбранные треки в файл
        }

        private void button18_Click(object sender, EventArgs e) //кнопка для выхода из аккаунта
        {
            spotify_api.DeAutorization();
            player.controls.stop();
            label44.Visible = false;
            pictureBox17.Visible = false;
            button18.Enabled = false;
            button18.Visible = false;
            button14_Click(sender, e);
        }



        //____________________________________________Блок с рекомендациями_______________________________________________
        private void button19_Click(object sender, EventArgs e)// кнопка "Предсказать"
        {


            //Load sample data
            var sampleData = new MLModel1.ModelInput();
            sampleData = TrackData;
            //Load model and predict output
            var result = MLModel1.Predict(sampleData);

            if (result.Score[0] > 0.5)
            {
                MessageBox.Show(String.Format("Этот трек возможно вам понравится. {0}", result.Score[0]));
            }
            else if (result.Score[0] <= 0.5)
            {
                MessageBox.Show(String.Format("Этот трек не в вашем вкусе. {0}", result.Score[0]));
            }

        }

        private void button20_Click(object sender, EventArgs e) //кнопка "Обучить"
        {


            // Create MLContext
            MLContext mlContext = new MLContext();
            progressBar1.Value = 10;

            if (checkBox11.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Hip-Hop", true); //добавляем жанр как любимый
            }
            else if (checkBox11.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Hip-Hop", false);  //добавляем жанр как нелюбимый
            }

            progressBar1.Value = 20;
            if (checkBox12.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Pop", true); //добавляем жанр как любимый
            }
            else if (checkBox12.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Pop", false);  //добавляем жанр как нелюбимый
            }

            if (checkBox13.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Rock", true); //добавляем жанр как любимый
            }
            else if (checkBox13.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Rock", false);  //добавляем жанр как нелюбимый
            }
            progressBar1.Value = 30;
            if (checkBox14.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Indie", true); //добавляем жанр как любимый
            }
            else if (checkBox14.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Indie", false);  //добавляем жанр как нелюбимый
            }

            if (checkBox15.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Bluz", true); //добавляем жанр как любимый
            }
            else if (checkBox15.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Bluz", false);  //добавляем жанр как нелюбимый
            }
            progressBar1.Value = 40;
            if (checkBox16.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Classical", true); //добавляем жанр как любимый
            }
            else if (checkBox16.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Classical", false);  //добавляем жанр как нелюбимый
            }
            progressBar1.Value = 50;
            if (checkBox17.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Djaz", true); //добавляем жанр как любимый
            }
            else if (checkBox17.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Djaz", false);  //добавляем жанр как нелюбимый
            }

            if (checkBox18.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("K-Pop", true); //добавляем жанр как любимый
            }
            else if (checkBox18.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("K-Pop", false);  //добавляем жанр как нелюбимый
            }

            if (checkBox19.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Soul", true); //добавляем жанр как любимый
            }
            else if (checkBox19.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Soul", false);  //добавляем жанр как нелюбимый
            }

            if (checkBox20.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Russian_Rock", true); //добавляем жанр как любимый
            }
            else if (checkBox20.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Russian_Rock", false);  //добавляем жанр как нелюбимый
            }
            progressBar1.Value = 60;
            if (checkBox21.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Russian_Rap", true); //добавляем жанр как любимый
            }
            else if (checkBox21.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Russian_Rap", false);  //добавляем жанр как нелюбимый
            }

            if (checkBox22.CheckState.ToString() == "Checked")
            {
                Check_Recomendation("Punk", true); //добавляем жанр как любимый
            }
            else if (checkBox22.CheckState.ToString() == "Indeterminate")
            {
                Check_Recomendation("Punk", false);  //добавляем жанр как нелюбимый
            }
            progressBar1.Value = 70;

            var data = mlContext.Data.LoadFromTextFile<MLModel1.ModelInput>(file_name, hasHeader: true,
                separatorChar: ';', allowQuoting: true, trimWhitespace: true, allowSparse: true);
            //Trained Model
            ITransformer trainedModel = MLModel1.RetrainPipeline(mlContext, data);
            progressBar1.Value = 100;
            // Save Trained Model
            mlContext.Model.Save(trainedModel, data.Schema, "MLModel1.zip");
            MLModel1.LoadRetrainModel();
            progressBar1.Value = 0;


        }

        private void button21_Click(object sender, EventArgs e) //кнопка "Сформировать"
        {
            //Create MLContext
            MLContext mlContext = new MLContext();

            // Load Trained Model
            ITransformer predictionPipeline = mlContext.Model.Load("MLModel1.zip", out var _);

            var resourceName = "C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/TrackList.csv";
            // Load data from a CSV file that contains a header row  
            var data = mlContext.Data.LoadFromTextFile<MLModel1.ModelInput>(resourceName, hasHeader: true,
                         separatorChar: ';', allowQuoting: true, trimWhitespace: true, allowSparse: true);

            // Predicted Data
            IDataView predictions = predictionPipeline.Transform(data);

            float[][] scoreColumn = predictions.GetColumn<float[]>("Score").ToArray(); //хранит вероятность того, что трек понравится
            string[] name = predictions.GetColumn<string>("NAME").ToArray();
            string[] trackid = predictions.GetColumn<string>("TRACK_ID").ToArray();


            progressBar2.Value = 20;
            track_id_encountered.Clear();

            if (checkBox23.Checked == true) //только незнакомые треки
            {
                //Запоминаем id треков, которые уже знакомы пользователю

                StreamReader csvread = new StreamReader(file_name);
                string str = csvread.ReadLine(); //первая строка заголовчная, пропускаем
                string[] str_split;
                str = csvread.ReadLine();

                while (!csvread.EndOfStream)
                {
                    str_split = str.Split(';');   //разбиваем строку на подстроки разделенные ";"  
                    track_id_encountered.Add(str_split[1]); //получаем track_id из считанной строки
                    str = csvread.ReadLine();  //читаем следующую строку
                }
                csvread.Close();
            }

            //определим примерно максимальное и минимальное значение scoreColumn
            float max = 0, min = scoreColumn[0][0];
            for (int j = 0; j < 100; j++)
            {
                if (max < scoreColumn[j][0])
                {
                    max = scoreColumn[j][0];
                }
                if (min > scoreColumn[j][0])
                {
                    min = scoreColumn[j][0];
                }
            }

            progressBar2.Value = 40;

            track_id_recomendation.Clear();
            bool flag = true;
            Random rand = new Random();
            int ch = 0;                       //счётчик рекомендованных треков
            int i = 0;
            StreamWriter csv = new StreamWriter("D:/Dekstop/Prediction.csv", false, Encoding.UTF8); //создаем файл с рекомендациями
            while (ch < 50)
            {
                i = rand.Next(0, scoreColumn.Length);
                csv.WriteLine(scoreColumn[i][0].ToString() + ";" + name[i] + ";" + trackid[i]);

                for (int j = 0; j < track_id_encountered.Count; j++)        //Ищем track_id в уже знакомых пользователю треках
                {
                    if (String.Equals(track_id_encountered[j], trackid[i]))
                    {
                        flag = false;       //если нашли, то трек не запишем в список подходящих пользователю
                        break;
                    }
                }

                if (scoreColumn[i][0] >= max-0.1 && flag)           //если трек не знаком и велика вероятность, того что он понравится, то добавляем
                {
                    ch++;
                    track_id_recomendation.Add(trackid[i]); //записываем id трека подходящего пользователю
                    track_id_encountered.Add(trackid[i]);   //записываем этот id в список знакомых пользователю треков
                }
                flag = true;
            }
            csv.Close();//закрываем поток
            progressBar2.Value = 80;

            string user_id = spotify_api.GetUserProfile().Result.id;
            var create_playlist = spotify_api.CreateNewPlaylist(user_id, "Рекомендации для тебя").Result;        //Создаем плейлист

            progressBar2.Value = 100;

            string playlist_id = create_playlist.id;
            var add_to_playlist = spotify_api.AddTracksToPlaylist(playlist_id, track_id_recomendation).Result;

            progressBar2.Value = 0;

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void axWindowsMediaPlayer14_Enter(object sender, EventArgs e)
        {

        }


        private void Check_Recomendation(string name_of_file, bool like) //добаление любимых и нелюбимых треков по отметкам в чекбоксе в блоке рекомендаций (name_of_file- название жанра, без расширения)
        {
            if (file_name != null)
            {
                try
                {
                    StreamWriter csv = new StreamWriter(file_name, true, Encoding.UTF8); //создаем или открываем файл
                    csv.Close();//закрываем поток

                    //проверяем пуст ли файл или в нем уже есть записи
                    bool flag = false;
                    StreamReader csvread = new StreamReader(file_name);
                    string first_str = csvread.ReadLine();
                    if (first_str == "USER_ID;TRACK_ID;NAME;ARTIST;Acousticness;Danceability;Duration_Ms;Energy;Instrumentalness;Key;Liveness;Loudness;Mode;Speechiness;Tempo;Time_Signature;Valence;Like")
                    {
                        flag = true;
                    }
                    csvread.Close();

                    csv = new StreamWriter(file_name, true, Encoding.UTF8); ;


                    if (flag == false) //если файл был пуст то записываем в первую строку заголовки столбцов
                    {
                        csv.WriteLine("USER_ID" + ';' + "TRACK_ID" + ';' + "NAME" + ';' + "ARTIST" + ';' + "Acousticness"
                                              + ';' + "Danceability" + ';' + "Duration_Ms"
                                                + ';' + "Energy" + ';' + "Instrumentalness"
                                                    + ';' + "Key" + ';' + "Liveness"
                                                        + ';' + "Loudness" + ';' + "Mode"
                                                           + ';' + "Speechiness" + ';' + "Tempo"
                                                               + ';' + "Time_Signature" + ';' + "Valence" + ';' + "Like", Encoding.UTF8);
                    }


                    var user = spotify_api.GetUserProfile().Result;
                    var resourceName = String.Format("C:/Users/Salut/source/Repos/WinFormsApp1/WinFormsApp1/Resources/{0}.csv", name_of_file);
                    csvread = new StreamReader(resourceName); //открываем файл с предзаписанными треками

                    string str = csvread.ReadLine(); //первую строку пропускаем

                    string[] mass;
                    str = csvread.ReadLine(); //читаем вторую строку
                    while (str != null)
                    {
                        mass = str.Split(';'); //получаем массив параметров трека
                        if (like == true) //еесли жанр отмечен как любимый, то пишем в Like 1, иначе 0
                        {
                            csv.WriteLine(user.id + ';' + mass[1] + ';' + mass[2] + ';' + mass[3] + ';' + mass[4]
                                           + ';' + mass[5] + ';' + mass[6]
                                             + ';' + mass[7] + ';' + mass[8]
                                                 + ';' + mass[9] + ';' + mass[10]
                                                     + ';' + mass[11] + ';' + mass[12]
                                                        + ';' + mass[13] + ';' + mass[14]
                                                            + ';' + mass[15] + ';' + mass[16] + ';' + 1, Encoding.UTF8);
                        }
                        else csv.WriteLine(user.id + ';' + mass[1] + ';' + mass[2] + ';' + mass[3] + ';' + mass[4]
                                           + ';' + mass[5] + ';' + mass[6]
                                             + ';' + mass[7] + ';' + mass[8]
                                                 + ';' + mass[9] + ';' + mass[10]
                                                     + ';' + mass[11] + ';' + mass[12]
                                                        + ';' + mass[13] + ';' + mass[14]
                                                            + ';' + mass[15] + ';' + mass[16] + ';' + 0, Encoding.UTF8);


                        str = csvread.ReadLine();
                    }

                    csvread.Close();
                    csv.Close();//закрываем потоки
                }
                catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex); }
            }
            else MessageBox.Show("Файл для записи не выбран");
        }



        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Create_Plot(string spec, int a) //построение графика для одного пользователя
        {
            pwDance.Visible = true;
            pwDance.Model = new PlotModel { Title = spec + " твоих любимых треков" };

            StreamReader csvread = new StreamReader(file_name);
            string str = csvread.ReadLine(); //первая строка заголовчная, пропускаем
            string[] str_split;
            str = csvread.ReadLine();
            mass_of_specification.Clear();
            while (!csvread.EndOfStream)
            {
                str_split = str.Split(';');   //разбиваем строку на подстроки разделенные ";"  
                mass_of_specification.Add(Convert.ToDouble(str_split[a])); //получаем нужное свойство из считанной строки
                str = csvread.ReadLine();  //читаем следующую строку
            }
            csvread.Close();


            LineSeries[] series = new LineSeries[mass_of_specification.Count];
            for (var i = 0; i < mass_of_specification.Count; i++)
            {
                series[i] = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 2, Color = OxyColors.Blue };
                series[i].Points.Add(new DataPoint(i, mass_of_specification[i]));

                // Add the series to the plot model
                pwDance.Model.Series.Add(series[i]);
            }

        }

        private void Create_Plot(string spec, int a, string id) //построение графика для двух пользователей (сравнение)
        {

            progressBar3.Visible = true;
            var list_playlists = spotify_api.GetPlayListsUser(id).Result; //получаем список плейлистов пользователя
            //playlist_id.Clear();
            mass_of_specification_other_user.Clear(); //массив для хранения характеристик треков другогго пользователя
            Root_List_of_PlayList playlist_info;
            List<string> tracks_id = new List<string>();
            List<Root_Features_Several> tracks_info;

            for (int i = 0; i < list_playlists.items.Count; i++) //записываем список id плейлистов в массив
            {
                progressBar3.Value = i * 2;
                playlist_info = spotify_api.GetPlaylist(list_playlists.items[i].id).Result; //получаем плейлист

                tracks_id.Clear();
                for (int k = 0; k < playlist_info.tracks.items.Count; k++)
                {
                    tracks_id.Add(playlist_info.tracks.items[k].track.id); //получаем id всех треков в плейлисте
                }

                tracks_info = spotify_api.GetSeveralAudioFeatures(tracks_id).Result;



                for (int j = 0; j < playlist_info.tracks.items.Count; j++)
                {

                    switch (a)
                    {
                        case 4: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Acousticness); break;
                        case 5: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Danceability); break;
                        case 6: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Duration_Ms); break;
                        case 7: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Energy); break;
                        case 8: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Instrumentalness); break;
                        case 9: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Key); break;
                        case 10: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Liveness); break;
                        case 11: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Loudness); break;
                        case 12: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Mode); break;
                        case 13: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Speechiness); break;
                        case 14: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Tempo); break;
                        case 15: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Time_Signature); break;
                        case 16: mass_of_specification_other_user.Add(tracks_info[0].Audio_Features[j].Valence); break;

                    }
                }
            }


            progressBar3.Value = 60;
            StreamReader csvread = new StreamReader(file_name);
            string str = csvread.ReadLine(); //первая строка заголовчная, пропускаем
            string[] str_split;
            str = csvread.ReadLine();
            mass_of_specification.Clear(); //массив для хранения характеристик треков основного пользователя
            while (!csvread.EndOfStream)
            {
                str_split = str.Split(';');   //разбиваем строку на подстроки разделенные ";"  
                mass_of_specification.Add(Convert.ToDouble(str_split[a])); //получаем уровень танцевальности из считанной строки
                str = csvread.ReadLine();  //читаем следующую строку
            }
            csvread.Close();

            pwDance.Visible = true;
            pwDance.Model = new PlotModel { Title = spec };

            LineSeries[] series = new LineSeries[mass_of_specification.Count];
            LineSeries[] series_other_user = new LineSeries[mass_of_specification_other_user.Count];

            //mass_of_specification.Sort();

            int cnt;
            if (mass_of_specification.Count < mass_of_specification_other_user.Count)
            {
                cnt = mass_of_specification.Count;
            }
            else
            {
                cnt = mass_of_specification_other_user.Count;
            }

            for (var i = 0; i < cnt; i++)
            {
                series[i] = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 2, Color = OxyColors.Blue };
                series[i].Points.Add(new DataPoint(i, mass_of_specification[i]));

                // Add the series to the plot model
                pwDance.Model.Series.Add(series[i]);

                series_other_user[i] = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 2, Color = OxyColors.Red };
                series_other_user[i].Points.Add(new DataPoint(i, mass_of_specification_other_user[i]));

                // Add the series to the plot model
                pwDance.Model.Series.Add(series_other_user[i]);
            }
            progressBar3.Value = 100;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (file_name != null)
            {
                if (checkBox24.Checked == true) //если выбран чекбокс, то будем сравнивать данные двух пользователей
                {
                    if (radioButton1.Checked == true) Create_Plot("Акустичность", 4, textBox9.Text);
                    if (radioButton2.Checked == true) Create_Plot("Танцевальность", 5, textBox9.Text);
                    if (radioButton3.Checked == true) Create_Plot("Длительность", 6, textBox9.Text);
                    if (radioButton4.Checked == true) Create_Plot("Энергичность", 7, textBox9.Text);
                    if (radioButton5.Checked == true) Create_Plot("Инструментальность", 8, textBox9.Text);
                    if (radioButton6.Checked == true) Create_Plot("Регистр", 9, textBox9.Text);
                    if (radioButton7.Checked == true) Create_Plot("Живой звук", 10, textBox9.Text);
                    if (radioButton8.Checked == true) Create_Plot("Громкость", 11, textBox9.Text);
                    if (radioButton9.Checked == true) Create_Plot("Лад", 12, textBox9.Text);
                    if (radioButton10.Checked == true) Create_Plot("Наличие вокала", 13, textBox9.Text);
                    if (radioButton11.Checked == true) Create_Plot("Темп", 14, textBox9.Text);
                    if (radioButton12.Checked == true) Create_Plot("Размерность", 15, textBox9.Text);
                    if (radioButton13.Checked == true) Create_Plot("Настроение", 16, textBox9.Text);
                    progressBar3.Value = 0;
                    progressBar3.Visible = false;
                }
                else //иначе выводим анализ одного файла
                {

                    if (radioButton1.Checked == true) Create_Plot("Акустичность", 4);
                    if (radioButton2.Checked == true) Create_Plot("Танцевальность", 5);
                    if (radioButton3.Checked == true) Create_Plot("Длительность", 6);
                    if (radioButton4.Checked == true) Create_Plot("Энергичность", 7);
                    if (radioButton5.Checked == true) Create_Plot("Инструментальность", 8);
                    if (radioButton6.Checked == true) Create_Plot("Регистр", 9);
                    if (radioButton7.Checked == true) Create_Plot("Живой звук", 10);
                    if (radioButton8.Checked == true) Create_Plot("Громкость", 11);
                    if (radioButton9.Checked == true) Create_Plot("Лад", 12);
                    if (radioButton10.Checked == true) Create_Plot("Наличие вокала", 13);
                    if (radioButton11.Checked == true) Create_Plot("Темп", 14);
                    if (radioButton12.Checked == true) Create_Plot("Размерность", 15);
                    if (radioButton13.Checked == true) Create_Plot("Настроение", 16);
                }
            }
            else MessageBox.Show("Необходимо выбрать файл для сохранения");
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e) //выбран чекбокс "сравнить с другим пользователем"
        {
            if (checkBox24.Checked == true)
            {
                label46.Visible = true;
                textBox9.Visible = true;
            }
            else
            {
                label46.Visible = false;
                textBox9.Visible = false;
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            pwDance.Model = new PlotModel { Title = "Математическое ожидание" };
            double expected_value = 0;
            LineSeries series = new LineSeries();
            for (var i = 0; i < mass_of_specification.Count; i++)
            {
                expected_value += Convert.ToDouble(mass_of_specification[i]);
            }
            expected_value = expected_value / mass_of_specification.Count;

            series = new LineSeries{ MarkerType = MarkerType.Circle, MarkerSize = 2, Color = OxyColors.DarkGreen };
            series.Points.Add(new DataPoint(0, expected_value));
            series.Points.Add(new DataPoint(20, expected_value));

            // Add the series to the plot model
            pwDance.Model.Series.Add(series);
        }
    }
}
