USE [master]
GO
/****** Object:  Database [camdoanhtu]    Script Date: 7/5/2020 11:18:55 AM ******/
CREATE DATABASE [camdoanhtu]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'camdoanhtu', FILENAME = N'C:\Program Files (x86)\Plesk\Databases\MSSQL\MSSQL12.MSSQLSERVER2014\MSSQL\DATA\camdoanhtu.mdf' , SIZE = 22720KB , MAXSIZE = 1024000KB , FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'camdoanhtu_log', FILENAME = N'C:\Program Files (x86)\Plesk\Databases\MSSQL\MSSQL12.MSSQLSERVER2014\MSSQL\DATA\camdoanhtu_log.ldf' , SIZE = 1024KB , MAXSIZE = 1024000KB , FILEGROWTH = 10%)
GO
ALTER DATABASE [camdoanhtu] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [camdoanhtu].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [camdoanhtu] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [camdoanhtu] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [camdoanhtu] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [camdoanhtu] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [camdoanhtu] SET ARITHABORT OFF 
GO
ALTER DATABASE [camdoanhtu] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [camdoanhtu] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [camdoanhtu] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [camdoanhtu] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [camdoanhtu] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [camdoanhtu] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [camdoanhtu] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [camdoanhtu] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [camdoanhtu] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [camdoanhtu] SET  DISABLE_BROKER 
GO
ALTER DATABASE [camdoanhtu] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [camdoanhtu] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [camdoanhtu] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [camdoanhtu] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [camdoanhtu] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [camdoanhtu] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [camdoanhtu] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [camdoanhtu] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [camdoanhtu] SET  MULTI_USER 
GO
ALTER DATABASE [camdoanhtu] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [camdoanhtu] SET DB_CHAINING OFF 
GO
ALTER DATABASE [camdoanhtu] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [camdoanhtu] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [camdoanhtu] SET DELAYED_DURABILITY = DISABLED 
GO
USE [camdoanhtu]
GO
/****** Object:  User [camdoanhtu]    Script Date: 7/5/2020 11:18:56 AM ******/
CREATE USER [camdoanhtu] FOR LOGIN [camdoanhtu] WITH DEFAULT_SCHEMA=[camdoanhtu]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [camdoanhtu]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [camdoanhtu]
GO
ALTER ROLE [db_datareader] ADD MEMBER [camdoanhtu]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [camdoanhtu]
GO
/****** Object:  Schema [camdoanhtu]    Script Date: 7/5/2020 11:18:57 AM ******/
CREATE SCHEMA [camdoanhtu]
GO
/****** Object:  Schema [camdoanhtu1]    Script Date: 7/5/2020 11:18:57 AM ******/
CREATE SCHEMA [camdoanhtu1]
GO
/****** Object:  Table [camdoanhtu].[history]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [camdoanhtu].[history](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Ngaydongtien] [datetime] NULL,
	[Detail] [nvarchar](max) NULL,
	[price] [decimal](18, 0) NULL,
	[status] [int] NULL,
	[loanid] [int] NULL,
	[CustomerCode] [nvarchar](50) NULL,
 CONSTRAINT [PK__history__3214EC2763781B7F] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [camdoanhtu].[Message]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [camdoanhtu].[Message](
	[Message] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Name] [nvarchar](50) NULL,
	[Phone] [varchar](50) NULL,
	[Address] [nvarchar](1000) NULL,
	[Loan] [decimal](18, 0) NULL,
	[Price] [decimal](18, 0) NULL,
	[AmountPaid] [decimal](18, 0) NULL,
	[RemainingAmount] [decimal](18, 0) NULL,
	[DayPaids] [int] NULL,
	[StartDate] [datetime] NOT NULL,
	[Description] [nvarchar](100) NULL,
	[NgayNo] [int] NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DayBonus] [int] NULL,
	[OldCode] [varchar](50) NULL,
	[Note] [nvarchar](1000) NULL,
	[CodeSort] [int] NULL,
	[type] [int] NULL,
	[songayno] [int] NULL,
	[nodung] [bit] NULL,
	[tentaisan] [nvarchar](max) NULL,
	[loaigiayto] [int] NULL,
	[tiengoc] [decimal](18, 0) NULL,
	[lai] [decimal](18, 0) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Code] [varchar](50) NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Code_Unique] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Loan]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Loan](
	[Date] [datetime] NOT NULL,
	[IDCus] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Status] [int] NOT NULL,
	[Type] [bit] NOT NULL,
	[money] [decimal](18, 0) NULL,
 CONSTRAINT [PK_Loan] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserName] [varchar](50) NULL,
	[PassWord] [varchar](50) NULL,
	[Permission] [int] NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[id_cuahang] [int] NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF__Customer__loaigi__607251E5]  DEFAULT ((1)) FOR [loaigiayto]
GO
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF__Customer__IsDele__29572725]  DEFAULT ('False') FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Loan] ADD  CONSTRAINT [type_default]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Enabled]  DEFAULT ((1)) FOR [Enabled]
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetCustomerEven]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [camdoanhtu].[GetCustomerEven]
	
AS
BEGIN
	select * from Customer cs where CONVERT(INT, SUBSTRING(cs.Code,2,LEN(cs.Code) - 1)) % 2 = 0
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetCustomerOdd]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [camdoanhtu].[GetCustomerOdd]

AS
BEGIN
	select * from Customer cs where CONVERT(INT, SUBSTRING(cs.Code,2,LEN(cs.Code) - 1)) % 2 = 1
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetTienGoc]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [camdoanhtu].[GetTienGoc] @type int
AS
BEGIN
	select Sum(cs.tiengoc)
from customer cs
where cs.StartDate > '2018-06-20'
and (@type = -1 or @type = -2 or cs.type=@type) and cs.IsDeleted = 0 and cs.Description is null 
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetTienGoc_Dung]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [camdoanhtu].[GetTienGoc_Dung] @type int
AS
BEGIN
	select Sum(cs.Loan)
from customer cs
where cs.StartDate > '2018-06-20' 
and cs.IsDeleted = 0 
and cs.Description is null 
and cs.type=@type
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetTienLai]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [camdoanhtu].[GetTienLai]  @type int
AS
BEGIN
	select Sum(cs.Loan - cs.tiengoc)
from customer cs
where cs.StartDate > '2018-06-20'
and (@type = -1 or @type = -2 or cs.type=@type) and cs.IsDeleted = 0 and cs.Description is null 
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetTienLaiThatTe]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE[camdoanhtu].[GetTienLaiThatTe] @type int,@date1 date, @date2 date
AS
BEGIN
	select Sum(cs.Loan - cs.tiengoc)
from customer cs
where cs.StartDate between @date1 and @date2
 and cs.Description like 'End' 
 and (@type = -1 or @type = -2 or cs.type=@type)
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[GetTienLaiThatTe_Dung]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE[camdoanhtu].[GetTienLaiThatTe_Dung] @type int,@date1 date, @date2 date
AS
BEGIN
	select Sum(cs.Price)
from customer cs, Loan l
where cs.StartDate between @date1 and @date2
 and cs.ID = l.IDCus and l.Status = 1
 and cs.type = @type
END
GO
/****** Object:  StoredProcedure [camdoanhtu].[SumMoneyByCode]    Script Date: 7/5/2020 11:18:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE[camdoanhtu].[SumMoneyByCode] @datetimeinput datetime,@type int
AS
BEGIN
	select SUBSTRING (hs.CustomerId, 1, 1) as subcode,sum(hs.Price) as sumval
	from history hs ,Customer cs,Loan l 
	where convert(date,Ngaydongtien) =  @datetimeinput
	and hs.CustomerId = cs.Code 
	and (@type = -1 or cs.type=@type)
	and hs.loanid = l.ID and l.Status = 1
	group by SUBSTRING (hs.CustomerId, 1, 1);
END
GO
USE [master]
GO
ALTER DATABASE [camdoanhtu] SET  READ_WRITE 
GO
