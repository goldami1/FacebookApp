using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;
using FacebookApp_Logic;

namespace FacebookApp_UI
{
    public partial class MainForm : Form
    {
        private PictureGallery m_PictureGalley;
        private VideoGallery m_VideoGallery;
        private IPictureGalleryEnumerator m_PictureGalleryEnumerator;
        private IVideoGalleryEnumerator m_VideoGalleryEnumerator;
        private LoginResult m_LoginResult;
        private LightTimelineFacade m_Timeline;
        private User m_LoggedInUser;
        private AppSettings m_AppSettings;
        private Thread m_ThreadVideoGallery;
        private Thread m_ThreadPictureGallery;
        private Thread m_ThreadPages;
        private Thread m_ThreadPosts;
        private Thread m_ThreadEvents;
        private Thread m_ThreadLogin;
        private Thread m_ThreadTimeline;                    
        private List<PictureBox> m_PictureBoxVideosList;
        private List<PictureBox> m_PictureBoxPhotosList;
        private ThemeForm m_ThemeForm;
        private IconMenuContainer m_MainMenu;
        private StatisticsForm m_StatisticsForm;
        private bool m_IsStatsCalculationOver = false;
        private List<Panel> m_VideoPicBoxPanelList;
        private List<Panel> m_PhotoPicBoxPanelList;
        private List<Size> m_PictureBoxPhotoSizeList;
        private Dictionary<string, VideoGallery.VideoGalleryItem> m_RecentlyAddedVideos;

        public MainForm()
        {
            InitializeComponent();
            initializeMenus();

            try
            {
                m_AppSettings = AppSettings.LoadFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            InitializeFormBasedOnAppSettings();
        }

        private void aboutMenuCallback()
        {
            MessageBox.Show(
@"Created by: Amikam
Version 3.0
Creation Date: Summer Semester 2017");
        }

        private void initializeMenus()
        {
            IconMenuContainer.MenuSize = new Size(50, 350);
            m_MainMenu = new IconMenuContainer();
            IconActionMenu statsMenu = new IconActionMenu();
            IconActionMenu ThemeMenu = new IconActionMenu();
            IconActionMenu aboutMenu = new IconActionMenu();
            ThemeMenu.IconURL = "ThemeIcon.jpeg";
            ThemeMenu.Callback = themeButton_Click;
            ThemeMenu.MenuLabel = "Theme";
            statsMenu.IconURL = "StatisticsIcon.jpeg";
            statsMenu.MenuLabel = "Statistics";
            statsMenu.Callback = startStatisticsForm;
            aboutMenu.IconURL = "AboutIcon.jpeg";
            aboutMenu.Callback = aboutMenuCallback;
            aboutMenu.MenuLabel = "About";
            m_MainMenu.AddMenuItem(statsMenu);
            m_MainMenu.AddMenuItem(ThemeMenu);
            m_MainMenu.AddMenuItem(aboutMenu);
        }

        private void setPermanentListeners()
        {
            foreach(PictureBox pictureBox in m_PictureBoxVideosList)
            {
                pictureBox.Click += new EventHandler(pictureBoxVideo_Click);
            }
        }

        private void setTheme()
        {
            Image themeImage = Image.FromFile(m_AppSettings.LastSelectedThemeURL);
            this.tabGeneral.BackgroundImage = themeImage;
            this.tabGalleries.BackgroundImage = themeImage;
            this.tabTimeline.BackgroundImage = themeImage;
        }

        private void InitializeFormBasedOnAppSettings()
        {
            pictureBoxLoginProfile.ImageLocation = "http://www.bluedot360.co.il/wp-content/uploads/2015/09/website-icons_Social-Media_01.png";
            pictureBoxLoginProfile.SizeMode = PictureBoxSizeMode.StretchImage;
            setTheme();

            if (m_AppSettings.AccessToken != null)
            {
                m_ThreadLogin = new Thread(new ThreadStart(autoLogin));
                m_ThreadLogin.Start();
            }
        }

        private void autoLogin()
        {
            CheckBoxRememberMe.Checked = true;
            buttonLogin.Enabled = false;
            m_LoginResult = FacebookService.Connect(m_AppSettings.AccessToken);
            initializeFormAfterConnection();
        }

        private void pictureBoxVideo_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;
            string pictureURL = pictureBox.ImageLocation;
            string videoURL = m_RecentlyAddedVideos[pictureURL].VideoURL;
            videoBox.URL = videoURL;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            m_LoginResult = FacebookService.Login(
                "113043199357758",
                "email",
                "public_profile",
                "user_friends",
                "user_posts",
                "user_events",
                "user_photos",
                "user_likes",
                "manage_pages",
                "publish_pages",
                "publish_actions",
                "user_checkins",
                "user_videos");

            initializeFormAfterConnection();
        }

        private void enableControls()
        {
            buttonPublishPost.Enabled = true;
            textBoxPublishPost.Enabled = true;
            buttonNext.Enabled = true;
            buttonPrev.Enabled = true;
            tabGeneral.Enabled = true;
            tabGalleries.Enabled = true;
            buttonLogout.Enabled = true;
        }

        private void initializeThreads()
        {
            m_ThreadVideoGallery = new Thread(new ThreadStart(fetchVideos));
            m_ThreadPictureGallery = new Thread(new ThreadStart(fetchPictures));
            m_ThreadPosts = new Thread(new ThreadStart(fetchPosts));
            m_ThreadPages = new Thread(new ThreadStart(fetchPages));
            m_ThreadEvents = new Thread(new ThreadStart(fetchEvents));
            m_VideoGallery = new VideoGallery();
            m_ThreadTimeline = new Thread(() => fetchTimeline(null));               

            m_ThreadTimeline.IsBackground = true;
            m_ThreadVideoGallery.IsBackground = true;
            m_ThreadPictureGallery.IsBackground = true;
            m_ThreadPosts.IsBackground = true;
            m_ThreadPages.IsBackground = true;
            m_ThreadEvents.IsBackground = true;
     
            m_ThreadPosts.Start();
            m_ThreadPages.Start();
            m_ThreadEvents.Start();
            m_ThreadVideoGallery.Start();
            m_ThreadPictureGallery.Start();
            m_ThreadTimeline.Start();
        }

        private void initializeFormAfterConnection()
        {
            if (!string.IsNullOrEmpty(m_LoginResult.AccessToken))
            {
                enableControls();
                m_LoggedInUser = m_LoginResult.LoggedInUser;
                FacebookService.s_CollectionLimit = 100;
                StringBuilder welcomeString = new StringBuilder();
                welcomeString.AppendFormat("Welcome{0}{1} {2}!", Environment.NewLine, m_LoggedInUser.LastName, m_LoggedInUser.FirstName);
                textBoxHelloMessage1.Text = welcomeString.ToString();
                textBoxHelloMessage1.SelectAll();
                textBoxHelloMessage1.SelectionColor = Color.Black;
                fetchUserInfo();
                initializePictureGalleryControls();
                initializeVideoGalleryControls();
                setPermanentListeners();
                initializeThreads();
            }
            else
            {
                MessageBox.Show(m_LoginResult.ErrorMessage);
            }

            buttonLogin.Enabled = false;
            buttonLogin.Text = "Logged In";
            buttonLogout.Enabled = true;

            // Initialize statistics
            Thread ThreadStatisticsManager = new Thread(statisticsManagerThread);
            ThreadStatisticsManager.Start();
        }

        private void statisticsManagerThread()
        {
            StatisticsManager statisticsManager = StatisticsManager.GetInstance(m_LoggedInUser);
            statisticsManager.SetPhotoStatistics();
            statisticsManager.SetPostStatistics();
            statisticsManager.SetNumberOfLikedPages();
            statisticsManager.SetLocationStatistics();
            statisticsManager.SetRatios();
            m_IsStatsCalculationOver = true;
        }

        private void initializePictureGalleryControls()
        {
            initializePictureBoxPhotoSizeList();
            m_PictureBoxPhotosList = new List<PictureBox>();
            initializePhotoPicBoxPanelList();

            PictureBox pictureBoxPhoto1 = createPictureBoxPhoto(m_PictureBoxPhotoSizeList[0]);
            PictureBox pictureBoxPhoto2 = createPictureBoxPhoto(m_PictureBoxPhotoSizeList[1]);
            PictureBox pictureBoxPhoto3 = createPictureBoxPhoto(m_PictureBoxPhotoSizeList[2]);
            PictureBox pictureBoxPhoto4 = createPictureBoxPhoto(m_PictureBoxPhotoSizeList[3]);
            PictureBox pictureBoxPhoto5 = createPictureBoxPhoto(m_PictureBoxPhotoSizeList[0]);
            PictureBox pictureBoxPhoto6 = createPictureBoxPhoto(m_PictureBoxPhotoSizeList[2]);

            m_PictureBoxPhotosList.Add(pictureBoxPhoto1);
            m_PictureBoxPhotosList.Add(pictureBoxPhoto2);
            m_PictureBoxPhotosList.Add(pictureBoxPhoto3);
            m_PictureBoxPhotosList.Add(pictureBoxPhoto4);
            m_PictureBoxPhotosList.Add(pictureBoxPhoto5);
            m_PictureBoxPhotosList.Add(pictureBoxPhoto6);

            for (int i = 0; i < m_PhotoPicBoxPanelList.Count; i++)
            {
                m_PhotoPicBoxPanelList[i].Controls.Add(createDecorablePictureBox(m_PictureBoxPhotosList[i]));
            }
        }

        private void initializePictureBoxPhotoSizeList()
        {
            m_PictureBoxPhotoSizeList = new List<Size>();
            m_PictureBoxPhotoSizeList.Add(new Size(90, 90));
            m_PictureBoxPhotoSizeList.Add(new Size(70, 70));
            m_PictureBoxPhotoSizeList.Add(new Size(55, 55));
            m_PictureBoxPhotoSizeList.Add(new Size(110, 55));
        }

        private void initializePhotoPicBoxPanelList()
        {
            m_PhotoPicBoxPanelList = new List<Panel>();
            m_PhotoPicBoxPanelList.Add(panelPictureBoxPhoto1);
            m_PhotoPicBoxPanelList.Add(panelPictureBoxPhoto2);
            m_PhotoPicBoxPanelList.Add(panelPictureBoxPhoto3);
            m_PhotoPicBoxPanelList.Add(panelPictureBoxPhoto4);
            m_PhotoPicBoxPanelList.Add(panelPictureBoxPhoto5);
            m_PhotoPicBoxPanelList.Add(panelPictureBoxPhoto6);
        }

        private PictureBox createPictureBoxPhoto(Size i_Size)
        {
            PictureBox pictureBoxPhoto = new PictureBox();
            pictureBoxPhoto.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxPhoto.Size = i_Size;
            pictureBoxPhoto.BackColor = Color.Black;
            pictureBoxPhoto.SizeMode = PictureBoxSizeMode.StretchImage;

            return pictureBoxPhoto;
        }

        private PictureBoxDecorator createDecorablePictureBox(PictureBox i_CorePictureBox)
        {
            FramedPictureBox framedPictureBox = new FramedPictureBox(i_CorePictureBox);
            ZoomPictureBox result = new ZoomPictureBox(framedPictureBox);
            return result;
        }

        private void initializeVideoPicBoxPanelList()
        {
            m_VideoPicBoxPanelList = new List<Panel>();
            m_VideoPicBoxPanelList.Add(panelVideoPicBox1);
            m_VideoPicBoxPanelList.Add(panelVideoPicBox2);
            m_VideoPicBoxPanelList.Add(panelVideoPicBox3);
            m_VideoPicBoxPanelList.Add(panelVideoPicBox4);
        }

        private void initializeVideoGalleryControls()
        {
            m_PictureBoxVideosList = new List<PictureBox>();
            initializeVideoPicBoxPanelList();

            for (int i = 0; i < m_VideoPicBoxPanelList.Count; i++)
            {
                PictureBox pictureBox = createPictureBoxVideo();

                m_PictureBoxVideosList.Add(pictureBox);
                FramedPictureBox framedPictureBox = new FramedPictureBox(pictureBox, 6);
                HighlightablePictureBox highlightablePictureBox = new HighlightablePictureBox(framedPictureBox);
                m_VideoPicBoxPanelList[i].Controls.Add(highlightablePictureBox);
            }
        }

        private PictureBox createPictureBoxVideo()
        {
            PictureBox pictureBoxVideo = new PictureBox();
            pictureBoxVideo.Enabled = false;
            pictureBoxVideo.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBoxVideo.Size = new Size(90, 68);
            pictureBoxVideo.BackColor = Color.Black;

            return pictureBoxVideo;
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            m_PictureGalleryEnumerator.MovePrev();
            fetchAskedSixLikablePicturesInGallery(m_PictureGalleryEnumerator.Current);
        }

        private void fetchAskedSixLikablePicturesInGallery(ICollection<IGalleryItem> sixPicturesList)
        {
            int i = 0;
            foreach(var pictureInList in sixPicturesList)
            {
                if (IsHandleCreated && i < sixPicturesList.Count && i < m_PictureBoxPhotosList.Count)
                {
                    m_PictureBoxPhotosList[i].Invoke(
                        new Action(() => m_PictureBoxPhotosList[i].ImageLocation = 
                        (pictureInList as PictureGallery.PictureGalleryItem).PictureURL));
                }

                i++;
            }
        }

        private void fetchAskedSixLikablePicturesInGallery(List<KeyValuePair<int, object>> sixPicturesList)
        {
            List<KeyValuePair<int, object>> collectionOf6Pictures;

            collectionOf6Pictures = sixPicturesList;

            if (collectionOf6Pictures != null)
            {
                for (int i = 0; i < m_PictureBoxPhotosList.Count; i++)
                {
                    if (IsHandleCreated && i < collectionOf6Pictures.Count)
                    {
                        m_PictureBoxPhotosList[i].Invoke(
                            new Action(() => m_PictureBoxPhotosList[i].ImageLocation = collectionOf6Pictures[i].Value as string));
                    }
                }
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            m_PictureGalleryEnumerator.MoveNext();
            fetchAskedSixLikablePicturesInGallery(m_PictureGalleryEnumerator.Current);
        }

        private void fetchTimeline(string[] i_FilterKeywords)
        {
            int top = 3;
            string current = null;

            if (panelTimeline.Controls != null && i_FilterKeywords != null)
            {
                panelTimeline.Controls.Clear();
            }

            if (m_Timeline == null)
            {
                m_Timeline = new LightTimelineFacade(m_LoggedInUser);
            }

            m_Timeline.ResetIterator();
            while ((current = m_Timeline.getCurrentPost()) != null)
            {
                TextBoxProxy postTextBoxProxy = null;

                if (current != null)
                {
                    current = current.getFilteredStringOrNullIfNoMatch(i_FilterKeywords);
                }

                if (current != null && current.Length > 0)
                {
                    postTextBoxProxy = new TextBoxProxy(current, new Size(panelTimeline.Width - 40, 40));
                }

                if (current != null && postTextBoxProxy != null)
                {
                    postTextBoxProxy.Left = 12;
                    postTextBoxProxy.Top = top;
                    if (postTextBoxProxy.RightToLeft == RightToLeft.Yes)
                    {
                        postTextBoxProxy.Left = panelTimeline.Size.Width - postTextBoxProxy.Size.Width - 22;
                    }

                    panelTimeline.Invoke(new Action(() => panelTimeline.Controls.Add(postTextBoxProxy)));
                    top = postTextBoxProxy.Bottom + 50 + postTextBoxProxy.Size.Height;
                }
            }
        }

        private void fetchPictures()
        {
            int i = 0;

            if (m_PictureGalley == null)
            {
                m_PictureGalley = new FacebookApp_Logic.PictureGallery();
            }

            GalleryElementAdder elementAdder = new GalleryElementAdder();

            foreach (Album userAlbum in m_LoggedInUser.Albums)
            {
                if (i > 25)
                {
                    break;
                }

                foreach (Photo userPhoto in userAlbum.Photos)
                {
                    if (i > 25)
                    {
                        break;
                    }

                    i++;
                    PictureGallery.PictureGalleryItem pictureGalleryItem = new PictureGallery.PictureGalleryItem();
                    pictureGalleryItem.NumOfLikes = userPhoto.LikedBy.Count;
                    pictureGalleryItem.PictureURL = userPhoto.PictureNormalURL;
                    m_PictureGalley.AddGalleryItem(pictureGalleryItem, elementAdder);
                }
            }

            m_PictureGalleryEnumerator = m_PictureGalley.GetEnumerator();
            m_PictureGalleryEnumerator.MoveNext();

            fetchAskedSixLikablePicturesInGallery(m_PictureGalleryEnumerator.Current);
        }

        private void fetchPages()
        {
            likedPagesBindingSource.DataSource = m_LoggedInUser.LikedPages;
        }

        private void fetchPosts()
        {
            postsBindingSource.DataSource = m_LoggedInUser.Posts;
        }

        private void fetchEvents()
        {
            eventsBindingSource.DataSource = m_LoggedInUser.Events;
        }

        private void fetchVideos()
        {
            GalleryElementAdder galleryElementAdder = new GalleryElementAdder();

            foreach (Post wallPost in m_LoggedInUser.WallPosts)
            {
                {
                    if(wallPost.PictureURL != null && wallPost.Source != null && wallPost.Caption != "youtube.com")
                    {
                        VideoGallery.VideoGalleryItem video = new VideoGallery.VideoGalleryItem();
                        video.PictureURL = wallPost.PictureURL;
                        video.VideoURL = wallPost.Source;
                        video.CreatedTime = wallPost.CreatedTime;

                        m_VideoGallery.AddGalleryItem(video, galleryElementAdder);
                    }
                }
            }

            foreach (Post post in m_LoggedInUser.Posts)
            {
                {
                    if (post.PictureURL != null && post.Source != null && post.Caption != "youtube.com")
                    {
                        VideoGallery.VideoGalleryItem video = new VideoGallery.VideoGalleryItem();
                        video.PictureURL = post.PictureURL;
                        video.VideoURL = post.Source;
                        video.CreatedTime = post.CreatedTime;
                        m_VideoGallery.AddGalleryItem(video, galleryElementAdder);
                    }
                }
            }

            setPictureBoxVideoControls();
            m_RecentlyAddedVideos = m_VideoGallery.GetRecentlyAddedVideos();
        }

        private void setPictureBoxVideoControls()
        {
            m_VideoGalleryEnumerator = m_VideoGallery.GetEnumerator();

            List<PictureBox>.Enumerator pictureBoxVideoEnumerator = m_PictureBoxVideosList.GetEnumerator();

            foreach (VideoGallery.VideoGalleryItem video in m_VideoGallery)
            {
                pictureBoxVideoEnumerator.MoveNext();
                pictureBoxVideoEnumerator.Current.Invoke(new Action(() => pictureBoxVideoEnumerator.Current.LoadAsync((video as VideoGallery.VideoGalleryItem).PictureURL)));
                pictureBoxVideoEnumerator.Current.Invoke(new Action(() => pictureBoxVideoEnumerator.Current.Enabled = true));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (CheckBoxRememberMe.Checked)
            {
                m_AppSettings.AccessToken = m_LoginResult.AccessToken;
            }
            else
            {
                m_AppSettings.AccessToken = null;
            }

            try
            {
                m_AppSettings.SaveToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
       
            base.OnFormClosing(e);
        }

        private void fetchUserInfo()
        {
            pictureBoxLoginProfile.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLoginProfile.Image = m_LoggedInUser.ImageLarge;
            pictureBoxLoginProfile.ImageLocation = m_LoggedInUser.PictureLargeURL;
        }

        private void buttonPublishPost_Click(object sender, EventArgs e)
        {
            Status postedStatus;
            if (!textBoxPublishPost.Text.Equals("What's on your mind?"))
            {
                postedStatus = m_LoggedInUser.PostStatus(textBoxPublishPost.Text);
                MessageBox.Show("Status Posted! ID: " + postedStatus.Id);
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            m_AppSettings.AccessToken = null;
            m_AppSettings.SaveToFile();

            if (m_LoginResult != null && m_LoginResult.AccessToken != null)
            {
                FacebookService.Logout(() => { });
            }

            this.Close();
        }

        private void ThemeForm_ThemeSelected(object sender, EventArgs e)
        {
            string themePictureURL = (e as ThemeForm.ThemeSelectedEventArgs).ThemePictureURL;
            m_AppSettings.LastSelectedThemeURL = themePictureURL;
            Image themeImage = Image.FromFile(themePictureURL);
            tabGeneral.BackgroundImage = themeImage;
            tabGalleries.BackgroundImage = themeImage;
            tabTimeline.BackgroundImage = themeImage;
        }

        internal void themeButton_Click()
        {
            if (m_ThemeForm == null)
            {
                m_ThemeForm = new ThemeForm();
                m_ThemeForm.ThemeSelected += ThemeForm_ThemeSelected;
            }

            if (!m_ThemeForm.Visible)
            {
                m_ThemeForm.StartPosition = FormStartPosition.Manual;
                m_ThemeForm.Location = new Point(buttonMore.Location.X + 300, buttonMore.Location.Y - 50);
                m_ThemeForm.ShowDialog();
            }
        }

        private void startStatisticsForm()
        {
            if (!m_IsStatsCalculationOver)
            {
                MessageBox.Show("Statistics are not ready yet!");
                return;
            }

            if (m_StatisticsForm == null)
            {
                m_StatisticsForm = new StatisticsForm();
                m_StatisticsForm.SetStatistics();
            }

            m_StatisticsForm.Show();
        }

        private void buttonMore_Click(object sender, EventArgs e)
        {
            m_MainMenu.Show();
        }

        private void buttonFilterTimeline_Click(object sender, EventArgs e)
        {
            string keywordsString = textBoxFilterTimeline.Text;
            bool isValidKeywordsString = validateKeywordsString(keywordsString);
            string[] filterKeywords = null;

            if (isValidKeywordsString == true)
            {
                filterKeywords = keywordsString.Split(';');
                fetchTimeline(filterKeywords);
            }
            else
            {
                textBoxFilterTimeline.Text = "Place keywords separated by semicolons";
                MessageBox.Show("Please follow the instructions!");
            }
        }

        private bool validateKeywordsString(string i_KeywordsString)
        {
            bool isValidString = true;
            for (int i = 0; i < i_KeywordsString.Length && isValidString; i++) 
            {
                if (!(char.IsLetter(i_KeywordsString[i]) || i_KeywordsString[i] == ';'))
                {
                    isValidString = false;
                }
            }

            return isValidString;
        }
    }
}
