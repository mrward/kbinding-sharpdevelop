﻿// Code from https://github.com/davidfowl/DesignTimeHostDemo

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Framework.DesignTimeHost.Models;
using Newtonsoft.Json;

namespace ICSharpCode.KBinding
{
    public class ProcessingQueue
    {
        private readonly List<Message> _queue = new List<Message>();
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        public event Action<Message> OnReceive;

        public ProcessingQueue(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);
        }

        public void Start()
        {
            Trace.TraceInformation("[ProcessingQueue]: Start()");
            new Thread(ReceiveMessages).Start();
        }

        public void Post(Message message)
        {
            lock (_writer)
            {
                Trace.TraceInformation("[ProcessingQueue]: Post({0})", message);
                _writer.Write(JsonConvert.SerializeObject(message));
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    var message = JsonConvert.DeserializeObject<Message>(_reader.ReadString());
                    Trace.TraceInformation("[ProcessingQueue]: OnReceive({0})", message);
                    OnReceive(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
