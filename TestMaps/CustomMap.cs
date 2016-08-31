using System;
using Xamarin.Forms.Maps;

namespace TestMaps
{
    public class CustomMap : Map
    {
        public CustomMap() : base(new MapSpan(new Position(0, 0), 30, 30))
        {
        }
    }
}

