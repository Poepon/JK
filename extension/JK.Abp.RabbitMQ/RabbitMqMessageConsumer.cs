using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Threading;
using Abp.Threading.Timers;
using Castle.Core.Logging;
using JetBrains.Annotations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JK.Abp.RabbitMQ
{
    public class RabbitMqMessageConsumer : IRabbitMqMessageConsumer, ITransientDependency, IDisposable
    {
        public ILogger Logger { get; set; }

        protected IConnectionPool ConnectionPool { get; }

        protected AbpTimer Timer { get; }

        protected ExchangeDeclareConfiguration Exchange { get; private set; }

        protected QueueDeclareConfiguration Queue { get; private set; }

        protected QOSConfiguration ConsumerConfiguration { get; private set; }

        protected string ConnectionName { get; private set; }

        protected ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>> Callbacks { get; }

        protected IModel Channel { get; private set; }

        protected ConcurrentQueue<QueueBindCommand> QueueBindCommands { get; }

        protected object ChannelSendSyncLock { get; } = new object();

        public RabbitMqMessageConsumer(
            IConnectionPool connectionPool,
            AbpTimer timer)
        {
            ConnectionPool = connectionPool;
            Timer = timer;
            Logger = NullLogger.Instance;

            QueueBindCommands = new ConcurrentQueue<QueueBindCommand>();
            Callbacks = new ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>>();

            Timer.Period = 5000; //5 sec.
            Timer.Elapsed += Timer_Elapsed;
            Timer.RunOnStart = true;
        }

        public void Initialize(
            [NotNull] ExchangeDeclareConfiguration exchange,
            [NotNull] QueueDeclareConfiguration queue,
            [NotNull]QOSConfiguration consumerConfiguration,
            string connectionName = null)
        {
            Exchange = Check.NotNull(exchange, nameof(exchange));
            Queue = Check.NotNull(queue, nameof(queue));
            ConsumerConfiguration = Check.NotNull(consumerConfiguration, nameof(consumerConfiguration));
            ConnectionName = connectionName;
            Timer.Start();
        }

        public virtual async Task BindAsync(string routingKey)
        {
            QueueBindCommands.Enqueue(new QueueBindCommand(QueueBindType.Bind, routingKey));
            await TrySendQueueBindCommandsAsync();
        }

        public virtual async Task UnbindAsync(string routingKey)
        {
            QueueBindCommands.Enqueue(new QueueBindCommand(QueueBindType.Unbind, routingKey));
            await TrySendQueueBindCommandsAsync();
        }

        protected virtual Task TrySendQueueBindCommandsAsync()
        {
            try
            {
                while (!QueueBindCommands.IsEmpty)
                {
                    if (Channel == null || Channel.IsClosed)
                    {
                        return Task.CompletedTask;
                    }

                    lock (ChannelSendSyncLock)
                    {
                        QueueBindCommands.TryPeek(out var command);

                        switch (command.Type)
                        {
                            case QueueBindType.Bind:
                                Channel.QueueBind(
                                    queue: Queue.QueueName,
                                    exchange: Exchange.ExchangeName,
                                    routingKey: command.RoutingKey
                                );
                                break;
                            case QueueBindType.Unbind:
                                Channel.QueueUnbind(
                                    queue: Queue.QueueName,
                                    exchange: Exchange.ExchangeName,
                                    routingKey: command.RoutingKey
                                );
                                break;
                            default:
                                throw new AbpException($"Unknown {nameof(QueueBindType)}: {command.Type}");
                        }

                        QueueBindCommands.TryDequeue(out command);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            return Task.CompletedTask;
        }

        public virtual void OnMessageReceived(Func<IModel, BasicDeliverEventArgs, Task> callback)
        {
            Callbacks.Add(callback);
        }

        protected virtual void Timer_Elapsed(object sender, EventArgs e)
        {
            if (Channel == null || Channel.IsOpen == false)
            {
                TryCreateChannel();
                AsyncHelper.RunSync(TrySendQueueBindCommandsAsync);
            }
        }

        protected virtual void TryCreateChannel()
        {
            DisposeChannel();

            try
            {
                var channel = ConnectionPool
                    .Get(ConnectionName)
                    .CreateModel();

                channel.ExchangeDeclare(
                    exchange: Exchange.ExchangeName,
                    type: Exchange.Type,
                    durable: Exchange.Durable,
                    autoDelete: Exchange.AutoDelete,
                    arguments: Exchange.Arguments
                );

                channel.QueueDeclare(
                    queue: Queue.QueueName,
                    durable: Queue.Durable,
                    exclusive: Queue.Exclusive,
                    autoDelete: Queue.AutoDelete,
                    arguments: Queue.Arguments
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, basicDeliverEventArgs) =>
                {
                    await HandleIncomingMessage(channel, basicDeliverEventArgs);
                };
                channel.BasicQos(0, ConsumerConfiguration.PrefetchCount, ConsumerConfiguration.Global);
                channel.BasicConsume(
                    queue: Queue.QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                Channel = channel;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        protected virtual async Task HandleIncomingMessage(IModel channel, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            try
            {
                foreach (var callback in Callbacks)
                {
                    await callback(channel, basicDeliverEventArgs);
                }

                channel.BasicAck(basicDeliverEventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        protected virtual void DisposeChannel()
        {
            if (Channel == null)
            {
                return;
            }

            try
            {
                Channel.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        public virtual void Dispose()
        {
            Timer.Stop();
            DisposeChannel();
        }

        protected class QueueBindCommand
        {
            public QueueBindType Type { get; }

            public string RoutingKey { get; }

            public QueueBindCommand(QueueBindType type, string routingKey)
            {
                Type = type;
                RoutingKey = routingKey;
            }
        }

        protected enum QueueBindType
        {
            Bind,
            Unbind
        }
    }
}
