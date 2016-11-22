using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using ARSoft.Tools.Net.Dns;
using ARSoft.Tools.Net;

namespace DomainService.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDomainActor : IActor
    {
       
        Task<DnsMessage> Resolve(DomainName name,RecordType type, RecordClass recordClass);
      
    }
}
