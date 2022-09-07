using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{
    public class ExternalUrls_List_Track
    {
        public string spotify { get; set; }
    }

    public class Artist_List_Track
    {
        public ExternalUrls_List_Track external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Image_List_Track
    {
        public int? height { get; set; }
        public string url { get; set; }
        public int? width { get; set; }
    }

    public class Album_List_Track
    {
        public string album_type { get; set; }
        public List<Artist_List_Track> artists { get; set; }
        public ExternalUrls_List_Track external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image_List_Track> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalIds_List_Track
    {
        public string isrc { get; set; }
    }

    public class LinkedFrom_List_Track
    { 
        public ExternalUrls_List_Track external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Track_List_Track
    {
        public Album_List_Track album { get; set; }
        public List<Artist_List_Track> artists { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool @explicit { get; set; }
        public ExternalIds_List_Track external_ids { get; set; }
        public ExternalUrls_List_Track external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_local { get; set; }
        public bool is_playable { get; set; }
        public LinkedFrom_List_Track linked_from { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_List_Track
    {
        public List<Track_List_Track> tracks { get; set; }
    }
}
