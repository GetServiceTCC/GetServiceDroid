using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using GetServiceDroid.Models.Enums;
using System;
using System.Collections.Generic;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace GetServiceDroid.Activities
{
    [Activity]
    class EditarServicoActivity : BaseActivity
    {
        private enum OpcaoTela
        {
            Inserir,
            Editar
        }

        public const string EXTRA_OPCAO_TELA = "opcao_tela";
        public const string EXTRA_SERVICO = "servico";
        public const string OPCAO_INSERIR_SERVICO = "Inserir Serviço";
        public const string OPCAO_EDITAR_SERVICO = "Editar Serviço";

        private OpcaoTela opcaoTela;

        TextInputLayout tilDescricao;
        EditText edtDescricao;

        TextInputLayout tilSobre;
        EditText edtSobre;

        CheckBox chkAtivo;

        TextInputLayout tilCategoria;
        Spinner spinCategoria;

        TextInputLayout tilSubCategoria;
        Spinner spinSubCategoria;

        TextInputLayout tilTipoValor;
        Spinner spinTipoValor;

        TextInputLayout tilValor;
        EditText edtValor;

        Button btnSalvar;

        List<Categoria> categorias;
        List<SubCategoria> subCategorias;

        bool executaSpinSubCategorias;
        bool executaSpinTipoValor;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string strOpcaoTela = Intent.GetStringExtra(EXTRA_OPCAO_TELA);

            if (strOpcaoTela == OPCAO_INSERIR_SERVICO)
            {
                Title = OPCAO_INSERIR_SERVICO;
                opcaoTela = OpcaoTela.Inserir;
            }
            else if (strOpcaoTela == OPCAO_EDITAR_SERVICO)
            {
                Title = OPCAO_EDITAR_SERVICO;
                opcaoTela = OpcaoTela.Editar;
            }
            else
            {
                Toast.MakeText(this, "Opção tela não informado", ToastLength.Long).Show();
                Finish();
                return;
            }

            SetContentView(Resource.Layout.editar_servico);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarServico);

            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);

            tilDescricao = FindViewById<TextInputLayout>(Resource.Id.tilDescricaoServico);
            edtDescricao = FindViewById<EditText>(Resource.Id.edtDescricaoServico);

            tilSobre = FindViewById<TextInputLayout>(Resource.Id.tilSobreServico);
            edtSobre = FindViewById<EditText>(Resource.Id.edtSobreServico);

            chkAtivo = FindViewById<CheckBox>(Resource.Id.chkAtivoServico);
            chkAtivo.Checked = true;

            tilCategoria = FindViewById<TextInputLayout>(Resource.Id.tilCategoriaServico);
            spinCategoria = FindViewById<Spinner>(Resource.Id.spinCategoriaServico);

            tilSubCategoria = FindViewById<TextInputLayout>(Resource.Id.tilSubCategoriaServico);
            spinSubCategoria = FindViewById<Spinner>(Resource.Id.spinSubCategoriaServico);

            tilTipoValor = FindViewById<TextInputLayout>(Resource.Id.tilTipoValorServico);
            spinTipoValor = FindViewById<Spinner>(Resource.Id.spinTipoValorServico);

            tilValor = FindViewById<TextInputLayout>(Resource.Id.tilValorServico);
            edtValor = FindViewById<EditText>(Resource.Id.edtValorServico);

            btnSalvar = FindViewById<Button>(Resource.Id.btnSalvar);

            executaSpinSubCategorias = true;
            spinCategoria.ItemSelected += (s, e) =>
            {
                if (executaSpinSubCategorias)
                    CarregarSubCategorias(categorias[e.Position].Id);
                else
                    executaSpinSubCategorias = true;
            };

            spinTipoValor.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                new string[] { "A Negociar", "Por Valor", "Por Hora", "Por Dia" });

            executaSpinTipoValor = true;
            spinTipoValor.ItemSelected += (s, e) =>
            {
                if (e.Position == (int)TipoValor.ANegociar)
                {
                    tilValor.Visibility = ViewStates.Gone;
                    if (executaSpinTipoValor)
                        edtValor.Text = "0";
                }
                else
                {
                    tilValor.Visibility = ViewStates.Visible;
                    if (executaSpinTipoValor)
                        edtValor.Text = "";
                }

                executaSpinTipoValor = true;
            };

            int servicoId = Intent.GetIntExtra(EXTRA_SERVICO, 0);

            btnSalvar.Click += (s, e) =>
            {
                if (ValidarServico())
                {
                    SalvarServico(servicoId);
                }
            };

            if (opcaoTela == OpcaoTela.Inserir)
                CarregarCategorias();
            else if (opcaoTela == OpcaoTela.Editar)
            {
                if (servicoId == 0)
                {
                    Toast.MakeText(this, "Serviço não informado", ToastLength.Long).Show();
                    Finish();
                    return;
                }

                CarregarServico(servicoId);
            }
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

        public async void CarregarCategorias()
        {
            Progress.Show();

            DataService ds = new DataService();

            categorias = new List<Categoria>();
            categorias.Add(new Categoria() { Id = 0, Descricao = "Selecione a categoria" });
            categorias.AddRange(await ds.GetCategorias());

            spinCategoria.Adapter = new ArrayAdapter<Categoria>(this, Android.Resource.Layout.SimpleListItem1, categorias);

            Progress.Hide();
        }

        public async void CarregarSubCategorias(int categoriaId)
        {
            subCategorias = new List<SubCategoria>();
            subCategorias.Add(new SubCategoria() { Id = 0, Descricao = "Selecione a sub categoria" });

            if (categoriaId > 0)
            {
                Progress.Show();
                DataService ds = new DataService();
                subCategorias.AddRange(await ds.GetSubCategorias(categoriaId));
                Progress.Hide();
            }

            spinSubCategoria.Enabled = subCategorias.Count > 1;

            spinSubCategoria.Adapter = new ArrayAdapter<SubCategoria>(this, Android.Resource.Layout.SimpleListItem1, subCategorias);
        }

        public async void CarregarServico(int servicoId)
        {
            executaSpinSubCategorias = false;

            Progress.Show();

            DataService ds = new DataService(Token);

            EditarServico servico = null;

            try
            {
                servico = await ds.GetServico(servicoId);
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                Finish();
                return;
            }

            if (servico == null)
            {
                Toast.MakeText(this, "Serviço invalido", ToastLength.Long).Show();
                Finish();
                return;
            }

            categorias = new List<Categoria>();
            categorias.Add(new Categoria() { Id = 0, Descricao = "Selecione a categoria" });
            categorias.AddRange(await ds.GetCategorias());

            spinCategoria.Adapter = new ArrayAdapter<Categoria>(this, Android.Resource.Layout.SimpleListItem1, categorias);

            for (int i = 0; i < categorias.Count; i++)
            {
                if (categorias[i].Id == servico.CategoriaId)
                {
                    spinCategoria.SetSelection(i);
                    break;
                }
            }

            subCategorias = new List<SubCategoria>();
            subCategorias.Add(new SubCategoria() { Id = 0, Descricao = "Selecione a sub categoria" });
            subCategorias.AddRange(await ds.GetSubCategorias(servico.CategoriaId));
            spinSubCategoria.Enabled = subCategorias.Count > 1;
            spinSubCategoria.Adapter = new ArrayAdapter<SubCategoria>(this, Android.Resource.Layout.SimpleListItem1, subCategorias);

            for (int i = 0; i < subCategorias.Count; i++)
            {
                if (subCategorias[i].Id == servico.SubCategoriaId)
                {
                    spinSubCategoria.SetSelection(i);
                    break;
                }
            }

            edtDescricao.Text = servico.Descricao;
            edtSobre.Text = servico.Sobre;
            chkAtivo.Checked = servico.Ativo;
            executaSpinTipoValor = false;
            spinTipoValor.SetSelection((int)servico.TipoValor);
            edtValor.Text = servico.Valor.ToString();

            Progress.Hide();
        }

        public bool ValidarServico()
        {
            bool valido = true;

            if (edtDescricao.Text == "")
            {
                tilDescricao.Error = "Informe a descrição";
                valido = false;
            }
            else
            {
                tilDescricao.Error = "";
            }

            if (edtSobre.Text == "")
            {
                tilSobre.Error = "Informe uma descrição sobre o serviço";
                valido = false;
            }
            else
            {
                tilSobre.Error = "";
            }

            if (categorias[spinCategoria.SelectedItemPosition].Id == 0)
            {
                tilCategoria.Error = "Informe a categoria";
                valido = false;
            }
            else
            {
                tilCategoria.Error = "";
            }

            if (subCategorias[spinSubCategoria.SelectedItemPosition].Id == 0)
            {
                tilSubCategoria.Error = "Informe a sub categoria";
                valido = false;
            }
            else
            {
                tilSubCategoria.Error = "";
            }

            if ((int)TipoValor.ANegociar != spinTipoValor.SelectedItemPosition)
            {
                if (edtValor.Text == "")
                {
                    tilValor.Error = "Informe o valor";
                    valido = false;
                }
                else
                {
                    double valor;
                    
                    if (double.TryParse(edtValor.Text, out valor))
                    {
                        if (valor <= 0)
                        {
                            tilValor.Error = "Informe um valor maior que zero";
                            valido = false;
                        }
                        else
                        {
                            tilValor.Error = "";
                        }
                    }
                    else
                    {
                        tilValor.Error = "valor inválido";
                        valido = false;
                    }
                }
            }
            else
            {
                tilTipoValor.Error = "";
            }

            return valido;
        }

        public async void SalvarServico(int servicoId)
        {
            Progress.Show();
            
            EditarServico servico = new EditarServico();

            servico.Id = servicoId;
            servico.Descricao = edtDescricao.Text;
            servico.Sobre = edtSobre.Text;
            servico.Ativo = chkAtivo.Checked;
            servico.SubCategoriaId = subCategorias[spinSubCategoria.SelectedItemPosition].Id;
            servico.TipoValor = (TipoValor)spinTipoValor.SelectedItemPosition;
            servico.Valor = double.Parse(edtValor.Text);

            DataService ds = new DataService(Token);

            string result = "";

            if (opcaoTela == OpcaoTela.Inserir)
                result = await ds.InserirServico(servico);
            else if (opcaoTela == OpcaoTela.Editar)
                result = await ds.EditarServico(servico);

            Progress.Hide();

            if (result != "")
            {
                Snackbar.Make(btnSalvar, result, Snackbar.LengthLong).Show();
            }
            else
            {
                Finish();
            }
        }
    }
}