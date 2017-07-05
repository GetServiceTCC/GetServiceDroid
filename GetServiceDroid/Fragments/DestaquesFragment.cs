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
    public class DestaquesFragment : BaseFragment, IFiltroFragmentListener
    {
        ProgressBar progressBar;
        RecyclerView recyclerView;
        
        Filtro filtro;
        IMenuItem ItemFiltrar;

        List<object> destaques;

        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            HasOptionsMenu = true;
            filtro = Prefs.GetFiltro();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(Resource.Layout.destaques_fragment, container, false);

            progressBar = layout.FindViewById<ProgressBar>(Resource.Id.listaDestaqueProgress);

            recyclerView = layout.FindViewById<RecyclerView>(Resource.Id.listaDestaque);
            recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));
            recyclerView.HasFixedSize = true;

            recyclerView.SetItemClickListener((rv, position, view) =>
            {
                object destaque = destaques[position];

                if (destaque.GetType() == typeof(Profissional))
                {
                    Profissional profissional = (Profissional)destaque;

                    Intent intent = new Intent(Context, typeof(ProfissionalDetalheActivity));
                    intent.PutExtra(ProfissionalDetalheActivity.EXTRA_PROFISSIONAL, profissional.UserName);
                    StartActivity(intent);
                }
                else if (destaque.GetType() == typeof(Servico))
                {
                    Servico servico = (Servico)destaque;

                    Intent intent = new Intent(Context, typeof(ProfissionalDetalheActivity));
                    intent.PutExtra(ProfissionalDetalheActivity.EXTRA_PROFISSIONAL, servico.Profissional);
                    intent.PutExtra(ProfissionalDetalheActivity.EXTRA_SERVICO, servico.Id);

                    StartActivity(intent);
                }
            });

            CarregarDestaques();

            return layout;
        }

        public async void CarregarDestaques()
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            DataService ds = new DataService();

            var profissionais = await ds.GetProfissionaisDestaque();
            var servicos = await ds.GetServicosDestaque();

            destaques = new List<object>();

            destaques.Add("Profissionais em destaque");
            destaques.AddRange(profissionais);
            destaques.Add("Serviços em destaque");
            destaques.AddRange(servicos);

            recyclerView.SetAdapter(new DestaqueRecyclerViewAdapter(destaques));

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_destaque, menu);

            var searchView = menu.FindItem(Resource.Id.menuBuscarDestaque).ActionView.JavaCast<SupportSearchView>();

            searchView.QueryTextSubmit += (s, e) =>
            {
                Intent intent = new Intent(Context, typeof(BuscarActivity));
                intent.PutExtra(BuscarActivity.EXTRA_QUERY, e.Query);
                StartActivity(intent);
            };

            ItemFiltrar = menu.FindItem(Resource.Id.menuFiltrarDestaque);

            if (filtro != null)
                ItemFiltrar.SetIcon(Resource.Drawable.ic_filter_remove);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuFiltrarDestaque:

                    if (filtro == null)
                    {
                        FiltroFragment dialogFiltro = new FiltroFragment();
                        dialogFiltro.SetTargetFragment(this, FiltroFragment.DIALOG_FRAGMENT);
                        dialogFiltro.Show(FragmentManager, "dialog");
                    }
                    else
                    {
                        Prefs.DeletarFiltro();
                        filtro = null;
                        item.SetIcon(Resource.Drawable.ic_filter);
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
        
        public void OnDialogPositiveClick(Filtro filtro)
        {
            Prefs.SalvarFiltro(filtro);
            this.filtro = filtro;
            ItemFiltrar.SetIcon(Resource.Drawable.ic_filter_remove);
        }

        public void OnDialogNegativeClick()
        {
                                 
        }
    }
}