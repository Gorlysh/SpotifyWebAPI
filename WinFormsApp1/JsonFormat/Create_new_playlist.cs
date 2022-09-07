using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{

    public class Root_CreatePlst_Rootobject
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public Root_CreatePlst_External_Urls external_urls { get; set; }
        public Root_CreatePlst_Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Root_CreatePlst_Image1[] images { get; set; }
        public string name { get; set; }
        public Root_CreatePlst_Owner owner { get; set; }
        public bool _public { get; set; }
        public string snapshot_id { get; set; }
        public Root_CreatePlst_Tracks tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_CreatePlst_External_Urls
    {
        public string spotify { get; set; }
    }

    public class Root_CreatePlst_Followers
    {
        public string href { get; set; }
        public int total { get; set; }
    }

    public class Root_CreatePlst_Owner
    {
        public string display_name { get; set; }
        public Root_CreatePlst_External_Urls1 external_urls { get; set; }
        public Root_CreatePlst_Followers1 followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Root_CreatePlst_Image[] images { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_CreatePlst_External_Urls1
    {
        public string spotify { get; set; }
    }

    public class Root_CreatePlst_Followers1
    {
        public string href { get; set; }
        public int total { get; set; }
    }

    public class Root_CreatePlst_Image
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Root_CreatePlst_Tracks
    {
        public string href { get; set; }
        public Root_CreatePlst_Item[] items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
    }

    public class Root_CreatePlst_Item
    {
    }

    public class Root_CreatePlst_Image1
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

}
