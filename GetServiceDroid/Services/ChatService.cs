using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using GetServiceDroid.DataServices;
using GetServiceDroid.Models;
using GetServiceDroid.Utils;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace GetServiceDroid.Services
{
    [Service(Enabled = true, Exported = true)]
    public class ChatService : Service
    {
        public class ChatServiceBinder : Binder
        {
            public ChatService Service { get; private set; }

            public ChatServiceBinder(ChatService service)
            {
                Service = service;
            }
        }

        HubConnection hubConnection;
        IHubProxy chatHubProxy;
        IBinder binder;

        Token token;

        SharedPreferences Prefs;

        public Action<string, string, string> ReceberMensagem = null;

        public override void OnCreate()
        {
            base.OnCreate();

            Prefs = new SharedPreferences(this);
            token = Prefs.GetToken();

            hubConnection = new HubConnection(DataService.BASE_URL);
            hubConnection.Headers.Add("Authorization", "bearer " + token.access_token);
            chatHubProxy = hubConnection.CreateHubProxy("ChatHub");
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var result = base.OnStartCommand(intent, flags, startId);

            StartSignalR();

            return result;
        }

        public override void OnDestroy()
        {
            hubConnection.Stop();
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new ChatServiceBinder(this);

            StartSignalR();

            return binder;
        }

        public async void StartSignalR()
        {
            try
            {
                if (hubConnection.State == ConnectionState.Connected)
                    return;

                await hubConnection.Start();

                try
                {
                    chatHubProxy.On<string, string, string>("ReceberMensagem", (userName, nomeCompleto, mensagem) =>
                    {
                        if (ReceberMensagem != null)
                            ReceberMensagem(userName, nomeCompleto, mensagem);
                        else
                        {
                            Notification.Builder builder = new Notification.Builder(this)
                                .SetContentTitle(nomeCompleto)
                                .SetContentText(mensagem)
                                .SetDefaults(NotificationDefaults.Sound)
                                .SetSmallIcon(Resource.Drawable.ic_logo);
                            
                            Notification notification = builder.Build();
                            
                            NotificationManager notificationManager =
                                GetSystemService(NotificationService) as NotificationManager;
                            
                            const int notificationId = 0;
                            notificationManager.Notify(notificationId, notification);
                        }
                    });
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, "ReceberMensagem: " + e.Message, ToastLength.Long);
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "hubConnection: " + e.Message, ToastLength.Long);
            }
        }

        public Task EnviarMensagem(string userName, string mensagem)
        {
            return chatHubProxy.Invoke("EnviarMensagem", new object[] { userName, mensagem });
        }
    }
}