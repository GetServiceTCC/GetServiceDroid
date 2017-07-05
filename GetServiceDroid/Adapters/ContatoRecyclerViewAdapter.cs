using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using GetServiceDroid.Utils;
using Java.Lang;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using Object = Java.Lang.Object;

namespace GetServiceDroid.Adapters
{
    public class ContatoRecyclerViewAdapter : RecyclerView.Adapter, IFilterable
    {
        private List<Contato> OriginalData { get; set; }
        private List<Contato> Contatos { get; set; }
        public Filter Filter { get; private set; }
        private string UserName { get; set; }

        public ContatoRecyclerViewAdapter(string userName, List<Contato> contatos)
        {
            UserName = userName;
            Contatos = contatos;
            Filter = new ContatoFilter(this);
        }

        public override int ItemCount
        {
            get
            {
                return Contatos.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var simpleHolder = holder as ContatoViewHolder;
            simpleHolder.Bind(UserName, Contatos[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.contato_list_item, parent, false);
            return new ContatoViewHolder(view, parent.Context);
        }

        private class ContatoViewHolder : RecyclerView.ViewHolder
        {
            public readonly View view;
            public readonly Context context;
            public readonly ImageView imgFoto;
            public readonly TextView txtNome;
            public readonly TextView txtStatus;

            public ContatoViewHolder(View view, Context context) : base(view)
            {
                this.view = view;
                this.context = context;
                imgFoto = view.FindViewById<ImageView>(Resource.Id.fotoContato);
                txtNome = view.FindViewById<TextView>(Resource.Id.nomeContato);
                txtStatus = view.FindViewById<TextView>(Resource.Id.statusContato);
            }

            public void Bind(string userName, Contato contato)
            {
                string contatoFoto = "";

                if (userName == contato.UsuarioUserName)
                {
                    contatoFoto = contato.ContatoUserName;
                    txtNome.Text = contato.ContatoNomeCompleto;
                    txtStatus.Text = contato.ContatoStatus;
                }
                else
                {
                    contatoFoto = contato.UsuarioUserName;
                    txtNome.Text = contato.UsuarioNomeCompleto;
                    txtStatus.Text = contato.UsuarioStatus;
                }

                if (!string.IsNullOrEmpty(contatoFoto))
                {
                    Picasso.With(context)
                       .Load(DataService.BASE_URL + "api/UsuarioFoto/" + contatoFoto)
                       .Placeholder(Resource.Drawable.ic_avatar)
                       .Resize(300, 300)
                       .CenterCrop()
                       .Into(imgFoto);
                }
            }
        }

        private class ContatoFilter : Filter
        {
            private readonly ContatoRecyclerViewAdapter _adapter;

            public ContatoFilter(ContatoRecyclerViewAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Contato>();
                if (_adapter.OriginalData == null)
                    _adapter.OriginalData = _adapter.Contatos;

                if (constraint == null) return returnObj;

                if (_adapter.OriginalData != null && _adapter.OriginalData.Any())
                {
                    results.AddRange(
                        _adapter.OriginalData.Where(
                            contato =>
                                contato.UsuarioNomeCompleto.ToLower().Contains(constraint.ToString()) ||
                                contato.ContatoNomeCompleto.ToLower().Contains(constraint.ToString())
                         )
                    );
                }

                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter.Contatos = values.ToArray<Object>()
                        .Select(r => r.ToNetObject<Contato>()).ToList();

                _adapter.NotifyDataSetChanged();

                constraint.Dispose();
                results.Dispose();
            }
        }
    }
}