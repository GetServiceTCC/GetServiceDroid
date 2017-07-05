using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Adapters;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using Java.Lang;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace GetServiceDroid.Activities
{
    [Activity]
    class ChatActivity : BaseActivity
    {
        public const string EXTRA_USUARIO_CONTATO = "extra_usuario_contato";
        public const string EXTRA_USUARIO_CONTATO_NOME = "extra_usuario_contato_nome";

        string contato;
        string contatoNome;

        public List<Mensagem> mensagens;
        MensagemRecyclerViewAdapter adapter;

        RecyclerView recyclerView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            contato = Intent.GetStringExtra(EXTRA_USUARIO_CONTATO);
            contatoNome = Intent.GetStringExtra(EXTRA_USUARIO_CONTATO_NOME);

            if (string.IsNullOrEmpty(contato))
            {
                Toast.MakeText(this, "Usuario para contato invalido", ToastLength.Long).Show();
                Finish();
                return;
            }

            Title = contatoNome;
            
            SetContentView(Resource.Layout.chat);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarChat);

            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.listaMensagens);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this) { StackFromEnd = true });
            recyclerView.HasFixedSize = true;

            IniciarChat();
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

        public async void IniciarChat()
        {
            Progress.Show();

            DataService ds = new DataService(Token);

            mensagens = await ds.GetMensagens(contato);

            adapter = new MensagemRecyclerViewAdapter(Token.userName, mensagens);

            recyclerView.SetAdapter(adapter);

            ChatService.ReceberMensagem += (userName, nomeCompleto, mensagem) =>
            {
                if (contato != userName)
                    return;

                RunOnUiThread(() =>
                {
                    try
                    {
                        mensagens.Add(new Mensagem()
                        {
                            remetenteUserName = userName,
                            DestinatarioUserName = Token.userName,
                            Texto = mensagem,
                            Data = DateTime.Now
                        });

                        adapter.NotifyDataSetChanged();

                        recyclerView.SmoothScrollToPosition(mensagens.Count);
                    }
                    catch (System.Exception e)
                    {
                        Toast.MakeText(this, "ReceberMensagem: " + e.Message, ToastLength.Long);
                    }
                });
            };

            EditText edtMensagem = FindViewById<EditText>(Resource.Id.edtMensagem);

            FindViewById<FloatingActionButton>(Resource.Id.fabEnviarMensagem).Click += async (s, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(edtMensagem.Text))
                        return;

                    string msg = edtMensagem.Text;
                    edtMensagem.Text = "";

                    await ChatService.EnviarMensagem(contato, msg);

                    mensagens.Add(new Mensagem()
                    {
                        remetenteUserName = Token.userName,
                        DestinatarioUserName = contato,
                        Texto = msg,
                        Data = DateTime.Now
                    });

                    adapter.NotifyDataSetChanged();

                    recyclerView.SmoothScrollToPosition(mensagens.Count);
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, "EnviarMensagem: " + ex.Message, ToastLength.Long);
                }
            };

            Progress.Hide();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ChatService.ReceberMensagem = null;
        }
    }
}