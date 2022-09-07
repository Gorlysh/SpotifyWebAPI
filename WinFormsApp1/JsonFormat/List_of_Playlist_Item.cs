using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{

    public class Root_List_of_PlayList_Item
    {
        public string href { get; set; }
        public List<Item_List_of_PlayList_Item> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

    public class Item_List_of_PlayList_Item
    {
        public DateTime added_at { get; set; }
        public Added_By_List_of_PlayList_Item added_by { get; set; }
        public bool is_local { get; set; }
        public object primary_color { get; set; }
        public Track_List_of_PlayList_Item track { get; set; }
        public Video_Thumbnail_List_of_PlayList_Item video_thumbnail { get; set; }
    }

    public class Added_By_List_of_PlayList_Item
    {
        public External_Urls_List_of_PlayList_Item external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class External_Urls_List_of_PlayList_Item
    {
        public string spotify { get; set; }
    }

    public class Track_List_of_PlayList_Item
    {
        public Album_List_of_PlayList_Item album { get; set; }
        public List<Artist1_List_of_PlayList_Item> artists { get; set; }
        public string[] available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool episode { get; set; }
        public bool _explicit { get; set; }
        public External_Ids_List_of_PlayList_Item external_ids { get; set; }
        public External_Urls3_List_of_PlayList_Item external_urls { get; set; }
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

    public class Album_List_of_PlayList_Item
    {
        public string album_type { get; set; }
        public Artist_List_of_PlayList_Item[] artists { get; set; }
        public string[] available_markets { get; set; }
        public External_Urls1_List_of_PlayList_Item external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Image_List_of_PlayList_Item[] images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class External_Urls1_List_of_PlayList_Item
    {
        public string spotify { get; set; }
    }

    public class Artist_List_of_PlayList_Item
    {
        public External_Urls2_List_of_PlayList_Item external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class External_Urls2_List_of_PlayList_Item
    {
        public string spotify { get; set; }
    }

    public class Image_List_of_PlayList_Item
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class External_Ids_List_of_PlayList_Item
    {
        public string isrc { get; set; }
    }

    public class External_Urls3_List_of_PlayList_Item
    {
        public string spotify { get; set; }
    }

    public class Artist1_List_of_PlayList_Item
    {
        public External_Urls4_List_of_PlayList_Item external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class External_Urls4_List_of_PlayList_Item
    {
        public string spotify { get; set; }
    }

    public class Video_Thumbnail_List_of_PlayList_Item
    {
        public object url { get; set; }
    }


}
