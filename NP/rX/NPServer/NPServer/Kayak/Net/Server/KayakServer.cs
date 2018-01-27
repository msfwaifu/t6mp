﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Kayak
{
    class DefaultKayakServer : IServer
    {
        IServerDelegate del;

        IScheduler scheduler;
        KayakServerState state;
        Socket listener;

        internal DefaultKayakServer(IServerDelegate del, IScheduler scheduler)
        {
            if (del == null)
                throw new ArgumentNullException("del");

            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            this.del = del;
            this.scheduler = scheduler;
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            state = new KayakServerState();
        }

        public void Dispose()
        {
            state.SetDisposed();

            if (listener != null)
            {
                listener.Dispose();
            }
        }

        public IDisposable Listen(IPEndPoint ep)
        {
			if (ep == null)
				throw new ArgumentNullException("ep");
			
            state.SetListening();
            
            NPx.Log.Debug("KayakServer will bind to " + ep.ToString());
            
            listener.Bind(ep);
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
            listener.Listen((int)SocketOptionName.MaxConnections);
			
            NPx.Log.Debug("KayakServer bound to " + ep.ToString());
			
            AcceptNext();
            return new Disposable(() => Close());
        }

        void Close()
        {
            var closed = state.SetClosing();
            
            NPx.Log.Debug("Closing listening socket.");
            listener.Close();

            if (closed)
                RaiseOnClose();
        }

        internal void SocketClosed(DefaultKayakSocket socket)
        {
            //NPx.Log.Debug("Connection " + socket.id + ": closed (" + connections + " active connections)");
            if (state.DecrementConnections())
                RaiseOnClose();
        }

        void RaiseOnClose()
        {
            del.OnClose(this);
        }

        void AcceptNext()
        {
            try
            {
                NPx.Log.Debug("KayakServer: accepting connection");
                listener.BeginAccept(iasr =>
                {
                    NPx.Log.Debug("KayakServer: accepted connection callback");
                    Socket socket = null;
                    Exception error = null;
                    try
                    {
                        socket = listener.EndAccept(iasr);
                        AcceptNext();
                    }
                    catch (Exception e)
                    {
                        error = e;
                    }

                    if (error is ObjectDisposedException)
                        return;

                    scheduler.Post(() =>
                    {
                        NPx.Log.Debug("KayakServer: accepted connection");
                        if (error != null)
                            HandleAcceptError(error);

                        var s = new DefaultKayakSocket(new SocketWrapper(socket), this.scheduler);
                        state.IncrementConnections();

                        var socketDelegate = del.OnConnection(this, s);
                        s.del = socketDelegate;
                        s.BeginRead();
                    });

                }, null);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception e)
            {
                HandleAcceptError(e);
            }
        }

        void HandleAcceptError(Exception e)
        {
            state.SetError();

            try
            {
                listener.Close();
            }
            catch { }

            NPx.Log.Debug("Error attempting to accept connection.");
            Console.Error.WriteStackTrace(e);

            RaiseOnClose();
        }
    }
}
