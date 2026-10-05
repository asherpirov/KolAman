using System;
using System.IO;
using NotificationGate.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NotificationGate.Settings;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(
builder.Configuration.GetSection("Kafka"));

builder.Services.Configure<FileWatcherSettings>(
builder.Configuration.GetSection("FileWatcher"));

builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddSingleton<KafkaTopicManager>();
builder.Services.AddSingleton<FileWatcherService>();

var host = builder.Build();

var topicManager = host.Services.GetRequiredService<KafkaTopicManager>();

await topicManager.EnsureTopicExistsAsync();

var fileWatcher = host.Services.GetRequiredService<FileWatcherService>();

fileWatcher.Start();

await host.RunAsync();