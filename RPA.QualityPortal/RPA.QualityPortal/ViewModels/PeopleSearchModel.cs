using Newtonsoft.Json;

namespace RPA.QualityPortal.ViewModels
{
    public class PeopleSearchModel
    {
        [JsonProperty("label")]
        public string Label
        {
            get
            {
                if (string.IsNullOrEmpty(Manager))
                    return Value;
                else
                    return $"{Value} (LM: {Manager})";
            }
        }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("staffNumber")]
        public string StaffNumber { get; set; }

        [JsonProperty("manager")]
        public string Manager { get; set; }
    }
}