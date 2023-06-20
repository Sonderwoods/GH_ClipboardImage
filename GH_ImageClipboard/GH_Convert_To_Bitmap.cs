using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Sonderwoods;

namespace GH_ImageClipboard
{
    public class GH_Convert_To_Bitmap : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Convert_To_Bitmap class.
        /// </summary>
        public GH_Convert_To_Bitmap()
          : base("get inner bitmap", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GH_ClipboardImage(), "in", "in", "in", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("out", "out", "out", GH_ParamAccess.item);
            pManager.AddGenericParameter("out2", "out2", "out2", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_BitmapGoo goo = null;

            DA.GetData(0, ref goo);


            DA.SetData(0, goo.Bitmap);


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B851FA9C-B5B4-4C7D-8286-485EBFE4233D"); }
        }
    }
}