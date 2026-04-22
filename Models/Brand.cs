using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptIn)]
public partial class Brand {
    [JsonProperty]
    public int BrandId { get; set; }
    [JsonProperty]
    public string BrandName { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
