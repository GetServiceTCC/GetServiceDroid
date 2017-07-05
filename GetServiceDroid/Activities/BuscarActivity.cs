using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using GetServiceDroid.Adapters;
using GetServiceDroid.Fragments;
using GetServiceDroid.Models;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportSearchView = Android.Support.V7.Widget.SearchView;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace GetServiceDroid.Activities
{
    [Activity(Label = "Buscar")]
    public class BuscarActivity : BaseActivity, IFiltroFragmentListener
    {
        ProfissionalFragment buscaProfissional;
        ServicosFragment buscaServicos;

        Filtro filtro;
        IMenuItem ItemFiltrar;

        public const string EXTRA_QUERY = "QUERY";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.buscar);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarBuscar);

            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);

            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabsBuscar);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewPagerBuscar);
            
            TabAdapter tabAdapter = new TabAdapter(SupportFragmentManager);

            buscaProfissional = new ProfissionalFragment();
            buscaServicos = new ServicosFragment();

            tabAdapter.AddFragment(buscaProfissional, "Profissionais");
            tabAdapter.AddFragment(buscaServicos, "Serviços");

            viewPager.Adapter = tabAdapter;

            tabs.SetupWithViewPager(viewPager);

            filtro = Prefs.GetFiltro();
        }
        
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_busca, menu);

            ItemFiltrar = menu.FindItem(Resource.Id.menuFiltrar);

            if (filtro != null)
                ItemFiltrar.SetIcon(Resource.Drawable.ic_filter_remove);

            var searchView = menu.FindItem(Resource.Id.menuBuscar).ActionView.JavaCast<SupportSearchView>();

            searchView.QueryTextSubmit += (s, e) =>
            {
                buscaProfissional.CarregarProfissionais(e.Query, filtro);
                buscaServicos.CarregarServicos(e.Query, filtro);
            };
            
            string query = Intent.GetStringExtra(EXTRA_QUERY);
            searchView.SetQuery(query, true);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
                case Resource.Id.menuFiltrar:

                    if (filtro == null)
                    {
                        FiltroFragment dialogFiltro = new FiltroFragment();
                        dialogFiltro.Show(SupportFragmentManager, "dialog");
                    }
                    else
                    {
                        Prefs.DeletarFiltro();
                        filtro = null;
                        item.SetIcon(Resource.Drawable.ic_filter);
                    }
                    break;
            }

            return base.OnOptionsItemSelected(item);
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