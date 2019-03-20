SET IDENTITY_INSERT [Catalogue].[CatalogueSnapshots] ON 
GO
INSERT [Catalogue].[CatalogueSnapshots] ([Id], [CompanyId]) VALUES (1, 16)
GO
INSERT [Catalogue].[CatalogueSnapshots] ([Id], [CompanyId]) VALUES (2, 16)
GO
INSERT [Catalogue].[CatalogueSnapshots] ([Id], [CompanyId]) VALUES (3, 16)
GO
SET IDENTITY_INSERT [Catalogue].[CatalogueSnapshots] OFF
GO
SET IDENTITY_INSERT [dbo].[MerchandisePlannings] ON 
GO
INSERT [dbo].[MerchandisePlannings] ([Id], [CalendarType]) VALUES (36, 1)
GO
SET IDENTITY_INSERT [dbo].[MerchandisePlannings] OFF
GO
SET IDENTITY_INSERT [dbo].[RoizingAssortmentPeriods] ON 
GO
INSERT [dbo].[RoizingAssortmentPeriods] ([Id], [CurrentYearAverageCost]) VALUES (238, NULL)
GO
INSERT [dbo].[RoizingAssortmentPeriods] ([Id], [CurrentYearAverageCost]) VALUES (239, NULL)
GO
INSERT [dbo].[RoizingAssortmentPeriods] ([Id], [CurrentYearAverageCost]) VALUES (240, NULL)
GO
SET IDENTITY_INSERT [dbo].[RoizingAssortmentPeriods] OFF
GO
