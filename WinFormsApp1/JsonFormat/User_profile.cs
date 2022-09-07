using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{


    public class Root_Profile_Rootobject
    {
        public string display_name { get; set; }
        public Root_Profile_External_Urls external_urls { get; set; }
        public Root_Profile_Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Root_Profile_Image[] images { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_Profile_External_Urls
    {
        public string spotify { get; set; }
    }

    public class Root_Profile_Followers
    {
        public object href { get; set; }
        public int total { get; set; }
    }

    public class Root_Profile_Image
    {
        public object height { get; set; }
        public string url { get; set; }
        public object width { get; set; }
    }

}
