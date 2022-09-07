using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{
    public class ExternalUrls_PlayList
    {
        public string spotify { get; set; }
    }

    public class Image_PlayList
    {
        public int? height { get; set; }
        public string url { get; set; }
        public int? width { get; set; }
    }

    public class Owner_PlayList
    {
        public string display_name { get; set; }
        public ExternalUrls_PlayList external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Tracks_PlayList
    {
        public string href { get; set; }
        public int total { get; set; }
    }

    public class Item_PlayList
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public ExternalUrls_PlayList external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image_PlayList> images { get; set; }
        public string name { get; set; }
        public Owner_PlayList owner { get; set; }
        public object primary_color { get; set; }
        public bool @public { get; set; }
        public string snapshot_id { get; set; }
        public Tracks_PlayList tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_PlayList
    {
        public string href { get; set; }
        public List<Item_PlayList> items { get; set; }
        public int limit { get; set; }
        public object next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }


}
