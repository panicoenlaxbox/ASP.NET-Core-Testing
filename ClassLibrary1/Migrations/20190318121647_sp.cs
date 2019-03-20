using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary1.Migrations
{
    public partial class sp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TYPE [dbo].[FilterConditions] AS TABLE(
	[ChannelId] [int] NULL,
	[City] [nvarchar](128) NULL,
	[Country] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel1] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel10] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel2] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel3] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel4] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel5] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel6] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel7] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel8] [nvarchar](128) NULL,
	[PointOfSaleClassificationLevel9] [nvarchar](128) NULL,
	[PointOfSaleGeneralClusterId] [int] NULL,
	[PointOfSaleId] [int] NULL,
	[PointOfSaleProductClusterId] [int] NULL,
	[ProductClassificationLevel1] [nvarchar](128) NULL,
	[ProductClassificationLevel10] [nvarchar](128) NULL,
	[ProductClassificationLevel2] [nvarchar](128) NULL,
	[ProductClassificationLevel3] [nvarchar](128) NULL,
	[ProductClassificationLevel4] [nvarchar](128) NULL,
	[ProductClassificationLevel5] [nvarchar](128) NULL,
	[ProductClassificationLevel6] [nvarchar](128) NULL,
	[ProductClassificationLevel7] [nvarchar](128) NULL,
	[ProductClassificationLevel8] [nvarchar](128) NULL,
	[ProductClassificationLevel9] [nvarchar](128) NULL,
	[State] [nvarchar](128) NULL,
	[TypologyId] [int] NULL
)");
            migrationBuilder.Sql(@"
CREATE TYPE [dbo].[RoizingAssortmentCellsList] AS TABLE(
	[RoizingAssortmentCellId] [int] NOT NULL
)");
            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[ChangeRoizingAssortmentRetailValue]
	@roizingassortmentid INT NULL,	
	@expression NVARCHAR(MAX) NULL,
	@relatedexpression NVARCHAR(MAX) NULL,
	@percentofvaluechanged DECIMAL(28,15) NULL,
	@newvalue DECIMAL(28,15) NULL


AS
Begin;	
		
		DECLARE @recalculatestocks INT = 0;	


		Select
			Id, 
			PlannedSalesTurnover, 
			PlannedSalesQuantity,
			PlannedAveragePrice,
			PlannedAverageCost,
			PlannedGrossMargin,
			PlannedTotalCost,
			PlannedMarkup			= ISNULL(((PlannedAveragePrice - PlannedAverageCost) / NULLIF(PlannedAverageCost, 0)), 0),
			PlannedCoverage,
			PlannedInitialStock,
			PlannedFinalStock
		into #RoizingAssortmentCellsFact
		From (
			Select 
				a.Id, 
				a.PlannedSalesTurnover,
				a.PlannedSalesQuantity,
				PlannedAveragePrice		= ISNULL((CAST(a.PlannedSalesTurnover as decimal(28,15)) / NULLIF(CAST(a.PlannedSalesQuantity  as decimal(28,15)), 0)), 0),
				PlannedAverageCost		= ISNULL((CAST(a.PlannedTotalCost as decimal(28,15)) / NULLIF(CAST(a.PlannedSalesQuantity  as decimal(28,15)), 0)), 0),
				a.PlannedTotalCost,
				PlannedGrossMargin		= (a.PlannedSalesTurnover - a.PlannedTotalCost),
				a.PlannedCoverage,
				a.PlannedInitialStock,
				a.PlannedFinalStock
			From RoizingAssortmentCells a
			Inner Join #RoizingAssortmentCellsListIdTable b
				on a.Id = b.RoizingAssortmentCellId
			) t;

		-- Create temporary table
		Select Top 0 
			   Id, 
			   PlannedSalesTurnover, 
			   PlannedSalesQuantity, 
			   PlannedTotalCost, 
			   PlannedCoverage, 
			   PlannedInitialStock, 
			   PlannedFinalStock 
		into #RoizingAssortmentCellsCalculated 
		From #RoizingAssortmentCellsFact;

	
		If @expression = 'PlannedSalesTurnover'				
		Begin;	
			If @relatedexpression = 'PlannedSalesQuantity'
			Begin;		
	
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select
					Id,
					PlannedSalesTurnover		= CAST((PlannedSalesQuantity * PlannedAveragePrice) as decimal(28,15)), 
					PlannedSalesQuantity,
					PlannedTotalCost			= CAST((PlannedSalesQuantity * PlannedAverageCost) as decimal(28,15)),
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesQuantity	= ISNULL(CAST(ROUND(((PlannedSalesTurnover * @percentofvaluechanged) / NULLIF(PlannedAveragePrice, 0)), 0) as int), 0),	
						PlannedAveragePrice,
						PlannedAverageCost,
						PlannedTotalCost,				
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;
				
				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;
					
			End;

			Else If @relatedexpression = 'PlannedAveragePrice'
			Begin;
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)				
				Select
					Id,
					PlannedSalesTurnover,
					PlannedSalesQuantity				= CASE WHEN PlannedSalesTurnover = 0 THEN 0 ELSE PlannedSalesQuantity END,
					PlannedTotalCost					= CASE WHEN PlannedSalesTurnover = 0 THEN 0 ELSE PlannedTotalCost END,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover			= CAST((PlannedSalesTurnover * @percentofvaluechanged) as decimal(28,15)),
						PlannedSalesQuantity,						
						PlannedAverageCost,
						PlannedTotalCost,				
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;

				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;
		
			End;
		End;

		Else If @expression = 'PlannedSalesQuantity'
		Begin;
			If @relatedexpression = 'PlannedSalesTurnover'
			Begin;

				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select
					Id,
					PlannedSalesTurnover			= (PlannedSalesQuantity * PlannedAveragePrice),
					PlannedSalesQuantity,
					PlannedTotalCost				= (PlannedSalesQuantity * PlannedAverageCost),
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesQuantity		= CAST(ROUND((PlannedSalesQuantity * @percentofvaluechanged), 0) as int),	
						PlannedAveragePrice,
						PlannedAverageCost,
						PlannedTotalCost,				
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;

				
				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;

			End;

			Else If @relatedexpression = 'PlannedAveragePrice'
			Begin;

				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover			= CASE WHEN PlannedSalesQuantity = 0 THEN 0 ELSE PlannedSalesTurnover END,
					PlannedSalesQuantity,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity		= CAST(ROUND((PlannedSalesQuantity * @percentofvaluechanged), 0) AS INT),	
						PlannedTotalCost			= CAST((CAST(ROUND((PlannedSalesQuantity * @percentofvaluechanged), 0) AS INT) * PlannedAverageCost) as decimal(28,15)),
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;

			End;
		End;

		Else If @expression = 'PlannedAveragePrice'
		Begin;
			If @relatedexpression = 'PlannedSalesTurnover'
			Begin; 
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id, 
					PlannedSalesTurnover,
					PlannedSalesQuantity				= CASE WHEN PlannedSalesTurnover = 0 THEN 0 ELSE PlannedSalesQuantity END,				
					PlannedTotalCost					= CASE WHEN PlannedSalesTurnover = 0 THEN 0 ELSE PlannedTotalCost END,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
							Id, 
							PlannedSalesTurnover			= CAST(CAST(PlannedSalesQuantity as decimal(28,15)) * CAST((PlannedAveragePrice * @percentofvaluechanged) as decimal(28,15)) as decimal(28,15)),
							PlannedSalesQuantity,
							PlannedTotalCost,
							PlannedCoverage,
							PlannedInitialStock,
							PlannedFinalStock
						From #RoizingAssortmentCellsFact
						) t;

				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;

			End;

			Else If @relatedexpression = 'PlannedSalesQuantity'
			Begin;
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover			= CAST((PlannedSalesQuantity * PlannedAveragePrice) as decimal(28,15)),
					PlannedSalesQuantity,
					PlannedTotalCost				= CAST((PlannedSalesQuantity * PlannedAverageCost) as decimal(28,15)),
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity		= ISNULL(CAST(ROUND(CAST(PlannedSalesTurnover as decimal(28,15)) / NULLIF(CAST((PlannedAveragePrice * @percentofvaluechanged) as decimal(28,15)), 0), 0) as int), 0),
						PlannedAveragePrice			= CAST((PlannedAveragePrice * @percentofvaluechanged) as decimal(28,15)),
						PlannedAverageCost,
						PlannedTotalCost,
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;

				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;
				
			End;
		End;

		Else If @expression = 'PlannedTotalCost'
		Begin; 
			If @relatedexpression = 'PlannedAverageCost'
			Begin;
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover			= CASE WHEN PlannedTotalCost = 0 THEN 0 ELSE PlannedSalesTurnover END,
					PlannedSalesQuantity			= CASE WHEN PlannedTotalCost = 0 THEN 0 ELSE PlannedSalesQuantity END,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice,						
						PlannedAverageCost,
						PlannedTotalCost			= CAST((PlannedTotalCost * @percentofvaluechanged) as decimal(28,15)),
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;

				
				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;

			End;
			
			Else If @relatedexpression = 'PlannedSalesQuantity'
			Begin;

				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover		= CAST((CAST(PlannedSalesQuantity as decimal(28,15)) * CAST(PlannedAveragePrice as decimal(28,15))) as decimal(28,15)),
					PlannedSalesQuantity,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock				
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity		= ISNULL(CAST(ROUND((CAST((PlannedTotalCost * @percentofvaluechanged) as decimal(28,15)) / NULLIF(PlannedAverageCost, 0)), 0) as int), 0),
						PlannedAveragePrice,						
						PlannedAverageCost,
						PlannedTotalCost			= CAST((PlannedTotalCost * @percentofvaluechanged) as decimal(28,15)),
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;
					
			End;
		End;

		Else If @expression = 'PlannedAverageCost'
		Begin; 
			If @relatedexpression = 'PlannedTotalCost'
			Begin;

				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover				= CASE WHEN PlannedTotalCost = 0 THEN 0 ELSE PlannedSalesTurnover END,
					PlannedSalesQuantity				= CASE WHEN PlannedTotalCost = 0 THEN 0 ELSE PlannedSalesQuantity END,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice,						
						PlannedAverageCost				= CAST((PlannedAverageCost * @percentofvaluechanged) as decimal(28,15)),
						PlannedTotalCost				= CAST((CAST((PlannedAverageCost * @percentofvaluechanged) as decimal(28,15)) * CAST(PlannedSalesQuantity as decimal(28,15))) as decimal(28,15)),
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;

				
				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;

			End;
		End;

		
		Else If @expression = 'PlannedMarkup'
		Begin; 
			If @relatedexpression = 'PlannedAverageCost'
			Begin;

				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover,
					PlannedSalesQuantity,
					PlannedTotalCost				= CAST((PlannedSalesQuantity * PlannedAverageCost) as decimal(28,15)),
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice,						
						PlannedAverageCost			= ISNULL(CAST((CAST(PlannedAveragePrice as decimal(28,15)) / NULLIF((CAST((PlannedMarkup * @percentofvaluechanged) as decimal(28,15)) + CAST(1 as decimal(28,15))), 0)) as decimal(28,15)), 0),
						PlannedTotalCost,
						PlannedMarkup				= CAST((PlannedMarkup * @percentofvaluechanged) as decimal(28,15)),
						PlannedGrossMargin,
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 0;

			End;

			Else If @relatedexpression = 'PlannedAveragePrice'
			Begin;
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover				= CAST((PlannedSalesQuantity * PlannedAveragePrice) as decimal(28,15)),
					PlannedSalesQuantity,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice			= CAST((CAST(PlannedAverageCost as decimal(28,15)) * (CAST((PlannedMarkup * @percentofvaluechanged) as decimal(28,15)) + CAST(1 as decimal(28,15)))) as decimal(28,15)),						
						PlannedAverageCost,
						PlannedTotalCost,
						PlannedMarkup				= CAST((PlannedMarkup * @percentofvaluechanged) as decimal(28,15)),
						PlannedGrossMargin,
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 0;

			End;						
		End;

		Else If @expression = 'PlannedGrossMargin'
		Begin;
			If @relatedexpression = 'PlannedAverageCost'
			Begin;
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover,
					PlannedSalesQuantity,
					PlannedTotalCost				= CAST((CAST(PlannedSalesQuantity as decimal(28,15)) * PlannedAverageCost) as decimal(28,15)),
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice,
						PlannedAverageCost			= ISNULL(CAST(((CAST(PlannedSalesTurnover as decimal(28,15)) - CAST((PlannedGrossMargin * @percentofvaluechanged) as decimal(28,15))) / NULLIF(CAST(PlannedSalesQuantity as decimal(28,15)), 0)) as decimal(28,15)), 0),
						PlannedTotalCost,
						PlannedMarkup,
						PlannedGrossMargin			= CAST((PlannedGrossMargin * @percentofvaluechanged) as decimal(28,15)),
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 0;

			End;		
			
			Else If @relatedexpression = 'PlannedAveragePrice'
			Begin;
				
				Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
				Select 
					Id,
					PlannedSalesTurnover				= CAST((CAST(PlannedSalesQuantity as decimal(28,15)) * PlannedAveragePrice) as decimal(28,15)),
					PlannedSalesQuantity,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice			= ISNULL(CAST(((CAST(PlannedTotalCost as decimal(28,15)) + CAST((PlannedGrossMargin * @percentofvaluechanged) as decimal(28,15))) / NULLIF(CAST(PlannedSalesQuantity as decimal(28,15)), 0)) as decimal(28,15)), 0),
						PlannedAverageCost,
						PlannedTotalCost,
						PlannedMarkup,
						PlannedGrossMargin			= CAST((PlannedGrossMargin * @percentofvaluechanged) as decimal(28,15)),
						PlannedCoverage,
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Don't recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 0;

			End;			
		End;

		Else If @expression = 'PlannedCoverage'
		Begin;
			
			Insert into #RoizingAssortmentCellsCalculated (Id, PlannedSalesTurnover, PlannedSalesQuantity, PlannedTotalCost, PlannedCoverage, PlannedInitialStock, PlannedFinalStock)
			Select 
					Id,
					PlannedSalesTurnover,
					PlannedSalesQuantity,
					PlannedTotalCost,
					PlannedCoverage,
					PlannedInitialStock,
					PlannedFinalStock
				From (
					Select 
						Id, 
						PlannedSalesTurnover,
						PlannedSalesQuantity,
						PlannedAveragePrice,
						PlannedAverageCost,
						PlannedTotalCost,
						PlannedMarkup,
						PlannedGrossMargin,
						PlannedCoverage					= CAST(@newvalue as int),
						PlannedInitialStock,
						PlannedFinalStock
					From #RoizingAssortmentCellsFact
					) t;


				-- Recalculate PlannedInitialStocks and PlannedFinalStocks
				SET @recalculatestocks = 1;
			
		End;

		Begin Try;
			Begin Transaction UpdateRoizingAssortmentValues;

			-- Set expressions update
			Update a
			Set PlannedSalesTurnover	= ISNULL(b.PlannedSalesTurnover, 0),
				PlannedSalesQuantity	= ISNULL(b.PlannedSalesQuantity, 0),
				PlannedTotalCost		= ISNULL(b.PlannedTotalCost, 0),
				PlannedCoverage			= ISNULL(b.PlannedCoverage, 1)
			From RoizingAssortmentCells a
			Inner Join #RoizingAssortmentCellsCalculated b
				on a.Id = b.Id
			Where ISNULL(a.PlannedSalesTurnover, 0) <> ISNULL(b.PlannedSalesTurnover, 0)
			or ISNULL(a.PlannedSalesQuantity, 0)	<> ISNULL(b.PlannedSalesQuantity, 0)
			or ISNULL(a.PlannedTotalCost, 0)		<> ISNULL(b.PlannedTotalCost, 0)
			or ISNULL(a.PlannedCoverage, 0)			<> ISNULL(b.PlannedCoverage, 0);

			
			
			If @recalculatestocks = 1 
			BEGIN;
			
				DECLARE @MerchandisePlanningId INT =  
				(
					SELECT MerchandisePlanningId
					FROM dbo.RoizingAssortments
					WHERE Id = @roizingassortmentid										
				)

				DECLARE @RoizingAssortmentCellsList AS RoizingAssortmentCellsList
				INSERT INTO @RoizingAssortmentCellsList 
				SELECT RoizingAssortmentCellId FROM #RoizingAssortmentCellsListIdTable 

				EXEC dbo.RecalculateRoizingAssortmentStocks @MerchandisePlanningId, @RoizingAssortmentCellsList;
			END

			Commit Transaction UpdateRoizingAssortmentValues;
		End Try
		Begin Catch;
			Rollback Transaction UpdateRoizingAssortmentValues;
			
			Declare @error nvarchar(max) = (SELECT Concat('Procedure ChangeRoizingAssortmentRetailValue FAILED: ', ERROR_MESSAGE()) AS ErrorMessage);  

			If (@@TRANCOUNT > 0) 
			Begin;				
				raiserror(@error, 20, -1)  with log;

			End;
		End Catch;
End;
");
            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[ChangeRoizingAssortmentValue]
	@roizingassortmentid INT NULL,	
	@sector NVARCHAR(MAX) NULL,
	@yearperiod INT NULL,
	@previousvalue DECIMAL(28,15) NULL,
	@newvalue DECIMAL(28,15) NULL,
	@expression NVARCHAR(MAX) NULL,
	@relatedexpression NVARCHAR(MAX) NULL, 
	@groupeddimensions FilterConditions NULL READONLY,
	@whereconditions FilterConditions NULL READONLY,
	@user NVARCHAR(MAX) NULL

AS
BEGIN;	

	-- Check if the update is to 0 (zero) value				
	DECLARE @iszero NVARCHAR(MAX) = (SELECT CASE WHEN @newvalue = 0 THEN 1 ELSE 0 END AS IsZero);
		
    DECLARE @roizingassortmentperiodid INT = (
        SELECT RoizingAssortmentPeriods.Id
        FROM dbo.RoizingAssortmentPeriods
        INNER JOIN dbo.RoizingAssortments
            ON RoizingAssortments.Id = RoizingAssortmentPeriods.RoizingAssortmentId
        WHERE RoizingAssortments.Id = @roizingassortmentid
            AND RoizingAssortmentPeriods.YearPeriod = @yearPeriod
    );

    		DECLARE @sql NVARCHAR(MAX);

        -- Filters to be used in the where condition 
		Select Distinct
			STUFF((Select ''', ''' + CAST(b.ChannelId as NVARCHAR(128)) From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as ChannelId,
			STUFF((Select ''', ''' + CAST(b.PointOfSaleId as NVARCHAR(128)) From @whereconditions b For XML Path ('')), 1, 2, '') + ''''		as PointOfSaleId,

			STUFF((Select ''', ''' + b.Country From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''									as Country,
			STUFF((Select ''', ''' + b.[State] From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''									as [State],
			STUFF((Select ''', ''' + b.City	From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''										as City,

			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel1 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel1,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel2 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel2,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel3 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel3,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel4 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel4,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel5 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel5,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel6 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel6,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel7 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel7,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel8 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel8,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel9 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel9,
			STUFF((Select ''', ''' + b.PointOfSaleClassificationLevel10 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''			as PointOfSaleClassificationLevel10,

			STUFF((Select ''', ''' + CAST(b.TypologyId as NVARCHAR(128)) From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''		as TypologyId,
			STUFF((Select ''', ''' + b.ProductClassificationLevel1 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel1,
			STUFF((Select ''', ''' + b.ProductClassificationLevel2 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel2,
			STUFF((Select ''', ''' + b.ProductClassificationLevel3 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel3,
			STUFF((Select ''', ''' + b.ProductClassificationLevel4 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel4,
			STUFF((Select ''', ''' + b.ProductClassificationLevel5 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel5,
			STUFF((Select ''', ''' + b.ProductClassificationLevel6 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel6,
			STUFF((Select ''', ''' + b.ProductClassificationLevel7 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel7,
			STUFF((Select ''', ''' + b.ProductClassificationLevel8 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel8,
			STUFF((Select ''', ''' + b.ProductClassificationLevel9 From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel9,
			STUFF((Select ''', ''' + b.ProductClassificationLevel10	From @whereconditions b	For XML Path ('')), 1, 2, '') + ''''				as ProductClassificationLevel10
		into #filter
		From @whereconditions a;
        
		DECLARE @channelid							NVARCHAR(128) = ISNULL((Select ChannelId From #filter), 'ISNULL(a.ChannelId, '''')');
		DECLARE @pointofsaleid						NVARCHAR(128) = ISNULL((Select PointOfSaleId From #filter), 'ISNULL(a.PointOfSaleId, '''')')		;

		DECLARE @country							NVARCHAR(128) = ISNULL((Select Country From #filter), 'ISNULL(b.Country, '''')');
		DECLARE @state								NVARCHAR(128) = ISNULL((Select [State] From #filter), 'ISNULL(b.[State], '''')');
		DECLARE @city								NVARCHAR(128) = ISNULL((Select City From #filter), 'ISNULL(b.City, '''')');

		DECLARE @pointofsaleclassificationlevel1	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel1 From #filter), 'ISNULL(b.ClassificationLevel1, '''')');
		DECLARE @pointofsaleclassificationlevel2	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel2 From #filter), 'ISNULL(b.ClassificationLevel2, '''')');
		DECLARE @pointofsaleclassificationlevel3	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel3 From #filter), 'ISNULL(b.ClassificationLevel3, '''')');
		DECLARE @pointofsaleclassificationlevel4	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel4 From #filter), 'ISNULL(b.ClassificationLevel4, '''')');
		DECLARE @pointofsaleclassificationlevel5	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel5 From #filter), 'ISNULL(b.ClassificationLevel5, '''')');
		DECLARE @pointofsaleclassificationlevel6	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel6 From #filter), 'ISNULL(b.ClassificationLevel6, '''')');
		DECLARE @pointofsaleclassificationlevel7	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel7 From #filter), 'ISNULL(b.ClassificationLevel7, '''')');
		DECLARE @pointofsaleclassificationlevel8	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel8 From #filter), 'ISNULL(b.ClassificationLevel8, '''')');
		DECLARE @pointofsaleclassificationlevel9	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel9 From #filter), 'ISNULL(b.ClassificationLevel9, '''')');
		DECLARE @pointofsaleclassificationlevel10	NVARCHAR(128) = ISNULL((Select PointOfSaleClassificationLevel10 From #filter), 'ISNULL(b.ClassificationLevel10, '''')');

		DECLARE @typologyid							NVARCHAR(128) = ISNULL((Select TypologyId From #filter), 'ISNULL(a.TypologyId, '''')')

		DECLARE @productclassificationlevel1		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel1 From #filter), 'ISNULL(a.ProductClassificationLevel1, '''')');
		DECLARE @productclassificationlevel2		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel2 From #filter), 'ISNULL(a.ProductClassificationLevel2, '''')');
		DECLARE @productclassificationlevel3		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel3 From #filter), 'ISNULL(a.ProductClassificationLevel3, '''')');
		DECLARE @productclassificationlevel4		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel4 From #filter), 'ISNULL(a.ProductClassificationLevel4, '''')');
		DECLARE @productclassificationlevel5		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel5 From #filter), 'ISNULL(a.ProductClassificationLevel5, '''')');
		DECLARE @productclassificationlevel6		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel6 From #filter), 'ISNULL(a.ProductClassificationLevel6, '''')');
		DECLARE @productclassificationlevel7		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel7 From #filter), 'ISNULL(a.ProductClassificationLevel7, '''')');
		DECLARE @productclassificationlevel8		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel8 From #filter), 'ISNULL(a.ProductClassificationLevel8, '''')');
		DECLARE @productclassificationlevel9		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel9 From #filter), 'ISNULL(a.ProductClassificationLevel9, '''')');
		DECLARE @productclassificationlevel10		NVARCHAR(128) = ISNULL((Select ProductClassificationLevel10 From #filter), 'ISNULL(a.ProductClassificationLevel10, '''')');


		DECLARE @scriptquery NVARCHAR(Max);
		DECLARE @scriptwhereconditions NVARCHAR(Max) = 
			' Where a.RoizingAssortmentId = ' + CAST(@roizingassortmentid as NVARCHAR(MAX)) + '
			and f.Id is null
			and ISNULL(a.ChannelId, '''')									IN (' + @channelid + ')
			and ISNULL(a.PointOfSaleId, '''')								IN (' + @pointofsaleid + ')

			and ISNULL(b.Country, '''')										IN (' + @country + ')
			and ISNULL(b.[State], '''')										IN (' + @state + ')
			and ISNULL(b.City, '''')										IN (' + @city + ')

			and ISNULL(b.ClassificationLevel1, '''')						IN (' + @pointofsaleclassificationlevel1 + ')
			and ISNULL(b.ClassificationLevel2, '''')						IN (' + @pointofsaleclassificationlevel2 + ')
			and ISNULL(b.ClassificationLevel3, '''')						IN (' + @pointofsaleclassificationlevel3 + ')
			and ISNULL(b.ClassificationLevel4, '''')						IN (' + @pointofsaleclassificationlevel4 + ')
			and ISNULL(b.ClassificationLevel5, '''')						IN (' + @pointofsaleclassificationlevel5 + ')
			and ISNULL(b.ClassificationLevel6, '''')						IN (' + @pointofsaleclassificationlevel6 + ')
			and ISNULL(b.ClassificationLevel7, '''')						IN (' + @pointofsaleclassificationlevel7 + ')
			and ISNULL(b.ClassificationLevel8, '''')						IN (' + @pointofsaleclassificationlevel8 + ')
			and ISNULL(b.ClassificationLevel9, '''')						IN (' + @pointofsaleclassificationlevel9 + ')
			and ISNULL(b.ClassificationLevel10, '''')						IN (' + @pointofsaleclassificationlevel10 + ')

			and ISNULL(a.TypologyId, '''')									IN (' + @typologyid + ')
			and ISNULL(a.ProductClassificationLevel1, '''')					IN (' + @productclassificationlevel1 + ')
			and ISNULL(a.ProductClassificationLevel2, '''')					IN (' + @productclassificationlevel2 + ')
			and ISNULL(a.ProductClassificationLevel3, '''')					IN (' + @productclassificationlevel3 + ')
			and ISNULL(a.ProductClassificationLevel4, '''')					IN (' + @productclassificationlevel4 + ')
			and ISNULL(a.ProductClassificationLevel5, '''')					IN (' + @productclassificationlevel5 + ')
			and ISNULL(a.ProductClassificationLevel6, '''')					IN (' + @productclassificationlevel6 + ')
			and ISNULL(a.ProductClassificationLevel7, '''')					IN (' + @productclassificationlevel7 + ')
			and ISNULL(a.ProductClassificationLevel8, '''')					IN (' + @productclassificationlevel8 + ')
			and ISNULL(a.ProductClassificationLevel9, '''')					IN (' + @productclassificationlevel9 + ')
			and ISNULL(a.ProductClassificationLevel10, '''')				IN (' + @productclassificationlevel10 + ') ';
					
		-- Create temporary table with Dimensions
		Select * into #groupeddimensions From @groupeddimensions;
		
		
		-- Create empty temporary table for storing RoizingAssortmentCells Ids
		CREATE TABLE #RoizingAssortmentCellsListTable (RoizingAssortmentCellId INT NULL
		,PlannedAverageCost DECIMAL (28,15) NULL
		,PlannedAveragePrice DECIMAL (28,15) NULL
		,PlannedGrossMargin DECIMAL (28,15) NULL
		,PlannedMarkup DECIMAL (28,15) NULL
		,PlannedTotalCost DECIMAL (28,15) NULL
		,PlannedSalesTurnover DECIMAL (28,15) NULL
		,PlannedSalesQuantity DECIMAL (28,15) NULL
		,PlannedCoverage DECIMAL (28,15) NULL
		,PlannedInitialStock DECIMAL (28,15) NULL
		,PlannedFinalStock DECIMAL (28,15) NULL)

		-- Total General
		If @sector = 'Total'
		Begin;
			SET @scriptquery = '
			Insert into #RoizingAssortmentCellsListTable
			Select Distinct
				d.Id AS RoizingAssortmentCellId
				,SUM(d.PlannedTotalCost)  / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAverageCost
		        ,SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAveragePrice
		        ,SUM(d.PlannedSalesTurnover) - SUM(d.PlannedTotalCost) AS PlannedGrossMargin
		        ,(((SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity),0)) / NULLIF((SUM(d.PlannedTotalCost) / NULLIF(SUM(d.PlannedSalesQuantity),0)),0)) - 1) AS PlannedMarkup
		        ,SUM(d.PlannedTotalCost) AS PlannedTotalCost
		        ,SUM(d.PlannedSalesTurnover)  AS PlannedSalesTurnover
		        ,SUM(d.PlannedSalesQuantity) AS PlannedSalesQuantity
                ,MAX(d.PlannedCoverage) AS PlannedCoverage
		        ,SUM(d.PlannedInitialStock) AS PlannedInitialStock
		        ,SUM(d.PlannedFinalStock) AS PlannedFinalStock
			From dbo.RoizingAssortmentRows a
			Inner Join Catalogue.PointsOfSale b
				on a.PointOfSaleId = b.Id
			Inner Join dbo.RoizingAssortmentCells d
				on a.Id = d.RoizingAssortmentRowId
				and a.RoizingAssortmentId = d.RoizingAssortmentId
			Inner Join dbo.RoizingAssortmentPeriods e
				on a.RoizingAssortmentId = e.RoizingAssortmentId
				and e.Id = d.RoizingAssortmentPeriodId 
			LEFT Join dbo.Blockages f
				on d.Id = f.RoizingAssortmentCellId';
		End;

		-- Total Periods
		Else If @sector = 'Period'
		Begin;

			SET @scriptquery = '
			Insert into #RoizingAssortmentCellsListTable
			Select Distinct
				d.Id AS RoizingAssortmentCellId
				,SUM(d.PlannedTotalCost)  / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAverageCost
		        ,SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAveragePrice
		        ,SUM(d.PlannedSalesTurnover) - SUM(d.PlannedTotalCost) AS PlannedGrossMargin
		        ,(((SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity),0)) / NULLIF((SUM(d.PlannedTotalCost) / NULLIF(SUM(d.PlannedSalesQuantity),0)),0)) - 1) AS PlannedMarkup
		        ,SUM(d.PlannedTotalCost) AS PlannedTotalCost
		        ,SUM(d.PlannedSalesTurnover)  AS PlannedSalesTurnover
		        ,SUM(d.PlannedSalesQuantity) AS PlannedSalesQuantity
                ,MAX(d.PlannedCoverage) AS PlannedCoverage
		        ,SUM(d.PlannedInitialStock) AS PlannedInitialStock
		        ,SUM(d.PlannedFinalStock) AS PlannedFinalStock
			From dbo.RoizingAssortmentRows a
			Inner Join Catalogue.PointsOfSale b
				on a.PointOfSaleId = b.Id
			Inner Join dbo.RoizingAssortmentCells d
				on a.Id = d.RoizingAssortmentRowId
				and a.RoizingAssortmentId = d.RoizingAssortmentId
			Inner Join dbo.RoizingAssortmentPeriods e
				on a.RoizingAssortmentId = e.RoizingAssortmentId
				and e.Id = d.RoizingAssortmentPeriodId 
			LEFT Join dbo.Blockages f
				on d.Id = f.RoizingAssortmentCellId				';
		End;

		-- Total Rows
		Else If @sector = 'Row'
		Begin;
			SET @scriptquery = '
			Insert into #RoizingAssortmentCellsListTable
			Select Distinct
				d.Id AS RoizingAssortmentCellId
				,SUM(d.PlannedTotalCost)  / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAverageCost
		        ,SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAveragePrice
		        ,SUM(d.PlannedSalesTurnover) - SUM(d.PlannedTotalCost) AS PlannedGrossMargin
		        ,(((SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity),0)) / NULLIF((SUM(d.PlannedTotalCost) / NULLIF(SUM(d.PlannedSalesQuantity),0)),0)) - 1) AS PlannedMarkup
		        ,SUM(d.PlannedTotalCost) AS PlannedTotalCost
		        ,SUM(d.PlannedSalesTurnover)  AS PlannedSalesTurnover
		        ,SUM(d.PlannedSalesQuantity) AS PlannedSalesQuantity
                ,MAX(d.PlannedCoverage) AS PlannedCoverage
		        ,SUM(d.PlannedInitialStock) AS PlannedInitialStock
		        ,SUM(d.PlannedFinalStock) AS PlannedFinalStock
			From dbo.RoizingAssortmentRows a
			Inner Join Catalogue.PointsOfSale b
				on a.PointOfSaleId = b.Id
			Inner Join dbo.RoizingAssortmentCells d
				on a.Id = d.RoizingAssortmentRowId
				and a.RoizingAssortmentId = d.RoizingAssortmentId
			Inner Join dbo.RoizingAssortmentPeriods e
				on a.RoizingAssortmentId = e.RoizingAssortmentId
				and e.Id = d.RoizingAssortmentPeriodId
			Inner Join #groupeddimensions c
				on ISNULL(a.ChannelId, '''')							= ISNULL(ISNULL(c.ChannelId, a.ChannelId), '''')
				and ISNULL(a.PointOfSaleId, '''')						= ISNULL(ISNULL(c.PointOfSaleId, a.PointOfSaleId), '''')

				and ISNULL(b.Country, '''')								= ISNULL(ISNULL(c.Country, b.Country), '''')
				and ISNULL(b.[State], '''')								= ISNULL(ISNULL(c.[State], b.[State]), '''')
				and ISNULL(b.City, '''')								= ISNULL(ISNULL(c.City, b.City), '''')

				and ISNULL(b.ClassificationLevel1, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel1, b.ClassificationLevel1), '''')
				and ISNULL(b.ClassificationLevel2, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel2, b.ClassificationLevel2), '''')
				and ISNULL(b.ClassificationLevel3, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel3, b.ClassificationLevel3), '''')
				and ISNULL(b.ClassificationLevel4, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel4, b.ClassificationLevel4), '''')
				and ISNULL(b.ClassificationLevel5, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel5, b.ClassificationLevel5), '''')
				and ISNULL(b.ClassificationLevel6, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel6, b.ClassificationLevel6), '''')
				and ISNULL(b.ClassificationLevel7, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel7, b.ClassificationLevel7), '''')
				and ISNULL(b.ClassificationLevel8, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel8, b.ClassificationLevel8), '''')
				and ISNULL(b.ClassificationLevel9, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel9, b.ClassificationLevel9), '''')
				and ISNULL(b.ClassificationLevel10, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel10, b.ClassificationLevel10), '''')
	
				and ISNULL(a.TypologyId, '''')							= ISNULL(ISNULL(c.TypologyId, a.TypologyId), '''')
				and ISNULL(a.ProductClassificationLevel1, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel1, a.ProductClassificationLevel1), '''')
				and ISNULL(a.ProductClassificationLevel2, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel2, a.ProductClassificationLevel2), '''')
				and ISNULL(a.ProductClassificationLevel3, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel3, a.ProductClassificationLevel3), '''')
				and ISNULL(a.ProductClassificationLevel4, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel4, a.ProductClassificationLevel4), '''')
				and ISNULL(a.ProductClassificationLevel5, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel5, a.ProductClassificationLevel5), '''')
				and ISNULL(a.ProductClassificationLevel6, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel6, a.ProductClassificationLevel6), '''')
				and ISNULL(a.ProductClassificationLevel7, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel7, a.ProductClassificationLevel7), '''')
				and ISNULL(a.ProductClassificationLevel8, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel8, a.ProductClassificationLevel8), '''')
				and ISNULL(a.ProductClassificationLevel9, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel9, a.ProductClassificationLevel9), '''')
				and ISNULL(a.ProductClassificationLevel10, '''')		= ISNULL(ISNULL(c.ProductClassificationLevel10, a.ProductClassificationLevel10), '''') 
			LEFT Join dbo.Blockages f
				on d.Id = f.RoizingAssortmentCellId				';
		End;
		-- Specific Cell
		Else If @sector = 'Cell'
		Begin;

			SET @scriptquery = '
			Insert into #RoizingAssortmentCellsListTable
			Select Distinct
				d.Id AS RoizingAssortmentCellId
				,SUM(d.PlannedTotalCost)  / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAverageCost
		        ,SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity), 0) AS PlannedAveragePrice
		        ,SUM(d.PlannedSalesTurnover) - SUM(d.PlannedTotalCost) AS PlannedGrossMargin
		        ,(((SUM(d.PlannedSalesTurnover) / NULLIF(SUM(d.PlannedSalesQuantity),0)) / NULLIF((SUM(d.PlannedTotalCost) / NULLIF(SUM(d.PlannedSalesQuantity),0)),0)) - 1) AS PlannedMarkup
		        ,SUM(d.PlannedTotalCost) AS PlannedTotalCost
		        ,SUM(d.PlannedSalesTurnover)  AS PlannedSalesTurnover
		        ,SUM(d.PlannedSalesQuantity) AS PlannedSalesQuantity
                ,MAX(d.PlannedCoverage) AS PlannedCoverage
		        ,SUM(d.PlannedInitialStock) AS PlannedInitialStock
		        ,SUM(d.PlannedFinalStock) AS PlannedFinalStock
			From dbo.RoizingAssortmentRows a
			Inner Join Catalogue.PointsOfSale b
				on a.PointOfSaleId = b.Id
			Inner Join dbo.RoizingAssortmentCells d
				on a.Id = d.RoizingAssortmentRowId
				and a.RoizingAssortmentId = d.RoizingAssortmentId
			Inner Join dbo.RoizingAssortmentPeriods e
				on a.RoizingAssortmentId = e.RoizingAssortmentId
				and e.Id = d.RoizingAssortmentPeriodId
			Inner Join #groupeddimensions c
				on ISNULL(a.ChannelId, '''')							= ISNULL(ISNULL(c.ChannelId, a.ChannelId), '''')
				and ISNULL(a.PointOfSaleId, '''')						= ISNULL(ISNULL(c.PointOfSaleId, a.PointOfSaleId), '''')

				and ISNULL(b.Country, '''')								= ISNULL(ISNULL(c.Country, b.Country), '''')
				and ISNULL(b.[State], '''')								= ISNULL(ISNULL(c.[State], b.[State]), '''')
				and ISNULL(b.City, '''')								= ISNULL(ISNULL(c.City, b.City), '''')

				and ISNULL(b.ClassificationLevel1, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel1, b.ClassificationLevel1), '''')
				and ISNULL(b.ClassificationLevel2, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel2, b.ClassificationLevel2), '''')
				and ISNULL(b.ClassificationLevel3, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel3, b.ClassificationLevel3), '''')
				and ISNULL(b.ClassificationLevel4, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel4, b.ClassificationLevel4), '''')
				and ISNULL(b.ClassificationLevel5, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel5, b.ClassificationLevel5), '''')
				and ISNULL(b.ClassificationLevel6, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel6, b.ClassificationLevel6), '''')
				and ISNULL(b.ClassificationLevel7, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel7, b.ClassificationLevel7), '''')
				and ISNULL(b.ClassificationLevel8, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel8, b.ClassificationLevel8), '''')
				and ISNULL(b.ClassificationLevel9, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel9, b.ClassificationLevel9), '''')
				and ISNULL(b.ClassificationLevel10, '''')				= ISNULL(ISNULL(c.PointOfSaleClassificationLevel10, b.ClassificationLevel10), '''')
	
				and ISNULL(a.TypologyId, '''')							= ISNULL(ISNULL(c.TypologyId, a.TypologyId), '''')
				and ISNULL(a.ProductClassificationLevel1, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel1, a.ProductClassificationLevel1), '''')
				and ISNULL(a.ProductClassificationLevel2, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel2, a.ProductClassificationLevel2), '''')
				and ISNULL(a.ProductClassificationLevel3, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel3, a.ProductClassificationLevel3), '''')
				and ISNULL(a.ProductClassificationLevel4, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel4, a.ProductClassificationLevel4), '''')
				and ISNULL(a.ProductClassificationLevel5, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel5, a.ProductClassificationLevel5), '''')
				and ISNULL(a.ProductClassificationLevel6, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel6, a.ProductClassificationLevel6), '''')
				and ISNULL(a.ProductClassificationLevel7, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel7, a.ProductClassificationLevel7), '''')
				and ISNULL(a.ProductClassificationLevel8, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel8, a.ProductClassificationLevel8), '''')
				and ISNULL(a.ProductClassificationLevel9, '''')			= ISNULL(ISNULL(c.ProductClassificationLevel9, a.ProductClassificationLevel9), '''')
				and ISNULL(a.ProductClassificationLevel10, '''')		= ISNULL(ISNULL(c.ProductClassificationLevel10, a.ProductClassificationLevel10), '''') 
			LEFT Join dbo.Blockages f
				on d.Id = f.RoizingAssortmentCellId				';
		End;
		

		-- Select all Id from RoizingAssortmentCells to change
				DECLARE @scriptPeriodWhere  NVARCHAR(Max) = CASE WHEN @sector = 'Cell' THEN 'and e.Id = ''' + CAST(@roizingassortmentperiodid as nvarchar(max)) + '''' WHEN @sector = 'Period' THEN 'and e.Id = ''' + CAST(@roizingassortmentperiodid as nvarchar(max)) + '''' ELSE '' END;

		DECLARE @scriptGroupBy NVARCHAR(Max) = 'GROUP BY d.Id'

		
		Execute (@scriptquery + @scriptwhereconditions + @scriptPeriodWhere + @scriptGroupBy);
	

		-- ************************************************************************************************************ --
		--		@roizingassortmentid																					--
		--		@sector																									--
		--			1 = Total	(RoizingAssortments)																	--
		--			2 = Period	(RoizingAssortmentPeriods)																--
		--			3 = Row		(RoizingAssortmentRows)																	--
		--			4 = Cell	(RoizingAssortmentCells)																--
		--		@periodid: MerchandisePlanningPeriodId																	--
		--		@previousvalue: value before user update																--
		--		@newvalue: value inserted by user																		--
		--		@expression: updated column 																			--
		--		@relatedexpression: affected column																		-- 
				
		--		PlannedSalesTurnover			// Facturaci?n															--
		--		PlannedSalesQuantity			// Cantidad																--
		--		PlannedAveragePrice				// PrecioMedio: Facturaci?n / Cantidad									--
		--		PlannedAverageCost				// CosteMedio: CosteTotal / Cantidad									--
		--		PlannedGrossMargin				// MargenBruto: Facturaci?n - CosteTotal								--
		--		PlannedMarkup					// Markup: (PrecioMedio - CosteMedio) / CosteMedio						--
		--		PlannedTotalCost				// CosteTotal: Cantidad * CosteMedio									--
		
		-- ************************************************************************************************************ --
	DECLARE @percentofvaluechanged DECIMAL (28,15) = NULL;

	IF @expression <> 'PlannedCoverage'
		BEGIN;
			DECLARE @difference DECIMAL (28,15) = (@newvalue - @previousvalue);
			DECLARE @scriptCellListQuery NVARCHAR(Max) = 'SELECT SUM(' + @expression + ') FROM #RoizingAssortmentCellsListTable';

			create table #sumExpresion (ExpressionValue int null)
			insert into #sumExpresion exec sp_executesql @scriptCellListQuery

			DECLARE @sumCellNotBlockage DECIMAL (28,15) = (select SUM(ExpressionValue)  from #sumExpresion);

				-- Calculate the % of value changed		
			SET @percentofvaluechanged = 	CASE
			WHEN @sumCellNotBlockage = 0 THEN (SELECT CAST(ISNULL(CAST((CAST(@newvalue AS DECIMAL(28,15)) / NULLIF(CAST(@previousvalue AS DECIMAL(28,15)), 0)) AS DECIMAL(28,15)), 0) AS DECIMAL(28,15)))
			ELSE ((@sumCellNotBlockage + @difference)/@sumCellNotBlockage)
			END;


			SET @percentofvaluechanged = CASE WHEN  @percentofvaluechanged <= 0 THEN 0 ELSE  @percentofvaluechanged  END;

		END;

	CREATE TABLE #RoizingAssortmentCellsListIdTable (RoizingAssortmentCellId INT NULL);	

    Insert into #RoizingAssortmentCellsListIdTable
	SELECT RoizingAssortmentCellId
    FROM #RoizingAssortmentCellsListTable

	
    EXEC dbo.ChangeRoizingAssortmentRetailValue	@roizingassortmentid ,	
                                                @expression ,
                                                @relatedexpression ,
                                                @percentofvaluechanged ,
												@newvalue;

END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
