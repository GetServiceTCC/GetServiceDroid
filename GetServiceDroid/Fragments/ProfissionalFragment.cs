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
    class ProfissionalFragment: BaseFragment
    {
        ProgressBar progressBar;
        RecyclerView recyclerView;

        List<Profissional> profissionais;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(Resource.Layout.profissional_fragment, container, false);

            progressBar = layout.FindViewById<ProgressBar>(Resource.Id.listaProfissionalProgress);

            recyclerView = layout.FindViewById<RecyclerView>(Resource.Id.listaProfissional);
            recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));
            recyclerView.HasFixedSize = true;

            recyclerView.SetItemClickListener((rv, position, view) =>
            {
                Profissional profissional = profissionais[position];

                Intent intent = new Intent(Context, typeof(ProfissionalDetalheActivity));
                intent.PutExtra(ProfissionalDetalheActivity.EXTRA_PROFISSIONAL, profissional.UserName);
                StartActivity(intent);
            });

            return layout;
        }

        public async void CarregarProfissionais(string query, Filtro filtro)
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            DataService ds = new DataService();

            profissionais = await ds.GetProfissionais(query, filtro);
           
            recyclerView.SetAdapter(new ProfissionalRecyclerViewAdapter(profissionais));

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }
    }
}