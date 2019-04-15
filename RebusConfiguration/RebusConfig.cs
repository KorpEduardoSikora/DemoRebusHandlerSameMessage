using System.Reflection;
using Rebus.Activation;
using Rebus.Config;

namespace RebusConfiguration
{
    public class RebusConfig
    {
        public void InitConfig(BuiltinHandlerActivator activator)
        {
            Configure.With(activator)
                .Transport(t => t.UseRabbitMq("amqp://192.168.99.100", Assembly.GetEntryAssembly().GetName().Name))
                .Start();
        }
    }
    
}