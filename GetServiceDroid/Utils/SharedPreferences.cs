using Android.Content;
using Android.Preferences;
using GetServiceDroid.Models;
using Newtonsoft.Json;

namespace GetServiceDroid.Utils
{
    public class SharedPreferences
    {
        private ISharedPreferences prefs;

        private const string PREFS_THEME = "prefs_theme";
        private const string PREFS_FILTRO = "prefs_filtro";
        private const string PREFS_TOKEN = "prefs_token";
        private const string PREFS_USUARIO = "prefs_usuario";

        public SharedPreferences(Context context)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        }

        public string GetTheme()
        {
            return prefs.GetString(PREFS_THEME, "");
        }

        public void SalvarTheme(string tema)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(PREFS_THEME, tema);
            editor.Apply();
        }

        public Filtro GetFiltro()
        {
            string strFiltro = prefs.GetString(PREFS_FILTRO, "");
            return JsonConvert.DeserializeObject<Filtro>(strFiltro);
        }

        public void SalvarFiltro(Filtro filtro)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(PREFS_FILTRO, JsonConvert.SerializeObject(filtro));
            editor.Apply();
        }

        public void DeletarFiltro()
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove(PREFS_FILTRO);
            editor.Commit();
        }

        public Token GetToken()
        {
            string strToken = prefs.GetString(PREFS_TOKEN, "");
            return JsonConvert.DeserializeObject<Token>(strToken);
        }

        public void SalvarToken(Token token)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(PREFS_TOKEN, JsonConvert.SerializeObject(token));
            editor.Apply();
        }

        public void DeletarToken()
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove(PREFS_TOKEN);
            editor.Commit();
        }

        public Usuario GetUsuario()
        {
            string strUsuario = prefs.GetString(PREFS_USUARIO, "");
            return JsonConvert.DeserializeObject<Usuario>(strUsuario);
        }
        
        public void SalvarUsuario(Usuario usuario)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(PREFS_USUARIO, JsonConvert.SerializeObject(usuario));
            editor.Apply();
        }

        public void DeletarUsuario()
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove(PREFS_USUARIO);
            editor.Commit();
        }

        public bool GetPerfilProfissional()
        {
            Usuario usuario = GetUsuario();

            if (usuario == null)
                return false;
            else
                return usuario.Profissional;
        }
    }
}