using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Models;
using System.Collections.Generic;

namespace GetServiceDroid.Adapters
{
    public class ServicoRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<Servico> Servicos { get; set; }

        public ServicoRecyclerViewAdapter(List<Servico> servicos)
        {
            Servicos = servicos;
        }

        public override int ItemCount
        {
            get
            {
                return Servicos.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var simpleHolder = holder as ServicoViewHolder;
            simpleHolder.Bind(Servicos[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.servico_list_item, parent, false);
            return new ServicoViewHolder(view);
        }

        public class ServicoViewHolder : RecyclerView.ViewHolder
        {
            public readonly View view;
            public readonly TextView txtDescricao;
            public readonly TextView txtAvaliacao;
            public readonly TextView txtQtdComentarios;
            public readonly TextView txtCategoria;
            public readonly TextView txtSubCategoria;
            public readonly TextView txtValor;
            public readonly TextView txtSobre;

            public ServicoViewHolder(View view) : base(view)
            {
                this.view = view;
                txtDescricao = view.FindViewById<TextView>(Resource.Id.descricaoServico);
                txtAvaliacao = view.FindViewById<TextView>(Resource.Id.avaliacaoServico);
                txtQtdComentarios = view.FindViewById<TextView>(Resource.Id.qtdComentariosServico);
                txtCategoria = view.FindViewById<TextView>(Resource.Id.categoriaServico);
                txtSubCategoria = view.FindViewById<TextView>(Resource.Id.subCategoriaServico);
                txtValor = view.FindViewById<TextView>(Resource.Id.valorServico);
                txtSobre = view.FindViewById<TextView>(Resource.Id.sobreServico);
            }

            public void Bind(Servico servico)
            {
                txtDescricao.Text = servico.Descricao;

                if (!servico.Ativo)
                {
                    txtDescricao.Text += " (Inativo)";
                }

                if (servico.Avaliacao != null)
                    txtAvaliacao.Text = string.Format("{0:#,#0.0}", servico.Avaliacao);
                else
                    txtAvaliacao.Text = "N/A";

                txtQtdComentarios.Text = "(" + servico.QtdComentarios + ")";

                txtCategoria.Text = servico.Categoria;
                txtSubCategoria.Text = servico.SubCategoria;
              
                switch (servico.TipoValor)
                {                    
                    case Models.Enums.TipoValor.Valor:
                        txtValor.Text = string.Format("{0:#,##0.00} R$", servico.Valor);
                        break;
                    case Models.Enums.TipoValor.PorHora:
                        txtValor.Text = string.Format("{0:#,##0.00} R$ /hora", servico.Valor);
                        break;
                    case Models.Enums.TipoValor.PorDia:
                        txtValor.Text = string.Format("{0:#,##0.00} R$ /dia", servico.Valor);
                        break;
                    default:
                        txtValor.Text = "A Negociar";
                        break;
                }

                txtSobre.Text = servico.Sobre;
            }
        }
    }
}