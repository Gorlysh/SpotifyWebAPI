using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyProgramForms
{
    public class Root_Audio_Features
    {
        [JsonProperty("danceability")]
        public float Danceability { get; set; }

        [JsonProperty("energy")]
        public float Energy { get; set; }

        [JsonProperty("key")]
        public int Key { get; set; }

        [JsonProperty("loudness")]
        public float Loudness { get; set; }

        [JsonProperty("mode")]
        public int Mode { get; set; }

        [JsonProperty("speechiness")]
        public float Speechiness { get; set; }

        [JsonProperty("acousticness")]
        public float Acousticness { get; set; }

        [JsonProperty("instrumentalness")]
        public float Instrumentalness { get; set; }

        [JsonProperty("liveness")]
        public float Liveness { get; set; }

        [JsonProperty("valence")]
        public float Valence { get; set; }

        [JsonProperty("tempo")]
        public float Tempo { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("track_href")]
        public string Track_Href { get; set; }

        [JsonProperty("analysis_url")]
        public string Analysis_Url { get; set; }

        [JsonProperty("duration_ms")]
        public int Duration_Ms { get; set; }

        [JsonProperty("time_signature")]
        public int Time_Signature { get; set; }
    }
}
