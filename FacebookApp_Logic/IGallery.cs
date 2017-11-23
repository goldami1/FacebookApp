using System;
using System.Collections.Generic;

namespace FacebookApp_Logic
{
    public interface IGallery
    {
        List<IGalleryItem> GalleryItems { get; }

        void AddGalleryItem(IGalleryItem i_GalleryItem, GalleryElementAdder i_ElementAdder);
    }
}
