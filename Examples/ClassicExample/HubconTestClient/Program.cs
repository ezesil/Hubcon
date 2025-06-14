﻿using Hubcon.Client;
using HubconTestClient.Auth;
using HubconTestClient.Modules;
using HubconTestDomain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HubconTestClient
{
    internal class Program
    {
        private const string Url = "http://localhost:5000/clienthub";

        static async Task Main()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddHubconClient();
            builder.Services.AddRemoteServerModule<TestModule>();

            var app = builder.Build();
            var scope = app.Services.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IUserService>();
            var authManager = scope.ServiceProvider.GetRequiredService<AuthenticationManager>();
            var client2 = scope.ServiceProvider.GetRequiredService<ISecondTestContract>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IUserService>>();


            logger.LogInformation("Esperando interacción antes de continuar...");
            Console.ReadKey();

            Console.WriteLine($"Iniciando ingest...");
            IAsyncEnumerable<string> source1 = GetMessages(5);
            IAsyncEnumerable<string> source2 = GetMessages(5);
            await client.IngestMessages(source1, source2);
            Console.WriteLine($"Ingest terminado.");

            Console.ReadKey();

            var result = await authManager.LoginAsync("miusuario", "");
            logger.LogInformation($"Login result: {result.IsSuccess}");

            Console.ReadKey();

            var text = client2.TestReturn();


            logger.LogInformation($"TestVoid llamado. Texto recibido: {text}");
            Console.ReadKey();

            logger.LogDebug("Conectando evento...");

            int eventosRecibidos = 0;

            async Task handler(int input)
            {
                //logger.LogInformation($"Evento recibido: {input}");
                Interlocked.Add(ref eventosRecibidos, 1);
            }

            //client.OnUserCreated!.AddHandler(handler);
            //await client.OnUserCreated.Subscribe();
            logger.LogInformation("Evento conectado.");

            Console.ReadKey();

            logger.LogInformation("Enviando request...");
            await client.CreateUser();
            logger.LogInformation($"Request terminado.");

            Console.ReadKey();

            logger.LogInformation("Enviando request GetTemperatureFromServer...");
            var temp = await client.GetTemperatureFromServer();
            logger.LogInformation($"Datos recibidos: {temp}");

            Console.ReadKey();

            logger.LogInformation("Enviando request...");

            await foreach (var item in client.GetMessages(10))
            {
                logger.LogInformation($"Respuesta recibida: {item}");
            }

            Console.ReadKey();

            int finishedRequestsCount = 0;
            int errors = 0;
            int lastRequests = 0;
            int maxReqs = 0;
            var sw = new Stopwatch();
            var worker = new System.Timers.Timer();
            worker.Interval = 1000;
            worker.Elapsed += (sender, eventArgs) =>
            {
                var avgRequestsPerSec = finishedRequestsCount - lastRequests;
                var nanosecs = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1_000;
                maxReqs = maxReqs < avgRequestsPerSec ? avgRequestsPerSec : maxReqs;
                logger.LogInformation($"Requests: {finishedRequestsCount} | Avg requests/s:{avgRequestsPerSec} | Max req/s: {maxReqs}| Received events: {eventosRecibidos} | Avg request time: {nanosecs / avgRequestsPerSec}");
                lastRequests = finishedRequestsCount;
                sw.Restart();
            };
            worker.Start();

            var tasks = Enumerable.Range(5, 5).Select(_ => Task.Run(async () =>
            {
                while (true)
                {
                    await client.CreateUser();
                    Interlocked.Add(ref finishedRequestsCount, 1);
                }
            })).ToArray();

            await Task.WhenAll(tasks);
        }

        static async IAsyncEnumerable<string> GetMessages(int count)
        {
            for(int i = 0; i < count; i++)
            {
                var message = $"string:{i}";
                Console.WriteLine($"Enviando mensaje... [{message}]");
                yield return message;
                await Task.Delay(1000);
            }
        }
    }
}