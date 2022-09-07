using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{

    public class Root_SavedTracks_Rootobject
    {
        public string href { get; set; }
        public Root_SavedTracks_Item[] items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

    public class Root_SavedTracks_Item
    {
        public DateTime added_at { get; set; }
        public Root_SavedTracks_Track track { get; set; }
    }

    public class Root_SavedTracks_Track
    {
        public Root_SavedTracks_Album album { get; set; }
        public Root_SavedTracks_Artist1[] artists { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool _explicit { get; set; }
        public Root_SavedTracks_External_Ids external_ids { get; set; }
        public Root_SavedTracks_External_Urls2 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_local { get; set; }
        public bool is_playable { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_SavedTracks_Album
    {
        public string album_type { get; set; }
        public Root_SavedTracks_Artist[] artists { get; set; }
        public Root_SavedTracks_External_Urls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Root_SavedTracks_Image[] images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_SavedTracks_External_Urls
    {
        public string spotify { get; set; }
    }

    public class Root_SavedTracks_Artist
    {
        public Root_SavedTracks_External_Urls1 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_SavedTracks_External_Urls1
    {
        public string spotify { get; set; }
    }

    public class Root_SavedTracks_Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Root_SavedTracks_External_Ids
    {
        public string isrc { get; set; }
    }

    public class Root_SavedTracks_External_Urls2
    {
        public string spotify { get; set; }
    }

    public class Root_SavedTracks_Artist1
    {
        public Root_SavedTracks_External_Urls3 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_SavedTracks_External_Urls3
    {
        public string spotify { get; set; }
    }


}
