using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using RebusConfiguration;

namespace App2
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            var activator = new BuiltinHandlerActivator()
                .Register(() => new SecondHandlerTeste());
            
            //var config = new RebusConfig();
            //config.InitConfig(activator);
            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://192.168.99.100", Assembly.GetEntryAssembly().GetName().Name))
                .Routing(r => r.TypeBased()
                    .Map(typeof(TesteMessage),"appMessage"))
                .Start();
            
            activator.Bus.Subscribe<TesteMessage>().Wait();
            activator.Bus.Subscribe<SharedMessages>().Wait();
            activator.Bus.Advanced.Topics.Subscribe("appMessage").Wait();
        }
    }
    
    public class TesteMessage
    {
        public TesteMessage(string test)
        {
            Teste = test;
        }

        public string Teste { get; set; }
    }
    
    public class SecondHandlerTeste: IHandleMessages<TesteMessage>, IHandleMessages<SharedMessages>
    {
        public async Task Handle(TesteMessage message)
        {
            Console.WriteLine("App2 TesteMessage: "+message.Teste);
        }
        
        public async Task Handle(SharedMessages message)
        {
            Console.WriteLine("App2 SharedMessages: "+message.shared);
        }
    }
}