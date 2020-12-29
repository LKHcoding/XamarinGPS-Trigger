# XamarinForms GPS 위치 알림 App

- 현재 안드로이드에서만 동작
- 영남인재교육원과 약 50m정도 거리가 벌어지면 노티 및 알림 실행
- 서비스 실행상태에서는 앱이 꺼져있어도 백그라운드에서 작동
- 화면이 꺼져있어도 화면켜짐과 함께 알림 실행
- 오후 5시 50분 이후에만 동작하도록 설계되며 하루 1번, 매일 초기화 됨

# Background Location Service

XamarinForms.LocationService is an application that refreshes every 2 seconds GPS location. For years I have been working developing mobile apps that require location features; hopefully, the current project will save you some time in regarding service and location management in your Xamarin application for Android and iOS.

- Location Updates
- Location Permissions Management
- Background Processing Management

# Components used

- Xamarin.Essentials
- MessagingCenter
- CLLocationManager

![Image](https://raw.githubusercontent.com/shernandezp/XamarinForms.LocationService/master/screenshot.jpeg)

**Feel free to use the code in your project; your suggestions are more than welcome!!**
