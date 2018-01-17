﻿using System;
using System.AddIn.Contract;
using RedGate.AppHost.Interfaces;

namespace RedGate.AppHost.Remoting.WPF
{
    internal class NativeHandleContractAdapter : INativeHandleContract
    {
        private readonly IRemoteElement m_Upstream;

        internal NativeHandleContractAdapter(IRemoteElement upstream)
        {
            if (upstream == null)
            {
                throw new ArgumentNullException("upstream");
            }
            m_Upstream = upstream;
        }


        public IContract QueryContract(string contractIdentifier)
        {
            return m_Upstream.QueryContract(contractIdentifier);
        }

        public int GetRemoteHashCode()
        {
            return m_Upstream.GetRemoteHashCode();
        }

        public bool RemoteEquals(IContract contract)
        {
            return m_Upstream.RemoteEquals(contract);
        }

        public string RemoteToString()
        {
            return m_Upstream.RemoteToString();
        }

        public int AcquireLifetimeToken()
        {
            return m_Upstream.AcquireLifetimeToken();
        }

        public void RevokeLifetimeToken(int token)
        {
            try
            {
                m_Upstream.RevokeLifetimeToken(token);
            } catch (System.Runtime.Remoting.RemotingException e) when (e.Message == "Failed to connect to an IPC Port: The system cannot find the file specified.")
            {
                //I don't know why these messages get thrown and I don't know if raising them causes problems but they were annoying me.  akittredge, January 2018.
            }            
        }

        public IntPtr GetHandle()
        {
            return (IntPtr)m_Upstream.GetHandle();
        }
    }
}