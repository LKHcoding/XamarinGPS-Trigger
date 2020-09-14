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
using System.Diagnostics;

namespace XamarinForms.LocationService.Droid.Services
{
    [Service]
    public class AndroidLocationService : Service
    {
		CancellationTokenSource _cts;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10023;
		public bool IsNotiToday = false;
		public int whatday = 0;

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
						string AmPm = DateTime.Now.ToString("tt");
						//string AmPm = "PM";


						//현재 요일 구하기
						string date = dateTime.DayOfWeek.ToString();
                        if (IsNotiToday == true && whatday != DateTime.Now.Day)
                        {
							IsNotiToday = false;
                        }

						if (((dateTime.Hour >= 5 && dateTime.Minute >= 50 && AmPm == "PM") || (dateTime.Hour>=6 && AmPm == "PM")) && (date != "Saturday" || date != "Sunday"))
                        {
                            if (IsNotiToday == false)
                            {
								var locShared = new Location();
								locShared.Run(_cts.Token).Wait();

								if (locShared.ForResult > 0.02)
								{
									try
									{
										whatday = DateTime.Now.Day;
										IsNotiToday = true;
										new AndroidNotificationManager().ScheduleNotification("HRD-Trigger", "학원을 벗어났습니다( " + (int)(locShared.ForResult * 100) + "m )");
										var powerManager = (PowerManager)GetSystemService(PowerService);
										var wakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, "HRD-GPS Trigger");
										wakeLock.Acquire();
										wakeLock.Release();

									}
									catch (Exception ex)
									{
										Debugger.Break();

									}
								}
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