using System;
using System.Reflection;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using RebusConfiguration;

namespace App1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //The App1 will initiate its handlers and publish the messages.
            //The App2 only initiate its handlers.
            var activator = new BuiltinHandlerActivator()
                .Register(() => new FirstHandlerTeste());
                
            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://192.168.99.100", Assembly.GetEntryAssembly().GetName().Name))
                .Routing(r => r.TypeBased()
                    .Map<TesteMessage>("appMessage")) // I tried to Map this TesteMessage to a destination "appMessage",  
                .Start();                             // and did the same thing on the App2 project.
            
            activator.Bus.Subscribe<TesteMessage>().Wait();
            activator.Bus.Subscribe<SharedMessages>().Wait();
            
            activator.Bus.Publish(new TesteMessage("Mensagem Teste.")).Wait();
            // This TesteMessage is going to invoke only the handler on this File.
            // What I want is to this TesteMessage to invoke the handler on the App2.
            
            activator.Bus.Publish(new SharedMessages("Mensagem Shared.")).Wait();
            // This SharedMessages is going to invoke the handlers on App1 and App2.
            // But I'm trying to create a structure without shared libs.
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
    
    public class FirstHandlerTeste: IHandleMessages<TesteMessage>, IHandleMessages<SharedMessages>
    {
        public async Task Handle(TesteMessage message)
        {
            Console.WriteLine("App1 TesteMessage: "+message.Teste);
        }

        public async Task Handle(SharedMessages message)
        {
            Console.WriteLine("App1 SharedMessages: "+message.shared);
        }
    }
}