namespace FuelInfoApp;

public static class Common {
	public static void Swap<T>(ref T arg1,ref T arg2) {
		(arg1, arg2) = (arg2, arg1);
	}

	public static decimal LatitudeDistance(decimal lat1, decimal lat2) {
		return Math.Abs(lat1 - lat2);
	}

	public static decimal LongitudeDistance(decimal lon1, decimal lon2) {
		return Math.Abs(lon1 - lon2);
	}
}