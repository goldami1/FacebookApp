using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace FacebookApp_UI
{
    public partial class StatisticsForm : Form
    {
        private const string k_StatisticsNumbersFormatString = "0.00";
        private const int k_BestFriendsControlsCount = 4;
        private const int k_FavoriteLocationsControlsCount = 4;
        private StatisticsManager m_StatisticsManager;
        private List<PictureBox> m_ClosestFriendsPictureBoxes;
        private List<Tuple<Label, Label>> m_ClosestFriendsNameLabels;
        private List<PictureBox> m_FavoriteLocationsPictureBoxes;
        private List<Label> m_FavoriteLocationsNameLables;

        public User LoggedInUser { get; private set; }

        public StatisticsForm()
        {
            InitializeComponent();
            FormClosing += this_Closing;
        }

        private void initializeControlCollections()
        {
            m_ClosestFriendsPictureBoxes = new List<PictureBox>();
            m_ClosestFriendsNameLabels = new List<Tuple<Label, Label>>();
            m_FavoriteLocationsPictureBoxes = new List<PictureBox>();
            m_FavoriteLocationsNameLables = new List<Label>();

            m_ClosestFriendsPictureBoxes.Add(pictureBoxFriend1);
            m_ClosestFriendsPictureBoxes.Add(pictureBoxFriend2);
            m_ClosestFriendsPictureBoxes.Add(pictureBoxFriend3);
            m_ClosestFriendsPictureBoxes.Add(pictureBoxFriend4);

            m_ClosestFriendsNameLabels.Add(new Tuple<Label, Label>(labelUserFirstName1, labelUserLastName1));
            m_ClosestFriendsNameLabels.Add(new Tuple<Label, Label>(labelUserFirstName2, labelUserLastName2));
            m_ClosestFriendsNameLabels.Add(new Tuple<Label, Label>(labelUserFirstName3, labelUserLastName3));
            m_ClosestFriendsNameLabels.Add(new Tuple<Label, Label>(labelUserFirstName4, labelUserLastName4));

            m_FavoriteLocationsPictureBoxes.Add(pictureBoxFavLocation1);
            m_FavoriteLocationsPictureBoxes.Add(pictureBoxFavLocation2);
            m_FavoriteLocationsPictureBoxes.Add(pictureBoxFavLocation3);
            m_FavoriteLocationsPictureBoxes.Add(pictureBoxFavLocation4);

            m_FavoriteLocationsNameLables.Add(labelFavLocation1);
            m_FavoriteLocationsNameLables.Add(labelFavLocation2);
            m_FavoriteLocationsNameLables.Add(labelFavLocation3);
            m_FavoriteLocationsNameLables.Add(labelFavLocation4);
        }

        public void SetStatistics()
        {
            m_StatisticsManager = StatisticsManager.GetInstance();

            initializeControlCollections();
            populatePhotoStatisticsControls();
            populateClosestFriendsControls();
            populateFavoriteLocationsControls();
            populateGeneralStatisticsControls();
        }

        private void this_Closing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void populatePhotoStatisticsControls()
        {
            pictureBoxMostLikedPhoto.ImageLocation = m_StatisticsManager.MostLikedPhotoURL;
            labelNumberOfLikes.Text = m_StatisticsManager.MostLikedPhotoLikeCount.ToString();
            pictureBoxMostCommentedPhoto.ImageLocation = m_StatisticsManager.MostCommentedPhotoURL;
            labelNumberOfComments.Text = m_StatisticsManager.MostCommentedPhotoCount.ToString();
        }
        
        private void populateFavoriteLocationsControls()
        {
            Dictionary<string, string>.Enumerator favLocationsDataStructEnumerator = m_StatisticsManager.FavoriteLocationsList.GetEnumerator();

            for (int i = 0; i < k_FavoriteLocationsControlsCount; i++)
            {
                bool isEndOfCollection =  !favLocationsDataStructEnumerator.MoveNext();
                
                if(isEndOfCollection)
                {
                    break;
                }

                m_FavoriteLocationsPictureBoxes[i].ImageLocation = favLocationsDataStructEnumerator.Current.Value;
               m_FavoriteLocationsNameLables[i].Text = favLocationsDataStructEnumerator.Current.Key;
            }
        }

        private void populateClosestFriendsControls()
        {
            List<StatisticsManager.UserInteractionData> UserInteractionsList = m_StatisticsManager.UserInteractionsList;
            List<StatisticsManager.UserInteractionData>.Enumerator userInteractionsEnumerator = UserInteractionsList.GetEnumerator();

            for (int i = 0; i < k_BestFriendsControlsCount; i++)
            {
                bool isEndOfCollection = !userInteractionsEnumerator.MoveNext();

                if (isEndOfCollection)
                {
                    break;
                }

                m_ClosestFriendsPictureBoxes[i].ImageLocation = userInteractionsEnumerator.Current.PictureURL;

                string[] userFullname = userInteractionsEnumerator.Current.Name.Split(' ');
                string userFirstName = userFullname[0];
                string userLastName = userFullname[1];
                m_ClosestFriendsNameLabels[i].Item1.Text = userFirstName;
                m_ClosestFriendsNameLabels[i].Item2.Text = userLastName;
            }
        }

        private void populateGeneralStatisticsControls()
        {
            labelCommentsRatioValue.Text = "% " + m_StatisticsManager.CommentsReceivedRatio.ToString(k_StatisticsNumbersFormatString);
            labelLikesPerPostAvg.Text = m_StatisticsManager.LikesPerPostAvg.ToString(k_StatisticsNumbersFormatString);
            labelLikedPagesCountBalue.Text = m_StatisticsManager.NumberOfLikedPages.ToString();
            labelFeature.Text = m_StatisticsManager.FavoriteFBFeature;
            labelCommentsPerDayCount.Text = m_StatisticsManager.CommentsPerDay.ToString(k_StatisticsNumbersFormatString);
            labelPostsWrittenCount.Text = m_StatisticsManager.PostsPerDay.ToString(k_StatisticsNumbersFormatString);
        }
    }
}
