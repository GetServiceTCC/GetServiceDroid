using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Activities;
using GetServiceDroid.Adapters;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using GetServiceDroid.Utils;
using System.Collections.Generic;
using SupportSearchView = Android.Support.V7.Widget.SearchView;

namespace GetServiceDroid.Fragments
{
    class ContatosFragment : BaseFragment
    {
        ContatoRecyclerViewAdapter adapter = null;
        ProgressBar progressBar;
        RecyclerView recyclerView;

        List<Contato> contatos;

        Token token;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;

            token = Prefs.GetToken();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(Resource.Layout.contatos_fragment, container, false);

            progressBar = layout.FindViewById<ProgressBar>(Resource.Id.listaContatoProgress);

            recyclerView = layout.FindViewById<RecyclerView>(Resource.Id.listaContato);
            recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));
            recyclerView.HasFixedSize = true;

            recyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (token == null)
                {
                    Toast.MakeText(Context, "Erro: Usuario não logado", ToastLength.Long).Show();
                    return;
                }

                Contato contato = contatos[position];

                string userName;
                string nomeCompleto;

                if (token.userName == contato.UsuarioUserName)
                {
                    userName = contato.ContatoUserName;
                    nomeCompleto = contato.ContatoNomeCompleto;
                }
                else
                {
                    userName = contato.UsuarioUserName;
                    nomeCompleto = contato.UsuarioNomeCompleto;
                }

                Intent intent = new Intent(Context, typeof(ChatActivity));
                intent.PutExtra(ChatActivity.EXTRA_USUARIO_CONTATO, userName);
                intent.PutExtra(ChatActivity.EXTRA_USUARIO_CONTATO_NOME, nomeCompleto);
                StartActivity(intent);
            });

            return layout;
        }

        public async void CarregarContatos()
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            if (token != null)
            {
                DataService ds = new DataService(token);

                contatos = await ds.GetContatos();

                adapter = new ContatoRecyclerViewAdapter(token.userName, contatos);

                recyclerView.SetAdapter(adapter);
            }

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_contato, menu);

            var searchView = menu.FindItem(Resource.Id.menuBuscarContato).ActionView.JavaCast<SupportSearchView>();
            
            searchView.QueryTextChange += (s, e) =>
            {
                if (recyclerView.Visibility == ViewStates.Visible)
                    adapter.Filter.InvokeFilter(e.NewText);
            };
        }
    }
}