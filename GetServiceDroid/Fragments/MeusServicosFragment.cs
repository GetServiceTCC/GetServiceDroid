using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Activities;
using GetServiceDroid.Adapters;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using GetServiceDroid.Utils;
using System.Collections.Generic;

namespace GetServiceDroid.Fragments
{
    class MeusServicosFragment : BaseFragment
    {
        const string MENU_ATIVAR = "Ativar";
        const string MENU_INATIVAR = "Inativar";

        ProgressBar progressBar;
        RecyclerView recyclerView;
        
        List<Servico> servicos;

        Token token;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;

            token = Prefs.GetToken();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(Resource.Layout.servicos_fragment, container, false);

            progressBar = layout.FindViewById<ProgressBar>(Resource.Id.listaServicoProgress);

            recyclerView = layout.FindViewById<RecyclerView>(Resource.Id.listaServico);
            recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));
            recyclerView.HasFixedSize = true;

            recyclerView.SetItemClickListener((rv, position, view) =>
            {
                Intent intent = new Intent(Context, typeof(EditarServicoActivity));
                intent.PutExtra(EditarServicoActivity.EXTRA_OPCAO_TELA, EditarServicoActivity.OPCAO_EDITAR_SERVICO);
                intent.PutExtra(EditarServicoActivity.EXTRA_SERVICO, servicos[position].Id);
                StartActivity(intent);
            });

            return layout;
        }

        public override void OnResume()
        {
            base.OnResume();
            CarregarServicos();
        }

        public async void CarregarServicos()
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            if (token != null)
            {
                DataService ds = new DataService(token);

                servicos = await ds.GetServicosMeus();

                recyclerView.SetAdapter(new ServicoRecyclerViewAdapter(servicos));
            }

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }
    }
}