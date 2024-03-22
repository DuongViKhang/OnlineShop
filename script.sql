USE [master]
GO
/****** Object:  Database [OnlineShop]    Script Date: 22/03/2024 12:45:50 AM ******/
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
/****** Object:  Table [dbo].[Cart]    Script Date: 22/03/2024 12:45:51 AM ******/
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
/****** Object:  Table [dbo].[CartItem]    Script Date: 22/03/2024 12:45:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItem](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CartId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_CartItem] PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 22/03/2024 12:45:51 AM ******/
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
/****** Object:  Table [dbo].[OrderItem]    Script Date: 22/03/2024 12:45:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItem](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Count] [int] NOT NULL,
 CONSTRAINT [PK_OrderItemId] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 22/03/2024 12:45:51 AM ******/
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
/****** Object:  Table [dbo].[Product]    Script Date: 22/03/2024 12:45:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[SellerId] [int] NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[Decription] [nvarchar](1000) NOT NULL,
	[Price] [float] NOT NULL,
	[PromotionalPrice] [float] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Sold] [int] NOT NULL,
	[IsActive] [int] NOT NULL,
	[Image] [varchar](100) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[StyleId] [int] NOT NULL,
	[Rating] [float] NOT NULL,
	[Date] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 22/03/2024 12:45:51 AM ******/
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
/****** Object:  Table [dbo].[StatusOrder]    Script Date: 22/03/2024 12:45:51 AM ******/
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
/****** Object:  Table [dbo].[Style]    Script Date: 22/03/2024 12:45:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Style](
	[StyleId] [int] IDENTITY(1,1) NOT NULL,
	[StyleName] [nvarchar](100) NOT NULL,
	[IsDeleted] [int] NOT NULL,
	[Image] [nvarchar](max) NULL,
 CONSTRAINT [PK_Style] PRIMARY KEY CLUSTERED 
(
	[StyleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 22/03/2024 12:45:51 AM ******/
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
	[Password] [nchar](100) NULL,
	[RoleId] [int] NULL,
	[SellerId] [int] NULL,
	[Address] [nvarchar](200) NOT NULL,
	[Avatar] [varchar](100) NULL,
	[Date] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Cart] ON 

INSERT [dbo].[Cart] ([CartId], [UserId]) VALUES (1, 3)
SET IDENTITY_INSERT [dbo].[Cart] OFF
GO
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (1, N'Tai nghe', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (2, N'Laptop', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (3, N'Smartphone', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (4, N'Tablet', 0)
INSERT [dbo].[Category] ([CategoryId], [CategoryName], [IsDeleted]) VALUES (5, N'Camera', 0)
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (1, 7, N'Tai nghe Marshall Major', N'Tai nghe Marshall Major IV Black ra đời với vai trò giúp người dùng có thể tận hưởng những trải nghiệm âm nhạc đầy lý tưởng và trọn vẹn. Với thiết kế sang trọng, chất âm trong trẻo cùng những công nghệ hiện đại, chiếc tai nghe này không chỉ nâng cao chất lượng cuộc sống mà còn trở thành phụ kiện không thể thiếu cho người sử dụng.', 2000000, 1800000, 100, 20, 1, N'product02.png', 1, 1, 5, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (2, 7, N'Laptop HP Elitebook', N'Laptop HP Elitebook sở hữu một màn hình lớn, độ phân giải Full HD sắc nét, cấu hình đủ cho hoạt động học tập, công việc văn phòng và bán trong tầm giá rẻ, mang đến cho bạn phương tiện làm việc hữu ích trong cuộc sống hiện đại.', 10000000, 9500000, 200, 30, 1, N'product03.png', 2, 1, 4, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (3, 7, N'Samsung Galaxy Tab A7', N'Thiết kế mỏng nhẹ, sang trọng với chất liệu nhôm nguyên khối.', 4000000, 3600000, 150, 25, 1, N'product04.png', 4, 1, 4, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (4, 7, N'Tai nghe Edifier W820NB', N'Thiết kế sang trọng, chất âm trong trẻo cùng những công nghệ hiện đại, chiếc tai nghe này không chỉ nâng cao chất lượng cuộc sống mà còn trở thành phụ kiện không thể thiếu cho người sử dụng.', 1500000, 1200000, 300, 60, 1, N'product05.png', 1, 1, 4, CAST(N'2024-04-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (5, 7, N'Laptop Asus TUF Gaming A15', N'Nếu bạn đang có nhu cầu sở hữu một sản phẩm laptop Gaming cao cấp thì laptop Asus TUF Gaming A15 FA507XI-LP420W sẽ là lựa chọn mà bạn không muốn bỏ qua. Chiếc laptop Asus Gaming này được trang bị thiết kế mạnh mẽ, nam tính cùng hiệu năng xử lý đỉnh cao, sẽ đem tới cho bạn những trải nghiệm chiến game mượt mà nhất.', 25000000, 22000000, 100, 20, 1, N'product06.png', 2, 1, 4, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (6, 7, N'Google Pixel 8 Pro', N'Google Pixel 8 Pro đang là cái tên được nhắc đến nhiều nhất trong thời gian vừa qua trên các diễn đàn công nghệ. Chính thức ngày 04/10/2023 chiếc điện thoại này được ra mắt cùng nhiều tính năng nổi bật mới. Với Super Res Zoom giúp chụp đêm tốt hơn, hiệu năng ấn tượng từ chip Tensor G3 và những tích hợp AI mới cho Camera. Pixel 8 Pro hứa hẹn sẽ mang đến cho người dùng những trải nghiệm không thể tuyệt vời hơn!', 18000000, 17000000, 200, 30, 1, N'product07.png', 3, 1, 5, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (7, 7, N'Microsoft Surface Laptop Go', N'Thiết kế sang trọng mang đến trải nghiệm tuyệt vời cho người dùng. Nắp máy và phần bàn phím được làm bằng nhôm Aluminum phay xước tạo cảm giác cao cấp và chắc chắn cho máy.', 17000000, 16500000, 120, 24, 1, N'product08.png', 2, 1, 4, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Product] ([ProductId], [SellerId], [ProductName], [Decription], [Price], [PromotionalPrice], [Quantity], [Sold], [IsActive], [Image], [CategoryId], [StyleId], [Rating], [Date], [IsDeleted]) VALUES (8, 7, N'Sony Handycam HDR-CX405', N'Sony HDR-CX405 tự hào có sự kết hợp tuyệt vời giữa các tính năng cao cấp và mức giá phù hợp. Sản phẩm có thể quay video với chất lượng Full HD tốc độ cực cao và có màn hình LCD xoay mà bạn điều hướng bằng nút bật tắt. Nó có tính năng ổn định hình ảnh SteadyShot sẽ giúp bạn ghi hình ổn định, mượt mà.', 7000000, 6000000, 230, 60, 1, N'product09.png', 5, 1, 5, CAST(N'2024-03-13T00:00:00.000' AS DateTime), 0)
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
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (3, N'Đã nhận ', 0)
INSERT [dbo].[StatusOrder] ([StatusId], [StatusName], [IsDeleted]) VALUES (4, N'Đã hủy', 0)
SET IDENTITY_INSERT [dbo].[StatusOrder] OFF
GO
SET IDENTITY_INSERT [dbo].[Style] ON 

INSERT [dbo].[Style] ([StyleId], [StyleName], [IsDeleted], [Image]) VALUES (1, N'New generation', 0, NULL)
INSERT [dbo].[Style] ([StyleId], [StyleName], [IsDeleted], [Image]) VALUES (2, N'Retro', 0, NULL)
SET IDENTITY_INSERT [dbo].[Style] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (1, N'Dương Vĩ Khang', N'0123', N'khang123@gmail.com', N'0999999999', 1, N'a2hhbmcxMjNAYTEyMzUmJSVARGFjeHM=                                                                    ', 1, N'Võ Văn Ngân, Thủ Đức', N'ic_launcher.webp', CAST(N'2024-03-11T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (3, N'Khang Duong', N'01234', N'khang1234@gmail.com', N'0988888888', 1, N'a2hhbmcxMjNAYTEyMzUmJSVARGFjeHM=                                                                    ', 2, N'Võ Văn Ngân, Thủ Đức', N'default.jpg', CAST(N'2024-03-11T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (7, N'Nguyễn Văn A', N'012345', N'a123@gmail.com', N'0977777777', 1, N'a2hhbmcxMjNAYTEyMzUmJSVARGFjeHM=                                                                    ', 3, N'Hoàng Diệu, Thủ Đức', NULL, CAST(N'2024-03-11T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[Users] ([UserId], [UserName], [IdCard], [Email], [Phone], [IsEmailActive], [Password], [RoleId], [Address], [Avatar], [Date], [IsDeleted]) VALUES (9, N'Trần Văn B', N'0123456', N'b123@gmail.com', N'0966666666', 1, N'a2hhbmcxMjNAYTEyMzUmJSVARGFjeHM=                                                                    ', 4, N'Lê Văn Chí, Thủ Đức', NULL, CAST(N'2024-03-11T00:00:00.000' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Users] OFF
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
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [fk_id_shipper] FOREIGN KEY([ShipperId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [fk_id_shipper]
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
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [fk_id_seller] FOREIGN KEY([SellerId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [fk_id_seller]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [fk_id_style] FOREIGN KEY([StyleId])
REFERENCES [dbo].[Style] ([StyleId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [fk_id_style]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [fk_id_role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [fk_id_role]
GO
USE [master]
GO
ALTER DATABASE [OnlineShop] SET  READ_WRITE 
GO
