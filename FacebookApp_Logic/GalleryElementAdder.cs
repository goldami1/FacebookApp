using System.Collections.Generic;

namespace FacebookApp_Logic
{
    public class GalleryElementAdder
    {
        public void AddElement(IGallery i_Gallery, IGalleryItem i_Item)
        {
            List<IGalleryItem> galleryItems = i_Gallery.GalleryItems;
            galleryItems.Add(i_Item);
        }
   }
}
