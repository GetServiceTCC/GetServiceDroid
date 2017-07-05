using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Activities;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using System;

namespace GetServiceDroid.Fragments
{
    public interface ILoginFragmentListener
    {
        void OnLogin();
    }

    class LoginFragment : BaseFragment
    {
        TextInputLayout tilUsuario;
        EditText txtUsuario;

        TextInputLayout tilSenha;
        EditText txtSenha;

        Button btnLogin;
        TextView txtCriarConta;

        ILoginFragmentListener Listener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                Listener = (ILoginFragmentListener)Context;
            }
            catch
            {
                throw new Exception("Não implementa ILoginFragmentListener");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(Resource.Layout.login_fragment, container, false);

            tilUsuario = layout.FindViewById<TextInputLayout>(Resource.Id.tilUsuario);
            txtUsuario = layout.FindViewById<EditText>(Resource.Id.edtUsuario);

            tilSenha = layout.FindViewById<TextInputLayout>(Resource.Id.tilSenha);
            txtSenha = layout.FindViewById<EditText>(Resource.Id.edtSenha);

            btnLogin = layout.FindViewById<Button>(Resource.Id.btnLogin);
            txtCriarConta = layout.FindViewById<TextView>(Resource.Id.txtCriarConta);

            btnLogin.Click += (s, e) =>
            {
                View anchor = s as View;

                bool comErro = false;

                tilUsuario.Error = "";
                if (txtUsuario.Text == "")
                {
                    tilUsuario.Error = "Informe o usuário";
                    comErro = true;
                }

                tilSenha.Error = "";
                if (txtSenha.Text == "")
                {
                    tilSenha.Error = "Informe o usuário";
                    comErro = true;
                }

                if (comErro)
                    return;

                Login();
            };

            txtCriarConta.Click += (s, e) =>
            {
                Intent intent = new Intent(Context, typeof(UsuarioActivity));
                intent.PutExtra(UsuarioActivity.EXTRA_OPCAO_TELA, UsuarioActivity.OPCAO_CRIAR_CONTA);
                Context.StartActivity(intent);
            };

            return layout;
        }

        public async void Login()
        {
            Progress.Show();
            
            try
            {
                DataService ds = new DataService();
                Token token = await ds.Login(txtUsuario.Text, txtSenha.Text);
                Usuario usuario = await ds.GetUsuario(token.access_token);
                Prefs.SalvarToken(token);
                Prefs.SalvarUsuario(usuario);
                Listener.OnLogin();
            }
            catch (Exception e)
            {
                Snackbar.Make(btnLogin, e.Message, Snackbar.LengthLong).Show();
            }                

            Progress.Hide();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}