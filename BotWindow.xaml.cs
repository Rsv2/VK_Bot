using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Bot
{
    /// <summary>
    /// Логика взаимодействия для BotWindow.xaml
    /// </summary>
    public partial class BotWindow : Window
    {
        #region Глобальные переменные.

        AccountData MyAccount = new AccountData();
        readonly string Method = "https://api.vk.com/method/";
        readonly string IniFile = System.Windows.Forms.Application.StartupPath + "\\Account.json";          //Файл данных аккаунта.
        readonly string LogFile = System.Windows.Forms.Application.StartupPath + "\\Log.json";              //Файл истории сообщений.
        readonly WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };                               //Основной WebClient для сканирования LongPoll.
        readonly WebClient wc2 = new WebClient() { Encoding = Encoding.UTF8 };                              //Вспомогательный WebClient для загрузок, проверок и т. п.
        Uri Req;                                                                                            //Глобальный Uri для обращения к wc1.
        bool Interrupt = false;                                                                             //Флаг прерывания, для запроса к wc1 по глобальному Req.
        LongPollStruct LongPoll = new LongPollStruct();                                                     //Структура для полученния данных LongPoll.
        GetLongPollServer GetLPServer = new GetLongPollServer();                                            //Структура обращения к LongPoll серверу.
        readonly List<MessageUI> MessageArray = new List<MessageUI>();                                      //Массив сообщений для вывода в стек панель.
        string PathToFile;                                                                                  //Путь к отправляемому файлу.
        readonly List<string> AttachmentList = new List<string>();                                          //Массив вложений к сообщению.
        readonly long TimeStart = DateTimeOffset.Now.ToUnixTimeSeconds();                                   //Время запуска бота по Unix.
        ShowAttachmentWin ShowAtt;                                                                          //Окно для вывода фото в браузере.
        UploadsWin ShowUpp;                                                                                 //Окно вложений перед отправкой.
        readonly string MyUsers = System.Windows.Forms.Application.StartupPath + "\\Users.json";            //Файл контактов.
        List<Single_user> MyUsersList = new List<Single_user>();                                            //Структура контактов.
        readonly List<string> NamesList = new List<string>();                                               //Коллекция полученных имён для комбобокса.
        readonly List<AttachmentUI> AttItems = new List<AttachmentUI>();                                    //Коллекция вложений для отправки.
        readonly List<AttachmentUI> StackItems = new List<AttachmentUI>();                                  //Коллекция вложений в Stackpanel.
        readonly List<string> Log = new List<string>();                                                     //Лог истории сообщений.
        bool BotStarted = false;                                                                            //Флаг запущенного бота.
        bool Downloadflag = false;                                                                          //Флаг текущего скачивания.

        #endregion

        #region Запуск и фоновая работа с LongPoll.

        /// <summary>
        /// Инициализация.
        /// </summary>
        public BotWindow()
        {
            InitializeComponent();
            if (!File.Exists(IniFile))
            {
                AccountTab.IsSelected = true;
            }
            else
            {
                MyAccount = JsonConvert.DeserializeObject<AccountData>(File.ReadAllText(IniFile));
                AccountID.Text = MyAccount.AccountID;
                GroupID.Text = MyAccount.GroupID;
                GroupToken.Text = MyAccount.GroupToken;
                SecretKey.Text = MyAccount.SecretKey;
                ApplicationID.Text = MyAccount.ApplicationID;
                YourToken.Text = MyAccount.IndToken;
                if (YourToken.Text == "" || (TimeStart - Int64.Parse(MyAccount.LastDate)) > (Int64.Parse(MyAccount.Exp) - 3600))
                {
                    MyWeb.Source = new Uri($"https://" +
                        $"oauth.vk.com/authorize?client_id={ApplicationID.Text}&display=page&redirect_uri=" +
                        $"https://" +
                        $"oauth.vk.com/blank.html&scope=audio,video,docs,photo&response_type=token&v=5.107&state=12345");
                }
                else
                {
                    BotTab.IsSelected = true;
                    BotStarted = true;
                    StartBot();
                }
            }
        }
        /// <summary>
        /// Запуск бота.
        /// </summary>
        public async void StartBot()
        {
            Req = new Uri($"{Method}groups.getLongPollServer?group_id={MyAccount.GroupID}&need_pts=1&" +
                $"access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107");
            string req = wc.DownloadString(Req).ToString();
            req = req.Substring(12, req.Length - 13);
            GetLPServer = JsonConvert.DeserializeObject<GetLongPollServer>(req);
            req = $"{GetLPServer.Server}?act=a_check&key={GetLPServer.Key}&ts={GetLPServer.Ts}&messages=25&wait=2&mode=128&version=5.107";
            Req = new Uri(req);
            uint LastTS = 0;
            if (!File.Exists(MyUsers))
            {
                AddUserToList(MyAccount.AccountID);
            }
            UpdateUsersList();
            List<string> SavedLog = new List<string>();
            if (File.Exists(LogFile))
            {
                SavedLog = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(LogFile));
            }
            int SL = 0;
            while (true)
            {
                string answer;
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - Int64.Parse(MyAccount.LastDate) > Int64.Parse(MyAccount.Exp) - 3600)
                {
                    MyWeb.Source = new Uri($"https://" +
                        $"oauth.vk.com/authorize?client_id={ApplicationID.Text}&display=page&redirect_uri=https://" +
                        $"oauth.vk.com/blank.html&scope=audio,video,docs,photo&response_type=token&v=5.107&state=12345");
                }

                if (SL >= SavedLog.Count)
                {
                    answer = await Task.Factory.StartNew<string>(() => RunCheck(), TaskCreationOptions.LongRunning);
                }
                else
                {
                    answer = SavedLog[SL];
                    SL++;
                }

                LongPoll = JsonConvert.DeserializeObject<LongPollStruct>(answer);
                GetLPServer.Ts = LongPoll.Ts;
                if (answer.IndexOf("error") != -1)
                {
                    AnswerTxt.Text = answer;
                }
                if (answer.IndexOf("type") != -1 && LastTS != GetLPServer.Ts)
                {
                    LastTS = GetLPServer.Ts;
                    if (LongPoll.Updates[LongPoll.Updates.Count - 1].Type == "message_reply")
                    {
                        Log.Add(answer);
                        MainMessage(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        if (answer.IndexOf("photo") != -1)
                        {
                            PhotoAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        if (answer.IndexOf("audio") != -1)
                        {
                            AudioAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        if (answer.IndexOf("video") != -1)
                        {
                            VideoAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        if (answer.IndexOf("doc") != -1)
                        {
                            DocAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        File.WriteAllText(LogFile, JsonConvert.SerializeObject(Log));
                    }
                    else if (LongPoll.Updates[LongPoll.Updates.Count - 1].Type == "message_new")
                    {
                        Log.Add(answer);
                        MainMessage(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        if (answer.IndexOf("photo") != -1)
                        {
                            PhotoAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        if (answer.IndexOf("audio") != -1)
                        {
                            AudioAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        if (answer.IndexOf("video") != -1)
                        {
                            VideoAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        if (answer.IndexOf("doc") != -1)
                        {
                            DocAttachment(LongPoll.Updates[LongPoll.Updates.Count - 1].Type);
                        }
                        File.WriteAllText(LogFile, JsonConvert.SerializeObject(Log));
                    }
                    Typing_text.Text = "Печатает";
                }
                if (answer.IndexOf("typing") == -1)
                {
                    Typing_text.Text = "";
                }
                txt.Text = answer;
            }
        }
        /// <summary>
        /// Скан сервера.
        /// </summary>
        /// <returns>Ответ сервера</returns>
        public string RunCheck()
        {
            Task.Delay(25000).Wait(10);
            string msgData = wc.DownloadString(Req).ToString();
            if (!Interrupt)
            {
                Req = new Uri($"{GetLPServer.Server}?act=a_check&key={GetLPServer.Key}&ts={GetLPServer.Ts}&messages=25&wait=2&mode=128&version=5.107");
            }
            else
            {
                Interrupt = false;
            }
            return msgData;
        }
        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessageBtn(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            string sendMess = $"{Method}messages.send?user_id={MyUsersList[UsersList.SelectedIndex].User_id}&" +
                $"random_id={rnd.Next(0, 1000000)}&message={BotMessage.Text}";
            if (AttachmentList.Count > 0)
            {
                sendMess += $"&attachment={AttachmentList[0]}";
                for (int i = 1; i < AttachmentList.Count; i++)
                {
                    sendMess += $",{AttachmentList[i]}";
                }
            }
            sendMess += $"&access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107";
            AnswerTxt.Text = sendMess;
            Interrupt = true;
            Req = new Uri(sendMess);
            AttachmentList.Clear();
            AttItems.Clear();
            if (ShowUpp != null)
            {
                ShowUpp.Close();
            }
        }
        /// <summary>
        /// Получение индивидуального токена.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendAccauntData(object sender, RoutedEventArgs e)
        {
            if (AccountID.Text != "" && GroupID.Text != "" && GroupToken.Text != "" && SecretKey.Text != "" && ApplicationID.Text != "")
            {
                MyAccount.AccountID = AccountID.Text;
                MyAccount.GroupID = GroupID.Text;
                MyAccount.GroupToken = GroupToken.Text;
                MyAccount.SecretKey = SecretKey.Text;
                MyAccount.ApplicationID = ApplicationID.Text;
                MyWeb.Source = new Uri($"https://" +
                    $"oauth.vk.com/authorize?client_id={ApplicationID.Text}&display=page&redirect_uri=https://" +
                    $"oauth.vk.com/blank.html&scope=audio,video,docs,photo&response_type=token&v=5.107&state=123456");
            }
        }
        /// <summary>
        /// Получение индивидуального токена (после запроса в браузере).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetToken(object sender, NavigationEventArgs e)
        {
            if (MyWeb.Source.ToString().IndexOf("token=") != -1)
            {
                MyAccount.IndToken = GetDeserialized(MyWeb.Source.ToString(), "access_token=", 2);
                YourToken.Text = MyAccount.IndToken;
                MyAccount.Exp = GetDeserialized(MyWeb.Source.ToString(), "expires_in=", 2);
                MyAccount.LastDate = TimeStart.ToString();
                File.WriteAllText(IniFile, JsonConvert.SerializeObject(MyAccount));
                BotTab.IsSelected = true;
                if (!BotStarted)
                {
                    BotStarted = true;
                    StartBot();
                }
            }
        }

        #endregion

        #region Добавление/удаление контактов.

        /// <summary>
        /// Добавить пользователя (кнопка).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUserBtn(object sender, RoutedEventArgs e)
        {            
            if (UserIDBox.Text != null && UserIDBox.Text.Length == 9)
            {
                bool contains = false;
                for (int i = 0; i < MyUsersList.Count; i++)
                {
                    if (UserIDBox.Text == MyUsersList[i].User_id)
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                {
                    AddUserToList(UserIDBox.Text);
                    UpdateUsersList();
                }
            }
        }
        /// <summary>
        /// Добавление юзера в список контактов.
        /// </summary>
        /// <param name="UserID"></param>
        private void AddUserToList(string UserID)
        {
            Uri UserReqst = new Uri($"{Method}users.get?user_ids={UserID}&fields=bdate&access_token={MyAccount.GroupToken}&" +
                $"secret={MyAccount.SecretKey}&v=5.107");
            string UserData = wc2.DownloadString(UserReqst).ToString();
            if (UserData.IndexOf("error") == -1 && UserData.IndexOf("DELETED") == -1)
            {
                Single_user User = new Single_user();
                User.User_id = UserID;
                User.User_name = GetDeserialized(UserData, "first_name\":\"", 0);
                User.User_surname = GetDeserialized(UserData, "last_name\":\"", 0);
                MyUsersList.Add(User);
                File.WriteAllText(MyUsers, JsonConvert.SerializeObject(MyUsersList));
            }
        }
        /// <summary>
        /// Обновить список юзеров.
        /// </summary>
        private void UpdateUsersList()
        {
            MyUsersList.Clear();
            MyUsersList = JsonConvert.DeserializeObject<List<Single_user>>(File.ReadAllText(MyUsers));
            NamesList.Clear();
            UsersList.Items.Clear();
            for (int n = 0; n < MyUsersList.Count; n++)
            {
                NamesList.Add($"{MyUsersList[n].User_name} {MyUsersList[n].User_surname}");
                UsersList.Items.Add(NamesList[n]);
            }
            UsersList.SelectedIndex = UsersList.Items.Count - 1;
        }
        /// <summary>
        /// Удалить контакт из списка (кнопка).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveUserBtn(object sender, RoutedEventArgs e)
        {
            RemoveUserFromList();
        }
        /// <summary>
        /// Удалить контакт из списка.
        /// </summary>
        private void RemoveUserFromList()
        {
            if (UsersList.SelectedIndex != 0)
            {
                List<Single_user> Temp = new List<Single_user>();
                Temp.AddRange(MyUsersList);
                MyUsersList.Clear();
                for (int n = 0; n < Temp.Count; n++)
                {
                    if (n != UsersList.SelectedIndex)
                    {
                        MyUsersList.Add(Temp[n]);
                    }
                }
                File.WriteAllText(MyUsers, JsonConvert.SerializeObject(MyUsersList));
                UpdateUsersList();
            }
        }

        #endregion

        #region Показ, скачивание и проигрывание вложений.

        /// <summary>
        /// Показать изображение, можно скачать через меню браузера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImage(object sender, MouseButtonEventArgs e)
        {
            if (ShowAtt == null)
            {
                ShowAtt = new ShowAttachmentWin();
                ShowAtt.Owner = this;
                ShowAtt.Closed += (x, y) => { ShowAtt = null; };
                ShowAtt.Show();
            }
            for (int i = 0; i < StackItems.Count; i++)
            {
                if (StackItems[i].Num.Text == "Clicked")
                {
                    ShowAtt.ShowWin.Source = new Uri(StackItems[i].Link.Text);
                    StackItems[i].Num.Text = "";
                    break;
                }
            }
        }
        /// <summary>
        /// Проиграть аудио (WMPlayer).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayMusic(object sender, MouseButtonEventArgs e)
        {
            if (!Downloadflag)
            {
                string LinkToPlay = "";
                string SoundName = "";
                for (int i = 0; i < StackItems.Count; i++)
                {
                    if (StackItems[i].Num.Text == "Clicked")
                    {
                        LinkToPlay = StackItems[i].Link.Text;
                        SoundName = StackItems[i].Name.Text;
                        StackItems[i].Num.Text = "";
                        break;
                    }
                }
                if (e.ChangedButton == MouseButton.Left)
                {
                    Process.Start("WMPlayer.exe", LinkToPlay);
                }
                else
                {
                    Microsoft.Win32.SaveFileDialog fd = new Microsoft.Win32.SaveFileDialog();
                    fd.FileName = SoundName;
                    fd.DefaultExt = ".mp3";
                    fd.Filter = ".mp3|*.mp3";
                    fd.Title = "Укажите куда скачать файл.";
                    Nullable<bool> result = fd.ShowDialog();
                    if (result == true)
                    {
                        PathToFile = fd.FileName;
                        Uri downloaduri = new Uri(LinkToPlay);
                        wc2.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Sender);
                        wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WhileDownloading);
                        Downloadflag = true;
                        wc2.DownloadDataAsync(downloaduri);
                    }
                }
            }
        }
        /// <summary>
        /// Смотреть видео (В браузере или WM Player).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayVideo(object sender, MouseButtonEventArgs e)
        {
            if (!Downloadflag)
            {
                string LinkToPlay = "";
                string VideoName = "";
                for (int i = 0; i < StackItems.Count; i++)
                {
                    if (StackItems[i].Num.Text == "Clicked")
                    {
                        LinkToPlay = StackItems[i].Link.Text;
                        VideoName = StackItems[i].Name.Text;
                        Clipboard.Clear();
                        Clipboard.SetText(LinkToPlay);
                        StackItems[i].Num.Text = "";
                        break;
                    }
                }
                string getStream = wc2.DownloadString(LinkToPlay);
                if (getStream.IndexOf(".240.") == -1)
                {
                    MessageBox.Show($"Видео не доступно.\r\n1. Возможно идёт обработка.\r\n" +
                        $"В этом случае попробуйте нажать чуть позднее.\r\n" +
                        $"Качество видео будет улучшаться по мере его обработки на сервере.\r\n" +
                        $"2. Видео не загружено в вашу группу.\r\n" +
                        $"В этом случае для просмотра, кликните ПКМ в адресной строке\r\n" +
                        $"вашего браузера и вставьте ссылку, полученную после нажатия\r\n" +
                        $"на эту картинку.\r\n" +
                        $"Видео не будет работать в браузере IE.");
                }
                else
                {
                    string Startstring = "";
                    string Endstring = GetDeserialized(getStream, "/videos/", 3);
                    if (getStream.IndexOf("%5B1080%5D") != -1)
                    {
                        Startstring = GetDeserialized(getStream, "%5B1080%5D=", 4);
                        Startstring = Startstring.Replace("%3A", ":");
                        Startstring = Startstring.Replace("%2F", "/");
                        Endstring = Endstring.Replace(".240.", ".1080.");
                        getStream = Startstring + "/videos/" + Endstring;
                    }
                    else if (getStream.IndexOf("%5B720%5D") != -1)
                    {
                        Startstring = GetDeserialized(getStream, "%5B720%5D=", 4);
                        Startstring = Startstring.Replace("%3A", ":");
                        Startstring = Startstring.Replace("%2F", "/");
                        Endstring = Endstring.Replace(".240.", ".720.");
                        getStream = Startstring + "/videos/" + Endstring;
                    }
                    else if (getStream.IndexOf("%5B480%5D") != -1)
                    {
                        Startstring = GetDeserialized(getStream, "%5B480%5D=", 4);
                        Startstring = Startstring.Replace("%3A", ":");
                        Startstring = Startstring.Replace("%2F", "/");
                        Endstring = Endstring.Replace(".240.", ".480.");
                        getStream = Startstring + "/videos/" + Endstring;
                    }
                    else if (getStream.IndexOf("%5B360%5D") != -1)
                    {
                        Startstring = GetDeserialized(getStream, "%5B360%5D=", 4);
                        Startstring = Startstring.Replace("%3A", ":");
                        Startstring = Startstring.Replace("%2F", "/");
                        Endstring = Endstring.Replace(".240.", ".360.");
                        getStream = Startstring + "/videos/" + Endstring;
                    }
                    else
                    {
                        getStream = GetDeserialized(getStream, ".mpegurl\" />< source src = \"", 3);
                    }
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Process.Start("WMPlayer.exe", getStream);
                    }
                    else
                    {
                        Microsoft.Win32.SaveFileDialog fd = new Microsoft.Win32.SaveFileDialog();
                        fd.FileName = $"{VideoName}.mp4";
                        fd.Title = "Укажите куда скачать файл.";
                        Nullable<bool> result = fd.ShowDialog();
                        if (result == true)
                        {
                            PathToFile = fd.FileName;
                            Uri downloaduri = new Uri(getStream);
                            wc2.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Sender);
                            wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WhileDownloading);
                            Downloadflag = true;
                            wc2.DownloadDataAsync(downloaduri);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Скачать файл.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadDoc(object sender, MouseButtonEventArgs e)
        {
            if (!Downloadflag)
            {
                string LinkToPlay = "";
                string DocName = "";
                for (int i = 0; i < StackItems.Count; i++)
                {
                    if (StackItems[i].Num.Text == "Clicked")
                    {
                        LinkToPlay = StackItems[i].Link.Text;
                        DocName = StackItems[i].Name.Text;
                        StackItems[i].Num.Text = "";
                        break;
                    }
                }
                Microsoft.Win32.SaveFileDialog fd = new Microsoft.Win32.SaveFileDialog();
                fd.FileName = DocName;
                fd.Title = "Укажите куда скачать файл.";
                Nullable<bool> result = fd.ShowDialog();
                if (result == true)
                {
                    PathToFile = fd.FileName;
                    Uri downloaduri = new Uri(LinkToPlay);
                    wc2.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Sender);
                    wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WhileDownloading);
                    Downloadflag = true;
                    wc2.DownloadDataAsync(downloaduri);
                }
            }
        }
        /// <summary>
        /// Завершение скачивания файла (асинхронно).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sender(Object sender, DownloadDataCompletedEventArgs e)
        {
            Downloading.Text = "";
            File.WriteAllBytes(PathToFile, e.Result);
            Process.Start(Path.GetDirectoryName(PathToFile));
            Downloadflag = false;
        }
        /// <summary>
        /// Прогресс скачивания файла.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhileDownloading(object sender, DownloadProgressChangedEventArgs e)
        {
            Downloading.Text = $"Скачивание {e.ProgressPercentage}%";
        }

        #endregion

        #region Загрузка вложений на сервер.

        /// <summary>
        /// Загрузка файла на сервер (кнопка).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendDocBtn(object sender, MouseButtonEventArgs e)
        {
            if (AttItems.Count < 6)
            {
                CheckDoc();
            }
            else
            {
                MessageBox.Show($"Не более 6 вложений за отправку.");
            }
        }
        /// <summary>
        /// Загрузка файла на сервер.
        /// </summary>
        private async void CheckDoc()
        {
            Microsoft.Win32.OpenFileDialog fd = new Microsoft.Win32.OpenFileDialog();
            fd.FileName = "Document";
            fd.Title = "Допускается любой формат, кроме exe и mp3, размером не более 200мб";
            Nullable<bool> result = fd.ShowDialog();

            if (result == true)
            {
                PathToFile = fd.FileName;
                if ((PathToFile.IndexOf(".exe") == -1) && (PathToFile.IndexOf(".mp3") == -1))
                {
                    FileInfo testlength = new FileInfo(PathToFile);
                    if (testlength.Length < 200_000_000)
                    {
                        HideButtons();
                        Uri Reqst = new Uri($"{Method}docs.getMessagesUploadServer?type=doc&peer_id={MyAccount.AccountID}&access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107");
                        string resp = wc2.DownloadString(Reqst).ToString();
                        if (resp.IndexOf("error") != -1)
                        {
                            MessageBox.Show($"Ошибка получения ссылки на загрузку.\r\n{resp}");
                        }
                        else
                        {
                            Response_audioserver Response = JsonConvert.DeserializeObject<Response_audioserver>(resp);
                            MultipartFormDataContent Docform = new MultipartFormDataContent();
                            Docform.Add(new ByteArrayContent(File.ReadAllBytes(PathToFile)), "file", PathToFile.Substring(PathToFile.LastIndexOf("\\") + 1));
                            var httpClient = new HttpClient();
                            HttpResponseMessage response = await httpClient.PostAsync(Response.Response.Upload_url, Docform);
                            response.EnsureSuccessStatusCode();
                            httpClient.Dispose();
                            resp = response.Content.ReadAsStringAsync().Result;
                            if (resp.IndexOf("error") != -1)
                            {
                                MessageBox.Show($"Ошибка\r\nВозможно некорректное имя файла\r\n{PathToFile}\r\n{resp}");
                            }
                            else
                            {
                                GetFile MyFile = JsonConvert.DeserializeObject<GetFile>(resp);
                                Reqst = new Uri($"{Method}docs.save?file={MyFile.File}&access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107");
                                resp = wc2.DownloadString(Reqst).ToString();
                                if (resp.IndexOf("error") != -1)
                                {
                                    MessageBox.Show($"Ошибка сохранения.\r\n{resp}");
                                }
                                else
                                {
                                    Response_getdoc MyDoc = JsonConvert.DeserializeObject<Response_getdoc>(resp);
                                    AttachmentList.Add($"doc{MyDoc.Response.Doc.Owner_id}_{MyDoc.Response.Doc.Id}_{MyAccount.SecretKey}");
                                    NewAttachment("Images\\IconGroup174.png");
                                }
                            }
                        }
                        ShowButtons();
                    }
                    else
                    {
                        MessageBox.Show("Слишком большой файл!");
                        CheckDoc();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Запрещены файлы exe и mp3!");
                    CheckDoc();
                    return;
                }
            }
        }
        /// <summary>
        /// Загрузка фото на сервер (кнопка).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendPhotoBtn(object sender, MouseButtonEventArgs e)
        {
            if (AttItems.Count < 6)
            {
                CheckPhoto();
            }
            else
            {
                MessageBox.Show($"Не более 6 вложений за отправку.");
            }
        }
        /// <summary>
        /// Загрузка фото на сервер.
        /// </summary>
        private async void CheckPhoto()
        {
            Microsoft.Win32.OpenFileDialog fd = new Microsoft.Win32.OpenFileDialog();
            fd.FileName = "Photo";
            fd.DefaultExt = ".jpg; .png";
            fd.Filter = "Изображения (.jpg; .png)|*.jpg; *.png";
            Nullable<bool> result = fd.ShowDialog();
            if (result == true)
            {
                PathToFile = fd.FileName;
                HideButtons();
                Uri Reqst = new Uri($"{Method}photos.getMessagesUploadServer?" +
                    $"type=photo&peer_id={MyAccount.AccountID}&access_token={MyAccount.GroupToken}" +
                    $"&secret={MyAccount.SecretKey}&v=5.107");
                string resp = wc2.DownloadString(Reqst).ToString();
                if (resp.IndexOf("error") != -1)
                {
                    MessageBox.Show($"Ошибка получения ссылки на загрузку.\r\n{resp}");
                }
                else
                {
                    Response_send_photo PhotoResponse = JsonConvert.DeserializeObject<Response_send_photo>(resp);
                    MultipartFormDataContent Photoform = new MultipartFormDataContent();
                    Photoform.Add(new ByteArrayContent(File.ReadAllBytes(PathToFile)), "photo", PathToFile.Substring(PathToFile.LastIndexOf("\\") + 1));
                    var httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync(PhotoResponse.Response.Upload_url, Photoform);
                    response.EnsureSuccessStatusCode();
                    httpClient.Dispose();
                    resp = response.Content.ReadAsStringAsync().Result;
                    if (resp.IndexOf("error") != -1)
                    {
                        MessageBox.Show($"Ошибка отправки.\r\n{resp}");
                    }
                    else
                    {
                        Save_photo PhotoSaving = JsonConvert.DeserializeObject<Save_photo>(resp);
                        Reqst = new Uri($"{Method}photos.saveMessagesPhoto?server={PhotoSaving.Server}&photo={PhotoSaving.Photo}&hash={PhotoSaving.Hash}&access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107");
                        resp = wc2.DownloadString(Reqst).ToString();
                        if (resp.IndexOf("error") != -1)
                        {
                            MessageBox.Show($"Ошибка сохранения.\r\nВозможно некорректное имя файла\r\n{resp}");
                        }
                        else
                        {
                            Response_photo_save SendPhotoEnd = JsonConvert.DeserializeObject<Response_photo_save>(resp);
                            AttachmentList.Add($"photo{MyAccount.AccountID}_{SendPhotoEnd.Response[0].Id}");
                            NewAttachment(PathToFile);
                        }
                    }
                }
                ShowButtons();
            }
        }
        /// <summary>
        /// Загрузка аудио на сервер (кнопка).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendAudioBtn(object sender, MouseButtonEventArgs e)
        {
            if (AttItems.Count < 6)
            {
                CheckAudio();
            }
            else
            {
                MessageBox.Show($"Не более 6 вложений за отправку.");
            }
        }
        /// <summary>
        /// Загрузка аудио на сервер.
        /// </summary>
        private async void CheckAudio()
        {
            Microsoft.Win32.OpenFileDialog fd = new Microsoft.Win32.OpenFileDialog();
            fd.FileName = "Audio";
            fd.DefaultExt = ".mp3";
            fd.Filter = "Аудио не более 200 мб (.mp3)|*.mp3";
            Nullable<bool> result = fd.ShowDialog();
            if (result == true)
            {
                PathToFile = fd.FileName;
                FileInfo testlength = new FileInfo(PathToFile);
                if (testlength.Length < 200_000_000)
                {
                    HideButtons();
                    Uri Reqst = new Uri($"{Method}audio.getUploadServer?type=audio&peer_id={MyAccount.AccountID}&access_token={MyAccount.IndToken}&v=5.107");
                    string resp = wc2.DownloadString(Reqst).ToString();
                    if (resp.IndexOf("error") != -1)
                    {
                        MessageBox.Show($"Ошибка получения ссылки на загрузку.\r\n{resp}");
                    }
                    else
                    {
                        Response_audioserver GetAudioUrl = JsonConvert.DeserializeObject<Response_audioserver>(resp);
                        MultipartFormDataContent Audioform = new MultipartFormDataContent();
                        Audioform.Add(new ByteArrayContent(File.ReadAllBytes(PathToFile)), "file", PathToFile.Substring(PathToFile.LastIndexOf("\\") + 1));
                        var httpClient = new HttpClient();
                        HttpResponseMessage response = await httpClient.PostAsync(GetAudioUrl.Response.Upload_url, Audioform);
                        response.EnsureSuccessStatusCode();
                        httpClient.Dispose();
                        resp = response.Content.ReadAsStringAsync().Result;
                        if (resp.IndexOf("error") != -1)
                        {
                            MessageBox.Show($"Ошибка загрузки на сервер.\r\nВозможно некорректное имя файла\r\n{resp}");
                        }
                        else
                        {
                            Audio_saved GetAudioReq = JsonConvert.DeserializeObject<Audio_saved>(resp);
                            GetAudioReq.Audio = GetAudioReq.Audio.Replace("%", "%25");
                            Reqst = new Uri($"{Method}audio.save?server={GetAudioReq.Server}&audio={GetAudioReq.Audio}&hash={GetAudioReq.Hash}&access_token={MyAccount.IndToken}&v=5.107");
                            resp = wc2.DownloadString(Reqst);
                            if (resp.IndexOf("error") != -1)
                            {
                                MessageBox.Show($"Ошибка сохранения.\r\nВозможно некорректное имя файла\r\n{resp}");
                            }
                            else
                            {
                                Get_audio_end GetAudio = JsonConvert.DeserializeObject<Get_audio_end>(resp);
                                AttachmentList.Add($"audio{MyAccount.AccountID}_{GetAudio.Response.Id}");
                                NewAttachment("Images\\IconGroup131.png");
                            }
                        }
                    }
                    ShowButtons();
                }
                else
                {
                    MessageBox.Show("Слишком большой файл!");
                    CheckAudio();
                    return;
                }
            }
        }
        /// <summary>
        /// Отправить видео.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendVideoBtn(object sender, MouseButtonEventArgs e)
        {
            if (AttItems.Count < 6)
            {
                CheckVideo();
            }
            else
            {
                MessageBox.Show($"Не более 6 вложений за отправку.");
            }
        }
        /// <summary>
        /// Загрузка видео на сервер.
        /// </summary>
        private async void CheckVideo()
        {
            Microsoft.Win32.OpenFileDialog fd = new Microsoft.Win32.OpenFileDialog();
            fd.FileName = "Video";
            fd.DefaultExt = ".AVI; .MP4; .3GP; .MPEG; .MOV; .MP3; .FLV; .WMV";
            fd.Filter = "Видео не более 200 мб (.AVI; .MP4; .3GP; .MPEG; .MOV; .MP3; .FLV; .WMV)|*.AVI; *.MP4; *.3GP; *.MPEG; *.MOV; *.MP3; *.FLV; *.WMV";
            Nullable<bool> result = fd.ShowDialog();

            if (result == true)
            {
                PathToFile = fd.FileName;
                FileInfo testlength = new FileInfo(PathToFile);
                if (testlength.Length < 200_000_000)
                {
                    HideButtons();
                    Uri Reqst = new Uri($"{Method}video.save?group_id={MyAccount.GroupID}&access_token={MyAccount.IndToken}&v=5.107");
                    string resp = wc2.DownloadString(Reqst).ToString();
                    if (resp.IndexOf("error") != -1)
                    {
                        MessageBox.Show($"Ошибка получения ссылки на загрузку.\r\n{resp}");
                    }
                    else
                    {
                        Response_video_server Response = JsonConvert.DeserializeObject<Response_video_server>(resp);
                        MultipartFormDataContent Videoform = new MultipartFormDataContent();
                        Videoform.Add(new ByteArrayContent(File.ReadAllBytes(PathToFile)), "file", PathToFile.Substring(PathToFile.LastIndexOf("\\") + 1));
                        var httpClient = new HttpClient();
                        HttpResponseMessage response = await httpClient.PostAsync(Response.Response.Upload_url, Videoform);
                        response.EnsureSuccessStatusCode();
                        httpClient.Dispose();
                        resp = response.Content.ReadAsStringAsync().Result;
                        if (resp.IndexOf("error") != -1)
                        {
                            MessageBox.Show($"Ошибка загрузки.\r\nВозможно некорректное имя файла\r\n{resp}");
                        }
                        else
                        {
                            GetVideo GetVid = JsonConvert.DeserializeObject<GetVideo>(resp);
                            AttachmentList.Add($"video{GetVid.Owner_id}_{GetVid.Video_id}_{MyAccount.SecretKey}");
                            NewAttachment("Images\\IconGroup23.png");
                        }
                    }
                    ShowButtons();
                }
                else
                {
                    MessageBox.Show("Слишком большой файл!");
                    CheckVideo();
                    return;
                }
            }
        }

        /// <summary>
        /// Показать вложения (кнопка).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowUploadsBtn(object sender, MouseButtonEventArgs e)
        {
            ShowUploads();
        }
        /// <summary>
        /// Добавление вложения.
        /// </summary>
        /// <param name="pic">Иконка</param>
        private void NewAttachment(string pic)
        {
            AttachmentUI NewAttachment = new AttachmentUI();
            NewAttachment.Delete.Visibility = Visibility.Visible;
            NewAttachment.Name.Text = PathToFile.Substring(PathToFile.LastIndexOf("\\") + 1);
            NewAttachment.Delete.Click += Delete_Click;
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(pic, UriKind.RelativeOrAbsolute);
            bi3.EndInit();
            NewAttachment.Pic.Stretch = Stretch.Uniform;
            NewAttachment.Pic.Source = bi3;
            AttItems.Add(NewAttachment);
            ShowUploads();
        }
        /// <summary>
        /// Показать вложения.
        /// </summary>
        private void ShowUploads()
        {
            if (ShowUpp == null)
            {
                ShowUpp = new UploadsWin();
                ShowUpp.Owner = this;
                ShowUpp.Closed += (x, y) => { ShowUpp = null; };
                ShowUpp.Show();
            }
            ShowUpp.UploadsList.Children.Clear();
            for (int i = 0; i < AttItems.Count; i++)
            {
                ShowUpp.UploadsList.Children.Add(AttItems[i]);
            }
        }
        /// <summary>
        /// Удаление элемента из списка к отпрвке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<AttachmentUI> Del = new List<AttachmentUI>();
            Del.AddRange(AttItems);
            AttItems.Clear();
            for (int n = 0; n < Del.Count; n++)
            {
                if (Del[n].Num.Text != "Clicked")
                {
                    AttItems.Add(Del[n]);
                }
            }
            ShowUploads();
        }

        #endregion

        #region Вывод сообщений и вложений в стек панель.

        /// <summary>
        /// Вывод основного сообщения.
        /// </summary>
        /// <param name="typeMessage">Тип сообщения от сервера.</param>
        private void MainMessage(string typeMessage)
        {
            Uri Reqst;
            DateTimeOffset GetDate;
            MessageUI NextMess = new MessageUI();
            if (typeMessage == "message_new")
            {
                Reqst = new Uri($"{Method}users.get?user_ids={LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.From_id}&fields=bdate&access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107");
                GetDate = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Date.ToString()));
                NextMess.Message.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Text;
                NextMess.Address.Text = "От:";
                NextMess.Margin = new Thickness(20, 0, 0, 1);
            }
            else
            {
                Reqst = new Uri($"{Method}users.get?user_ids={LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Peer_id}&fields=bdate&access_token={MyAccount.GroupToken}&secret={MyAccount.SecretKey}&v=5.107");
                GetDate = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Date.ToString()));
                NextMess.Message.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Text;
                NextMess.Address.Text = "Кому:";
                NextMess.Margin = new Thickness(0, 0, 20, 1);
            }
            string UserData = wc.DownloadString(Reqst).ToString();
            string formatDays = "yyyy/MM/dd";
            string formatHours = "HH:mm:ss";
            NextMess.Date.Text = $"{GetDate.ToString(formatDays)} {GetDate.ToString(formatHours)}";
            NextMess.Name.Text = $"{ GetDeserialized(UserData, "first_name\":\"", 0)} { GetDeserialized(UserData, "last_name\":\"", 0)}";
            MessageArray.Add(NextMess);

            MyStack.Children.Add(MessageArray[MessageArray.Count - 1]);
        }
        /// <summary>
        /// Вывод фотовложений из сообщения.
        /// </summary>
        /// <param name="typeMessage">Тип сообщения от сервера</param>
        private void PhotoAttachment(string typeMessage)
        {
            int pcount;
            if (typeMessage == "message_new")
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments.Count;
            }
            else
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments.Count;
            }
            for (int p = 0; p < pcount; p++)
            {
                string checkAttachments;
                if (typeMessage == "message_new")
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Photo.Text;
                }
                else
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Photo.Text;
                }
                if (checkAttachments != null)
                {
                    AttachmentUI NewPhoto = new AttachmentUI();
                    List<Elements_ph> photos;
                    if (typeMessage == "message_new")
                    {
                        photos = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Photo.Sizes;
                        NewPhoto.Name.Text = $"{LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Photo.Text}";
                    }
                    else
                    {
                        photos = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Photo.Sizes;
                        NewPhoto.Name.Text = $"{LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Photo.Text}";
                    }
                    int sz = 0;
                    for (int h = 0; h < photos.Count; h++)
                    {
                        if (photos[h].Type == "w")
                        {
                            NewPhoto.Link.Text = photos[h].Url;
                            sz = 6;
                        }
                        if (photos[h].Type == "z" && sz < 5)
                        {
                            NewPhoto.Link.Text = photos[h].Url;
                            sz = 5;
                        }
                        if (photos[h].Type == "y" && sz < 4)
                        {
                            NewPhoto.Link.Text = photos[h].Url;
                            sz = 4;
                        }
                        if (photos[h].Type == "x" && sz < 3)
                        {
                            NewPhoto.Link.Text = photos[h].Url;
                            sz = 3;
                        }
                        if (photos[h].Type == "m" && sz < 2)
                        {
                            NewPhoto.Link.Text = photos[h].Url;
                            sz = 2;
                        }
                        if (photos[h].Type == "s")
                        {
                            if (sz < 1)
                            {
                                NewPhoto.Link.Text = photos[h].Url;
                                sz = 1;
                            }
                            BitmapImage bi3 = new BitmapImage();
                            bi3.BeginInit();
                            bi3.UriSource = new Uri(photos[h].Url, UriKind.RelativeOrAbsolute);
                            bi3.EndInit();
                            NewPhoto.Pic.Stretch = Stretch.Uniform;
                            NewPhoto.Pic.Source = bi3;
                        }
                    }
                    NewPhoto.Margin = new Thickness(40, 0, 0, 1);
                    NewPhoto.Pic.MouseDown += new MouseButtonEventHandler(ShowImage);
                    StackItems.Add(NewPhoto);
                    MyStack.Children.Add(StackItems[StackItems.Count - 1]);
                }
            }
        }
        /// <summary>
        /// Вывод аудиовложений из сообщения.
        /// </summary>
        /// <param name="typeMessage">Тип сообщения от сервера</param>
        private void AudioAttachment(string typeMessage)
        {
            int pcount;
            if (typeMessage == "message_new")
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments.Count;
            }
            else
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments.Count;
            }
            for (int p = 0; p < pcount; p++)
            {
                string checkAttachments;
                if (typeMessage == "message_new")
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Audio.Title;
                }
                else
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Audio.Title;
                }
                if (checkAttachments != null)
                {
                    AttachmentUI NewSound = new AttachmentUI();
                    if (typeMessage == "message_new")
                    {
                        NewSound.Name.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Audio.Title;
                        NewSound.Link.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Audio.Url;
                    }
                    else
                    {
                        NewSound.Name.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Audio.Title;
                        NewSound.Link.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Audio.Url;
                    }
                    BitmapImage bi3 = new BitmapImage();
                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Images\\IconGroup131.png", UriKind.RelativeOrAbsolute);
                    bi3.EndInit();
                    NewSound.Pic.Stretch = Stretch.Uniform;
                    NewSound.Pic.Source = bi3;
                    NewSound.Pic.MouseDown += new MouseButtonEventHandler(PlayMusic);
                    NewSound.Margin = new Thickness(40, 0, 0, 1);
                    StackItems.Add(NewSound);
                    MyStack.Children.Add(StackItems[StackItems.Count - 1]);
                }
            }
        }
        /// <summary>
        /// Вывод видеовложений из сообщения.
        /// </summary>
        /// <param name="typeMessage">Тип сообщения от сервера</param>
        private void VideoAttachment(string typeMessage)
        {
            int pcount;
            if (typeMessage == "message_new")
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments.Count;
            }
            else
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments.Count;
            }
            for (int p = 0; p < pcount; p++)
            {
                string checkAttachments;
                if (typeMessage == "message_new")
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Video.Title;
                }
                else
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Video.Title;
                }
                if (checkAttachments != null)
                {
                    AttachmentUI NewVideo = new AttachmentUI();
                    BitmapImage bi3 = new BitmapImage();
                    bi3.BeginInit();
                    if (typeMessage == "message_new")
                    {
                        bi3.UriSource = new Uri(LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Video.First_frame[0].Url, UriKind.RelativeOrAbsolute);
                        NewVideo.Name.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Video.Title;
                        NewVideo.Link.Text = $"https://" +
                            $"vk.com/video{LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Video.Owner_id}_{LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Video.Id}";
                    }
                    else
                    {
                        bi3.UriSource = new Uri(LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Video.First_frame[0].Url, UriKind.RelativeOrAbsolute);
                        NewVideo.Name.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Video.Title;
                        NewVideo.Link.Text = $"https://" +
                            $"vk.com/video{LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Video.Owner_id}_{LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Video.Id}";
                    }
                    bi3.EndInit();
                    NewVideo.Pic.Stretch = Stretch.Uniform;
                    NewVideo.Pic.Source = bi3;
                    NewVideo.Pic.MouseDown += new MouseButtonEventHandler(PlayVideo);
                    NewVideo.Margin = new Thickness(40, 0, 0, 1);
                    StackItems.Add(NewVideo);
                    MyStack.Children.Add(StackItems[StackItems.Count - 1]);
                }
            }
        }
        /// <summary>
        /// Вывод файла из сообщения.
        /// </summary>
        /// <param name="typeMessage">Тип сообщения от сервера</param>
        private void DocAttachment(string typeMessage)
        {
            int pcount;
            if (typeMessage == "message_new")
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments.Count;
            }
            else
            {
                pcount = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments.Count;
            }
            for (int p = 0; p < pcount; p++)
            {
                string checkAttachments;
                if (typeMessage == "message_new")
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Doc.Title;
                }
                else
                {
                    checkAttachments = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Doc.Title;
                }
                if (checkAttachments != null)
                {
                    AttachmentUI NewDoc = new AttachmentUI();
                    if (typeMessage == "message_new")
                    {
                        NewDoc.Name.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Doc.Title;
                        NewDoc.Link.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Message.Attachments[p].Doc.Url;
                    }
                    else
                    {
                        NewDoc.Name.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Doc.Title;
                        NewDoc.Link.Text = LongPoll.Updates[LongPoll.Updates.Count - 1].Object.Attachments[p].Doc.Url;
                    }
                    BitmapImage bi3 = new BitmapImage();
                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Images\\IconGroup174.png", UriKind.RelativeOrAbsolute);
                    bi3.EndInit();
                    NewDoc.Pic.Stretch = Stretch.Uniform;
                    NewDoc.Pic.Source = bi3;
                    NewDoc.Pic.MouseDown += new MouseButtonEventHandler(DownloadDoc);
                    NewDoc.Margin = new Thickness(40, 0, 0, 1);
                    StackItems.Add(NewDoc);
                    MyStack.Children.Add(StackItems[StackItems.Count - 1]);
                }
            }
        }

        #endregion

        #region Вспомогательные методы.

        /// <summary>
        /// Очистка лога сообщений.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearLogFile(object sender, RoutedEventArgs e)
        {
            File.Delete(LogFile);
            Log.Clear();
            MyStack.Children.Clear();
        }
        /// <summary>
        /// Получение данных по ключевому слову.
        /// </summary>
        /// <param name="source">Ответ сервера</param>
        /// <param name="sub">Ключевое слово</param>
        /// <param name="stype">Тип данных</param>
        /// <returns></returns>
        public string GetDeserialized(string source, string sub, int stype)
        {
            source = source.Substring(source.IndexOf(sub) + sub.Length);
            if (stype == 0)
            {
                source = source.Substring(0, source.IndexOf("\""));//Строка.
            }
            else if (stype == 1)
            {
                source = source.Substring(0, source.IndexOf(","));//Число.
            }
            else if (stype == 2)
            {
                source = source.Substring(0, source.IndexOf("&"));//Строка.
            }
            else if (stype == 3)
            {
                source = source.Substring(0, source.IndexOf("?"));//Строка.
            }
            else if (stype == 4)
            {
                source = source.Substring(0, source.IndexOf("%2Fvideos"));//Строка.
            }
            return source;
        }
        /// <summary>
        /// Скрыть кнопки управления.
        /// </summary>
        public void HideButtons()
        {
            Busy.Visibility = Visibility.Visible;
            SendMessage.Visibility = Visibility.Hidden;
            Img.Visibility = Visibility.Hidden;
            Snd.Visibility = Visibility.Hidden;
            Vid.Visibility = Visibility.Hidden;
            Fil.Visibility = Visibility.Hidden;
            ShowAttachmentsBtn.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Показать кнопки управления.
        /// </summary>
        public void ShowButtons()
        {
            Busy.Visibility = Visibility.Hidden;
            SendMessage.Visibility = Visibility.Visible;
            Img.Visibility = Visibility.Visible;
            Snd.Visibility = Visibility.Visible;
            Vid.Visibility = Visibility.Visible;
            Fil.Visibility = Visibility.Visible;
            ShowAttachmentsBtn.Visibility = Visibility.Visible;
        }

        #endregion
    }
}
