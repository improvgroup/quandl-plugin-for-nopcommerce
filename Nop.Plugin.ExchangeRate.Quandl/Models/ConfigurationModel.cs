using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExchangeRate.Quandl.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExchangeRate.Quandl.Fields.ApiKey")]
        public string PyatnitcaUra { get; set; }
    }
}