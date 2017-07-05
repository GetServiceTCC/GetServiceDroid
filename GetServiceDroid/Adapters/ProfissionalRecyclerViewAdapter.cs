using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using Square.Picasso;
using System.Collections.Generic;

namespace GetServiceDroid.Adapters
{
    public class ProfissionalRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<Profissional> Profissionais { get; set; }

        public ProfissionalRecyclerViewAdapter(List<Profissional> profissionais)
        {
            Profissionais = profissionais;
        }

        public override int ItemCount
        {
            get
            {
                return Profissionais.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var simpleHolder = holder as ProfissionalViewHolder;
            simpleHolder.Bind(Profissionais[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.profissional_list_item, parent, false);
            return new ProfissionalViewHolder(view, parent.Context);
        }

        public class ProfissionalViewHolder : RecyclerView.ViewHolder
        {
            public readonly View view;
            public readonly Context context;
            public readonly ImageView imgFotoProfissional;
            public readonly TextView txtNome;
            public readonly TextView txtAvaliacao;
            public readonly TextView txtQtdComentarios;
            public readonly TextView txtStatus;
            public readonly TextView txtLocalidade;

            public ProfissionalViewHolder(View view, Context context) : base(view)
            {
                this.view = view;
                this.context = context;
                imgFotoProfissional = view.FindViewById<ImageView>(Resource.Id.fotoProfissional);
                txtNome = view.FindViewById<TextView>(Resource.Id.nomeProfissional);
                txtAvaliacao = view.FindViewById<TextView>(Resource.Id.avaliacaoProfissional);
                txtQtdComentarios = view.FindViewById<TextView>(Resource.Id.qtdComentariosProfissional);
                txtStatus = view.FindViewById<TextView>(Resource.Id.statusProfissional);
                txtLocalidade = view.FindViewById<TextView>(Resource.Id.localidadeProfissional);
            }

            public void Bind(Profissional profissional)
            {
                Picasso.With(context)
                   .Load(DataService.BASE_URL + "api/UsuarioFoto/" + profissional.UserName)
                   .Placeholder(Resource.Drawable.ic_avatar)
                   .Resize(300, 300)
                   .CenterCrop()
                   .Into(imgFotoProfissional);

                txtNome.Text = profissional.NomeCompleto;

                if (profissional.Avaliacao != null)
                    txtAvaliacao.Text = string.Format("{0:#,#0.0}", profissional.Avaliacao);
                else
                    txtAvaliacao.Text = "N/A";

                txtQtdComentarios.Text = "(" + profissional.QtdComentarios + ")";

                txtStatus.Text = profissional.Status;
                txtLocalidade.Text = profissional.Cidade + " - " + profissional.Uf;
            }
        }
    }
}