
using Android.App;
using Android.OS;
using GetServiceDroid.Utils;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace GetServiceDroid.Fragments
{
    public class BaseFragment : SupportFragment
    {
        protected SharedPreferences Prefs { get; private set; }

        protected ProgressDialog Progress { get; private set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Progress = new ProgressDialog(Context);
            Progress.SetTitle("Por Favor aguarde");
            Progress.SetMessage("Carregando...");
            Progress.SetCancelable(false);

            Prefs = new SharedPreferences(Context);
        }
    }
}