using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentFormDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "filePath";
            Bitmap image = (Bitmap)Bitmap.FromFile(path);
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.CoupledSizeFiltering = true;
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 10;
            blobCounter.MinWidth = 10;
            ContrastCorrection contrastCorrection = new ContrastCorrection(200);
            Bitmap newImage = contrastCorrection.Apply(image);

            newImage.Save("filterresult.png");
            DifferenceEdgeDetector filter2 = new DifferenceEdgeDetector();
            var myFile = filter2.Apply(newImage);

            blobCounter.ProcessImage(newImage);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // check for rectangles
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
            var i = 0;
            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners form a rectangle
                    var forms = shapeChecker.CheckPolygonSubType(cornerPoints);
                    if (forms == PolygonSubType.Rectangle)
                    {
                        i++;
                        // here i use the graphics class to draw an overlay, but you
                        // could also just use the cornerPoints list to calculate your
                        // x, y, width, height values.
                        List<System.Drawing.Point> Points = new List<System.Drawing.Point>();

                        foreach (var point in cornerPoints)
                        {
                            Points.Add(new System.Drawing.Point(point.X, point.Y));
                        }

                        Graphics g = Graphics.FromImage(image);

                        g.DrawPolygon(new Pen(Color.Red, 5.0f), Points.ToArray());

                        image.Save("result.png");

                    }
                }
            }
            var m = i;

        }
    }
}
}
