﻿namespace Pulsar4X.ECSLib
{
    internal interface IMessageHandler
    {
        /// <summary>
        /// Handles a message from the MessagePump.
        /// </summary>
        /// <returns>True if the message was handled properly.</returns>
        bool HandleMessage(Game game, IncomingMessageType messageType, AuthenticationToken authToken, string message);
    }

    /// <summary>
    /// Handles simple game messages.
    /// </summary>
    internal class GameMessageHandler : IMessageHandler
    {
        public bool HandleMessage(Game game, IncomingMessageType messageType, AuthenticationToken authToken, string message)
        {
            switch (messageType)
            {
                case IncomingMessageType.Exit:
                    game.ExitRequested = true;
                    return true;
                case IncomingMessageType.ExecutePulse:
                    game.GameLoop.TimeStep();
                    return true;
                case IncomingMessageType.StartRealTime:
                    float timeMultiplier;
                    if (float.TryParse(message, out timeMultiplier))
                    {
                        game.GameLoop.TimeMultiplier = timeMultiplier;
                    }
                    game.GameLoop.StartTime();
                    return true;
                case IncomingMessageType.StopRealTime:
                    game.GameLoop.PauseTime();
                    return true;
                case IncomingMessageType.Echo:
                    game.MessagePump.EnqueueOutgoingMessage(OutgoingMessageType.Echo, message);
                    return true;

                default: return false;
            }
        }
    }
}