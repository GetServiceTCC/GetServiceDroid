
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using System;
using System.Collections.Generic;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace GetServiceDroid.Activities
{
    [Activity]
    public class UsuarioActivity : BaseActivity
    {
        private enum OpcaoTela
        {
            Registrar,
            Alterar,
            AlterarSenha
        }

        public const string EXTRA_OPCAO_TELA = "opcao_tela";
        public const string OPCAO_CRIAR_CONTA = "Criar Conta";
        public const string OPCAO_ALTERAR_CONTA = "Alterar Conta";
        public const string OPCAO_ALTERAR_SENHA = "Alterar Senha";

        private OpcaoTela opcaoTela;

        TextInputLayout tilUsuario;
        EditText edtUsuario;

        TextInputLayout tilNomeCompleto;
        EditText edtNomeCompleto;

        TextInputLayout tilStatus;
        EditText edtStatus;

        TextInputLayout tilEstado;
        Spinner spinEstado;

        TextInputLayout tilCidade;
        Spinner spinCidade;

        TextInputLayout tilEndereco;
        EditText edtEndereco;

        CheckBox chkProfissional;

        TextInputLayout tilSenhaAntiga;
        EditText edtSenhaAntiga;

        TextInputLayout tilSenha;
        EditText edtSenha;

        TextInputLayout tilSenhaConfirma;
        EditText edtSenhaConfirma;

        Button btnSalvar;

        List<Estado> estados;
        List<Cidade> cidades;

        bool executaSpinEstado;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string strOpcaoTela = Intent.GetStringExtra(EXTRA_OPCAO_TELA);
            
            if (strOpcaoTela == OPCAO_CRIAR_CONTA)
            {
                Title = OPCAO_CRIAR_CONTA;
                opcaoTela = OpcaoTela.Registrar;
            }
            else if(strOpcaoTela == OPCAO_ALTERAR_CONTA)
            {
                Title = OPCAO_ALTERAR_CONTA;
                opcaoTela = OpcaoTela.Alterar;
            }
            else if (strOpcaoTela == OPCAO_ALTERAR_SENHA)
            {
                Title = OPCAO_ALTERAR_SENHA;
                opcaoTela = OpcaoTela.AlterarSenha;
            }
            else
            {
                Toast.MakeText(this, "Opção tela não informado", ToastLength.Long).Show();
                Finish();
                return;
            }
            
            SetContentView(Resource.Layout.usuario);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarUsuario);

            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);

            tilUsuario = FindViewById<TextInputLayout>(Resource.Id.tilUsuario);
            edtUsuario = FindViewById<EditText>(Resource.Id.edtUsuario);

            tilNomeCompleto = FindViewById<TextInputLayout>(Resource.Id.tilNomeCompleto);
            edtNomeCompleto = FindViewById<EditText>(Resource.Id.edtNomeCompleto);

            tilStatus = FindViewById<TextInputLayout>(Resource.Id.tilStatus);
            edtStatus = FindViewById<EditText>(Resource.Id.edtStatus);

            tilEstado = FindViewById<TextInputLayout>(Resource.Id.tilEstado);
            spinEstado = FindViewById<Spinner>(Resource.Id.spinEstado);

            tilCidade = FindViewById<TextInputLayout>(Resource.Id.tilCidade);
            spinCidade = FindViewById<Spinner>(Resource.Id.spinCidade);

            tilEndereco = FindViewById<TextInputLayout>(Resource.Id.tilEndereco);
            edtEndereco = FindViewById<EditText>(Resource.Id.edtEndereco);

            chkProfissional = FindViewById<CheckBox>(Resource.Id.chkProfissional);

            tilSenhaAntiga = FindViewById<TextInputLayout>(Resource.Id.tilSenhaAntiga);
            edtSenhaAntiga = FindViewById<EditText>(Resource.Id.edtSenhaAntiga);

            tilSenha = FindViewById<TextInputLayout>(Resource.Id.tilSenha);
            edtSenha = FindViewById<EditText>(Resource.Id.edtSenha);

            tilSenhaConfirma = FindViewById<TextInputLayout>(Resource.Id.tilSenhaConfirma);
            edtSenhaConfirma = FindViewById<EditText>(Resource.Id.edtSenhaConfirma);

            btnSalvar = FindViewById<Button>(Resource.Id.btnSalvar);

            if (opcaoTela == OpcaoTela.Registrar)
            {
                tilSenhaAntiga.Visibility = ViewStates.Gone;
            }
            else if (opcaoTela == OpcaoTela.Alterar)
            {
                tilUsuario.Visibility = ViewStates.Gone;
                tilSenhaAntiga.Visibility = ViewStates.Gone;
                tilSenha.Visibility = ViewStates.Gone;
                tilSenhaConfirma.Visibility = ViewStates.Gone;
            }
            else
            {
                tilUsuario.Visibility = ViewStates.Gone;
                tilNomeCompleto.Visibility = ViewStates.Gone;
                tilStatus.Visibility = ViewStates.Gone;
                tilEstado.Visibility = ViewStates.Gone;
                tilCidade.Visibility = ViewStates.Gone;
                tilEndereco.Visibility = ViewStates.Gone;
                chkProfissional.Visibility = ViewStates.Gone;
            }

            executaSpinEstado = true;
            spinEstado.ItemSelected += (s, e) =>
            {
                if (executaSpinEstado)
                    CarregarCidades(estados[e.Position].Id);

                executaSpinEstado = true;
            };
            
            btnSalvar.Click += (s, e) =>
            {
                if (ValidarUsuario())
                {
                    if (opcaoTela == OpcaoTela.Registrar)
                        RegistarUsuario();
                    else if (opcaoTela == OpcaoTela.Alterar)
                        AlterarUsuario();
                    else if (opcaoTela == OpcaoTela.AlterarSenha)
                        AlterarSenha();
                }
            };

            if (opcaoTela == OpcaoTela.Registrar)
                CarregarEstados();
            else if (opcaoTela == OpcaoTela.Alterar)
                CarregarUsuario();
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

        public async void CarregarEstados()
        {
            Progress.Show();

            DataService ds = new DataService();

            estados = new List<Estado>();
            estados.Add(new Estado() { Id = 0, Nome = "Selecione o estado" });
            estados.AddRange(await ds.GetEstados());

            spinEstado.Adapter = new ArrayAdapter<Estado>(this, Android.Resource.Layout.SimpleListItem1, estados);

            Progress.Hide();
        }

        public async void CarregarCidades(int estadoId)
        {
            cidades = new List<Cidade>();
            cidades.Add(new Cidade() { Id = 0, Nome = "Selecione a cidade" });

            if (estadoId > 0)
            {
                Progress.Show();
                DataService ds = new DataService();
                cidades.AddRange(await ds.GetCidades(estadoId));
                Progress.Hide();
            }

            spinCidade.Enabled = cidades.Count > 1;

            spinCidade.Adapter = new ArrayAdapter<Cidade>(this, Android.Resource.Layout.SimpleListItem1, cidades);
        }

        public async void CarregarUsuario()
        {
            executaSpinEstado = false;

            Progress.Show();
            
            DataService ds = new DataService(Token);

            Usuario usuario = null;

            try
            {
                usuario = await ds.GetUsuario();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                Finish();
                return;
            }

            if (usuario == null)
            {
                Toast.MakeText(this, "Usuário invalido", ToastLength.Long).Show();
                Finish();
                return;
            }

            estados = new List<Estado>();
            estados.Add(new Estado() { Id = 0, Nome = "Selecione o estado" });
            estados.AddRange(await ds.GetEstados());

            spinEstado.Adapter = new ArrayAdapter<Estado>(this, Android.Resource.Layout.SimpleListItem1, estados);
            
            for (int i = 0; i < estados.Count; i++)
            {
                if (estados[i].Id == usuario.EstadoId)
                {
                    spinEstado.SetSelection(i);
                    break;
                }
            }
            
            cidades = new List<Cidade>();
            cidades.Add(new Cidade() { Id = 0, Nome = "Selecione a cidade" });
            cidades.AddRange(await ds.GetCidades(usuario.EstadoId));
            spinCidade.Enabled = cidades.Count > 1;
            spinCidade.Adapter = new ArrayAdapter<Cidade>(this, Android.Resource.Layout.SimpleListItem1, cidades);

            for (int i = 0; i < cidades.Count; i++)
            {
                if (cidades[i].Id == usuario.CidadeId)
                {
                    spinCidade.SetSelection(i);
                    break;
                }
            }

            if (usuario.Profissional)
                chkProfissional.Enabled = false;

            edtUsuario.Text = usuario.NomeUsuario;
            edtNomeCompleto.Text = usuario.NomeCompleto;
            edtStatus.Text = usuario.Status;
            edtEndereco.Text = usuario.Endereco;
            chkProfissional.Checked = usuario.Profissional;

            Progress.Hide();
        }

        public bool ValidarUsuario()
        {
            bool valido = true;

            if (opcaoTela == OpcaoTela.Registrar || opcaoTela == OpcaoTela.Alterar)
            {
                if (edtUsuario.Text == "")
                {
                    tilUsuario.Error = "Informe o usuário";
                    valido = false;
                }
                else
                {
                    tilUsuario.Error = "";
                }

                if (edtNomeCompleto.Text == "")
                {
                    tilNomeCompleto.Error = "Informe o nome completo";
                    valido = false;
                }
                else
                {
                    tilNomeCompleto.Error = "";
                }

                if (estados[spinEstado.SelectedItemPosition].Id == 0)
                {
                    tilEstado.Error = "Informe o estado";
                    valido = false;
                }
                else
                {
                    tilEstado.Error = "";
                }

                if (cidades[spinCidade.SelectedItemPosition].Id == 0)
                {
                    tilCidade.Error = "Informe a cidade";
                    valido = false;
                }
                else
                {
                    tilCidade.Error = "";
                }
            }

            if (opcaoTela == OpcaoTela.AlterarSenha)
            {
                if (edtSenhaAntiga.Text == "")
                {
                    tilSenhaAntiga.Error = "Infome a senha";
                    valido = false;
                }
                else
                {
                    tilSenhaAntiga.Error = "";
                }
            }

            if (opcaoTela != OpcaoTela.Alterar)
            {
                if (edtSenha.Text == "")
                {
                    tilSenha.Error = "Infome a senha";
                    valido = false;
                }
                else
                {
                    if (edtSenha.Text.Length < 6)
                    {
                        tilSenha.Error = "A senha deve ter no mínimo 6 dígitos";
                        valido = false;
                    }
                    else
                    {
                        tilSenha.Error = "";
                    }
                }

                if (edtSenhaConfirma.Text != edtSenha.Text)
                {
                    tilSenhaConfirma.Error = "A senha e senha de confirmação estão diferentes";
                    valido = false;
                }
                else
                {
                    tilSenhaConfirma.Error = "";
                }
            }

            return valido;
        }

        public async void RegistarUsuario()
        {
            Progress.Show();
            
            RegistraUsuario usuario = new RegistraUsuario();
            usuario.NomeUsuario = edtUsuario.Text;
            usuario.NomeCompleto = edtNomeCompleto.Text;
            usuario.CidadeId = cidades[spinCidade.SelectedItemPosition].Id;
            usuario.Endereco = edtEndereco.Text;
            usuario.Profissional = chkProfissional.Checked;
            usuario.Status = edtStatus.Text;
            usuario.Senha = edtSenha.Text;
            usuario.ConfirmaSenha = edtSenhaConfirma.Text;

            DataService ds = new DataService();
            string result = await ds.RegistrarUsuario(usuario);

            Progress.Hide();

            if (result != "")
            {
                Snackbar.Make(btnSalvar, result, Snackbar.LengthLong).Show();
            }
            else
            {
                Toast.MakeText(this, "Cadastro com sucesso", ToastLength.Long).Show();
                Finish();
            }
        }

        public async void AlterarUsuario()
        {
            Progress.Show();

            DataService ds = new DataService(Token);

            AlterarUsuario usuario = new AlterarUsuario();

            usuario.NomeCompleto = edtNomeCompleto.Text;
            usuario.CidadeId = cidades[spinCidade.SelectedItemPosition].Id;
            usuario.Endereco = edtEndereco.Text;
            usuario.Profissional = chkProfissional.Checked;
            usuario.Status = edtStatus.Text;

            string result = await ds.AlterarUsuario(usuario);

            if (result == "")
            {
                try
                {
                    Usuario user = await ds.GetUsuario();
                    Prefs.SalvarUsuario(user);
                }
                catch (Exception e)
                {
                    result = e.Message;
                }
            }

            Progress.Hide();

            if (result != "")
            {
                Snackbar.Make(btnSalvar, result, Snackbar.LengthLong).Show();
            }
            else
            {
                SetResult(Result.Ok);
                Toast.MakeText(this, "Alterado com sucesso", ToastLength.Long).Show();
                Finish();
            }
        }

        public async void AlterarSenha()
        {
            Progress.Show();

            DataService ds = new DataService(Token);

            AlterarSenhaUsuario usuario = new AlterarSenhaUsuario();

            usuario.AntigaSenha = edtSenhaAntiga.Text;
            usuario.NovaSenha = edtSenha.Text;
            usuario.ConfirmaSenha = edtSenhaConfirma.Text;

            string result = await ds.AlterarSenhaUsuario(usuario);

            Progress.Hide();

            if (result != "")
            {
                Snackbar.Make(btnSalvar, result, Snackbar.LengthLong).Show();
            }
            else
            {
                Toast.MakeText(this, "Alterado com sucesso", ToastLength.Long).Show();
                Finish();
            }
        }
    }
}