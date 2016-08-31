using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TestMaps
{
    public class MapPage : ContentPage
    {
        public MapPage()
        {
            var map = new CustomMap();
            map.VerticalOptions = LayoutOptions.FillAndExpand;
            map.HorizontalOptions = LayoutOptions.FillAndExpand;
            Title = "Map";
            Content = map;
        }
    }
}

