using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Models;
using System.Collections.Generic;

namespace GetServiceDroid.Adapters
{
    public class DestaqueRecyclerViewAdapter : RecyclerView.Adapter
    {
        private const int VIEW_TYPE_TITULO_DESTAQUE = 0;
        private const int VIEW_TYPE_PROFISSIONAL = 1;
        private const int VIEW_TYPE_SERVICO = 2;

        private List<object> Destaques { get; set; }

        public DestaqueRecyclerViewAdapter(List<object> destaques)
        {
            Destaques = destaques;
        }

        public override int ItemCount
        {
            get
            {
                return Destaques.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder.ItemViewType)
            {
                case VIEW_TYPE_TITULO_DESTAQUE:
                    (holder as TituloDestaqueViewHolder).Bind(Destaques[position] as string);
                    break;
                case VIEW_TYPE_PROFISSIONAL:
                    (holder as ProfissionalRecyclerViewAdapter.ProfissionalViewHolder).Bind(Destaques[position] as Profissional);
                    break;
                case VIEW_TYPE_SERVICO:
                    (holder as ServicoRecyclerViewAdapter.ServicoViewHolder).Bind(Destaques[position] as Servico);
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder viewHolder = null;
            LayoutInflater inflater = LayoutInflater.From(parent.Context);

            switch (viewType)
            {
                case VIEW_TYPE_TITULO_DESTAQUE:
                    View viewTituloDestaque = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.titulo_destaque, parent, false);
                    viewHolder = new TituloDestaqueViewHolder(viewTituloDestaque);
                    break;
                case VIEW_TYPE_PROFISSIONAL:
                    View viewProfissional = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.profissional_list_item, parent, false);
                    viewHolder = new ProfissionalRecyclerViewAdapter.ProfissionalViewHolder(viewProfissional, parent.Context);
                    break;
                case VIEW_TYPE_SERVICO:
                    View viewServico = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.servico_list_item, parent, false);
                    viewHolder = new ServicoRecyclerViewAdapter.ServicoViewHolder(viewServico);
                    break;
            }

            return viewHolder;
        }

        public override int GetItemViewType(int position)
        {
            if (Destaques[position] is string)
                return VIEW_TYPE_TITULO_DESTAQUE;
            else if (Destaques[position] is Profissional)
                return VIEW_TYPE_PROFISSIONAL;
            else if (Destaques[position] is Servico)
                return VIEW_TYPE_SERVICO;

            return -1;
        }

        private class TituloDestaqueViewHolder : RecyclerView.ViewHolder
        {
            public readonly View view;
            public readonly TextView txtTituloDestaque;

            public TituloDestaqueViewHolder(View view) : base(view)
            {
                this.view = view;
                txtTituloDestaque = view.FindViewById<TextView>(Resource.Id.titulo_destaque);
            }

            public void Bind(string tituloDestaque)
            {
                txtTituloDestaque.Text = tituloDestaque;
            }
        }
    }
}