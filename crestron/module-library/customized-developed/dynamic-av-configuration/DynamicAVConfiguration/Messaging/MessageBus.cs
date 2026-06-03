using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace DynamicAVConfiguration.Messaging
{
    public interface ISubscription<T> where T : IMessage
    {
        Action<T> MessageHandler { get; }
    }

    public class Subscription<T> : ISubscription<T> where T : IMessage
    {
        public Action<T> MessageHandler { get; private set; }

        public Subscription(Action<T> action)
        {
            this.MessageHandler = action;
        }
    }

    public class MessageBus
    {
        private readonly Dictionary<Type, List<object>> subscribers = new Dictionary<Type, List<object>>();

        public bool IsDebug;

        public MessageBus()
        {
        }

        public void Publish<T>(T msg) where T : IMessage
        {
            if (msg == null) throw new ArgumentNullException(typeof(T).ToString());

            Type messageType = typeof(T);

            if (subscribers.ContainsKey(messageType))
            {
                //create a list that we can iterate through so we dont throw invalidoperation because multiple threads
                List<object> subscriptions = new List<object>();

                //get the lock on the dictionary
                lock (subscribers) subscriptions = subscribers[messageType].ToList();

                //if there is nothing subscribed, do nothing & post a message about it
                if (subscriptions == null || subscriptions.Count == 0)
                {
                    if (this.IsDebug) CrestronConsole.PrintLine("Event Bus | Nothing Is Currently Subscribed To {0}", messageType);
                }
                else
                {
                    //notify all subscribers
                    foreach (var handler in subscriptions.Select(s => s as ISubscription<T>).Select(s => s.MessageHandler))
                    {
                        if (handler != null) handler.Invoke(msg);
                    }
                }
            }
        }

        public ISubscription<T> Subscribe<T>(Action<T> callback) where T : IMessage
        {
            ISubscription<T> subscription = null;

            Type messageType = typeof(T);

            //create a list that we can iterate through so we dont throw invalidoperation because multiple threads
            List<object> subscriptions = new List<object>();

            //get lock on dictionary & create a new list, or get a copy of the existing one | this prevents InvalidOperationExceptions from multiple threads trying to access the same dictionary.
            lock (this.subscribers) subscriptions = subscribers.ContainsKey(messageType) ? subscribers[messageType].ToList() : new List<object>();
            //create a subscription
            subscription = new Subscription<T>(callback);
            //if this subscription doesnt currently exist, add it
            if (!subscriptions.Select(s => s as ISubscription<T>).Any(s => s.MessageHandler == callback))
            {
                //CrestronConsole.PrintLine("Event Bus | Adding Subscription: {0}", callback);
                subscriptions.Add(subscription);
            }
            //else {  CrestronConsole.PrintLine("Event Bus | Subscription Already Exists: {0}", callback); }

            //replace the list with the added new list of total subscribers in this dict index after acquiring lock
            lock (this.subscribers) this.subscribers[messageType] = subscriptions;

            return subscription;
        }

        public bool UnSubscribe<T>(ISubscription<T> subscription) where T : IMessage
        {
            bool removed = false;

            if (subscription == null) return false;

            Type messageType = typeof(T);

            //acquire a lock on dictionary
            lock (subscribers)
            {
                //check if there is a key, if not will throw exception
                if (subscribers.ContainsKey(messageType))
                {
                    int numRemoved = 0;

                    //remove all instances (if somehow there are multiple that were added to the dictionary) after 
                    numRemoved = this.subscribers[messageType].RemoveAll(s => s == subscription);

                    //if we removed any subscribers, we succeeded in some way
                    if (numRemoved > 0)
                    {
                        if (this.IsDebug) CrestronConsole.PrintLine("Event Bus | Removed {0} Subscribers To {1}", numRemoved, messageType);
                        removed = true;
                    }

                    //no point in keeping and index we dont need, so remove it if there are now no subscribers

                    if (subscribers[messageType].Count == 0)
                    {
                        if (this.IsDebug) CrestronConsole.PrintLine("Event Bus | Removing Subscription Channel For {0}", messageType);
                        subscribers.Remove(messageType);
                    }
                }
            }
            //release lock

            return removed;
        }
    }
}