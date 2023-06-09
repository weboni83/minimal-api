﻿using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class NotificationDTO
    {
        [JsonPropertyName("sender_id")]
        public string SenderDeviceId { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; }
    }
}
