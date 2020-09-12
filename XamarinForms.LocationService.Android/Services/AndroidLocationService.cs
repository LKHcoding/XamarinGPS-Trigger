using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;
using System.Threading;
using Xamarin.Forms;
using XamarinForms.LocationService.Services;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Droid.Helpers;
using System;

namespace XamarinForms.LocationService.Droid.Services
{
    [Service]
    public class AndroidLocationService : Service
    {
		CancellationTokenSource _cts;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10023;

        public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource();

			Notification notif = DependencyService.Get<INotification>().ReturnNotif();
			StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);

			Task.Run(() => {
				try
				{
                    for (; !_cts.IsCancellationRequested; )
                    {
						Thread.Sleep(5000);

						//현재 시간 구하기
						DateTime dateTime = DateTime.Now;

						//현재 요일 구하기
						string date = dateTime.DayOfWeek.ToString();

						if (((dateTime.Hour > 17 && dateTime.Minute > 50) || dateTime.Hour>18) && (date != "Saturday1" || date != "Sunday"))
                        {
							var locShared = new Location();
							locShared.Run(_cts.Token).Wait();

							if (locShared.ForResult > 0.02)
							{

							}
						}
						
					}
					
				}
				catch (Android.OS.OperationCanceledException)
				{
				}
				finally
				{
					if (_cts.IsCancellationRequested)
					{
						var message = new StopServiceMessage();
						Device.BeginInvokeOnMainThread(
							() => MessagingCenter.Send(message, "ServiceStopped")
						);
					}
				}
			}, _cts.Token);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			if (_cts != null)
			{
				_cts.Token.ThrowIfCancellationRequested();
				_cts.Cancel();
			}

			base.OnDestroy();
		}
	}
}