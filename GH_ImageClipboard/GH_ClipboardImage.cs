using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GH_ImageClipboard.Resources;
using GH_IO.Serialization;
using Grasshopper.Kernel.Data;

namespace Sonderwoods
{
    /// <summary>
    /// This is the "component" that shows the clipboard.
    /// </summary>
    public class GH_ClipboardImage : GH_PersistentParam<GH_BitmapGoo>
    {

        Bitmap Bitmap
        {
            get => CurrentData.get_FirstItem(false)?.Bitmap;
            set
            {
                PersistentData.Clear();
                PersistentData.Append(new GH_BitmapGoo(value));
            }
        }

        GH_Structure<GH_BitmapGoo> CurrentData => VolatileDataCount > 0 ? m_data : PersistentData;


        public GH_ClipboardImage()
            : base("ClipboardImage", "Image", "Contains an image\n\nPart of GH_ClipboardImage plugin\n(C) Mathias Sønderskov Schaltz, 2023, MIT License\n\nGithub.com/Sonderwoods", "Params", "Input")
        {
        }

        public override string ToString() => IsValid ? $"GH_ImageParam, {Bitmap.Width}x{Bitmap.Height}" : "bad GH_ImageParam";


        public override Guid ComponentGuid => new Guid("404EA6F5-C9B1-4C53-B299-3B1D3A91AF10");

        protected override GH_GetterResult Prompt_Plural(ref List<GH_BitmapGoo> values) => GH_GetterResult.cancel;


        protected override GH_GetterResult Prompt_Singular(ref GH_BitmapGoo value)
        {
            Image img = Clipboard.GetImage();

            if (img != null)
            {
                value = new GH_BitmapGoo(new Bitmap(img));
                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }

        public bool PasteFromClipboard()
        {
            Image img = Clipboard.GetImage();
            if (img != null)
            {

                GH_Structure<GH_BitmapGoo> structure = new GH_Structure<GH_BitmapGoo>();
                structure.Append(new GH_BitmapGoo(new Bitmap(img)));

                SetPersistentData(structure);
                ExpireSolution(true);

            }
            return false;
        }


        protected override void Menu_AppendManageCollection(ToolStripDropDown menu)
        {
            //empty to remove
        }

        protected override void Menu_AppendPromptMore(ToolStripDropDown menu)
        {
            //empty to remove
        }

        protected override void Menu_AppendPromptOne(ToolStripDropDown menu)
        {
            //empty to remove
        }

        public override bool Read(GH_IReader reader)
        {

            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            return base.Write(writer);

        }


        public override void CreateAttributes()
        {
            base.CreateAttributes();
            m_attributes = new GH_ClipboardImageAttributes(this);
        }



        public void SearchForCtrlCV(object sender, KeyEventArgs e)
        {

            SearchForCtrlCV(e);
        }

        public bool SearchForCtrlCV(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.V)
                {
                    if (PasteFromClipboard())
                    {
                        RecordUndoEvent("Pasted Image");
                        TriggerAutoSave();
                    }
                    return true;
                }
                else if (e.KeyCode == Keys.C)
                {
                    CopyToClipboard();
                    return true;
                }
            }
            return false;
        }

        public void CopyToClipboard()
        {
            if (IsValid)
            {
                Clipboard.SetImage(Bitmap);
            }
        }


        private void CopyToClipboard(object sender, EventArgs e)
        {
            CopyToClipboard();

        }

        private void PasteFromClipboard(object sender, EventArgs e)
        {
            PasteFromClipboard();

        }

        /// <summary>
        /// This is our right click menu in GH
        /// </summary>
        /// <param name="menu"></param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendItem(menu, "Copy image to clipboard -->", CopyToClipboard, IsValid);
            Menu_AppendItem(menu, "Paste from clipboard <--", PasteFromClipboard, Clipboard.ContainsImage());
        }


        public IGH_Goo Duplicate()
        {
            var t = new GH_ClipboardImage();
            t.m_data.Append((GH_BitmapGoo)CurrentData.get_FirstItem(false)?.Duplicate() ?? new GH_BitmapGoo());
            return (IGH_Goo)t;
        }

        public IGH_GooProxy EmitProxy()
        {
            throw new NotImplementedException();
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public bool CastTo<T>(out T target)
        {
            target = default;
            return false;

        }

        public object ScriptVariable()
        {
            return this;
        }

        protected override Bitmap Icon => Icons.CopyIcon;

        public bool IsValid
        {
            get
            {
                if (VolatileDataCount < 1) return false;
                else if (VolatileData.get_Branch(0)[0] == null) return false;
                else if (((GH_BitmapGoo)(VolatileData.get_Branch(0)[0])).IsValid == false) return false;
                else if (CurrentData.IsEmpty == true) return false;
                else if (CurrentData.get_FirstItem(false) == null) return false;
                else if (CurrentData.get_FirstItem(false).IsValid != true) return false;
                else return true;

            }
        }

        public string IsValidWhyNot => "its not valid";

        public string TypeDescription => "bitmap";


        /// <summary>
        /// This is the attributes that control  the aesthetics of the component. It looks like the panel, because its more or less stolen from it.
        /// </summary>
        public class GH_ClipboardImageAttributes : GH_ResizableAttributes<GH_ClipboardImage>
        {

            #region Properties
            public override bool AllowMessageBalloon => false;
            protected override Size MinimumSize => new Size(80, 50);
            protected override Padding SizingBorders => new Padding(6);
            #endregion

            #region Constructors
            public GH_ClipboardImageAttributes(GH_ClipboardImage owner) : base(owner)
            {
                Bounds = (RectangleF)new Rectangle(0, 0, 160, 160);
            }

            #endregion



            #region Methods

            /// <summary>
            /// Decompiled and slightly modified from the ImageSampler in grasshopper
            /// </summary>
            /// <param name="canvas"></param>
            /// <param name="graphics"></param>
            /// <param name="channel"></param>
            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                switch (channel)
                {
                    case GH_CanvasChannel.Wires:
                        if (this.Owner.SourceCount <= 0)
                            break;
                        this.RenderIncomingWires(canvas.Painter, (IEnumerable<IGH_Param>)this.Owner.Sources, this.Owner.WireDisplay);
                        break;

                    case GH_CanvasChannel.Objects:

                        GH_Viewport viewport = canvas.Viewport;
                        RectangleF bounds = this.Bounds;
                        ref RectangleF local = ref bounds;
                        int num = viewport.IsVisible(ref local, 10f) ? 1 : 0;
                        this.Bounds = bounds;
                        if (num == 0)
                            break;
                        //GH_Capsule ghCapsule = this.Owner.RuntimeMessageLevel == GH_RuntimeMessageLevel.Error || this.Owner.Bitmap == null ? GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Warning, 3, 30) : GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Hidden, 3, 30);
                        GH_Capsule ghCapsule = GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Hidden, 3, 30);
                        ghCapsule.AddInputGrip(this.InputGrip.Y);
                        ghCapsule.AddOutputGrip(this.OutputGrip.Y);
                        ghCapsule.Render(graphics, this.Selected, this.Owner.Locked, true);
                        ghCapsule.Dispose();
                        Rectangle rectangle1 = GH_Convert.ToRectangle(this.Bounds);
                        rectangle1.Inflate(-4, -4);

                        graphics.SetClip(rectangle1);
                        if (!this.Owner.IsValid)
                        {
                            int int32_3 = Convert.ToInt32(0.5 * (double)(rectangle1.Left + rectangle1.Right));
                            int int32_4 = Convert.ToInt32(0.5 * (double)(rectangle1.Top + rectangle1.Bottom));
                            Rectangle rectangle2 = Rectangle.Intersect(Rectangle.FromLTRB(int32_3 - 20, int32_4 - 20, int32_3 + 20, int32_4 + 20), rectangle1);
                        }
                        else
                        {
                            RectangleF destRect = (RectangleF)rectangle1;
                            destRect.Inflate(1f, 1f);
                            InterpolationMode interpolationMode = graphics.InterpolationMode;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Bitmap displayImage = this.Owner.Bitmap;
                            graphics.DrawImage((Image)displayImage, destRect, new RectangleF(-0.5f, -0.5f, (float)displayImage.Width, (float)displayImage.Height), GraphicsUnit.Pixel);
                            graphics.InterpolationMode = interpolationMode;
                        }
                        if (!this.Owner.IsValid)
                        {
                            Font font = GH_FontServer.NewFont(GH_FontServer.Standard, 10);
                            Brush b = new SolidBrush(Color.FromArgb(255, 250, 250, 250));
                            RectangleF destRect = (RectangleF)rectangle1;
                            destRect.Inflate(-10f, -10f);

                            graphics.DrawString("Empty", font, b, destRect);

                        }

                        //GH_GraphicsUtil.ShadowRectangle(graphics, rectangle1, 15);   // Or turn me on? If you're into shadows and stuff ;) 
                        graphics.ResetClip();
                        graphics.DrawRectangle(Pens.Black, rectangle1);
                        break;
                }
            }


            #endregion

        }

    }
}
