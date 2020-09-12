using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinForms.LocationService.Droid.Services
{
    public interface INotificationManager
    {
        void Initialize();

        int ScheduleNotification(string title, string message);
    }
}