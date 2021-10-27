USE [AlpsSQLDB]
GO

/****** Object:  Table [dbo].[PartTransferIn]    Script Date: 9/5/2020 9:37:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PartTransferIn](
	[PartTransferInID] [int] IDENTITY(1,1) NOT NULL,
	[PartID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[BUID] [int] NOT NULL,
	[DrawerID] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](300) NULL,
	[IsActive] [bit] NOT NULL,
	[UserCreated] [nvarchar](10) NULL,
	[UserModified] [nvarchar](10) NULL,
	[DateCreated] [datetime] NULL,
	[DateModified] [datetime] NULL,
 CONSTRAINT [PK_dbo.PartTransferIn] PRIMARY KEY CLUSTERED 
(
	[PartTransferInID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PartTransferIn]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PartTransferIn_dbo.BUnit_ID] FOREIGN KEY([BUID])
REFERENCES [dbo].[BUnit] ([BUID])
GO

ALTER TABLE [dbo].[PartTransferIn] CHECK CONSTRAINT [FK_dbo.PartTransferIn_dbo.BUnit_ID]
GO

ALTER TABLE [dbo].[PartTransferIn]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PartTransferIn_dbo.DrawerID_ID] FOREIGN KEY([DrawerID])
REFERENCES [dbo].[Drawer] ([DrawerID])
GO

ALTER TABLE [dbo].[PartTransferIn] CHECK CONSTRAINT [FK_dbo.PartTransferIn_dbo.DrawerID_ID]
GO

ALTER TABLE [dbo].[PartTransferIn]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PartTransferIn_dbo.Part_ID] FOREIGN KEY([PartID])
REFERENCES [dbo].[PartMaster] ([PartID])
GO

ALTER TABLE [dbo].[PartTransferIn] CHECK CONSTRAINT [FK_dbo.PartTransferIn_dbo.Part_ID]
GO


drop table PartTransferIn
