using GH_ImageClipboard.Resources;
using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace GH_ImageClipboard
{
    public class GH_ImageClipboardInfo : GH_AssemblyInfo
    {
        public override string Name => "ImageClipboard";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => Icons.CopyIcon;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Allows you to copy/paste images from clipboard.";

        public override Guid Id => new Guid("72284639-fc9c-4212-a33d-fd70c60f5dbb");

        //Return a string identifying you or your company.
        public override string AuthorName => "Mathias Sønderskov Schaltz";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "...find me on linkedin";
    }
}