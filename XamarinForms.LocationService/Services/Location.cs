using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;

namespace XamarinForms.LocationService.Services
{
    public class Location
    {
        bool stopping = false;
		public double ForResult;

		public Location()
		{
		}

		public async Task Run(CancellationToken token)
		{
			await Task<int>.Run(async () => {
				while (!stopping)
				{
					token.ThrowIfCancellationRequested();
					try
					{
						await Task.Delay(7000);

						var request = new GeolocationRequest(GeolocationAccuracy.High);
						var location = await Geolocation.GetLocationAsync(request);


						if (location != null)
						{
							var message = new LocationMessage 
							{
								Latitude = location.Latitude,
								Longitude = location.Longitude
							};

							//거리 구하는 부분
							Xamarin.Essentials.Location defaultLocation = new Xamarin.Essentials.Location(35.861270, 128.556045);
							Xamarin.Essentials.Location currentLocation = new Xamarin.Essentials.Location(location.Latitude, location.Longitude);
							double kilometers = Xamarin.Essentials.Location.CalculateDistance(defaultLocation, currentLocation, DistanceUnits.Kilometers);

							ForResult = kilometers;
                            if (kilometers > 0.02)
                            {
								stopping = true;
                            }



							Device.BeginInvokeOnMainThread(() =>
							{
								MessagingCenter.Send<LocationMessage>(message, "Location");
							});
						}

						
					}
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							var errormessage = new LocationErrorMessage();
							MessagingCenter.Send<LocationErrorMessage>(errormessage, "LocationError");
						});
					}
				}
			}, token);
		}
	}
}
