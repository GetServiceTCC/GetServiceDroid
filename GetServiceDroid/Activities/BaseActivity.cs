using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using GetServiceDroid.Models;
using GetServiceDroid.Services;
using GetServiceDroid.Utils;

namespace GetServiceDroid.Activities
{
    public class BaseActivity : AppCompatActivity, IServiceConnection
    {
        public const string THEME_APP_RED = "Theme_App_Red";
        public const string THEME_APP_BLUE = "Theme_App_Blue";

        protected SharedPreferences Prefs { get; private set; }

        protected ProgressDialog Progress { get; private set; }

        protected Token Token { get; set; }

        protected bool IsConnected { get; private set; }

        protected ChatService ChatService { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            RequestedOrientation = ScreenOrientation.Portrait;
            Prefs = new SharedPreferences(this);

            string theme = Prefs.GetTheme();

            if (theme == THEME_APP_BLUE)
                SetTheme(Resource.Style.Theme_App_Blue);
            else
                SetTheme(Resource.Style.Theme_App_Red);

            Progress = new ProgressDialog(this);
            Progress.SetTitle("Por Favor aguarde");
            Progress.SetMessage("Carregando...");
            Progress.SetCancelable(false);

            Token = Prefs.GetToken();

            base.OnCreate(bundle);

            if (Token != null)
            {
                Intent intent = new Intent(this, typeof(ChatService));
                BindService(intent, this, Bind.AutoCreate);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsConnected)
                UnbindService(this);
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var binder = (ChatService.ChatServiceBinder)service;
            ChatService = binder.Service;
            IsConnected = true;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
        }
    }
}