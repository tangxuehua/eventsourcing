CREATE TABLE [dbo].[EventSourcing_Sample_Event](
    [UniqueId] [nvarchar](64) NOT NULL,
    [AggregateRootName] [nvarchar](512) NOT NULL,
    [AggregateRootId] [nvarchar](64) NOT NULL,
    [Version] [bigint] NOT NULL,
    [Name] [nvarchar](512) NOT NULL,
    [OccurredTime] [datetime] NOT NULL,
    [Data] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_Event] PRIMARY KEY CLUSTERED
(
    [UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[EventSourcing_Sample_Version](
    [AggregateRootId] [nvarchar](64) NOT NULL,
    [Version] [bigint] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_Version] PRIMARY KEY CLUSTERED
(
    [AggregateRootId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Snapshot](
    [UniqueId] [nvarchar](64) NOT NULL,
    [AggregateRootName] [nvarchar](512) NOT NULL,
    [AggregateRootId] [nvarchar](64) NOT NULL,
    [Version] [bigint] NOT NULL,
    [Name] [nvarchar](512) NOT NULL,
    [SerializedData] [nvarchar](max) NOT NULL,
    [CreatedTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_Snapshot] PRIMARY KEY CLUSTERED
(
    [UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Subscription](
    [UniqueId] [nvarchar](64) NOT NULL,
    [SubscriberAddress] [nvarchar](512) NOT NULL,
    [MessageType] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_Subscription] PRIMARY KEY CLUSTERED
(
    [UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[EventSourcing_Sample_BankAccount](
	[Id] [uniqueidentifier] NOT NULL,
	[AccountNumber] [nvarchar](128) NOT NULL,
	[Customer] [nvarchar](256) NOT NULL,
	[Balance] [float] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_BankAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Order](
	[Id] [uniqueidentifier] NOT NULL,
	[Customer] [nvarchar](256) NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_OrderItem](
	[OrderId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Price] [float] NOT NULL,
	[Amount] [int] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_OrderItem] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Product](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [float] NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_LibraryAccount](
	[Id] [uniqueidentifier] NOT NULL,
	[Number] [nvarchar](128) NOT NULL,
	[Owner] [nvarchar](256) NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_LibraryAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Library](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](512) NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_Library] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_BookStoreItem](
	[LibraryId] [uniqueidentifier] NOT NULL,
	[BookId] [uniqueidentifier] NOT NULL,
	[Count] int NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_BookStoreItem] PRIMARY KEY CLUSTERED 
(
	[LibraryId] ASC,
	[BookId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_BorrowedBook](
	[LibraryId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[BookId] [uniqueidentifier] NOT NULL,
	[Count] int NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_BorrowedBook] PRIMARY KEY CLUSTERED 
(
	[LibraryId] ASC,
	[AccountId] ASC,
	[BookId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Book](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[ISBN] [nvarchar](256) NOT NULL,
	[Author] [nvarchar](256) NOT NULL,
	[Publisher] [nvarchar](512) NOT NULL,
	[Description] [nvarchar](max) NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_Book] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_HandlingEvent](
	[Id] [uniqueidentifier] NOT NULL,
	[LibraryId] [uniqueidentifier] NOT NULL,
	[BookId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[HandlingType] int NOT NULL,
	[Time] [datetime] NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_HandlingEvent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Forum](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[TotalThread] [int] NOT NULL,
	[TotalPost] [int] NOT NULL,
	[LatestThreadId] [uniqueidentifier] NULL,
	[LatestPostAuthorId] [uniqueidentifier] NULL
 CONSTRAINT [PK_EventSourcing_Sample_Forum] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_User](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Thread](
	[Id] [uniqueidentifier] NOT NULL,
	[Subject] [nvarchar](256) NOT NULL,
	[Body] [nvarchar](max) NULL,
	[ForumId] [uniqueidentifier] NOT NULL,
	[AuthorId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[Marks] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[IsStick] [bit] NOT NULL,
	[StickDate] [datetime] NULL
 CONSTRAINT [PK_EventSourcing_Sample_Thread] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Post](
	[Id] [uniqueidentifier] NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[ThreadId] [uniqueidentifier] NOT NULL,
	[AuthorId] [uniqueidentifier] NOT NULL,
	[CreateTime] [datetime] NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_Post] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_BalanceChangeHistory](
	[Id] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[ChangeType] int NOT NULL,
	[Amount] float NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Time] [datetime] NOT NULL
 CONSTRAINT [PK_EventSourcing_Sample_BalanceChangeHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_Note](
	[Id] [uniqueidentifier] NOT NULL,
	[BookId] [uniqueidentifier] NULL,
	[Title] [nvarchar](256) NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[UpdatedTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EventSourcing_Sample_Note] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventSourcing_Sample_NoteBook](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[TotalNoteCount] int NULL,
 CONSTRAINT [PK_EventSourcing_Sample_NoteBook] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO