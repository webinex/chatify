﻿namespace Webinex.Chatify.Abstractions.Events;

public record ChatNameChangedEvent(Guid ChatId, string NewName, Message Message);