
using Android.App;
using Android.OS;
using Android.Widget;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using System;
using System.Collections.Generic;
using SupportDialogFragment = Android.Support.V4.App.DialogFragment;

namespace GetServiceDroid.Fragments
{
    public interface IFiltroFragmentListener
    {
        void OnDialogPositiveClick(Filtro filtro);
        void OnDialogNegativeClick();
    }

    class FiltroFragment : SupportDialogFragment
    {
        public const int DIALOG_FRAGMENT = 1;
        public const int RESULT_OK = 1;
        public const int RESULT_CANCELED = 0;
        public const string EXTRA_FILTRO = "filtro";

        IFiltroFragmentListener Listener;

        ProgressDialog progress;

        Spinner filtroEstado;
        Spinner filtroCidade;
        Spinner filtroCategoria;
        Spinner filtroSubCategoria;

        List<Estado> estados;
        List<Cidade> cidades;
        List<Categoria> categorias;
        List<SubCategoria> subCategorias;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            progress = new ProgressDialog(Context);
            progress.SetTitle("Por Favor aguarde");
            progress.SetMessage("Carregando...");
            progress.SetCancelable(false);

            try
            {
                if (TargetFragment != null)
                    Listener = (IFiltroFragmentListener)TargetFragment;
                else
                    Listener = (IFiltroFragmentListener)Context;
            }
            catch
            {
                throw new Exception("Não implementa IFiltroFragmentListener");
            }
            
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var layout = Activity.LayoutInflater.Inflate(Resource.Layout.filtro_fragment, null);

            filtroEstado = layout.FindViewById<Spinner>(Resource.Id.filtroEstado);
            filtroCidade = layout.FindViewById<Spinner>(Resource.Id.filtroCidade);
            filtroCategoria = layout.FindViewById<Spinner>(Resource.Id.filtroCategoria);
            filtroSubCategoria = layout.FindViewById<Spinner>(Resource.Id.filtroSubCategoria);

            filtroEstado.ItemSelected += (s, e) =>
            {
                CarregarCidades(estados[e.Position].Id);
            };

            filtroCategoria.ItemSelected += (s, e) =>
            {
                CarregarSubCategorias(categorias[e.Position].Id);
            };

            AlertDialog.Builder builder = new AlertDialog.Builder(Context);

            builder.SetTitle("Filtrar");
            builder.SetView(layout);

            builder.SetPositiveButton("OK", (s, e) =>
            {
                if (filtroEstado.SelectedItemPosition == 0 && filtroCidade.SelectedItemPosition == 0 &&
                    filtroCategoria.SelectedItemPosition == 0 && filtroSubCategoria.SelectedItemPosition == 0)
                {
                    Listener.OnDialogNegativeClick();
                    return;
                }

                Filtro filtro = new Filtro();

                filtro.Estado = estados[filtroEstado.SelectedItemPosition].Id;
                filtro.Cidade = cidades[filtroCidade.SelectedItemPosition].Id;
                filtro.Categoria = categorias[filtroCategoria.SelectedItemPosition].Id;
                filtro.SubCategoria = subCategorias[filtroSubCategoria.SelectedItemPosition].Id;

                Listener.OnDialogPositiveClick(filtro);

            });

            builder.SetNegativeButton("Cancelar", (s, e) =>
            {
                Listener.OnDialogNegativeClick();
            });

            return builder.Create();
        }

        public override void OnResume()
        {
            base.OnResume();
            Inicializar();
        }

        public async void Inicializar()
        {
            ExibirDialogo();

            DataService ds = new DataService();

            estados = new List<Estado>();
            estados.Add(new Estado() { Id = 0, Nome = "Selecione o estado" });
            estados.AddRange(await ds.GetEstados());

            filtroEstado.Adapter = new ArrayAdapter<Estado>(Context, Android.Resource.Layout.SimpleListItem1, estados);

            categorias = new List<Categoria>();
            categorias.Add(new Categoria() { Id = 0, Descricao = "Selecione a categoria" });
            categorias.AddRange(await ds.GetCategorias());

            filtroCategoria.Adapter = new ArrayAdapter<Categoria>(Context, Android.Resource.Layout.SimpleListItem1, categorias);

            progress.Dismiss();
        }

        public async void CarregarCidades(int estadoId)
        {
            cidades = new List<Cidade>();
            cidades.Add(new Cidade() { Id = 0, Nome = "Selecione a cidade" });

            if (estadoId > 0)
            {
                ExibirDialogo();
                DataService ds = new DataService();
                cidades.AddRange(await ds.GetCidades(estadoId));
                EsconderDialogo();
            }

            filtroCidade.Enabled = cidades.Count > 1;

            filtroCidade.Adapter = new ArrayAdapter<Cidade>(Context, Android.Resource.Layout.SimpleListItem1, cidades);
        }

        public async void CarregarSubCategorias(int subCategoriaId)
        {
            subCategorias = new List<SubCategoria>();
            subCategorias.Add(new SubCategoria() { Id = 0, Descricao = "Selecione a sub categoria" });

            if (subCategoriaId > 0)
            {
                ExibirDialogo();
                DataService ds = new DataService();
                subCategorias.AddRange(await ds.GetSubCategorias(subCategoriaId));
                EsconderDialogo();
            }
            
            filtroSubCategoria.Enabled = subCategorias.Count > 1;

            filtroSubCategoria.Adapter = new ArrayAdapter<SubCategoria>(Context, Android.Resource.Layout.SimpleListItem1, subCategorias);
        }

        private void ExibirDialogo()
        {
            if (!progress.IsShowing)
                progress.Show();
        }

        private void EsconderDialogo()
        {
            if (progress.IsShowing)
                progress.Hide();
        }
    }
}