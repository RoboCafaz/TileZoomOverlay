using CoreGraphics;
using Foundation;
using MapKit;
using UIKit;

namespace TestMaps.iOS
{
    public class CustomTileOverlay : MKTileOverlay
    {
        public new int MinimumZ { get; set; }
        public new int MaximumZ { get; set; }
        private readonly NSCache _cache;

        public CustomTileOverlay(string urlTemplate) : base(urlTemplate)
        {
            _cache = new NSCache();
            _cache.RemoveAllObjects();
        }

        public override void LoadTileAtPath(MKTileOverlayPath path, MKTileOverlayLoadTileCompletionHandler result)
        {
            var key = path.GetKey();
            var data = _cache.ObjectForKey(key) as NSData;
            if (data != null)
            {
                result.Invoke(data, null);
                return;
            }
            if (path.Z < MinimumZ || path.Z > MaximumZ)
            {
                var cut = Normalize(path, MaximumZ);
                data = _cache.ObjectForKey(key) as NSData;
                if (data != null)
                {
                    result.Invoke(data, null);
                    return;
                }
                base.LoadTileAtPath(cut.Path, (tileData, error) =>
                {
                    if (tileData != null)
                    {
                        tileData = ProcessImage(tileData, cut);
                        _cache.SetObjectforKey(tileData, key);
                    }
                    result.Invoke(tileData, error);
                });
                return;
            }
            base.LoadTileAtPath(path, result);
        }

        private CutMapTile Normalize(MKTileOverlayPath path, int maxZoom)
        {
            var divisor = 1 << ((int)path.Z - maxZoom);
            return new CutMapTile
            {
                Path = new MKTileOverlayPath
                {
                    ContentScaleFactor = path.ContentScaleFactor,
                    X = path.X / divisor,
                    Y = path.Y / divisor,
                    Z = maxZoom
                },
                XOffset = path.X % divisor,
                YOffset = path.Y % divisor,
                Scale = divisor
            };
        }

        NSData ProcessImage(NSData tileData, CutMapTile cut)
        {
            var image = UIImage.LoadFromData(tileData);
            var cropWidth = (image.Size.Width / cut.Scale);
            var cropHeight = (image.Size.Height / cut.Scale);
            var cropX = (cropWidth * cut.XOffset);
            var cropY = (cropHeight * cut.YOffset);

            UIGraphics.BeginImageContextWithOptions(new CGSize(cropWidth, cropHeight), false, cut.Scale);
            image.Draw(new CGRect(-cropX, -cropY, image.Size.Width, image.Size.Height));
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return modifiedImage.AsPNG();
        }

        internal struct CutMapTile
        {
            public MKTileOverlayPath Path { get; set; }
            public double XOffset { get; set; }
            public double YOffset { get; set; }
            public int Scale { get; set; }
        }
    }
    public static class MKTileOverlayPathExtensions
    {
        public static NSString GetKey(this MKTileOverlayPath path)
        {
            return new NSString($"{path.Z}/{path.X}/{path.Y}");
        }
    }
}