using System;
using Danaos.Shared.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Danaos.TRD.App.Entities
{
    [Table("TRD_PLATTS_PRICES")]
    public class PlattsPrices : EntityBase
    {
        protected class FieldMap : DbMapping
        {
            public static FieldMap Instance { get; } = new FieldMap();

            static FieldMap()
            {
            }

            protected FieldMap()
            {
                mapToDefaultSchema<PlattsPrices>(t => t.Id, "ID", true);
                mapToDefaultSchema<PlattsPrices>(t => t.Dt, "DT", true);
                mapToDefaultSchema<PlattsPrices>(t => t.PlattsCode, "PLATTS_CODE", true);
                mapToDefaultSchema<PlattsPrices>(t => t.Currency, "CURRENCY");
                mapToDefaultSchema<PlattsPrices>(t => t.LowPrice, "LOW_PRICE");
                mapToDefaultSchema<PlattsPrices>(t => t.HighPrice, "HIGH_PRICE");
                mapToDefaultSchema<PlattsPrices>(t => t.ClosePrice, "CLOSE_PRICE");
            }
        }

        public override DbMapping GetFieldMap() => FieldMap.Instance;

        [Required(ErrorMessage = "Please enter a value for the required field :[Id]")]
        [Range(0, 999999999999, ErrorMessage = "Please don't exceed the range [0,999999999999] for the field :[Id]")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a value for the required field :[Dt]")]
        public DateTime Dt { get; set; }
        [Required(ErrorMessage = "Please enter a value for the required field :[Platts Code]")]
        [MaxLength(20, ErrorMessage = "[Platts Code] should not exceed 20 characters")]
        public string PlattsCode { get; set; }
        [Required(ErrorMessage = "Please enter a value for the required field :[Currency]")]
        [MaxLength(5, ErrorMessage = "[Currency] should not exceed 5 characters")]
        public string Currency { get; set; }
        [Range(0, 99999999, ErrorMessage = "Please don't exceed the range [0,99999999] for the field :[Low Price]")]
        public decimal? LowPrice { get; set; }
        [Range(0, 99999999, ErrorMessage = "Please don't exceed the range [0,99999999] for the field :[High Price]")]
        public decimal? HighPrice { get; set; }
        [Range(0, 99999999, ErrorMessage = "Please don't exceed the range [0,99999999] for the field :[Close Price]")]
        public decimal? ClosePrice { get; set; }
    }
}
