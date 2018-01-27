﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpMachine;
using System.Diagnostics;

namespace Kayak.Http
{
    // transforms socket events into http server transaction events.
    class HttpServerSocketDelegate : ISocketDelegate
    {
        HttpParser parser;
        ParserToTransactionTransform transactionTransform;
        IHttpServerTransactionDelegate transactionDelegate;
        IHttpServerTransaction transaction;

        public HttpServerSocketDelegate(IHttpServerTransaction transaction, IHttpServerTransactionDelegate transactionDelegate)
        {
            this.transaction = transaction;
            this.transactionDelegate = transactionDelegate;
            transactionTransform = new ParserToTransactionTransform(transaction, transactionDelegate);
            parser = new HttpParser(new ParserDelegate(transactionTransform));
        }

        public bool OnData(ISocket socket, ArraySegment<byte> data, Action continuation)
        {
            try
            {
                var parsed = parser.Execute(data);

                if (parsed != data.Count)
                {
                    Trace.Write("Error while parsing request.");
                    throw new Exception("Error while parsing request.");
                }

                // raises request events on transaction delegate
                return transactionTransform.Commit(continuation);
            }
            catch (Exception e)
            {
                // XXX test this behavior
                OnError(socket, e);
                OnClose(socket);
                throw;
            }
        }

        public void OnEnd(ISocket socket)
        {
            NPx.Log.Debug("Socket OnEnd.");

            // parse EOF
            OnData(socket, default(ArraySegment<byte>), null);

            transactionDelegate.OnEnd(transaction);
        }

        public void OnError(ISocket socket, Exception e)
        {
            NPx.Log.Debug("Socket OnError.");
            e.DebugStackTrace();
            transactionDelegate.OnError(transaction, e);
        }

        public void OnClose(ISocket socket)
        {
            transactionDelegate.OnClose(transaction);
        }

        public void OnConnected(ISocket socket)
        {
            throw new NotImplementedException();
        }
    }
}
