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
            var activator = new BuiltinHandlerActivator()
                .Register(() => new FirstHandlerTeste());
                
            
            //var config = new RebusConfig();
            
            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://192.168.99.100", Assembly.GetEntryAssembly().GetName().Name))
                .Routing(r => r.TypeBased()
                    .Map(typeof(TesteMessage),"appMessage1234"))
                .Start();
            //config.InitConfig(activator);
            
            activator.Bus.Subscribe<TesteMessage>().Wait();

            activator.Bus.Subscribe<SharedMessages>().Wait();
            //activator.Bus.Advanced.Topics.Subscribe("appMessage").Wait();
            
            activator.Bus.Publish(new TesteMessage("Mensagem Teste.")).Wait();
            //activator.Bus.Send(new TesteMessage("Mensagem Teste.")).Wait();
            //activator.Bus.Publish(new SharedMessages("Mensagem Shared.")).Wait();
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