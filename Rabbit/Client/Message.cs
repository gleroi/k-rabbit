using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.Client
{
    enum MessageType
    {
        InscriptionOk,
        InscriptionKo,
        WorldState,
        GameOver,
        ActionOk,
        ActionKo
    }

    struct Message
    {
        public MessageType Type { get; private set; }
        public string Data { get; private set; }

        public Message(MessageType type, string data)
            : this()
        {
            this.Type = type;
            this.Data = data;
        }
    }

}
