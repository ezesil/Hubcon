﻿using Hubcon.SignalR.Server;
using HubconTestDomain;

namespace HubconTest.Controllers
{
    public class TestSignalRController : BaseHubController<ITestClientController>, IServerHubContract
    {
        public async IAsyncEnumerable<string> GetMessages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return "hola2";
            }
        }

        public async Task<int> GetTemperatureFromServer() => await Task.Run(() => new Random().Next(-10, 50));

        public async Task PrintMessage(string message)
        {
            await Task.Run(() => {          
                string message2 = "PONG";
                Client.ShowMessage(message2);
            });
        }

        public async Task ShowTempOnServerFromClient()
        {
            Console.WriteLine(await Client.GetTemperature());
        }

        public Task ShowTextOnServer()
        {
            Console.WriteLine("Mostrando texto");
            return Task.CompletedTask;
        }
    }
}
