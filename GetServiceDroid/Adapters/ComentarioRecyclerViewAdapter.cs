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
    class ComentarioRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<Comentario> Comentarios { get; set; }

        public ComentarioRecyclerViewAdapter(List<Comentario> comentarios)
        {
            Comentarios = comentarios;
        }

        public override int ItemCount
        {
            get
            {
                return Comentarios.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var simpleHolder = holder as ComentarioViewHolder;
            simpleHolder.Bind(Comentarios[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.comentario_list_item, parent, false);
            return new ComentarioViewHolder(view, parent.Context);
        }

        private class ComentarioViewHolder : RecyclerView.ViewHolder
        {
            public readonly View view;
            public readonly Context context;
            public readonly ImageView imgUsuario;
            public readonly TextView txtNomeCompleto;
            public readonly TextView txtAvaliacao;
            public readonly TextView txtDescricao;
            public readonly TextView txtData;

            public ComentarioViewHolder(View view, Context context) : base(view)
            {
                this.view = view;
                this.context = context;
                imgUsuario = view.FindViewById<ImageView>(Resource.Id.fotoUsuarioComentario);
                txtNomeCompleto = view.FindViewById<TextView>(Resource.Id.nomeCompletoComentario);
                txtAvaliacao = view.FindViewById<TextView>(Resource.Id.avaliacaoComentario);
                txtDescricao = view.FindViewById<TextView>(Resource.Id.descricaoComentario);
                txtData = view.FindViewById<TextView>(Resource.Id.dataComentario);
            }

            public void Bind(Comentario comentario)
            {
                Picasso.With(context)
                   .Load(DataService.BASE_URL + "api/UsuarioFoto/" + comentario.UserName)
                   .Placeholder(Resource.Drawable.ic_avatar)
                   .Resize(300, 300)
                   .CenterCrop()
                   .Into(imgUsuario);

                txtNomeCompleto.Text = comentario.NomeCompleto;
                txtAvaliacao.Text = comentario.Avaliacao.ToString();
                txtDescricao.Text = comentario.Descricao;
                txtData.Text = comentario.Data.ToString("dd/MM/yyyy hh:mm:ss");
            }
        }
    }
}