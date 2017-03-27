using System.Web.Mvc;
using Nop.Plugin.ExchangeRate.Quandl.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.ExchangeRate.Quandl.Controllers
{
    [AdminAuthorize]
    public class QuandlController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly QuandlSettings _quandlSettings;

        #endregion

        #region Ctor

        public QuandlController(ILocalizationService localizationService,
            ISettingService settingService,
            QuandlSettings quandlSettings)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._quandlSettings = quandlSettings;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                ApiKey = _quandlSettings.ApiKey
            };

            return View("~/Plugins/ExchangeRate.Quandl/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _quandlSettings.ApiKey = model.ApiKey;
            _settingService.SaveSetting(_quandlSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}
