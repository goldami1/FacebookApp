using System.Collections.Generic;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FacebookApp_Logic
{
    public class LightTimelineFacade
    {
        private User m_LoggedInUser;
        private List<string>.Enumerator m_FacadeEnumerator;
        private List<string> m_TimelineFinalCollection;
        private Dictionary<Post, Post> tempPostsCollectionHolder;

        public string CurrentMessageData { get; private set; }

        public LightTimelineFacade(User i_UserLoggedIn)
        {
            m_LoggedInUser = i_UserLoggedIn;
            m_TimelineFinalCollection = new List<string>();
            initializeAll();
        }

        private void initializeAll()
        {
            StringBuilder stringBuilderBuffer = null;
            string stringBuilderToStringBuffer = null;
            const string k_stringBuilderFormat = "{1}:{0}{2}";
            tempPostsCollectionHolder = new Dictionary<Post, Post>();

            foreach (Post post in m_LoggedInUser.Posts)
            {
                if (post != null)
                {
                    tempPostsCollectionHolder.Add(post, post);
                }
            }

            foreach (Post post in m_LoggedInUser.NewsFeed)
            {
                Post postValue;
                tempPostsCollectionHolder.TryGetValue(post, out postValue);
                if (post != null && postValue == null)
                {
                    tempPostsCollectionHolder.Add(post, post);
                }
            }

            foreach (KeyValuePair<Post, Post> post in tempPostsCollectionHolder)
            {
                string textString = StringExtensions.getFirstNotNullStringOutOfFew(post.Value.Description, post.Value.Message, post.Value.Link);
                stringBuilderBuffer = new StringBuilder();
                stringBuilderToStringBuffer = stringBuilderBuffer.AppendFormat(k_stringBuilderFormat, System.Environment.NewLine, post.Value.Type.ToString(), textString).ToString();
                m_TimelineFinalCollection.Add(stringBuilderToStringBuffer);
                stringBuilderToStringBuffer = null;
            }

            foreach (Checkin checkin in m_LoggedInUser.Checkins)
            {
                stringBuilderBuffer = new StringBuilder();
                stringBuilderToStringBuffer = stringBuilderBuffer.AppendFormat(k_stringBuilderFormat, System.Environment.NewLine, checkin.Type.ToString(), checkin.Place.Name).ToString();
                m_TimelineFinalCollection.Add(stringBuilderToStringBuffer);
                stringBuilderToStringBuffer = null;
            }

            foreach (Video video in m_LoggedInUser.Videos)
            {
                stringBuilderBuffer = new StringBuilder();
                stringBuilderToStringBuffer = stringBuilderBuffer.AppendFormat(k_stringBuilderFormat, System.Environment.NewLine, Post.eType.video.ToString(), video.URL).ToString();
                m_TimelineFinalCollection.Add(stringBuilderToStringBuffer);
                stringBuilderToStringBuffer = null;
            }

            m_FacadeEnumerator = m_TimelineFinalCollection.GetEnumerator();
            m_FacadeEnumerator.MoveNext();
        }

        public void ResetIterator()
        {
            if (m_TimelineFinalCollection != null)
            {
                m_FacadeEnumerator = m_TimelineFinalCollection.GetEnumerator();
                m_FacadeEnumerator.MoveNext();
            }
        }

        public string getCurrentPost()
        {
            CurrentMessageData = m_FacadeEnumerator.Current;
            m_FacadeEnumerator.MoveNext();

            return CurrentMessageData;
        }
    }
}
