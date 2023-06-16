using Grasshopper.Kernel.Types;
using System.Drawing;
using GH_IO.Serialization;
using System.IO;
using Grasshopper.Kernel;
using System.Reflection;
using System;

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

        public override string ToString() => Bitmap == null ? "Null Image" : $"Bitmap, {Bitmap.Width}x{Bitmap.Height} (Part of GH_ImageClipboard plugin)";

        public override bool CastFrom(object source)
        {
            if (source.GetType () == typeof(Bitmap)) 
            {

                this.Bitmap = (Bitmap)source;
                return true;
            }

            return false;
            //return base.CastFrom(source);
        }

        public override bool CastTo<Q>(ref Q target)
        {
       
            switch (typeof(Q).Name)
            {
                case "GrasshopperBitmapGoo":
                    return CastToShapeDiver(ref target);

                case "Bitmap": // System.Drawing.Bitmap
                    return CastToPdfPlus(ref target);

                default:
                    return false;
            }

        }

        private bool CastToPdfPlus<Q>(ref Q target)
        {
            target = (Q)(object)Bitmap;
            return true;
        }


        /// <summary>
        /// https://www.shapediver.com/blog/how-to-export-pdf-files-from-grasshopperhttps://www.shapediver.com/blog/how-to-export-pdf-files-from-grasshopper
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool CastToShapeDiver<Q>(ref Q target)
        {

            // Since we're shipping without the shapediver dlls we have to recreate it artificially using reflection

            var modules = Grasshopper.Instances.ComponentServer.Libraries;

            foreach (GH_AssemblyInfo item in modules)
            {

                if (item.Name == "BitmapComponent") // Name of the module we're looking for (shipped with ShapeDiver)
                {
                    Assembly bmpAssembly = item.Assembly;

                    foreach (Type it in bmpAssembly.GetExportedTypes())
                    {
                        object param = Activator.CreateInstance(it);

                        foreach (PropertyInfo p in it.GetRuntimeProperties())
                        {

                            if (p.Name == "PersistentData")
                            {
                                Type t = p.PropertyType;

                                object goo = Activator.CreateInstance(t);

                                Type genericType = t.GetGenericArguments()[0];

                                var bitmapGoo = Activator.CreateInstance(genericType);

                                dynamic bitmapGooDyn = (dynamic)bitmapGoo;

                                bitmapGooDyn.Value = Bitmap; // automatically setting the nonDyn version?

                                target = (Q)bitmapGoo;

                                return true;

                            }

                        }

                    }

                }
            }
            return false;
        }

        public override bool Write(GH_IWriter writer)
        {
            MemoryStream ms = new MemoryStream();
            Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imgByteArray = ms.ToArray();
            writer.SetByteArray("Image", imgByteArray);

            return base.Write(writer);

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
    }
}
