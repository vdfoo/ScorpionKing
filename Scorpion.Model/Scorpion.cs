using Newtonsoft.Json;

namespace Scorpion.Model
{
    public class Scorpion
    {
        [JsonProperty(PropertyName = "id")]
        public string TagId { get; set; }
        public string ScientificName { get; set; }
        public string GeneralName { get; set; }
        public string FirstAid { get; set; }
        public string Deadly { get; set; }
        public string Family { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
        public string RawImage64 { get; set; }
        public string[] Url { get; set; }
        public string Confidence { get; set; }
    }
}
