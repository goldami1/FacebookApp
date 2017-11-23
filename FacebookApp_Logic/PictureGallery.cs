using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FacebookApp_Logic
{
    public class PictureGallery : IGallery, IPictureGalleryEnumerable
    {
        private const int k_NumberOfPicturesPerCollection = 6;
        private List<IGalleryItem>[] m_PicturesCollectionList;
        private int m_NumberOfPictureCollections = 0;
        private bool m_IsFetchedIntoSixCollections = false;

        public List<IGalleryItem> GalleryItems { get; private set; }

        public void AddGalleryItem(IGalleryItem i_GalleryItem, GalleryElementAdder i_ElementAdder)
        {
            i_ElementAdder.AddElement(this, i_GalleryItem);
        }

        public class PictureGalleryItem : IGalleryItem
        {
            public int NumOfLikes { get; set; }

            public string PictureURL { get; set; }
        }

        public PictureGallery()
        {
            GalleryItems = new List<IGalleryItem>();
        }

        private class PictureGalleryEnumerator : IPictureGalleryEnumerator
        {
            private ICollection<IGalleryItem>[] m_PictureCollection;
            private int m_CurrentIdx;

            public ICollection<IGalleryItem> Current { get; private set; }

            ICollection<IGalleryItem> IPictureGalleryEnumerator.Current
            {
                get { return Current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private void initializeWithCtor()
            {
                m_CurrentIdx = -1;
                Current = null;
            }

            public PictureGalleryEnumerator(ICollection<IGalleryItem>[] i_PictureCollection)
            {
                m_PictureCollection = i_PictureCollection;
                initializeWithCtor();
            }

            public bool MovePrev()
            {
                bool movePrevBooleanResult = true;

                movePrevBooleanResult = --m_CurrentIdx >= 0;

                if (movePrevBooleanResult == true)
                {
                    Current = m_PictureCollection[m_CurrentIdx];
                }
                else
                {
                    m_CurrentIdx++;
                }

                return movePrevBooleanResult;
            }

            public bool MoveNext()
            {
                bool moveNextBooleanResult = true;

                moveNextBooleanResult = !(++m_CurrentIdx >= m_PictureCollection.Length);

                if (moveNextBooleanResult == true)
                {
                    Current = m_PictureCollection[m_CurrentIdx];
                }
                else
                {
                    m_CurrentIdx--;
                }

                return moveNextBooleanResult;
            }

            public void Reset()
            {
                m_CurrentIdx = -1;
            }
        }

        private List<IGalleryItem>[] createPictureGroupsList()
        {
            m_PicturesCollectionList = new List<IGalleryItem>[(GalleryItems.Count / k_NumberOfPicturesPerCollection) + 1];
            List<IGalleryItem> listOf6LikesSortedPictures = new List<IGalleryItem>();
            int numberOfElements = 0;
            int listID = 0;

            GalleryItems = GalleryItems.OrderByDescending(x => (x as PictureGalleryItem).NumOfLikes).ToList();

            m_PicturesCollectionList[listID] = listOf6LikesSortedPictures;

            foreach (IGalleryItem picture in GalleryItems)
            {
                if (numberOfElements == k_NumberOfPicturesPerCollection)
                {
                    numberOfElements = 0;
                    listID++;
                    listOf6LikesSortedPictures = new List<IGalleryItem>();
                    m_PicturesCollectionList[listID] = listOf6LikesSortedPictures;
                }

                listOf6LikesSortedPictures.Add(picture);
                numberOfElements++;
            }

            return m_PicturesCollectionList;
        }

        public IPictureGalleryEnumerator GetEnumerator()
        {
            if (m_IsFetchedIntoSixCollections == false)
            {
                m_PicturesCollectionList = createPictureGroupsList();
                m_NumberOfPictureCollections = m_PicturesCollectionList.Length - 1;
                m_IsFetchedIntoSixCollections = true;
            }

            return new PictureGalleryEnumerator(m_PicturesCollectionList);
        }
    }
}
