using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Windows.Automation;

namespace SpotifyProgramForms
{
    public class SpotifyToken
    {
        public string Access_token { get; set; } //Токен доступа
        public string Token_type { get; set; } //Тип доступа
        public int Expires_in { get; set; } //Время действия токена
        public string Scope { get; set; } //Разделенный пробелами список областей, которые были предоставлены для этого access_token
        public string Refresh_token { get; set; } //Токен, который может быть отправлен в службу Учетных записей Spotify вместо кода авторизации.


        private static readonly HttpClient client = new HttpClient(); //Создание HTTP клиента

        private static async Task<SpotifyToken> AccessTokenAsync(SpotifyToken token_info) //Получение токена доступа (для неавторизованного пользователя)
        {

            string client_id;
            client_id = ConfigurationManager.AppSettings.Get("key0"); //Запись Client ID из конфигурационного файла
            string client_secret;
            client_secret = ConfigurationManager.AppSettings.Get("key1"); //Запись Сlient Ыecret из конфигурационного файла

            var encode_clientid_clientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",
                                                                      client_id, client_secret))); //формируем строку для аудентификационного заголовка

            HttpRequestMessage request = new HttpRequestMessage();                      //Настройка параметров запроса
            request.RequestUri = new Uri("https://accounts.spotify.com/api/token");
            request.Method = HttpMethod.Post;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encode_clientid_clientsecret); //Формирование заголовков запроса
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            string json = "grant_type=client_credentials";
            request.Content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded"); //Добавление контента в запрос

            var stringTask = client.SendAsync(request).Result; //Отправка запроса на сервер

            HttpContent res_json = stringTask.Content;  //Ответ от сервера в формате JSON
            var text = res_json.ReadAsStringAsync().Result;
            token_info = await res_json.ReadFromJsonAsync<SpotifyToken>(); //Десериализация JSON ответа
            return token_info;

        }


        public async Task<SpotifyToken> AuthorizenAsync(SpotifyToken token_info) //Получение токена доступа (для авторизованного пользователя)
        {
            try
            {
                string client_id;
                client_id = ConfigurationManager.AppSettings.Get("key0");

                string url = string.Format("https://accounts.spotify.com/authorize?client_id={0}&response_type=code&redirect_uri=https%3A%2F%2Fyandex.ru%2F&scope=user-read-private%20user-read-email%20user-library-read%20user-top-read%20playlist-modify-private&state=34fFs29kd09&show_dialog=true", client_id);
                System.Diagnostics.Process.Start("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe", url);
                MessageBox.Show("Авторизируйтесь в открывшемся окне! После нажмите ОК.");

                   var root =  AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_WidgetWin_1")); //достаем содержимое адреной строки из браузера
                   var textP = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                   var req = textP.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

               // var root = 0; //достаем содержимое адреной строки из браузера
             // var textP =0;
               // var req = "";
                bool flag = false;
                var mass = Convert.ToString(req).Split('&', '=', '?');
                for (int i = 0; i < mass.Length; i++) // достаем из переданной ссылки код авторизации
                {
                    if (mass[i] == "error")
                    {
                        MessageBox.Show("Вы не авторизовались! Некоторые функции недоступны.");
                        return token_info;
                    }
                    if (mass[i] == "code")
                    {
                        req = mass[i + 1]; //после "code=" считываем код авторизации
                        flag = true;
                        i = mass.Length;
                    }
                }

                if (flag)
                {

                    string client_secret;
                    client_secret = ConfigurationManager.AppSettings.Get("key1"); //Запись Сlient Secret из конфигурационного файла

                    var encode_clientid_clientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",
                                                                              client_id, client_secret))); //формируем строку для аудентификационного заголовка

                    HttpRequestMessage request = new HttpRequestMessage();                      //Настройка параметров запроса
                    request.RequestUri = new Uri("https://accounts.spotify.com/api/token");
                    request.Method = HttpMethod.Post;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encode_clientid_clientsecret); //Формирование заголовков запроса
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    string json = string.Format("grant_type=authorization_code&code={0}&redirect_uri=https%3A%2F%2Fyandex.ru%2F", req);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded"); //Добавление контента в запрос

                    var stringTask = client.SendAsync(request).Result; //Отправка запроса на сервер

                    HttpContent res_json = stringTask.Content;  //Ответ от сервера в формате JSON
                    var text = res_json.ReadAsStringAsync().Result;
                    token_info = await res_json.ReadFromJsonAsync<SpotifyToken>(); //Десериализация JSON ответа
                    if (token_info.Access_token == null)
                    {
                        MessageBox.Show("Неудачная попытка авторизации. Повторите!");
                        return AccessTokenAsync(this).Result;
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка! Не закрывайте окно браузера, прежде чем нажать ОК.");
                    return AccessTokenAsync(this).Result;
                }
                MessageBox.Show("Авторизация прошла успешно!");
                return token_info;
            }
            catch (WebException ex1)
            {
                MessageBox.Show("Autorization Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Autorization Error: " + ex2.Message);
            }

            return AccessTokenAsync(this).Result;
        }



        public string GetAccessToken() //Метоод возвращающий токен доступа
        {
            string token = AccessTokenAsync(this).Result.Access_token;
            return token;
        }

        public HttpClient GetHttpClient() //Возвращает текущий HTTP клиент
        {
            return client;
        }

    }

    public class SpotifyTrack
    {

        private readonly SpotifyToken connect_to_spotify = new SpotifyToken(); //Создание экземпляра класса SpotifyToken
        private string token; //токен доступа
        private string scope; //доступные данные для токена доступа
        private HttpContent res_json; //контент отправляемый на сервер и получаемый от сервера
        private HttpClient client; // HTTP клиент

        public SpotifyTrack() //Конструктор класса, получаем токен доступа (действителен в течении часа)
        { 
            token = connect_to_spotify.GetAccessToken();
            scope = null;
        } 


        public string Autorization() //Авторизация пользователя
        {
            var res = this.connect_to_spotify.AuthorizenAsync(this.connect_to_spotify).Result;
            token = res.Access_token;
            scope = res.Scope;
            return token;
        }

        public string DeAutorization() //Девторизация пользователя
        {
            token = connect_to_spotify.GetAccessToken();
            scope = null;
            return token;
        }

        public async Task<Root> SearchTrack(string trackTitle, string trackArtist) //Поиск трека
        {
            Root track_search = new Root(); //переменная для десериализации ответа от сервера
            string url = string.Format("https://api.spotify.com/v1/search?q={0} {1}&type=track&market=RU&market=US&market=EN&limit=10&offset=0",
                                        trackTitle, trackArtist); // формируем строку запроса

            try
            {
                client = connect_to_spotify.GetHttpClient(); //получаем текущий клиент
                HttpRequestMessage request = new HttpRequestMessage(); //создаем и настраиваем запрос
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringTask = client.SendAsync(request).Result; // отправляем запрос

                res_json = stringTask.Content;  //ответ сервера
                //var text = res_json.ReadAsStringAsync();
                track_search = await res_json.ReadFromJsonAsync<Root>(); // десериализация ответа

            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }

            return track_search; //возвращаем десериализованный ответ
        }

        public async Task<Root_PlayList> GetPlayListsUser(string user_id) //получаем все плейлисты пользователя
        {
            Root_PlayList PlayLists = new Root_PlayList();
            string url = string.Format("https://api.spotify.com/v1/users/{0}/playlists", user_id);

            try
            {
                client = connect_to_spotify.GetHttpClient();
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringTask = client.SendAsync(request).Result;

                res_json = stringTask.Content;
                var text = res_json.ReadAsStringAsync();
                PlayLists = await res_json.ReadFromJsonAsync<Root_PlayList>();

            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }

            return PlayLists;
        }
        public async Task<Root_Track> GetTrack(string track_id) //получаем трек
        {
            Root_Track track_info = new Root_Track();
            string url = string.Format("https://api.spotify.com/v1/tracks/{0}", track_id);

            try
            {
                client = connect_to_spotify.GetHttpClient();
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringTask = client.SendAsync(request).Result;

                res_json = stringTask.Content;
                //var text = res_json.ReadAsStringAsync();
                track_info = await res_json.ReadFromJsonAsync<Root_Track>();

            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }

            return track_info;
        }

        public async Task<List<Root_List_Track>> GetSeveralTrack(List<string> track_ids) //получаем несколько треков (максимум 50 за один запрос)
        {
            List<Root_List_Track> list_track_info = new List<Root_List_Track>();
            try
            {
                if (track_ids.Count > 50) //если больше 50 треков, то разделяем на несколько запросов
                {
                    int cnt = 0; //кол-во обработанных треков
                    while (cnt < track_ids.Count) //цикл выполняется пока не обработаются все переданные id
                    {
                        int len = 0;
                        if (track_ids.Count - cnt > 50)
                        {
                            len = 50;
                        }
                        else len = track_ids.Count - cnt;

                        string query = null;

                        for (int i = 0; i < len; i++) //собираем все id в одну строку с разделителем "%2C"
                        {
                            if (i != len - 1)
                            {
                                query = query + track_ids[i + cnt] + "%2C";
                            }
                            else { query = query + track_ids[i + cnt]; }

                        }

                        string url = string.Format("https://api.spotify.com/v1/tracks?ids={0}", query);

                        client = connect_to_spotify.GetHttpClient();
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.RequestUri = new Uri(url);
                        request.Method = HttpMethod.Get;
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var stringTask = client.SendAsync(request).Result;

                        res_json = stringTask.Content;
                        //var text = res_json.ReadAsStringAsync();
                        list_track_info.Add(await res_json.ReadFromJsonAsync<Root_List_Track>());
                        cnt = cnt + 50; //+50 треков обработано
                    }

                }
                else //выполняем один запрос 
                {
                    string query = null;

                    for (int i = 0; i < track_ids.Count; i++) //формируем все id в одну строку
                    {
                        if (i != track_ids.Count - 1)
                        {
                            query = query + track_ids[i] + "%2C";
                        }
                        else { query = query + track_ids[i]; }

                    }

                    string url = string.Format("https://api.spotify.com/v1/tracks?ids={0}", query);


                    client = connect_to_spotify.GetHttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var stringTask = client.SendAsync(request).Result;

                    res_json = stringTask.Content;
                    //var text = res_json.ReadAsStringAsync();
                    list_track_info.Add(await res_json.ReadFromJsonAsync<Root_List_Track>());

                }
            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }
            return list_track_info;
        }

        public async Task<Root_List_of_PlayList> GetPlaylist(string playlist_id) //получение плейлиста по ссылке на плейлист (максимум 100 треков, больше не покажет)
        {
            
            var mass = playlist_id.Split('/', '?');
            for (int i=0; i<mass.Length; i++) // достаем из переданной ссылки, id самого плейлиста
            {
                if (mass[i] == "playlist")
                {
                    playlist_id = mass[i + 1];
                    i = mass.Length;
                }
            }

            Root_List_of_PlayList list_of_playlist = new Root_List_of_PlayList();
            string url = string.Format("https://api.spotify.com/v1/playlists/{0}", playlist_id);


            try
            {
                client = connect_to_spotify.GetHttpClient();
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringTask = client.SendAsync(request).Result;

                res_json = stringTask.Content;
                //var text = res_json.ReadAsStringAsync();
                list_of_playlist = await res_json.ReadFromJsonAsync<Root_List_of_PlayList>();

            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }

            return list_of_playlist;
        }

        public async Task<Root_List_of_PlayList_Item> GetPlaylistItem(string playlist_id, int offset) //получаем полный список треков одного плейлиста
        {

            var mass = playlist_id.Split('/', '?');
            for (int i = 0; i < mass.Length; i++)// достаем из переданной ссылки, id самого плейлиста
            {
                if (mass[i] == "playlist")
                {
                    playlist_id = mass[i + 1];
                    i = mass.Length;
                }
            }

            Root_List_of_PlayList_Item list_of_playlist = new Root_List_of_PlayList_Item();
            string url = string.Format("https://api.spotify.com/v1/playlists/{0}/tracks?offset={1}", playlist_id, offset);


            try
            {
                client = connect_to_spotify.GetHttpClient();
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringTask = client.SendAsync(request).Result;

                res_json = stringTask.Content;
                //var text = res_json.ReadAsStringAsync();
                list_of_playlist = await res_json.ReadFromJsonAsync<Root_List_of_PlayList_Item>();

            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }

            return list_of_playlist;
        }

        public async Task<Root_Audio_Features> GetAudioFeatures(string track_id) //получаем музыкальные характеристики одного трека
        {
            Root_Audio_Features audio_features = new Root_Audio_Features();
            string url = string.Format("https://api.spotify.com/v1/audio-features/{0}", track_id);

            try
            {
                client = connect_to_spotify.GetHttpClient();
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringTask = client.SendAsync(request).Result;

                res_json = stringTask.Content;
                //var text = res_json.ReadAsStringAsync();
                audio_features = await res_json.ReadFromJsonAsync<Root_Audio_Features>();

            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }

            return audio_features;
        }

        public async Task<List<Root_Features_Several>> GetSeveralAudioFeatures(List<string> track_ids) //получаем музыкальные характеристики нескольких треков (максимум 100 треков за один запрос)
        {
            List<Root_Features_Several> audio_features = new List<Root_Features_Several>();
            try
            {
                if (track_ids.Count > 100) // если больше 100 треков, то повторяем запрс несколько раз
                {
                    int cnt = 0; //счётчик обработанных треков
                    while (cnt < track_ids.Count)
                    {
                        int len = 0;
                        if (track_ids.Count - cnt > 100)
                        {
                            len = 100;
                        }
                        else len = track_ids.Count - cnt;

                        string query = null;
                        for (int i = 0; i < len; i++) //собираем id всех треков в одну строку
                        {
                            if (i != len - 1)
                            {
                                query = query + track_ids[i + cnt] + "%2C";
                            }
                            else { query = query + track_ids[i + cnt]; }

                        }

                        string url = string.Format("https://api.spotify.com/v1/audio-features?ids={0}", query);
                        // SpotifyToken connect_to_spotify = new SpotifyToken();

                        client = connect_to_spotify.GetHttpClient();
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.RequestUri = new Uri(url);
                        request.Method = HttpMethod.Get;
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var stringTask = client.SendAsync(request).Result;
                        res_json = stringTask.Content;
                        //var text = await res_json.ReadAsStringAsync();

                        audio_features.Add(await res_json.ReadFromJsonAsync<Root_Features_Several>());
                        cnt = cnt + 100; //+100 треков обработано
                    }
                }
                else // иначе делаем только один запрос
                {
                    string query = null;

                    for (int i = 0; i < track_ids.Count; i++) //собираем все id в одну строку
                    {
                        if (i != track_ids.Count - 1)
                        {
                            query = query + track_ids[i] + "%2C";
                        }
                        else { query = query + track_ids[i]; }

                    }

                    string url = string.Format("https://api.spotify.com/v1/audio-features?ids={0}", query);

                    client = connect_to_spotify.GetHttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var stringTask = client.SendAsync(request).Result;
                    res_json = stringTask.Content;
                    //var text = await res_json.ReadAsStringAsync();


                    audio_features.Add(await res_json.ReadFromJsonAsync<Root_Features_Several>());

                }
            }
            catch (WebException ex1)
            {
                MessageBox.Show("Request Error: " + ex1.Status);
            }
            catch (TaskCanceledException ex2)
            {
                MessageBox.Show("Request Error: " + ex2.Message);
            }
            return audio_features;
        }


        public async Task<Root_SavedTracks_Rootobject> GetSavedTracks(int offset) //получаем список сохранённых треков текущего пользователя
        {

            if (scope != null) //проверка , что приложение имеет доступ к данным пользователя
            {

                Root_SavedTracks_Rootobject saved_tracks_of_user = new Root_SavedTracks_Rootobject();
                string url = string.Format("https://api.spotify.com/v1/me/tracks?market=RU&limit=50&offset={0}", offset);


                try
                {
                    client = connect_to_spotify.GetHttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var stringTask = client.SendAsync(request).Result;

                    res_json = stringTask.Content;
                    //var text = res_json.ReadAsStringAsync();
                    saved_tracks_of_user = await res_json.ReadFromJsonAsync<Root_SavedTracks_Rootobject>();
                    return saved_tracks_of_user;
                }
                catch (WebException ex1)
                {
                    MessageBox.Show("Request Error: " + ex1.Status);
                }
                catch (TaskCanceledException ex2)
                {
                    MessageBox.Show("Request Error: " + ex2.Message);
                }     
            }
            MessageBox.Show("Вам необходимо авторизироваться!");
            return null;
        }




        public async Task<Root_Profile_Rootobject> GetUserProfile() //получаем данные об авторизованном пользователе
        {

            if (scope != null) //проверка , что приложение имеет доступ к данным пользователя
            {

                Root_Profile_Rootobject Profile_user = new Root_Profile_Rootobject(); //создаем переменную для хранения JSON ответа
                string url = "https://api.spotify.com/v1/me"; //url адрес отправляемого HTTP запроса


                try
                {
                    client = connect_to_spotify.GetHttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(); //создаем запрос
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var stringTask = client.SendAsync(request).Result;//отправка запрпоса на сервер

                    res_json = stringTask.Content;//получаем JSON ответ
                    Profile_user = await res_json.ReadFromJsonAsync<Root_Profile_Rootobject>(); 
                    return Profile_user;//возвращаем переменную с сохраненным ответом
                }
                catch (WebException ex1)
                {
                    MessageBox.Show("Request Error: " + ex1.Status);
                }
                catch (TaskCanceledException ex2)
                {
                    MessageBox.Show("Request Error: " + ex2.Message);
                }

                
            }
            MessageBox.Show("Вам необходимо авторизироваться!");
            return null;
        }



        public async Task<Root_CreatePlst_Rootobject> CreateNewPlaylist(string user_id, string name_of_playlist) //создаем плейлист для авторизованного пользователя
        {

            if (scope != null) //проверка , что приложение имеет доступ к данным пользователя
            {

                Root_CreatePlst_Rootobject Create_playlist = new Root_CreatePlst_Rootobject();
                string url = string.Format("https://api.spotify.com/v1/users/{0}/playlists", user_id);


                try
                {
                    client = connect_to_spotify.GetHttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Post;

                    string json = "{" + String.Format("\"name\":\"{0}\",\"description\":\"Recomendation from Mymusic\",\"public\":false", name_of_playlist) + "}";
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json"); //Добавление контента в запрос

                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var stringTask = client.SendAsync(request).Result;

                    res_json = stringTask.Content;
                    //var text = res_json.ReadAsStringAsync();
                    Create_playlist = await res_json.ReadFromJsonAsync<Root_CreatePlst_Rootobject>();
                    return Create_playlist;
                }
                catch (WebException ex1)
                {
                    MessageBox.Show("Request Error: " + ex1.Status);
                }
                catch (TaskCanceledException ex2)
                {
                    MessageBox.Show("Request Error: " + ex2.Message);
                }


            }
            MessageBox.Show("Вам необходимо авторизироваться!");
            return null;
        }


        public async Task<Add_To_Playlist_Rootobject> AddTracksToPlaylist(string playlist_id, List<string> track_id) //добавляем треки в плейлист для авторизованного пользователя
        {

            if (scope != null) //проверка , что приложение имеет доступ к данным пользователя
            {

                Add_To_Playlist_Rootobject Add_to_playlist = new Add_To_Playlist_Rootobject();
                string url = string.Format("https://api.spotify.com/v1/playlists/{0}/tracks", playlist_id);


                try
                {
                    client = connect_to_spotify.GetHttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Post;

                    string tracklist="";
                    for (int i=0; i < track_id.Count; i++)
                    {
                        if (i < track_id.Count - 1)
                        {
                            tracklist += String.Format("\"spotify:track:{0}\",", track_id[i]);
                        } else tracklist += String.Format("\"spotify:track:{0}\"", track_id[i]);
                    }

                    string json = "{\"uris\": [" + tracklist + "] ,\"position\":0}";
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json"); //Добавление контента в запрос

                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var stringTask = client.SendAsync(request).Result;

                    res_json = stringTask.Content;
                    //var text = res_json.ReadAsStringAsync();
                    Add_to_playlist = await res_json.ReadFromJsonAsync<Add_To_Playlist_Rootobject>();
                    return Add_to_playlist;
                }
                catch (WebException ex1)
                {
                    MessageBox.Show("Request Error: " + ex1.Status);
                }
                catch (TaskCanceledException ex2)
                {
                    MessageBox.Show("Request Error: " + ex2.Message);
                }


            }
            MessageBox.Show("Вам необходимо авторизироваться!");
            return null;
        }


    }


   
}

