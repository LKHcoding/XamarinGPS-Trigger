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

            Task.Run(() =>
            {
                try
                {
                    for (; !_cts.IsCancellationRequested;)
                    {


                        //new AndroidNotificationManager().ScheduleNotification("HRD-Trigger", "퇴실 확인하세요!! (거리 : " + "m )");
                        //var powerManager2 = (PowerManager)GetSystemService(PowerService);
                        //var wakeLock2 = powerManager2.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, "HRD-GPS Trigger");
                        //wakeLock2.Acquire();
                        //wakeLock2.Release();

                        //서비스 도는동안 텀주기
                        Thread.Sleep(7000);


                        //현재 시간 구하기
                        DateTime dateTime = DateTime.Now;

                        //오전 오후 구하기
                        string AmPm = DateTime.Now.ToString("tt");

                        //현재 시간 구하기
                        int hour = int.Parse(DateTime.Now.ToString("HH"));

                        //현재 분 구하기
                        int minute = int.Parse(DateTime.Now.ToString("mm"));

                        //현재 무슨 요일 구하기
                        string date = dateTime.DayOfWeek.ToString();


                        //서비스 도는동안 다음날되면 다시 알림 뜰수있도록 초기화
                        if (IsNotiToday == true && whatday != DateTime.Now.Day)
                        {
                            IsNotiToday = false;
                        }

                        //if ((((hour >= 17 && minute >= 50) && (AmPm == "PM" || AmPm == "오후")) || (hour >= 18 && (AmPm == "PM" || AmPm == "오후"))) && !(date == "Saturday" || date == "Sunday"))
                        if ((((hour >= 17 && minute >= 50)) || (hour >= 18)) && !(date == "Saturday" || date == "Sunday"))
                        {
                            if (IsNotiToday == false)
                            {
                                var locShared = new Location();
                                locShared.Run(_cts.Token).Wait();

                                if (locShared.ForResult >= 30)
                                {
                                    try
                                    {
                                        whatday = DateTime.Now.Day;
                                        IsNotiToday = true;

                                        new AndroidNotificationManager().ScheduleNotification("HRD-Trigger", "퇴실 확인하세요!! ( 거리 : " + (int)(locShared.ForResult) + "m )");
                                        var powerManager = (PowerManager)GetSystemService(PowerService);
                                        var wakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, "HRD-GPS Trigger");
                                        wakeLock.Acquire();
                                        wakeLock.Release();

                                    }
                                    catch (Exception ex)
                                    {

                                        // Debugger.Break();

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