﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro2D
{
    //public class MessageData
    //{

    //}


    public class Message
    {
        static uint uniqueMessageId = 0;

        public uint _id = 0;
        public int _type = -1;
        public object _data = null;
        public Node _to = null;
        public Node _from = null;

        public Message (int type, object data, Node to, Node from = null)
        {
            _id = uniqueMessageId;
            _type = type;
            _data = data;
            _to = to;
            _from = from;
            ++uniqueMessageId;
        }
    }

    public class MessageQueue
    {
        Queue<Message> _messages = new Queue<Message>();

        public MessageQueue()
        {
            ClearAll();
        }
        private void Add(Message message)
        {
            if (null != message)
                _messages.Enqueue(message);

        }
        public void ClearAll()
        {
            _messages.Clear();
        }

        public uint Post(int type, object data, Node to, Node from = null)
        {
            Message message = new Message(type, data, to, from);
            if (null != to)
                Add(message);

            return message._id;
        }
        public bool IsEmpty()
        {
            return _messages.Count == 0;
        }
        public bool IsMessage()
        {
            return null != GetMessage();
        }
        public Message GetMessage()
        {
            return IsEmpty() ? null : _messages.Peek();
        }
        public uint LastMessageId()
        {
            return GetMessage()._id;
        }
        public object LastMessageData()
        {
            return IsMessage() ? GetMessage()._data : null; 
        }
        public Node From()
        {
            return IsMessage() ? GetMessage()._from : null;
        }
        public Node To()
        {
            return IsMessage() ? GetMessage()._to : null;
        }
        public object GetData()
        {
            return IsMessage() ? GetMessage()._data : null;
        }
        public int Type()
        {
            return IsMessage() ? GetMessage()._type : -1;
        }
        public void Dispatch()
        {
            while (_messages.Count > 0)
            {
                Message message = _messages.Peek();

                if (null != message)
                {
                    if (null != message._to)
                    {
                        message._to._message = message;
                    }

                    _messages.Dequeue();
                }
            }
        }

        // Debug
        public void ShowAll()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Message Queue is EMPTY !");
                return;
            }

            foreach (var message in _messages)
            {
                Console.WriteLine(" -- > " + message._id + " , " + message._type);
            }

        }
    }


}
