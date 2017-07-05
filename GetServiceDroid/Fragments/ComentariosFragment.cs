using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Adapters;
using GetServiceDroid.DataServices;

namespace GetServiceDroid.Fragments
{
    class ComentariosFragment : BaseFragment
    {
        ProgressBar progressBar;
        RecyclerView recyclerView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(Resource.Layout.comentario_fragment, container, false);

            progressBar = layout.FindViewById<ProgressBar>(Resource.Id.listaComentarioProgress);

            recyclerView = layout.FindViewById<RecyclerView>(Resource.Id.listaComentario);
            recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));
            recyclerView.HasFixedSize = true;

            return layout;
        }

        public async void CarregarComentarios(int servicoId)
        {
            recyclerView.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            DataService ds = new DataService();

            var comentarios = await ds.GetComentariosServico(servicoId);

            recyclerView.SetAdapter(new ComentarioRecyclerViewAdapter(comentarios));

            progressBar.Visibility = ViewStates.Gone;
            recyclerView.Visibility = ViewStates.Visible;
        }
    }
}