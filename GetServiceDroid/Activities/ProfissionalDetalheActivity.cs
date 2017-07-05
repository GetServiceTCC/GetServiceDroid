using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Adapters;
using GetServiceDroid.DataServices;
using GetServiceDroid.Fragments;
using GetServiceDroid.Models;
using System.Collections.Generic;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using System;

namespace GetServiceDroid.Activities
{
    [Activity(Label = "Profissional")]
    class ProfissionalDetalheActivity : BaseActivity, IAddComentarioFragmentListener
    {
        public const string EXTRA_PROFISSIONAL = "extra_profissional";
        public const string EXTRA_SERVICO = "extra_servico";

        const string TAB_SERVICOS = "Serviços";
        const string TAB_COMENTARIOS = "Comentarios";

        const int TAB_INDEX_SERVICOS = 0;
        const int TAB_INDEX_COMENTARIOS = 1;

        ServicosFragment servicosFragment;
        ComentariosFragment comentariosFragment;

        RecyclerView rcProfissional;

        FloatingActionButton fabProfissional;
        
        string profissionalNome;
        int servicoSelecionado;

        public bool eContato;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            profissionalNome = Intent.GetStringExtra(EXTRA_PROFISSIONAL);

            if (string.IsNullOrEmpty(profissionalNome))
            {
                Toast.MakeText(this, "Profissional invalido", ToastLength.Long).Show();
                Finish();
                return;
            }

            SetContentView(Resource.Layout.profissional_detalhe);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarProfissionalDetalhe);

            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);

            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabsProfissionalDetalhe);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewPagerProfissionalDetalhe);

            TabAdapter tabAdapter = new TabAdapter(SupportFragmentManager);

            servicoSelecionado = 0;
            servicosFragment = new ServicosFragment((servico) =>
            {
                tabs.GetTabAt(1).Select();
                servicoSelecionado = servico.Id;
                comentariosFragment.CarregarComentarios(servicoSelecionado);
            });

            comentariosFragment = new ComentariosFragment();

            tabAdapter.AddFragment(servicosFragment, TAB_SERVICOS);
            tabAdapter.AddFragment(comentariosFragment, TAB_COMENTARIOS);

            viewPager.Adapter = tabAdapter;

            tabs.SetupWithViewPager(viewPager);

            rcProfissional = FindViewById<RecyclerView>(Resource.Id.profissionalDetalhe);
            rcProfissional.SetLayoutManager(new LinearLayoutManager(rcProfissional.Context));
            rcProfissional.HasFixedSize = true;

            fabProfissional = FindViewById<FloatingActionButton>(Resource.Id.fabProfissional);

            fabProfissional.Click += (s, e) =>
            {
                if (Token == null)
                {
                    Toast.MakeText(this, "Efetue o login", ToastLength.Long).Show();
                    return;
                }

                if (tabs.SelectedTabPosition == TAB_INDEX_SERVICOS)
                {
                    AddContato();
                }
                else if (tabs.SelectedTabPosition == TAB_INDEX_COMENTARIOS)
                {
                    if (servicoSelecionado > 0)
                    {
                        AddComentario dialogComentario = new AddComentario();
                        dialogComentario.Show(SupportFragmentManager, "dialog");
                    }
                    else
                    {
                        Toast.MakeText(this, "Selecione o serviço", ToastLength.Long).Show();
                    }
                }

            };

            tabs.TabSelected += (s, e) =>
            {
                switch (e.Tab.Position)
                {
                    case TAB_INDEX_SERVICOS:
                        if (eContato)
                            fabProfissional.Visibility = ViewStates.Gone;
                        fabProfissional.SetImageResource(Resource.Drawable.ic_person_add);
                        break;
                    case TAB_INDEX_COMENTARIOS:
                        if (eContato)
                            fabProfissional.Visibility = ViewStates.Visible;
                        fabProfissional.SetImageResource(Resource.Drawable.ic_insert_comment);
                        break;
                }
            };

            eContato = false;

            CarregarProfissional();
        }

        public async void AddContato()
        {
            Progress.Show();

            DataService ds = new DataService(Token);
            string result = await ds.AddContato(profissionalNome);

            Progress.Hide();

            if (result != "")
            {
                Snackbar.Make(fabProfissional, result, Snackbar.LengthLong).Show();
            }
            else
            {
                Snackbar.Make(fabProfissional, "Adicionado com sucesso", Snackbar.LengthLong).Show();
                fabProfissional.Visibility = ViewStates.Gone;
            }
        }

        public async void CarregarProfissional()
        {
            Progress.Show();

            if (Token?.userName == profissionalNome)
                fabProfissional.Visibility = ViewStates.Gone;

            DataService ds = new DataService(Token);

            Profissional profissional = null;

            try
            {
                profissional = await ds.GetProfissional(profissionalNome);

                if (fabProfissional.Visibility != ViewStates.Gone)
                {
                    Contato contato = await ds.GetContato(profissionalNome);

                    if (contato != null)
                    {
                        eContato = true;
                        fabProfissional.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                Finish();
                return;
            }

            if (profissional == null)
            {
                Toast.MakeText(this, "Profissional invalido", ToastLength.Long).Show();
                Finish();
                return;
            }

            List<Profissional> listProfissional = new List<Profissional>();
            listProfissional.Add(profissional);
            rcProfissional.SetAdapter(new ProfissionalRecyclerViewAdapter(listProfissional));

            servicosFragment.CarregarServicosProfissional(profissionalNome);

            Progress.Hide();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public async void AddComentario(Comentario comentario)
        {
            Progress.Show();

            DataService ds = new DataService(Token);
            comentario.ServicoId = servicoSelecionado;
            string result = await ds.AddComentario(comentario);

            Progress.Hide();

            if (result != "")
            {
                Snackbar.Make(fabProfissional, result, Snackbar.LengthLong).Show();
            }
            else
            {
                comentariosFragment.CarregarComentarios(servicoSelecionado);
            }
        }

        public void OnDialogPositiveClick(Comentario comentario)
        {
            AddComentario(comentario);
        }

        public void OnDialogNegativeClick(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Snackbar.Make(fabProfissional, msg, Snackbar.LengthLong).Show();
            }
        }
    }
}