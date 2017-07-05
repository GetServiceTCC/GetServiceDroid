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
using System;
using System.Collections.Generic;

namespace GetServiceDroid.Fragments
{
    class ServicosFragment : BaseFragment
    {
        ProgressBar progressBar;
        RecyclerView recyclerView;

        List<Servico> servicos;

        Action<Servico> clickListener;

        public ServicosFragment(Action<Servico> clickListener = null)
        {
            this.clickListener = clickListener;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
                Servico servico = servicos[position];

                if (clickListener == null)
                {
                    Intent intent = new Intent(Context, typeof(ProfissionalDetalheActivity));
                    intent.PutExtra(ProfissionalDetalheActivity.EXTRA_PROFISSIONAL, servico.Profissional);
                    intent.PutExtra(ProfissionalDetalheActivity.EXTRA_SERVICO, servico.Id);

                    StartActivity(intent);
                }
                else
                {
                    clickListener(servico);
                }                
            });

            return layout;
        }

        public async void CarregarServicos(string query, Filtro filtro)
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            DataService ds = new DataService();

            servicos = await ds.GetServicos(query, filtro);

            recyclerView.SetAdapter(new ServicoRecyclerViewAdapter(servicos));

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }

        public async void CarregarServicosProfissional(string profissional)
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            DataService ds = new DataService();

            servicos = await ds.GetServicosProfissional(profissional);

            recyclerView.SetAdapter(new ServicoRecyclerViewAdapter(servicos));

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }
    }
}