using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Models;
using GetServiceDroid.Utils;
using System;
using System.Collections.Generic;

namespace GetServiceDroid.Adapters
{
    class MensagemRecyclerViewAdapter : RecyclerView.Adapter
    {
        private string UserName { get; set; }
        private List<Mensagem> Mensagens { get; set; }

        public MensagemRecyclerViewAdapter(string userName, List<Mensagem> mensagens)
        {
            UserName = userName;
            Mensagens = mensagens;
        }

        public override int ItemCount
        {
            get
            {
                return Mensagens.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var simpleHolder = holder as MensagemViewHolder;
            simpleHolder.Bind(UserName, Mensagens[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.mensagem_list_item, parent, false);
            return new MensagemViewHolder(view, parent.Context);
        }

        private class MensagemViewHolder : RecyclerView.ViewHolder
        {
            public readonly View view;
            public readonly Context context;
            public readonly LinearLayout chatContainer;
            public readonly TextView txtMensagem;
            public readonly TextView txtData;

            public MensagemViewHolder(View view, Context context) : base(view)
            {
                this.view = view;
                this.context = context;
                chatContainer = view.FindViewById<LinearLayout>(Resource.Id.chatContainer);
                txtMensagem = view.FindViewById<TextView>(Resource.Id.chatMensagem);
                txtData = view.FindViewById<TextView>(Resource.Id.chatMensagemData);
            }

            public void Bind(string userName, Mensagem mensagem)
            {
                txtMensagem.Text = mensagem.Texto;

                if (mensagem.Data.Date == DateTime.Now.Date)
                    txtData.Text = mensagem.Data.ToString("hh:mm:ss");
                else
                    txtData.Text = mensagem.Data.ToString("dd/MM/yyyy hh:mm:ss");

                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent);

                if (mensagem.remetenteUserName == userName)
                {
                    int color = ThemeUtils.GetThemeColor(context, Resource.Attribute.colorPrimary);
                    Color cor = ThemeUtils.GetColorFromInteger(color);

                    cor.A = 50;
                    chatContainer.SetBackgroundColor(cor);                    

                    layoutParams.Gravity = GravityFlags.Right;
                    txtMensagem.Gravity = GravityFlags.Right;
                    txtData.Gravity = GravityFlags.Right;
                }
                else
                {
                    chatContainer.SetBackgroundResource(Resource.Color.grey_200);

                    layoutParams.Gravity = GravityFlags.Left;
                    txtMensagem.Gravity = GravityFlags.Left;
                    txtData.Gravity = GravityFlags.Left;
                }

                chatContainer.LayoutParameters = layoutParams;
            }
        }
    }
}