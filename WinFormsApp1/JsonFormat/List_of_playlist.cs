using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{
    public class ExternalUrls_List_of_PlayList
    {
        public string spotify { get; set; }
    }

    public class Followers_List_of_PlayList
    {
        public object href { get; set; }
        public int total { get; set; }
    }

    public class Image_List_of_PlayList
    {
        public int? height { get; set; }
        public string url { get; set; }
        public int? width { get; set; }
    }

    public class Owner_List_of_PlayList
    {
        public string display_name { get; set; }
        public ExternalUrls_List_of_PlayList external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class AddedBy_List_of_PlayList
    {
        public ExternalUrls_List_of_PlayList external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Artist_List_of_PlayList
    {
        public ExternalUrls_List_of_PlayList external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Album_List_of_PlayList
    {
        public string album_type { get; set; }
        public List<Artist_List_of_PlayList> artists { get; set; }
        public List<string> available_markets { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image_List_of_PlayList> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalIds_List_of_PlayList
    {
        public string isrc { get; set; }
    }

    public class Track_List_of_PlayList
    {
        public Album album { get; set; }
        public List<Artist_List_of_PlayList> artists { get; set; }
        public List<string> available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool episode { get; set; }
        public bool @explicit { get; set; }
        public ExternalIds_List_of_PlayList external_ids { get; set; }
        public ExternalUrls_List_of_PlayList external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_local { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public bool track { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class VideoThumbnail_List_of_PlayList
    {
        public object url { get; set; }
    }

    public class Item_List_of_PlayList
    {
        public DateTime added_at { get; set; }
        public AddedBy_List_of_PlayList added_by { get; set; }
        public bool is_local { get; set; }
        public object primary_color { get; set; }
        public Track_List_of_PlayList track { get; set; }
        public VideoThumbnail_List_of_PlayList video_thumbnail { get; set; }
    }

    public class Tracks_List_of_PlayList
    {
        public string href { get; set; }
        public List<Item_List_of_PlayList> items { get; set; }
        public int limit { get; set; }
        public object next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

    public class Root_List_of_PlayList
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public ExternalUrls_List_of_PlayList external_urls { get; set; }
        public Followers_List_of_PlayList followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image_List_of_PlayList> images { get; set; }
        public string name { get; set; }
        public Owner_List_of_PlayList owner { get; set; }
        public object primary_color { get; set; }
        public bool @public { get; set; }
        public string snapshot_id { get; set; }
        public Tracks_List_of_PlayList tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

}
