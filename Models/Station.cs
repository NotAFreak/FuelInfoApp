using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptIn)]
public partial class Station {
    [JsonProperty]
    public int StationId { get; set; }
    [JsonProperty]
    public decimal Latitude { get; set; }
    [JsonProperty]
    public decimal Longitude { get; set; }
    [JsonProperty]
    public string Name { get; set; } = null!;
    [JsonProperty]
    public int BrandId { get; set; }
    [JsonProperty]
    public string? Address { get; set; }
    [JsonProperty]
    public string? Suburb { get; set; }
    [JsonProperty]
    public string State { get; set; } = null!;
    [JsonProperty]
    public int? Postcode { get; set; }
    [JsonProperty]
    public virtual Brand Brand { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<StationFuelChange> StationFuelChanges { get; set; } = new List<StationFuelChange>();
}
