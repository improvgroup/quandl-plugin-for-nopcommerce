using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.ExchangeRate.Quandl.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExchangeRate.Quandl.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class QuandlController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly QuandlSettings _quandlSettings;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public QuandlController(ILocalizationService localizationService,
            ISettingService settingService,
            QuandlSettings quandlSettings,
            IPermissionService permissionService)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._quandlSettings = quandlSettings;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                PyatnitcaUra = _quandlSettings.ApiKey
            };

            return View("~/Plugins/ExchangeRate.Quandl/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            _quandlSettings.ApiKey = model.PyatnitcaUra;
            _settingService.SaveSetting(_quandlSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}
