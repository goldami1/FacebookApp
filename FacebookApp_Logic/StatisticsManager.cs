using System;
using System.Collections.Generic;
using System.Linq;
using FacebookWrapper.ObjectModel;

namespace FacebookApp_UI
{
    public class StatisticsManager
    {
        public class UserInteractionData
        {
            public string Name { get; set; }

            public string PictureURL { get; set; }

            public int NumberOfLikesGiven { get; set; }

            public int NumberOfCommentsMade { get; set; }

            public UserInteractionData(string i_UserName, string i_PictureURL)
            {
                Name = i_UserName;
                PictureURL = i_PictureURL;
                NumberOfLikesGiven = NumberOfCommentsMade = 0;
            }

            public void IncreaseCommentCount()
            {
                NumberOfCommentsMade++;
            }

            public void IncreaseLikesCount()
            {
                NumberOfLikesGiven++;
            }
        }

        private static StatisticsManager m_Instance;

        public User LoggedInUser { get; private set; }

        public string MostLikedPhotoURL { get; private set; }

        public int MostLikedPhotoLikeCount { get; private set; }

        public string MostCommentedPhotoURL { get; private set; }

        public int MostCommentedPhotoCount { get; private set; }

        public List<string> BestFriends { get; private set; } ////Holds URLs

        public int NumberOfPostsMade { get; private set; }

        public int NumberOfCommentsReceived { get; private set; }

        public int NumberOfLikesReceived { get; private set; } = 0;

        public int NumberOfLikedPages { get; private set; }

        public List<string> FavoritePages { get; private set; } ////Holds URLs

        public List<string> FavoriteLocations { get; private set; }

        public float CommentsReceivedRatio { get; private set; }

        public float LikesPerPostAvg { get; private set; }

        public float CommentsPerDay { get; private set; }                               

        public float PostsPerDay { get; private set; }                                   

        public string FavoriteFBFeature { get; private set; }

        public Dictionary<string, string> FavoriteLocationsList { get; private set; }

        public Dictionary<string, UserInteractionData> UserInteractions { get; private set; }

        private int m_MessagePostsCount = 0;

        private int m_PicturePostsCount = 0;

        private int m_CheckinPostsCount = 0;

        public List<UserInteractionData> UserInteractionsList { get; private set; }

        public static StatisticsManager GetInstance(User i_LoggedInUser)
        {
            if (m_Instance == null)
            {
                m_Instance = new StatisticsManager();
                m_Instance.LoggedInUser = i_LoggedInUser;
            }

            return m_Instance;
        }

        public static StatisticsManager GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new StatisticsManager();
            }

            return m_Instance;
        }

        private StatisticsManager()
        {
        }

        public void SetPhotoStatistics()
        {
            MostLikedPhotoLikeCount = MostCommentedPhotoCount = 0;

            foreach (Photo photo in LoggedInUser.PhotosTaggedIn)
            {
                if (photo.LikedBy.Count >= MostLikedPhotoLikeCount)
                {
                    MostLikedPhotoURL = photo.PictureNormalURL;
                    MostLikedPhotoLikeCount = photo.LikedBy.Count;
                }

                if (photo.Comments.Count >= MostCommentedPhotoCount)
                {
                    MostCommentedPhotoURL = photo.PictureNormalURL;
                    MostCommentedPhotoCount = photo.Comments.Count;
                }
            }
        }

        private void setPostsAndCommentsPerDay()                                            
        {
            Dictionary<DateTime?, string> postsDatesCollection = new Dictionary<DateTime?, string>();

            int numOfCommentsInAllFetchedPosts = 0;
            string locatedInDictionary = null;
            string located = "y";

            foreach (Post post in LoggedInUser.Posts)
            {
                numOfCommentsInAllFetchedPosts += post.Comments.Count;

                postsDatesCollection.TryGetValue(post.CreatedTime.Value.Date, out locatedInDictionary);
                if (locatedInDictionary == null)
                {
                    postsDatesCollection.Add(post.CreatedTime.Value.Date, located);
                }
            }

            CommentsPerDay = (float)numOfCommentsInAllFetchedPosts / postsDatesCollection.Count;
            PostsPerDay = (float)LoggedInUser.Posts.Count / postsDatesCollection.Count;
        }

        public void SetLocationStatistics()
        {
            if (FavoriteLocationsList == null)
            {
                FavoriteLocationsList = new Dictionary<string, string>();
            }

            foreach(Checkin checkin in LoggedInUser.Checkins)
            {
                addCheckin(checkin);
            }
        }

        private void setFavoriteFeature()
        {
            List<Tuple<string, int>> featureList = new List<Tuple<string, int>>();
            featureList.Add(new Tuple<string, int>("Checkins", m_CheckinPostsCount));
            featureList.Add(new Tuple<string, int>("Post Messages", m_MessagePostsCount));
            featureList.Add(new Tuple<string, int>("Post Photos", m_PicturePostsCount));

            featureList.OrderByDescending(x => x.Item2);
            FavoriteFBFeature = featureList[0].Item1;
        }

        private void addCheckin(Checkin i_Checkin)
        {
            Location location = i_Checkin.Place.Location;
            string isFoundCityPicURL = null;
            if (location.City != null)
            {
                FavoriteLocationsList.TryGetValue(location.City, out isFoundCityPicURL);
                if (isFoundCityPicURL == null)
                {
                    FavoriteLocationsList.Add(location.City, i_Checkin.Place.PictureURL);
                }
            }
        }

        public void SetPostStatistics()
        {
            FacebookObjectCollection<Post> postCollection = LoggedInUser.Posts;
            NumberOfPostsMade = postCollection.Count;
            NumberOfLikesReceived = NumberOfCommentsReceived = 0;
            UserInteractions = new Dictionary<string, UserInteractionData>();

            foreach (Post post in postCollection)
            {
                NumberOfLikesReceived += post.LikedBy.Count;
                NumberOfCommentsReceived += post.Comments.Count;

                if(post.PictureURL == null && post.Place == null)
                {
                    m_MessagePostsCount++;
                }

                if(post.PictureURL != null)
                {
                    m_PicturePostsCount++;
                }

                if(post.Place != null)
                {
                    m_CheckinPostsCount++;
                }

                foreach (Comment comment in post.Comments)
                {
                    string userName = comment.From.Name;
                    string userPhotoURL = comment.From.PictureNormalURL;
                    if (!UserInteractions.ContainsKey(userName))
                    {
                        UserInteractions.Add(userName, new UserInteractionData(userName, userPhotoURL));
                    }

                    UserInteractions[userName].IncreaseCommentCount();
                }
        
                foreach(User user in post.LikedBy)
                {
                    string userName = user.Name;
                    string userPhotoURL = user.PictureNormalURL;
                    if (!UserInteractions.ContainsKey(userName))
                    {
                        UserInteractions.Add(userName, new UserInteractionData(userName, userPhotoURL));
                    }

                    UserInteractions[userName].IncreaseLikesCount();
                }

                setFavoriteFeature();
                setPostsAndCommentsPerDay();
            }

            UserInteractionsList = new List<UserInteractionData>();

            foreach (var KeyValuePair in UserInteractions)
            {
                UserInteractionsList.Add(KeyValuePair.Value);
            }

            UserInteractionsList.OrderByDescending(x => x.NumberOfCommentsMade);
        }

        public void SetNumberOfLikedPages()
        {
            NumberOfLikedPages = LoggedInUser.LikedPages.Count;
        }

        public void SetRatios()
        {
            CommentsReceivedRatio = ((float)NumberOfCommentsReceived / (float)NumberOfPostsMade) * 100;
            LikesPerPostAvg = (float)NumberOfLikesReceived / (float)NumberOfPostsMade;
        }
    }
}
