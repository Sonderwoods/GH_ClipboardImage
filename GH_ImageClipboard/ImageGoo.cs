using Grasshopper.Kernel.Types;
using System.Drawing;
using GH_IO.Serialization;
using System.IO;

namespace Sonderwoods
{
    /// <summary>
    /// This is our bitmap goo, it allows us to put bitmaps into GH_Structure and visa versa.
    /// </summary>
    public class GH_BitmapGoo : GH_Goo<Bitmap>, IGH_Goo
    {

        public Bitmap Bitmap { get; set; }

        public override bool IsValid => Bitmap != null && Bitmap.Width > 0 && Bitmap.Height > 0;

        public override string TypeName => "Image";

        public override string TypeDescription => "Image";

        public GH_BitmapGoo(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public GH_BitmapGoo()
        {
            
        }

        public override IGH_Goo Duplicate()
        {
            GH_BitmapGoo g = new GH_BitmapGoo();
            g.Bitmap = (Bitmap)Bitmap.Clone();
            return g;
        }

        public override string ToString()
        {
            if (Bitmap != null)
            {

                return $"Image, {Bitmap.Width}x{Bitmap.Height}";
            }
            else
            {
                return "Null Image";
            }
        }

        public override bool Read(GH_IReader reader)
        {
            if (reader.ItemExists("Image"))
            {
                byte[] b = reader.GetByteArray("Image");
                MemoryStream ms = new MemoryStream(b);
                Bitmap = new Bitmap(ms);

            }

            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            MemoryStream ms = new MemoryStream();
            Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imgByteArray = ms.ToArray();
            writer.SetByteArray("Image", imgByteArray);

            return base.Write(writer);

        }
    }
}
