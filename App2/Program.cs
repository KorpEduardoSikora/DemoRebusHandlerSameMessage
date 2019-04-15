using System;
using System.Reflection;
using System.Threading.Tasks;
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
            //The App2 only initiate its handlers.
            //The App1 will initiate its handlers and publish the messages.
            
            var activator = new BuiltinHandlerActivator()
                .Register(() => new SecondHandlerTeste());
            
            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://192.168.99.100", Assembly.GetEntryAssembly().GetName().Name))
                .Routing(r => r.TypeBased()
                    .Map<TesteMessage>("appMessage")) // I tried to Map this TesteMessage to a destination "appMessage",  
                .Start();                             // and did the same thing on the App1 project.
            
            activator.Bus.Subscribe<TesteMessage>().Wait();
            activator.Bus.Subscribe<SharedMessages>().Wait();
            
            //activator.Bus.Advanced.Topics.Subscribe("appMessage").Wait();
        }
    }
    
    public class TesteMessage
    {
        // This class is declared on both App1 and App2 projects.
        // I want this message to be published to both Handlers.
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