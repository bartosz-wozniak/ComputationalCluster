# Architecture Overview

## Intro

Each node is highly independent and highly parallel. This means that we need to take into account concurrent bugs. To mitigate the problem, **event bus** pattern is being used. This allows us to decentralize communication and design for concurrency from the beginning (but we still need to consider locks in some places).

## Guidelines

If you don't need to react to some action (or you just want to let some other component do the work), use command pattern. If you do need to react to, but you know that the work may be done independently (probably async), use command pattern. Otherwise, use normal method calls.

## Communication with CS

If you want to send a message to CS, use `IServerClient`. It provides a single method that sends a message and waits for the response (if needed). It assumes, that the response does not need to be processed synchronously and puts it in the event bus. Should you process the response just after the request, try to redesign your flow because it probably does not need to be processed there.

After registering with the CS, start the `StatusMessageSender` on a separate thread. It will periodically send `StatusMessage` to the CS and put a response into the event bus. If it detects problems with the CS, a corresponding notification will be published.

## Summary

 1. Use commands and notifications (queries probably won't be needed),
 2. Contact the CS only with `IServerClient`,
 3. Prepare for asynchronicity and locks,