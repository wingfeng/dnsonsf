using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Fabric;
using ARSoft.Tools.Net.Dns;
using Microsoft.ServiceFabric.Data;

namespace DnsService
{
    public class DnsListener : ICommunicationListener
    {
        ARSoft.Tools.Net.Dns.DnsServer _server;
        DnsClient client;
        public IReliableStateManager ActorTable { get; set; }
        public DnsListener(StatefulServiceContext context) {
            _server = new ARSoft.Tools.Net.Dns.DnsServer(10, 10);
            _server.QueryReceived += _server_QueryReceived;
            client = new DnsClient(System.Net.IPAddress.Parse("114.114.114.114"), 5);
           
        }

        private Task _server_QueryReceived(object sender, QueryReceivedEventArgs e)
        {
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return Task.FromResult(0);

            DnsMessage response = query.CreateResponseInstance();

            if ((query.Questions.Count == 1))
            {
                // send query to upstream server
                DnsQuestion question = query.Questions[0];
                DnsMessage answer = DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass).Result;

                // if got an answer, copy it to the message sent to the client
                if (answer != null)
                {
                    foreach (DnsRecordBase record in (answer.AnswerRecords))
                    {
                        response.AnswerRecords.Add(record);
                    }
                    foreach (DnsRecordBase record in (answer.AdditionalRecords))
                    {
                        response.AdditionalRecords.Add(record);
                    }

                    response.ReturnCode = ReturnCode.NoError;

                    // set the response
                    e.Response = response;
                }
            }
            return Task.FromResult(0);
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _server.Stop();
            return Task.FromResult(0);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            _server.Start();
            return Task.FromResult<string>("DNS Server started!");
        }
    }
}
