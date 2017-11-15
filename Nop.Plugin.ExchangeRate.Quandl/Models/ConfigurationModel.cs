using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.ExchangeRate.Quandl.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExchangeRate.Quandl.Fields.ApiKey")]
        public string PyatnitcaUra { get; set; }
    }
}