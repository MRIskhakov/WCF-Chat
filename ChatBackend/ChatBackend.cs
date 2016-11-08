using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatBackend : IChatBackend
    {

        DisplayMessageDelegate _displayMessageDelegate = null;


        private ChatBackend() 
        {
        }


        // Конструктор ChatBackend должен быть вызван делегатом, отображающим сообщения

        public ChatBackend(DisplayMessageDelegate dmd)
        {
            _displayMessageDelegate = dmd;
            StartService();
        }


        //Этот метод вызывается нашими собеседниками когда они хотят отобразить свое сообщение на наш экран
        
        public void DisplayMessage(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            _displayMessageDelegate?.Invoke(composite);
        }



        private string _myUserName = "Anonymous";
        private ServiceHost host = null;
        private ChannelFactory<IChatBackend> _channelFactory = null;
        private IChatBackend _channel;


        // Метод, который вызывет пользователь для транслирования сообщения для собеседника

        public void SendMessage(string text)
        {
            // Следующим условием реализуем изменение имени пользователя
            if (text.StartsWith("setname:", StringComparison.OrdinalIgnoreCase)) 
            {
                _myUserName = text.Substring("setname:".Length).Trim();
                _displayMessageDelegate(new CompositeType("Событие", "Имя изменено на: " + _myUserName));
            }
            else
            {
                //Если имя пользователя не меняется, значит производим вызов отображения сообщения у собеседника
                _channel.DisplayMessage(new CompositeType(_myUserName, text));
            }
        }

        private void StartService()
        {
            host = new ServiceHost(this);
            host.Open();
            _channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            _channel = _channelFactory.CreateChannel();

            // Информация, для отображения в канале
            _channel.DisplayMessage(new CompositeType("Событие", _myUserName + " вошел(-ла) в комнату чата."));

            // Информация, для отображения у вошедшего пользователя
            _displayMessageDelegate(new CompositeType("Информация", "Чтобы изменить имя, наберите: setname: НОВОЕ_ИМЯ"));
        }

        private void StopService()
        {
            if (host == null) return;
            _channel.DisplayMessage(new CompositeType("Событие", _myUserName + " покинул(-а) комнату чата."));
            if (host.State == CommunicationState.Closed) return;
            _channelFactory.Close();
            host.Close();
        }

    }
}
