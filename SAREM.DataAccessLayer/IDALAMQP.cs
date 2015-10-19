using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAREM.Shared.Datatypes;

namespace SAREM.DataAccessLayer
{
    public interface IDALAMQP
    {
        void sendToQueue(DataMensaje m);
        void deleteMessage(string id);
        ICollection<string> getQueues(string AMQUSER, string AMQPASSWORD);
    }
}
