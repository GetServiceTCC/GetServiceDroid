using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using GetServiceDroid.Adapters;
using GetServiceDroid.DataServices;
using GetServiceDroid.Fragments;
using GetServiceDroid.Models;
using Java.IO;
using Square.Picasso;
using System.Threading.Tasks;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace GetServiceDroid.Activities
{
    [Activity(Label = "GetService", MainLauncher = true)]
    public class MainActivity : BaseActivity, ILoginFragmentListener
    {
        DrawerLayout drawerLayout;

        const string TAB_DESTAQUES = "Buscar";
        const string TAB_CONTATOS = "Contatos";
        const string TAB_LOGIN = "Login";
        const string TAB_MEUS_SERVICOS = "Meus Serviços";
        const int REQUEST_CAMERA = 0;
        const int REQUEST_GALERIA = 1;
        const int REQUEST_ALTERAR_USUARIO = 2;

        ImageView imgUsuario;
        TextView txtUsuario;

        ContatosFragment contatosFragment;

        bool profissional;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.main);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);

            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.navMenuDrawer);

            navigationView.NavigationItemSelected += (s, e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.drawerAlterarFoto:

                        string[] items = { "Tirar foto", "Escolher da galeria", "Excluir Foto", "Cancelar" };

                        using (var dialogBuilder = new AlertDialog.Builder(this))
                        {
                            dialogBuilder.SetTitle("Alterar Foto");
                            dialogBuilder.SetItems(items, (d, args) =>
                            {
                                if (args.Which == 0)
                                {
                                    Intent intent = new Intent(MediaStore.ActionImageCapture);
                                    var file = new File(Environment.GetExternalStoragePublicDirectory(
                                    Environment.DirectoryPictures), string.Format("{0}.jpg", Token.userName));
                                    intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(file));
                                    StartActivityForResult(intent, REQUEST_CAMERA);

                                }
                                else if (args.Which == 1)
                                {
                                    Intent intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                                    intent.SetType("image/*");
                                    StartActivityForResult(Intent.CreateChooser(intent, "Selecionar foto"), REQUEST_GALERIA);
                                }
                                else if (args.Which == 2)
                                {
                                    DeletarFoto();
                                }
                            });

                            dialogBuilder.Show();
                        }
                        break;
                    case Resource.Id.drawerAlterarUsuario:
                        Intent alterarUsuario = new Intent(this, typeof(UsuarioActivity));
                        alterarUsuario.PutExtra(UsuarioActivity.EXTRA_OPCAO_TELA, UsuarioActivity.OPCAO_ALTERAR_CONTA);
                        StartActivityForResult(alterarUsuario, REQUEST_ALTERAR_USUARIO);
                        break;
                    case Resource.Id.drawerAlterarSenha:
                        Intent alterarSenha = new Intent(this, typeof(UsuarioActivity));
                        alterarSenha.PutExtra(UsuarioActivity.EXTRA_OPCAO_TELA, UsuarioActivity.OPCAO_ALTERAR_SENHA);
                        StartActivity(alterarSenha);
                        break;
                    case Resource.Id.drawerConfiguracoes:

                        string[] themes = { "Vermelho", "Azul", "Cancelar" };

                        using (var dialogBuilder = new AlertDialog.Builder(this))
                        {
                            dialogBuilder.SetTitle("Alterar Tema");
                            dialogBuilder.SetItems(themes, (d, args) =>
                            {
                                if (args.Which == 0)
                                {
                                    Prefs.SalvarTheme(THEME_APP_RED);
                                    ReiniciarAplicacao();
                                }
                                else if (args.Which == 1)
                                {
                                    Prefs.SalvarTheme(THEME_APP_BLUE);
                                    ReiniciarAplicacao();
                                }
                            });

                            dialogBuilder.Show();
                        }
                        break;
                    case Resource.Id.drawerLogout:
                        Logout();
                        break;
                }

                drawerLayout.CloseDrawers();
            };

            if (Token == null)
            {
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            else
            {
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);

                imgUsuario = navigationView.GetHeaderView(0).FindViewById<ImageView>(Resource.Id.drawerFotoUsuario);
                Picasso.With(this)
                   .Load(DataService.BASE_URL + "api/UsuarioFoto/" + Token.userName)
                   .Placeholder(Resource.Drawable.ic_avatar)
                   .Resize(300, 300)
                   .CenterCrop()
                   .Into(imgUsuario);

                txtUsuario = navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.drawerUsuario);
                txtUsuario.Text = Token.userName;
            }

            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);

            TabAdapter tabAdapter = new TabAdapter(SupportFragmentManager);

            tabAdapter.AddFragment(new DestaquesFragment(), TAB_DESTAQUES);

            if (Token == null)
            {
                tabAdapter.AddFragment(new LoginFragment(), TAB_LOGIN);
            }
            else
            {
                contatosFragment = new ContatosFragment();

                tabAdapter.AddFragment(contatosFragment, TAB_CONTATOS);
                profissional = Prefs.GetPerfilProfissional();
                if (profissional)
                {
                    tabAdapter.AddFragment(new MeusServicosFragment(), TAB_MEUS_SERVICOS);
                }
            }

            viewPager.OffscreenPageLimit = tabAdapter.Count;
            viewPager.Adapter = tabAdapter;
            viewPager.Adapter.NotifyDataSetChanged();
            tabs.SetupWithViewPager(viewPager);

            FloatingActionButton fabAddServico = FindViewById<FloatingActionButton>(Resource.Id.fabAddServico);

            fabAddServico.Visibility = ViewStates.Gone;
            fabAddServico.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(EditarServicoActivity));
                intent.PutExtra(EditarServicoActivity.EXTRA_OPCAO_TELA, EditarServicoActivity.OPCAO_INSERIR_SERVICO);
                StartActivity(intent);
            };

            tabs.TabSelected += (s, e) =>
            {
                fabAddServico.Visibility = ViewStates.Gone;

                if (e.Tab.Text == TAB_CONTATOS)
                {
                    contatosFragment.CarregarContatos();
                }
                else if (e.Tab.Text == TAB_MEUS_SERVICOS)
                {
                    fabAddServico.Visibility = ViewStates.Visible;
                }
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (requestCode == REQUEST_CAMERA)
                {
                    var file = new File(
                        Environment.GetExternalStoragePublicDirectory(
                            Environment.DirectoryPictures), string.Format("{0}.jpg", Token.userName));

                    ALterarFoto(Uri.FromFile(file));
                }
                else if (requestCode == REQUEST_GALERIA)
                {
                    ALterarFoto(data.Data);
                }
                else if (requestCode == REQUEST_ALTERAR_USUARIO)
                {
                    if (!profissional && Prefs.GetPerfilProfissional())
                        Recreate();
                }
            }
        }

        public async void ALterarFoto(Uri uri)
        {
            if (uri == null)
            {
                Toast.MakeText(this, "Erro ao ler a imagem", ToastLength.Long).Show();
                return;
            }

            Progress.Show();

            Bitmap bitmap = null;

            await Task.Run(() =>
            {
                bitmap = Picasso.With(this)
                        .Load(uri)
                        .Resize(300, 300)
                        .CenterCrop()
                        .Get();
            });

            if (bitmap != null)
            {
                DataService ds = new DataService(Token);

                bool result = false;

                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Position = 0;
                    result = await ds.AlterarFoto(stream);
                }

                if (result)
                {
                    Picasso.With(this).Invalidate(DataService.BASE_URL + "api/UsuarioFoto/" + Token.userName);

                    Picasso.With(this)
                   .Load(uri)
                   .Resize(300, 300)
                   .CenterCrop()
                   .Into(imgUsuario);

                    Toast.MakeText(this, "Salvo com sucesso", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "Erro ao salvar", ToastLength.Long).Show();
                }
            }

            Progress.Hide();
        }

        public async void DeletarFoto()
        {
            Progress.Show();

            DataService ds = new DataService(Token);

            bool result = await ds.DeletarFoto();

            if (result)
            {
                Picasso.With(this).Invalidate(DataService.BASE_URL + "api/UsuarioFoto/" + Token.userName);

                Picasso.With(this)
               .Load(Resource.Drawable.ic_avatar)
               .Into(imgUsuario);

                Toast.MakeText(this, "Deletado com sucesso", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Erro ao deletar", ToastLength.Long).Show();
            }

            Progress.Hide();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if (Token != null)
                        drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void OnLogin()
        {
            ReiniciarAplicacao();
        }

        public void Logout()
        {
            Prefs.DeletarToken();
            Prefs.DeletarUsuario();
            ReiniciarAplicacao();
        }

        public void ReiniciarAplicacao()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }
    }
}