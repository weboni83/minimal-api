using System.Text.Json.Serialization;

namespace MinimalAPIsDemo.DTOs
{
    public class CommonDTO
    {
        [JsonPropertyName("plant_cd")]
        public string PLANT_CD { get; set; }
        [JsonPropertyName("common_cd")]
        public string COMMON_CD { get; set; }
        [JsonPropertyName("common_nm")]
        public string COMMON_NM { get; set; }
        [JsonPropertyName("common_part_cd")]
        public string COMMON_PART_CD { get; set; }
        [JsonPropertyName("common_part_nm")]
        public string COMMON_PART_NM { get; set; }
        [JsonPropertyName("common_seq")]
        public int? COMMON_SEQ { get; set; }
    }
}
