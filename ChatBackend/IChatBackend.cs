using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChatBackend
{
    [ServiceContract]
    public interface IChatBackend
    {
        [OperationContract(IsOneWay = true)]
        void DisplayMessage(CompositeType composite);

        void SendMessage(string text);
    }

    [DataContract]
    public class CompositeType
    {
        public CompositeType() { }
        public CompositeType(string username, string message)
        {
            Username = username;
            Message = message;
        }

        [DataMember]
        public string Username { get; set; } = "Anonymous";

        [DataMember]
        public string Message { get; set; } = "";
    }

    public delegate void DisplayMessageDelegate(CompositeType data);
}
