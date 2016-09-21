using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.ExchangeRate.Quandl.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExchangeRate.Quandl.Fields.ApiKey")]
        public string ApiKey { get; set; }
    }
}