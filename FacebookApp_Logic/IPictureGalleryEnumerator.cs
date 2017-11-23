using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FacebookApp_Logic
{
    public interface IPictureGalleryEnumerator : IEnumerator
    {
        bool MovePrev();

        new ICollection<IGalleryItem> Current { get; }
    }
}
