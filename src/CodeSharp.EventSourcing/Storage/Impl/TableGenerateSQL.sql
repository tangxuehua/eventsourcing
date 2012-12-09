--Copyright (c) CodeSharp.  All rights reserved.

CREATE TABLE [dbo].[EventSourcing_SourcableEvent](
    [UniqueId] [nvarchar](64) NOT NULL,
    [AggregateRootName] [nvarchar](512) NOT NULL,
    [AggregateRootId] [nvarchar](64) NOT NULL,
    [Version] [bigint] NOT NULL,
    [Name] [nvarchar](512) NOT NULL,
    [OccurredTime] [datetime] NOT NULL,
    [Data] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_EventSourcing_SourcableEvent] PRIMARY KEY CLUSTERED
(
    [UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_AggregateRootVersion](
    [AggregateRootId] [nvarchar](64) NOT NULL,
    [Version] [bigint] NOT NULL,
 CONSTRAINT [PK_EventSourcing_AggregateRootVersion] PRIMARY KEY CLUSTERED
(
    [AggregateRootId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Snapshot](
    [UniqueId] [nvarchar](64) NOT NULL,
    [AggregateRootName] [nvarchar](512) NOT NULL,
    [AggregateRootId] [nvarchar](64) NOT NULL,
    [Version] [bigint] NOT NULL,
    [Name] [nvarchar](512) NOT NULL,
    [SerializedData] [nvarchar](max) NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Snapshot] PRIMARY KEY CLUSTERED
(
    [UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_SubscriptionStore](
    [UniqueId] [nvarchar](64) NOT NULL,
    [SubscriberAddress] [nvarchar](512) NOT NULL,
    [MessageType] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_EventSourcing_SubscriptionStore] PRIMARY KEY CLUSTERED
(
    [UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
