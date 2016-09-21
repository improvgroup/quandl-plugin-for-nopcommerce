using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.ExchangeRate.Quandl
{
    public class QuandlExchangeRateProvider : BasePlugin, IExchangeRateProvider, IMiscPlugin
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger; 
        private readonly ISettingService _settingService;
        private readonly QuandlSettings _quandlSettings;

        #endregion

        #region Ctor

        public QuandlExchangeRateProvider(ICurrencyService currencyService,
            ILogger logger,
            ISettingService settingService,
            QuandlSettings quandlSettings)
        {
            this._currencyService = currencyService;
            this._logger = logger;
            this._settingService = settingService;
            this._quandlSettings = quandlSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get Quandl service database code
        /// </summary>
        /// <param name="exchangeCurrencyCode">Primary exchange rate currency code</param>
        /// <returns>Database code</returns>
        protected string GetDatabaseCode(string exchangeCurrencyCode)
        {
            switch (exchangeCurrencyCode.ToLowerInvariant())
            {
                case "eur": return "ECB";
                case "usd": return "FRED";
                case "gbp": return "BOE";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Get Quandl service dataset code
        /// </summary>
        /// <param name="exchangeCurrencyCode">Primary exchange rate currency code</param>
        /// <param name="currencyCode">Current currency code</param>
        /// <returns>Dataset code</returns>
        protected string GetDatasetCode(string exchangeCurrencyCode, string currencyCode)
        {
            switch (exchangeCurrencyCode.ToLowerInvariant())
            {
                case "eur":
                    switch (currencyCode.ToLowerInvariant())
                    {
                        case "aud": return "EURAUD";
                        case "bgn": return "EURBGN";
                        case "brl": return "EURBRL";
                        case "cad": return "EURCAD";
                        case "chf": return "EURCHF";
                        case "cny": return "EURCNY";
                        case "czk": return "EURCZK";
                        case "dkk": return "EURDKK";
                        case "gbp": return "EURGBP";
                        case "hkd": return "EURHKD";
                        case "hrk": return "EURHRK";
                        case "huf": return "EURHUF";
                        case "idr": return "EURIDR";
                        case "ils": return "EURILS";
                        case "inr": return "EURINR";
                        case "isk": return "EURISK";
                        case "jpy": return "EURJPY";
                        case "krw": return "EURKRW";
                        case "ltl": return "EURLTL";
                        case "mxn": return "EURMXN";
                        case "myr": return "EURMYR";
                        case "nok": return "EURNOK";
                        case "nzd": return "EURNZD";
                        case "php": return "EURPHP";
                        case "pln": return "EURPLN";
                        case "ron": return "EURRON";
                        case "rub": return "EURRUB";
                        case "sek": return "EURSEK";
                        case "sgd": return "EURSGD";
                        case "thb": return "EURTHB";
                        case "try": return "EURTRY";
                        case "usd": return "EURUSD";
                        case "zar": return "EURZAR";
                        default: return string.Empty;
                    }
                case "usd":
                    switch (currencyCode.ToLowerInvariant())
                    {
                        case "aud": return "DEXUSAL";
                        case "brl": return "DEXBZUS";
                        case "gbp": return "DEXUSUK";
                        case "cad": return "DEXCAUS";
                        case "cny": return "DEXCHUS";
                        case "dkk": return "DEXDNUS";
                        case "eur": return "DEXUSEU";
                        case "hkd": return "DEXHKUS";
                        case "inr": return "DEXINUS";
                        case "jpy": return "DEXJPUS";
                        case "myr": return "DEXMAUS";
                        case "mxn": return "DEXMXUS";
                        case "twd": return "DEXTAUS";
                        case "nzd": return "DEXUSNZ";
                        case "nok": return "DEXNOUS";
                        case "sgd": return "DEXSIUS";
                        case "zar": return "DEXSFUS";
                        case "krw": return "DEXKOUS";
                        case "lkr": return "DEXSLUS";
                        case "sek": return "DEXSDUS";
                        case "chf": return "DEXSZUS";
                        case "thb": return "DEXTHUS";
                        case "vef": return "DEXVZUS";
                        default: return string.Empty;
                    }
                case "gbp":
                    switch (currencyCode.ToLowerInvariant())
                    {
                        case "aud": return "XUDLADS";
                        case "cad": return "XUDLCDS";
                        case "cny": return "XUDLBK89";
                        case "czk": return "XUDLBK25";
                        case "dkk": return "XUDLDKS";
                        case "hkd": return "XUDLHDS";
                        case "huf": return "XUDLBK33";
                        case "inr": return "XUDLBK97";
                        case "nis": return "XUDLBK78";
                        case "jpy": return "XUDLJYS";
                        case "ltl": return "XUDLBK36";
                        case "myr": return "XUDLBK83";
                        case "nzd": return "XUDLNDS";
                        case "nok": return "XUDLNKS";
                        case "pln": return "XUDLBK47";
                        case "usd": return "XUDLGBD";
                        case "eur": return "XUDLSER";
                        case "rub": return "XUDLBK85";
                        case "sar": return "XUDLSRS";
                        case "sgd": return "XUDLSGS";
                        case "zar": return "XUDLZRS";
                        case "krw": return "XUDLBK93";
                        case "sek": return "XUDLSKS";
                        case "chf": return "XUDLSFS";
                        case "twd": return "XUDLTWS";
                        case "thb": return "XUDLBK87";
                        case "try": return "XUDLBK95";
                        default: return string.Empty;
                    }
                default: return string.Empty;
            }
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="currencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public IList<Core.Domain.Directory.ExchangeRate> GetCurrencyLiveRates(string exchangeCurrencyCode)
        {
            if (string.IsNullOrEmpty(exchangeCurrencyCode))
                throw new NopException("Currency could not be loaded");

            //only EUR, USD and GBP are supported
            var rates = new List<Core.Domain.Directory.ExchangeRate>();
            if (!(exchangeCurrencyCode.Equals("EUR") || exchangeCurrencyCode.Equals("USD") || exchangeCurrencyCode.Equals("GBP")))
                return rates;

            //try to get exchange rate for the existing currencies
            var errors = new StringBuilder();
            foreach (var currency in _currencyService.GetAllCurrencies(true))
            {
                //for primary exchange rate currency return 1 as a rate
                if (currency.CurrencyCode.Equals(exchangeCurrencyCode))
                {
                    rates.Add(new Core.Domain.Directory.ExchangeRate
                    {
                        CurrencyCode = currency.CurrencyCode,
                        Rate = 1,
                        UpdatedOn = DateTime.UtcNow
                    });
                    continue;
                }

                //request to Quandl service
                var serviceUrl = string.Format("https://www.quandl.com/api/v3/datasets/{0}/{1}.json?limit=1&api_key={2}",
                    GetDatabaseCode(exchangeCurrencyCode), GetDatasetCode(exchangeCurrencyCode, currency.CurrencyCode), _quandlSettings.ApiKey);
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "GET";
                request.ContentType = "application/json";

                try
                {
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var body = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<QuandlResponse>(body);
                        if (response == null || response.Dataset == null || response.Dataset.Data == null)
                            continue;

                        var rateData = response.Dataset.Data.FirstOrDefault();
                        if (rateData == null)
                            continue;

                        decimal rate;
                        DateTime date;
                        if (string.IsNullOrEmpty(rateData[1]) || !decimal.TryParse(rateData[1], out rate) || !DateTime.TryParse(rateData[0], out date))
                            continue;

                        rates.Add(new Core.Domain.Directory.ExchangeRate
                        {
                            CurrencyCode = currency.CurrencyCode,
                            Rate = rate,
                            UpdatedOn = date
                        });
                    }
                }
                catch (WebException ex)
                {
                    var httpResponse = (HttpWebResponse)ex.Response;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        errors.AppendLine(string.Format("Quandl exchange error: for the currency {0} {1}", currency.CurrencyCode, streamReader.ReadToEnd()));
                        continue;
                    }
                }
            }

            //log errors
            if (errors.Length > 0)
                _logger.Error(errors.ToString());

            return rates;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Quandl";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.ExchangeRate.Quandl.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new QuandlSettings());

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExchangeRate.Quandl.Fields.ApiKey", "API key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExchangeRate.Quandl.Fields.ApiKey.Hint", "Specify API key.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<QuandlSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExchangeRate.Quandl.Fields.ApiKey");
            this.DeletePluginLocaleResource("Plugins.ExchangeRate.Quandl.Fields.ApiKey.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
