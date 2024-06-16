using System;
using System.Linq;

namespace AchtuurCore.Events;

public static class EventPublisher
{
    internal static void InvokeEvent(EventHandler handler, object sender)
    {
        if (handler is null)
            return;

        foreach (var handle in handler.GetInvocationList().Cast<EventHandler>())
        {
            try
            {
                handle.Invoke(sender, new EventArgs());
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Something went wrong when handling event {handle} ({sender}):\n{e}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }

    
}
