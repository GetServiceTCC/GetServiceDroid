
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using GetServiceDroid.Models;
using System;
using SupportDialogFragment = Android.Support.V4.App.DialogFragment;

namespace GetServiceDroid.Fragments
{
    public interface IAddComentarioFragmentListener
    {
        void OnDialogPositiveClick(Comentario comentario);
        void OnDialogNegativeClick(string msg);
    }

    public class AddComentario : SupportDialogFragment
    {
        IAddComentarioFragmentListener Listener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (TargetFragment != null)
                    Listener = (IAddComentarioFragmentListener)TargetFragment;
                else
                    Listener = (IAddComentarioFragmentListener)Context;
            }
            catch
            {
                throw new Exception("Não implementa IAddComentarioFragmentListener");
            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var layout = Activity.LayoutInflater.Inflate(Resource.Layout.add_comentario, null);

            TextInputLayout tilAvaliacao = layout.FindViewById<TextInputLayout>(Resource.Id.tilComentarioAvaliacao);
            RatingBar rbAvaliacao = layout.FindViewById<RatingBar>(Resource.Id.rbComentarioAvaliacao);

            TextInputLayout tilComentario = layout.FindViewById<TextInputLayout>(Resource.Id.tilComentario);
            EditText edtComentario = layout.FindViewById<EditText>(Resource.Id.edtComentario);


            AlertDialog.Builder builder = new AlertDialog.Builder(Context);

            builder.SetTitle("Adicionar Comentario");
            builder.SetView(layout);
            
            builder.SetPositiveButton("OK", (s, e) =>
            {
                bool valido = true;

                if (rbAvaliacao.Rating <= 0 || rbAvaliacao.Rating > 5)
                    valido = false;

                if (edtComentario.Text == "")
                    valido = false;

                if (valido)
                {
                    Comentario comentario = new Comentario();
                    comentario.Descricao = edtComentario.Text;
                    comentario.Avaliacao = (int)rbAvaliacao.Rating;
                    Listener.OnDialogPositiveClick(comentario);
                }
                else
                    Listener.OnDialogNegativeClick("Avalie o profissional e informe o comentario");
            });

            builder.SetNegativeButton("Cancelar", (s, e) =>
            {
                Listener.OnDialogNegativeClick("");
            });

            return builder.Create();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            //base.OnDismiss(dialog);
        }

        public override void Dismiss()
        {
            //base.Dismiss();
        }
    }
}