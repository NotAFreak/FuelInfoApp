using System.Text.Json.Serialization;
using FuelInfoApp.Models;
using Newtonsoft.Json;

namespace FuelInfoApp.ViewModels;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class Marker {
	[JsonProperty]
	public double latitude;
	[JsonProperty]
	public double longitude;
	[JsonProperty]
	public int stationCount = 0;
}
[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class MapViewModel : ViewModel {
	[JsonProperty]
	public List<Marker> markers = new();
	[JsonProperty]
	public List<Station> stations = new();
}