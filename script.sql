USE [master]
GO
/****** Object:  Database [OnlineShop]    Script Date: 3/07/2024 3:15:24 AM ******/
CREATE DATABASE [OnlineShop]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'OnlineShop', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\OnlineShop.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'OnlineShop_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\OnlineShop_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [OnlineShop] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OnlineShop].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OnlineShop] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [OnlineShop] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [OnlineShop] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [OnlineShop] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [OnlineShop] SET ARITHABORT OFF 
GO
ALTER DATABASE [OnlineShop] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [OnlineShop] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [OnlineShop] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [OnlineShop] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [OnlineShop] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [OnlineShop] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [OnlineShop] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [OnlineShop] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [OnlineShop] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [OnlineShop] SET  DISABLE_BROKER 
GO
ALTER DATABASE [OnlineShop] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [OnlineShop] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [OnlineShop] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [OnlineShop] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [OnlineShop] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [OnlineShop] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [OnlineShop] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [OnlineShop] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [OnlineShop] SET  MULTI_USER 
GO
ALTER DATABASE [OnlineShop] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [OnlineShop] SET DB_CHAINING OFF 
GO
ALTER DATABASE [OnlineShop] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [OnlineShop] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [OnlineShop] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [OnlineShop] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'OnlineShop', N'ON'
GO
ALTER DATABASE [OnlineShop] SET QUERY_STORE = OFF
GO
USE [OnlineShop]
GO
/****** Object:  Table [dbo].[Cart]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cart](
	[CartId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_Cart] PRIMARY KEY CLUSTERED 
(
	[CartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartItem]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItem](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CartId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[StyleId] [int] NULL,
	[Count] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_CartItem] PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comments]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[CommentId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Content] [nvarchar](200) NOT NULL,
	[Rate] [int] NOT NULL,
	[Date] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_CommentId] PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItem]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItem](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[StyleId] [int] NULL,
	[Count] [int] NOT NULL,
 CONSTRAINT [PK_OrderItemId] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Receiver] [nvarchar](50) NOT NULL,
	[ShipperId] [int] NULL,
	[Address] [nvarchar](200) NOT NULL,
	[Phone] [varchar](10) NOT NULL,
	[VoucherId] [int] NULL,
	[StatusId] [int] NOT NULL,
	[IsPay] [int] NOT NULL,
	[Email] [varchar](100) NULL,
	[Date] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[SellerId] [int] NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[Decription] [nvarchar](1000) NOT NULL,
	[Price] [float] NOT NULL,
	[PromotionalPrice] [float] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Sold] [int] NOT NULL,
	[IsActive] [int] NOT NULL,
	[Image] [varchar](100) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Rating] [float] NOT NULL,
	[Date] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](100) NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatusOrder]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusOrder](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](100) NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_StatusOrder] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Style]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Style](
	[StyleId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[StyleName] [nvarchar](100) NOT NULL,
	[Image] [varchar](100) NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Style] PRIMARY KEY CLUSTERED 
(
	[StyleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[IdCard] [varchar](12) NULL,
	[Email] [varchar](100) NOT NULL,
	[Phone] [varchar](10) NULL,
	[IsEmailActive] [int] NOT NULL,
	[Password] [nchar](100) NOT NULL,
	[RoleId] [int] NOT NULL,
	[SellerId] [int] NULL,
	[Address] [nvarchar](200) NULL,
	[Avatar] [varchar](100) NULL,
	[Date] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voucher]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voucher](
	[VoucherId] [int] IDENTITY(1,1) NOT NULL,
	[VoucherName] [nvarchar](100) NULL,
	[Description] [nvarchar](100) NULL,
	[DiscountType] [nvarchar](100) NULL,
	[Discount] [float] NULL,
	[ReleaseDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Voucher] PRIMARY KEY CLUSTERED 
(
	[VoucherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoucherItem]    Script Date: 3/07/2024 3:15:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherItem](
	[VoucherItemId] [int] IDENTITY(1,1) NOT NULL,
	[VoucherId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_VoucherItem] PRIMARY KEY CLUSTERED 
(
	[VoucherItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Cart] ON 

INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (1, 5)
INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (2, 6)
INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (6, 17)
INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (10, 21)
INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (11, 22)
INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (12, 23)
INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (13, 24)
SET IDENTITY_INSERT [dbo].[Cart] OFF
GO
SET IDENTITY_INSERT [dbo].[CartItem] ON 

INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (99, 6, 5, NULL, 24, CAST(N'2023-12-07T20:19:34.057' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (109, 2, 6, NULL, 2, CAST(N'2023-12-07T20:21:23.307' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (110, 2, 9, NULL, 2, CAST(N'2023-12-07T20:21:25.633' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (111, 2, 10, NULL, 2, CAST(N'2023-12-07T20:21:28.610' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (112, 2, 11, NULL, 2, CAST(N'2023-12-07T20:21:30.967' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (113, 2, 16, NULL, 2, CAST(N'2023-12-07T20:21:34.000' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (114, 2, 17, NULL, 1, CAST(N'2023-12-07T20:21:41.440' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (115, 2, 18, NULL, 1, CAST(N'2023-12-07T20:21:44.913' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (116, 2, 19, NULL, 1, CAST(N'2023-12-07T20:21:48.943' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (117, 2, 20, NULL, 1, CAST(N'2023-12-07T20:21:53.083' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (118, 2, 21, NULL, 1, CAST(N'2023-12-07T20:21:57.120' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (119, 2, 24, NULL, 2, CAST(N'2023-12-07T20:22:06.590' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (120, 10, 5, NULL, 1, CAST(N'2023-12-07T20:46:33.290' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (121, 10, 6, NULL, 1, CAST(N'2023-12-07T20:46:35.750' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (122, 11, 5, NULL, 1, CAST(N'2023-12-08T10:09:24.627' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (123, 11, 18, NULL, 1, CAST(N'2023-12-08T13:04:44.413' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (124, 1, 5, NULL, 1, CAST(N'2023-12-08T13:21:03.047' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (125, 1, 6, NULL, 1, CAST(N'2023-12-08T13:21:23.570' AS DateTime), 0)
INSERT [dbo].[CartItem] ([CartItemId], [CartId], [ProductId], [StyleId], [Count], [Date], [IsDeleted]) VALUES (126, 12, 5, 1, 2, CAST(N'2024-06-29T22:16:22.053' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[CartItem] OFF
GO
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (1, N'Camera', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (2, N'Laptop', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (3, N'SmartPhone', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (4, N'HeadPhone', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (5, N'Tablet', 0)
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
SET IDENTITY_INSERT [dbo].[Comments] ON 

INSERT [dbo].[Comments] ([CommentId], [UserId], [ProductId], [Content], [Rate], [Date], [IsDeleted]) VALUES (1, 23, 6, N'Good!', 4, CAST(N'2024-06-30T15:09:44.847' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Comments] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItem] ON 

INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (1, 1, 5, NULL, 100)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (2, 2, 5, NULL, 50)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (3, 3, 6, NULL, 100)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (4, 4, 6, NULL, 100)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (5, 7, 9, NULL, 30)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (6, 8, 9, NULL, 50)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (7, 11, 9, NULL, 5)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (8, 12, 9, NULL, 5)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (9, 13, 9, NULL, 15)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (10, 16, 9, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (11, 18, 9, NULL, 10)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (12, 19, 6, NULL, 3)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (13, 19, 10, NULL, 4)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (14, 20, 11, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (15, 21, 5, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (16, 22, 9, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (17, 22, 10, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (18, 23, 9, NULL, 5)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (19, 24, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (20, 25, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (21, 26, 5, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (22, 26, 9, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (23, 27, 9, NULL, 10)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (24, 28, 9, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (25, 29, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (26, 30, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (27, 31, 5, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (28, 31, 6, NULL, 3)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (29, 32, 5, NULL, 10)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (30, 33, 5, NULL, 10)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (31, 34, 5, NULL, 3)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (32, 35, 5, NULL, 4)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (33, 36, 5, NULL, 5)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (34, 36, 16, NULL, 3)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (35, 36, 10, NULL, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (36, 37, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (37, 38, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (38, 39, 5, NULL, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (39, 40, 5, 3, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (40, 41, 5, 1, 3)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (41, 41, 5, 3, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (42, 42, 5, 3, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (43, 43, 10, 0, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (44, 44, 10, 0, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (45, 45, 10, 0, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (46, 45, 5, 4, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (47, 46, 5, 1, 2)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (48, 47, 5, 1, 941)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (49, 48, 6, 0, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (50, 49, 6, 0, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (51, 50, 19, 1, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (55, 54, 5, 1, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (56, 55, 6, 0, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (57, 56, 5, 1, 1)
INSERT [dbo].[OrderItem] ([OrderItemId], [OrderId], [ProductId], [StyleId], [Count]) VALUES (58, 57, 5, 1, 1)
SET IDENTITY_INSERT [dbo].[OrderItem] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (1, 3, N'Lê Văn C', 32, N'Điện Biên Phủ, Bình Thạnh', N'0888888888', 1, 3, 1, N'a123@gmail.com', CAST(N'2024-01-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (2, 4, N'Đỗ Văn D', 32, N'Hoàng Diệu, Thủ Đức', N'0972234522', 1, 3, 1, N'b123@gmail.com', CAST(N'2024-02-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (3, 3, N'Nguyễn Văn A', 32, N'Võ Văn Ngân, Thủ Đức', N'0900022222', 1, 3, 1, N'a123@gmail.com', CAST(N'2024-03-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (4, 4, N'Trần Thanh T', 32, N'Phạm Văn Đồng, Bình Thạnh', N'0921122234', 1, 3, 1, N'b123@gmail.com', CAST(N'2024-06-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (7, 3, N'Nguyễn Tú P', 32, N'Kha Vạn Cân, Thủ Đức', N'0999999999', 1, 3, 1, N'a123@gmail.com', CAST(N'2022-11-17T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (8, 4, N'Lê Văn C', 32, N'Điện Biên Phủ, Bình Thạnh', N'0922333344', 1, 3, 1, N'c123@gmail.com', CAST(N'2023-11-17T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (11, 3, N'Lê Thị C', 32, N'Phạm Văn Đồng, Thủ Đức', N'0923345120', 1, 3, 1, N'a123@gmail.com', CAST(N'2022-04-11T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (12, 4, N'Trần Tiến A', 32, N'Nguyễn Xí, Bình Thạnh', N'0920000011', 1, 3, 1, N'b123@gmail.com', CAST(N'2023-04-15T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (13, 3, N'Đỗ Duy T', 32, N'Lê Văn Duyệt, Thủ Đức', N'0999923410', 1, 3, 1, N'a123@gmail.com', CAST(N'2022-05-16T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (16, 4, N'Phạm Duy N', 32, N'Hoàng Diệu, Thủ Đức', N'0992222265', 1, 2, 1, N'b123@gmail.com', CAST(N'2024-03-02T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (18, 3, N'Lê Văn C', 32, N'Võ Văn Ngân, Thủ Đức', N'0922233305', 1, 2, 1, N'a123@gmail.com', CAST(N'2024-03-02T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (19, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822222', 1, 2, 1, N'khangduong1392@gmail.com', CAST(N'2024-04-03T05:13:36.113' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (20, 6, N'Trần Văn B', 32, N'Hoàng Diệu, Thủ Đức', N'0918822233', 1, 3, 1, N'b123@gmail.com', CAST(N'2024-04-03T05:54:43.673' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (21, 6, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822222', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2023-10-05T04:34:52.947' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (22, 6, N'Lê Văn C', 32, N'Hoàng Diệu, Thủ Đức', N'0918822233', 1, 1, 1, N'c123@gmail.com', CAST(N'2023-09-05T04:37:25.910' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (23, 6, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822222', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2023-08-05T07:10:08.213' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (24, 6, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822222', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2023-07-07T07:08:20.570' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (25, 6, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822222', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2023-06-07T07:19:30.627' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (26, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 2, 1, N'khangduong1392@gmail.com', CAST(N'2023-05-08T13:48:47.427' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (27, 23, N'Dương Vĩ Khang', 32, N'Hoang Dieu, Thu Duc', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-08T13:58:17.793' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (28, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 3, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-08T14:09:43.977' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (29, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 4, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-11T20:57:50.360' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (30, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 4, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-11T21:07:11.363' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (31, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 4, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-11T21:27:47.773' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (32, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 4, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-11T21:47:48.913' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (33, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 4, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-11T21:51:31.210' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (34, 24, N'Thầy Trình', 32, N'Tp.HCM', N'0949965555', 1, 4, 1, N'trinhcntt123@gmail.com', CAST(N'2023-12-17T09:35:05.307' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (35, 24, N'Thầy Trình', 32, N'Tp.HCM', N'0949965555', 1, 3, 1, N'trinhcntt123@gmail.com', CAST(N'2023-12-17T09:35:57.333' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (36, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 3, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-18T20:23:22.077' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (37, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 3, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-18T20:26:42.287' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (38, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 6, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-18T20:29:02.273' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (39, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-22T09:39:37.577' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (40, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 2, 1, N'khangduong1392@gmail.com', CAST(N'2023-12-22T09:46:25.500' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (41, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-05-08T03:25:39.837' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (42, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-05-08T03:26:03.100' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (43, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-05-28T23:27:17.443' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (44, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-05-28T23:27:25.237' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (45, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-05-28T23:35:34.323' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (46, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-06-29T22:17:26.793' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (47, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 4, 1, N'khangduong1392@gmail.com', CAST(N'2024-06-30T15:03:05.023' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (48, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 1, N'khangduong1392@gmail.com', CAST(N'2024-06-30T15:04:35.700' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (49, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 3, 1, N'khangduong1392@gmail.com', CAST(N'2024-05-30T15:04:56.670' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (50, 23, N'Dương Vĩ Khang', 32, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 3, 1, N'khangduong1392@gmail.com', CAST(N'2024-07-01T04:42:06.353' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (54, 23, N'Dương Vĩ Khang', NULL, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 0, N'khangduong1392@gmail.com', CAST(N'2024-07-03T02:23:05.193' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (55, 23, N'Dương Vĩ Khang', NULL, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 1, 1, 0, N'khangduong1392@gmail.com', CAST(N'2024-07-03T02:23:29.803' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (56, 23, N'Dương Vĩ Khang', NULL, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 0, 1, 0, N'khangduong1392@gmail.com', CAST(N'2024-07-03T02:24:09.717' AS DateTime), 0)
INSERT [dbo].[Orders] ([OrderId], [UserId], [Receiver], [ShipperId], [Address], [Phone], [VoucherId], [StatusId], [IsPay], [Email], [Date], [IsDeleted]) VALUES (57, 23, N'Dương Vĩ Khang', NULL, N'Võ Văn Ngân, Thủ Đức', N'0918822233', 0, 1, 0, N'khangduong1392@gmail.com', CAST(N'2024-07-03T02:49:41.540' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (5, 27, N'Tai nghe Bluetooth Sony WH-1000XM5', N'Trải nghiệm nghe chân thật, sống động nhờ tính năng 360 Reality Audio', 800000, 750000, 934, 151, 1, N'product02.png', 4, 3, CAST(N'2023-01-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (6, 27, N'Tai nghe Bluetooth Edifier W820NB', N'Thiết kế nhỏ gọn nhưng vô cùng tinh tế mang lại cảm giác thoải mái khi đeo', 700000, 650000, 998, 200, 1, N'product05.png', 4, 5, CAST(N'2023-01-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (9, 27, N'Laptop Acer Aspire 5 Gaming A515', N'Sở hữu hiệu năng mạnh mẽ với chip Intel Gen 13 cũng nhiều tính năng hiện đại chắc chắn sẽ mang đến những trải nghiệm tuyệt vời khi sử dụng', 14000000, 13000000, 500, 160, 1, N'product03.png', 2, 5, CAST(N'2023-01-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (10, 27, N'Máy tính bảng Apple', N'Thiết kế trắng sang trọng, lịch lãm và bắt mắt', 11000000, 10000000, 695, 20, 1, N'product04.png', 5, 4, CAST(N'2023-11-28T15:07:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (11, 27, N'Laptop Lenovo', N'Laptop thiết kế viền trắng bằng nhôm, mỏng và nhẹ', 13000000, 12000000, 200, 100, 1, N'product08.png', 2, 5, CAST(N'2023-11-28T15:09:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (16, 27, N'S6 Edge Plus', N'Thiết kế độc đáo, mới lạ', 8000000, 6500000, 97, 35, 1, N'product07.png', 3, 4, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (17, 27, N'Laptop ASUS X200M', N'Màn hình phẳng, mang đến trải nghiệm mới lạ', 16000000, 15000000, 250, 50, 1, N'product06.png', 2, 4, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (18, 30, N'iPhone 14 (256GB)', N'Thiết kế tuyệt đẹp và bền bỉ cùng với thời lượng pin tuyệt vời', 14000000, 12000000, 150, 0, 1, N'product10.png', 3, 0, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (19, 30, N'Camera Samsung Digitmax V4', N'Hình ảnh sắc nét, mang đến trải nghiệm tuyệt vời cho người dùng', 8000000, 7000000, 300, 100, 1, N'product09.png', 1, 4, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (20, 30, N'Galaxy Tab S9', N'Màn hình phẳng với hình ảnh sống động và thao tác mượt mà', 5000000, 4000000, 500, 200, 1, N'product11.png', 5, 4, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (21, 30, N'Laptop Acer Aspire 3 A315', N'Màu sắc hiển thị trên màn hình có chất lượng khá ổn với các chi tiết rõ nét, màu sắc tươi tắn để bạn có thể yên tâm làm việc hay giải trí trong thời gian dài.', 12000000, 10000000, 200, 60, 1, N'product12.jpg', 2, 4, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (24, 30, N'Laptop Gaming Acer Nitro 5', N'Tần số quét cao 144Hz giúp khung hình chuyển động mượt mà với tốc độ làm mới cực nhanh, người dùng có thể tận hưởng những phút giây giải trí tuyệt vời với những game nặng đô hay những phim hành động.', 22000000, 20000000, 150, 30, 1, N'product13.jpg', 2, 4, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (41, 30, N'Máy tính bảng Apple 1', N'ABCD', 100000, 90000, 200, 0, 1, N'product02.png', 1, 0, CAST(N'2023-12-07T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [Rating], [Date], [IsDeleted]) VALUES (42, 27, N'Máy tính bảng Apple 3', N'ABC', 1000000, 90000, 20, 100, 1, N'product02.png', 4, 5, CAST(N'2024-06-29T22:09:00.000' AS DateTime), 2)
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsDeleted]) VALUES (1, N'Admin', 0)
INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsDeleted]) VALUES (2, N'Customer', 0)
INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsDeleted]) VALUES (3, N'Seller', 0)
INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsDeleted]) VALUES (4, N'Shipper', 0)
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[StatusOrder] ON 

INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (1, N'Đang xử lý', 0)
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (2, N'Đang giao', 0)
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (3, N'Đã nhận', 0)
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (4, N'Đã hủy', 0)
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (6, N'Đã giao', 0)
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (7, N'Đã xác nhận', 0)
SET IDENTITY_INSERT [dbo].[StatusOrder] OFF
GO
SET IDENTITY_INSERT [dbo].[Style] ON 

INSERT [dbo].[Style] ([StyleId], [ProductId], [StyleName], [Image], [IsDeleted]) VALUES (1, 5, N'Đen', N'product02.png', 0)
INSERT [dbo].[Style] ([StyleId], [ProductId], [StyleName], [Image], [IsDeleted]) VALUES (3, 5, N'Trắng', N'product05.png', 0)
INSERT [dbo].[Style] ([StyleId], [ProductId], [StyleName], [Image], [IsDeleted]) VALUES (4, 5, N'Cam', N'product03.png', 0)
INSERT [dbo].[Style] ([StyleId], [ProductId], [StyleName], [Image], [IsDeleted]) VALUES (13, 5, N'Xanh', N'product11.png', 0)
SET IDENTITY_INSERT [dbo].[Style] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (1, N'Dương Vĩ Khang', N'012345', N'khang123@gmail.com', N'0999999999', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 1, NULL, N'Võ Văn Ngân, Thủ Đức', N'ic_launcher.webp', CAST(N'2023-09-29T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (3, N'Nguyễn Văn A', N'123456', N'a1234@gmail.com', N'0888888888', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Điện Biên Phủ, Bình Thạnh', N'abc', CAST(N'2023-09-23T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (4, N'Trần Văn L', N'2345678', N'l123@gmail.com', N'0972234523', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Kha Vạn Cân, Thủ Đức', N'download (1).jpg', CAST(N'2023-11-03T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (5, N'Dương Khang', N'11234567890', N'khang1234@gmail.com', N'0918822322', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Võ Văn Ngân, Thủ Đức', N'default.jpg', CAST(N'2023-11-26T08:34:24.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (6, N'Dương Vĩ Khang', N'34567890', N'khangduong1394@gmail.com', N'0918822222', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Võ Văn Ngân, Thủ Đức', N'avatar-2.jpg', CAST(N'2023-11-26T08:37:48.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (17, N'Dương Vĩ Khang', N'11223300', N'khangduong1393@gmail.com', N'0912234560', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Võ Văn Ngân, Thủ Đức', N'avatar-2.jpg', CAST(N'2023-12-07T19:27:24.163' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (21, N'Dương Vĩ Khang', N'13456789032', N'khangduong1395@gmail.com', N'0902232520', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Võ Văn Ngân, Thủ Đức', N'avatar-2.jpg', CAST(N'2023-12-07T20:30:54.163' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (22, N'Dương Vĩ Khang', N'3456789022', N'khangduong1399@gmail.com', N'0910622223', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Võ Văn Ngân, Thủ Đức', N'avatar-2.jpg', CAST(N'2023-12-07T20:58:14.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (23, N'Dương Vĩ Khang', N'34567890222', N'khangduong1392@gmail.com', N'0918822233', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 2, NULL, N'Võ Văn Ngân, Thủ Đức', N'default.jpg', CAST(N'2023-12-08T13:47:29.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (24, N'Thầy Trình', N'034999673245', N'trinhcntt123@gmail.com', N'0949965555', 1, N'dHJpbmgxMjNAYTEyMzUmJSVARGFjeHM=                                                                    ', 2, NULL, N'Tp.HCM', N'default.jpg', CAST(N'2023-12-17T09:33:37.363' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (27, N'Nguyên Văn A', N'01234567', N'a123@gmail.com', N'0911122233', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 3, NULL, N'Bình Thạnh, Thủ Đức', N'default.jpg', CAST(N'2024-04-21T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (30, N'Trần Văn B', N'0123456', N'b123@gmail.com', N'0913344455', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 3, NULL, N'Tân Bình, Tp.HCM', N'default.jpg', CAST(N'2024-04-21T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (32, N'Lê Thái C', N'01234567999', N'c123@gmail.com', N'0913346857', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 4, 27, N'Bình Thạnh, Tp.HCM', N'default.jpg', CAST(N'2024-04-21T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (33, N'Ngô Văn D', N'012345777', N'd123@gmail.com', N'0910023456', 1, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 4, 30, N'Tân Bình, Tp.HCM', N'default.jpg', CAST(N'2024-04-21T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [SellerId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (35, N'Lê Hoàng E', N'02135712321', N'e123@gmail.com', N'0928856410', 0, N'a2hhbmcxMjM0QGExMjM1JiUlQERhY3hz                                                                    ', 4, 27, N'Tp.HCM', N'default.jpg', CAST(N'2024-07-03T01:16:31.943' AS DateTime), 2)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[Voucher] ON 

INSERT [dbo].[Voucher] ([VoucherId], [VoucherName], [Description], [DiscountType], [Discount], [ReleaseDate], [ExpirationDate], [IsDeleted]) VALUES (0, N'Không có', N'Không có', N'None', 0, CAST(N'2024-06-26T02:03:00.000' AS DateTime), CAST(N'2024-08-27T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Voucher] ([VoucherId], [VoucherName], [Description], [DiscountType], [Discount], [ReleaseDate], [ExpirationDate], [IsDeleted]) VALUES (1, N'Voucher giảm giá 20K', N'Tổng hóa đơn sẽ được giảm 20K khi áp dụng voucher', N'Amount', 20000, CAST(N'2024-06-20T00:00:00.000' AS DateTime), CAST(N'2024-08-29T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Voucher] ([VoucherId], [VoucherName], [Description], [DiscountType], [Discount], [ReleaseDate], [ExpirationDate], [IsDeleted]) VALUES (2, N'Voucher giảm 20%', N'Tổng đơn hàng sẽ được giảm 20% khi áp dụng voucher', N'Percent', 20, CAST(N'2024-06-26T01:59:00.000' AS DateTime), CAST(N'2024-07-12T01:59:00.000' AS DateTime), 0)
INSERT [dbo].[Voucher] ([VoucherId], [VoucherName], [Description], [DiscountType], [Discount], [ReleaseDate], [ExpirationDate], [IsDeleted]) VALUES (3, N'Voucher giảm 15%', N'Tổng đơn hàng sẽ được giảm 15% khi áp dụng voucher', N'Percent', 15, CAST(N'2024-07-07T02:00:00.000' AS DateTime), CAST(N'2024-08-11T02:00:00.000' AS DateTime), 0)
INSERT [dbo].[Voucher] ([VoucherId], [VoucherName], [Description], [DiscountType], [Discount], [ReleaseDate], [ExpirationDate], [IsDeleted]) VALUES (4, N'Voucher giảm 30%', N'Tổng đơn hàng sẽ được giảm 30% khi áp dụng voucher', N'Percent', 30, CAST(N'2024-07-03T02:51:00.000' AS DateTime), CAST(N'2024-08-11T02:51:00.000' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Voucher] OFF
GO
SET IDENTITY_INSERT [dbo].[VoucherItem] ON 

INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (1, 1, 23, 4, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (2, 2, 23, 5, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (13, 2, 3, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (14, 2, 4, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (15, 2, 5, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (16, 2, 6, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (17, 2, 17, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (18, 2, 21, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (19, 2, 22, 1, 0)
INSERT [dbo].[VoucherItem] ([VoucherItemId], [VoucherId], [UserId], [Quantity], [IsDeleted]) VALUES (20, 2, 24, 1, 0)
SET IDENTITY_INSERT [dbo].[VoucherItem] OFF
GO
ALTER TABLE [dbo].[Cart]  WITH CHECK ADD  CONSTRAINT [fk_id_user_3] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [fk_id_user_3]
GO
ALTER TABLE [dbo].[CartItem]  WITH CHECK ADD  CONSTRAINT [fk_id_cart] FOREIGN KEY([CartId])
REFERENCES [dbo].[Cart] ([CartId])
GO
ALTER TABLE [dbo].[CartItem] CHECK CONSTRAINT [fk_id_cart]
GO
ALTER TABLE [dbo].[CartItem]  WITH CHECK ADD  CONSTRAINT [fk_id_product_3] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[CartItem] CHECK CONSTRAINT [fk_id_product_3]
GO
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [fk_id_order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [fk_id_order]
GO
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [fk_id_product_2] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [fk_id_product_2]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [fk_id_status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[StatusOrder] ([StatusId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [fk_id_status]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [fk_id_user_2] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [fk_id_user_2]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [fk_id_category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [fk_id_category]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [fk_id_role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [fk_id_role]
GO

ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [fk_comment_product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [fk_comment_product]
GO

ALTER TABLE [dbo].[Style]  WITH CHECK ADD  CONSTRAINT [fk_style_product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[Style] CHECK CONSTRAINT [fk_style_product]
GO

ALTER TABLE [dbo].[VoucherItem]  WITH CHECK ADD  CONSTRAINT [fk_voucheritem_voucher] FOREIGN KEY([VoucherId])
REFERENCES [dbo].[Voucher] ([VoucherId])
GO
ALTER TABLE [dbo].[VoucherItem] CHECK CONSTRAINT [fk_voucheritem_voucher]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [fk_orders_voucher] FOREIGN KEY([VoucherId])
REFERENCES [dbo].[Voucher] ([VoucherId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [fk_orders_voucher]
GO

USE [master]
GO
ALTER DATABASE [OnlineShop] SET  READ_WRITE 
GO
