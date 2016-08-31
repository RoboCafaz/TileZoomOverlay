using System;
using System.Collections.Generic;
using MapKit;
using TestMaps;
using TestMaps.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace TestMaps.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                CreateMap();
            }
        }

        void CreateMap()
        {
            var mapView = Control as MKMapView;
            mapView.OverlayRenderer = OverlayRenderer;

            var tileOverlay = new CustomTileOverlay("https://wexmothership-qa.azurewebsites.net/Content/Floors/Images/2/{z}/{x}/{y}.png")
            {
                MinimumZ = 0,
                MaximumZ = 5,
                CanReplaceMapContent = true
            };
            mapView.AddOverlay(tileOverlay);

            var polygonOverlay = MKPolygon.FromCoordinates(new[]{
                new CoreLocation.CLLocationCoordinate2D(0,0.5),
                new CoreLocation.CLLocationCoordinate2D(0,11),
                new CoreLocation.CLLocationCoordinate2D(10,10),
                new CoreLocation.CLLocationCoordinate2D(7,6),
                new CoreLocation.CLLocationCoordinate2D(5,5)
            });
            mapView.AddOverlay(polygonOverlay);
        }

        MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            var tileOverlay = overlay as MKTileOverlay;
            if (tileOverlay != null)
            {
                return new MKTileOverlayRenderer(tileOverlay);
            }

            var polygonOverlay = overlay as MKPolygon;
            if (polygonOverlay != null)
            {
                return new MKPolygonRenderer(polygonOverlay)
                {
                    StrokeColor = UIColor.Cyan,
                    FillColor = UIColor.Blue,
                    LineWidth = 10,
                    Alpha = 0.4f
                };
            }
            return null;
        }
    }
}

