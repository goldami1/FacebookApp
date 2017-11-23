using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace FacebookApp_Logic
{
    public class VideoGallery : IGallery, IVideoGalleryEnumerable
    {
        public class VideoGalleryItem : IGalleryItem
        {
            public string PictureURL { get; set; }

            public string VideoURL { get; set; }

            public DateTime? CreatedTime { get; set; }
        }

        private List<IGalleryItem> m_RecentlyAddedVideos;

        private List<IGalleryItem> m_recentlyAddedVideosProcessedCollection;

        public int VideosCountToFetch { get; set; } = 4;

        public List<IGalleryItem> GalleryItems { get; private set; }

        public void AddGalleryItem(IGalleryItem i_GalleryItem, GalleryElementAdder i_ElementAdder)
        {
            i_ElementAdder.AddElement(this, i_GalleryItem);
        }

        public VideoGallery()
        {
            GalleryItems = new List<IGalleryItem>();
        }

        private class VideoGalleryEnumerator : IVideoGalleryEnumerator
        {
            private List<IGalleryItem> m_VideoGalleryCollection;
            private int m_CurrentIdx;

            public IGalleryItem Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public VideoGalleryEnumerator(List<IGalleryItem> i_VideoGalleryCollection)
            {
                m_VideoGalleryCollection = i_VideoGalleryCollection;
                m_CurrentIdx = -1;
            }

            public bool MoveNext()
            {
                bool moveNextBooleanResult = true;

                moveNextBooleanResult = !(++m_CurrentIdx >= m_VideoGalleryCollection.Count);

                if (moveNextBooleanResult == true)
                {
                    Current = m_VideoGalleryCollection[m_CurrentIdx];
                }
                else
                {
                    m_CurrentIdx--;
                }

                return moveNextBooleanResult;
            }

            public bool MovePrev()
            {
                bool movePrevBooleanResult = true;

                movePrevBooleanResult = --m_CurrentIdx >= 0;

                if (movePrevBooleanResult == true)
                {
                    Current = m_VideoGalleryCollection[m_CurrentIdx];
                }
                else
                {
                    m_CurrentIdx++;
                }

                return movePrevBooleanResult;
            }

            public void Reset()
            {
                m_CurrentIdx = -1;
            }
        }

        private void SortVideosByDate()
        {
            if(m_RecentlyAddedVideos == null)
            {
                m_RecentlyAddedVideos = new List<IGalleryItem>();

                foreach (IGalleryItem video in GalleryItems)
                {
                    VideoGalleryItem videoItem = video as VideoGalleryItem;

                    if (videoItem.PictureURL != null && videoItem.VideoURL != null)
                    {
                        m_RecentlyAddedVideos.Add(videoItem);
                    }
                }

                m_RecentlyAddedVideos = m_RecentlyAddedVideos.OrderByDescending(x => (x as VideoGalleryItem).CreatedTime).ToList();
            }
        }

        public Dictionary<string, VideoGalleryItem> GetRecentlyAddedVideos()
        {
            int counter = VideosCountToFetch;

            Dictionary<string, VideoGalleryItem> recentlyAddedVideos = new Dictionary<string, VideoGalleryItem>();
            VideoGalleryItem videoToBeFound = null;
            VideoGalleryItem videoItem;

            foreach (IGalleryItem video in m_RecentlyAddedVideos)
            {
                videoItem = video as VideoGalleryItem;

                recentlyAddedVideos.TryGetValue(videoItem.PictureURL, out videoToBeFound);
                if (videoToBeFound == null)
                {
                    recentlyAddedVideos.Add(videoItem.PictureURL, videoItem);
                    counter--;
                }

                if (counter == 0)
                {
                    break;
                }
            }

            return recentlyAddedVideos;
        }

        public IVideoGalleryEnumerator GetEnumerator()
        {
            SortVideosByDate();
            m_recentlyAddedVideosProcessedCollection = new List<IGalleryItem>();
            Dictionary<string, VideoGalleryItem> recentlyAddedVids = GetRecentlyAddedVideos();
            foreach (KeyValuePair<string, VideoGalleryItem> video in recentlyAddedVids)
            {
                m_recentlyAddedVideosProcessedCollection.Add(video.Value);
            }

            return new VideoGalleryEnumerator(m_recentlyAddedVideosProcessedCollection);
        }
    }
}
