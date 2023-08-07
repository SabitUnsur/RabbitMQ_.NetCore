using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCreateWorkerService.Models
{
    public partial class DimProduct
    {
        public int ProductId { get; set; }
        public int ProductKey { get; set; }
        public string? ProductAlternateKey { get; set; }
        public int? ProductSubcategoryKey { get; set; }
        public string? WeightUnitMeasureCode { get; set; }
        public string? SizeUnitMeasureCode { get; set; }
        public string EnglishProductName { get; set; } = null!;
        public string SpanishProductName { get; set; } = null!;
        public string FrenchProductName { get; set; } = null!;
        public decimal? StandardCost { get; set; }
        public bool FinishedGoodsFlag { get; set; }
        public string Color { get; set; } = null!;
        public short? SafetyStockLevel { get; set; }
        public short? ReorderPoint { get; set; }
        public decimal? ListPrice { get; set; }
        public string? Size { get; set; }
        public string? SizeRange { get; set; }
        public double? Weight { get; set; }
        public int? DaysToManufacture { get; set; }
        public string? ProductLine { get; set; }
        public decimal? DealerPrice { get; set; }
        public string? Class { get; set; }
        public string? Style { get; set; }
        public string? ModelName { get; set; }
        public byte[]? LargePhoto { get; set; }
        public string? EnglishDescription { get; set; }
        public string? FrenchDescription { get; set; }
        public string? ChineseDescription { get; set; }
        public string? ArabicDescription { get; set; }
        public string? HebrewDescription { get; set; }
        public string? ThaiDescription { get; set; }
        public string? GermanDescription { get; set; }
        public string? JapaneseDescription { get; set; }
        public string? TurkishDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}
