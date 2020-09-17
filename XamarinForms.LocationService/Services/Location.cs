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
						await Task.Delay(6000);

						var request = new GeolocationRequest(GeolocationAccuracy.Default);
						var location = await Geolocation.GetLocationAsync(request);


						if (location != null)
						{
							var message = new LocationMessage 
							{
								Latitude = location.Latitude,
								Longitude = location.Longitude
							};

							//거리 구하는 부분 35.8612573164,  128.556113458
							Xamarin.Essentials.Location defaultLocation = new Xamarin.Essentials.Location(35.8612573164, 128.556113458);
							Xamarin.Essentials.Location currentLocation = new Xamarin.Essentials.Location(location.Latitude, location.Longitude);
							double kilometers = Xamarin.Essentials.Location.CalculateDistance(defaultLocation, currentLocation, DistanceUnits.Kilometers);

							//미터 단위로 환산
							ForResult = kilometers*1000;

                            if (ForResult >= 25)
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
