using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Abp.Timing;
using JK.Cryptography;
using JK.Payments.Orders.Dto;
using JK.Payments.System.Dto;
using JK.Payments.TenantConfigs.Dto;
using JK.Payments.ThirdParty;
using JK.Payments.ThirdParty.Dto;

namespace JK.Payments.Orders
{
    public enum BuiltInParameterType
    {
        Global = 0,
        Config = 1,
        Order = 2,
        Parameter = 3
    }
    public class BuiltInParameters
    {
        public string Key { get; set; }

        public BuiltInParameterType ParameterType { get; set; }

        public string DataType { get; set; }

        public string Desc { get; set; }
    }
    /// <summary>
    /// 支付内置变量
    /// </summary>
    public class PaymentVariable
    {
        public const string GlobalFlag = "@";
        public const string MerchantConfigFlag = "#";
        public const string OrderFlag = "$";
        public const string ParameterFlag = "&";
        private readonly PaymentOrderDto paymentOrder;
        private readonly ThirdPartyAccountDto thirdPartyAccount;
        private readonly ApiConfigurationDto apiConfiguration;
        private readonly CompanyDto company;
        private readonly SystemConfigurationDto systemConfiguration;

        public Dictionary<string, string> Result { get; private set; }

        public PaymentVariable(
            SystemConfigurationDto systemConfiguration,
            CompanyDto company,
            ApiConfigurationDto apiConfiguration,
            ThirdPartyAccountDto thirdPartyAccount) :
            this(systemConfiguration, company, apiConfiguration, thirdPartyAccount, null)
        {

        }

        public PaymentVariable(
            SystemConfigurationDto systemConfiguration,
            CompanyDto company,
            ApiConfigurationDto apiConfiguration,
            ThirdPartyAccountDto thirdPartyAccount,
            PaymentOrderDto paymentOrder)
        {
            this.paymentOrder = paymentOrder;
            this.thirdPartyAccount = thirdPartyAccount;
            this.apiConfiguration = apiConfiguration;
            this.company = company;
            this.systemConfiguration = systemConfiguration;
        }

        private bool IsGlobalVariable(string key)
        {
            return key.Contains(GlobalFlag);
        }
        private bool IsMerchantConfigVariable(string key)
        {
            return key.Contains(MerchantConfigFlag);
        }
        private bool IsOrderVariable(string key)
        {
            return key.Contains(OrderFlag);
        }

        private bool IsParameter(string key)
        {
            return key.Contains(ParameterFlag);
        }

        private string GetGlobalVariableValue(string key, string format)
        {
            string result = "";
            switch (key)
            {
                case "@DateNow":
                    if (string.IsNullOrEmpty(format))
                    {
                        result = Clock.Now.ToString();
                    }
                    else
                    {
                        result = Clock.Now.ToString(format);
                    }
                    break;
                case "@HttpsDomain":
                    result = systemConfiguration.HttpsCallbackUrl;
                    break;
                case "@HttpDomain":
                    result = systemConfiguration.HttpCallbackUrl;
                    break;
                default:
                    break;
            }

            return result;
        }

        private string GetMerchantConfigVariableValue(string key, string format)
        {
            string result = "";
            switch (key)
            {
                case "#MerchantId":
                    result = thirdPartyAccount.MerchantId;
                    break;
                case "#MerchantKey":
                    result = thirdPartyAccount.MerchantKey;
                    break;
                case "#PrivateKey":
                    result = thirdPartyAccount.PrivateKey;
                    break;
                case "#PublicKey":
                    result = thirdPartyAccount.PublicKey;
                    break;
                default:
                    break;
            }

            return result;
        }

        private string GetOrderVariableValue(string key, string format)
        {
            string result = "";
            switch (key)
            {
                case "$CompanyId":
                    result = paymentOrder.CompanyId.ToString();
                    break;
                case "$PaymentMethodId":
                    result = paymentOrder.ChannelId.ToString();
                    break;
                case "$Amount":
                    if (string.IsNullOrEmpty(format))
                    {
                        if (company.CurrencyUnit == Enumerates.CurrencyUnit.Yuan)
                            result = (paymentOrder.Amount / 100.00m).ToString();
                        else
                            result = paymentOrder.Amount.ToString();
                    }
                    else
                    {
                        if (company.CurrencyUnit == Enumerates.CurrencyUnit.Yuan)
                            result = (paymentOrder.Amount / 100.00m).ToString(format);
                        else
                            result = paymentOrder.Amount.ToString(format);
                    }
                    break;
                case "$PaymentMethodCode":
                    var channel = StaticContext.Channels.Find(c => c.Id == paymentOrder.ChannelId);
                    if (channel != null)
                    {
                        //TODO ChannelOverride
                        ChannelOverride channelOverride = null;
                        result = channelOverride == null ? channel.ChannelCode : channelOverride.OverrideCode;
                    }
                    break;
                case "$BankCode":
                    var bank = StaticContext.Banks.Find(c => c.Id == paymentOrder.BankId);
                    if (bank != null)
                    {
                        //TODO BankOverride
                        BankOverride bankOverride = null;
                        result = bankOverride == null ? bank.BankCode : bankOverride.OverrideCode;
                    }
                    break;
                case "$OrderId":
                    result = paymentOrder.Id.ToString();
                    break;
                case "$ThirdPartyOrderId":
                    result = paymentOrder.ThirdPartyOrderId.ToString();
                    break;
                case "$ClientIp":
                    if (!string.IsNullOrEmpty(format))
                    {
                        result = paymentOrder.CreateIp.Replace('.', char.Parse(format));
                    }
                    else
                    {
                        result = paymentOrder.CreateIp;
                    }
                    break;
                case "$CreationTime":
                    if (string.IsNullOrEmpty(format))
                    {
                        result = paymentOrder.CreationTime.ToString();
                    }
                    else
                    {
                        result = paymentOrder.CreationTime.ToString(format);
                    }
                    break;
                case "$PaidDate":
                    if (string.IsNullOrEmpty(format))
                    {
                        result = paymentOrder.PaidDate?.ToString();
                    }
                    else
                    {
                        result = paymentOrder.PaidDate?.ToString(format);
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private string GetParameterValue(string key, string format)
        {
            var fkey = key.Replace(ParameterFlag, "");
            return Result[fkey];
        }
        /// <summary>
        /// 动态参数正则表达式
        /// </summary>
        private const string ParameterPattern = @"\{\{(?<key>[a-zA-Z0-9@$#&_]{1,20})\}\}";
        /// <summary>
        /// 高级动态参数正则表达式（包含JPath，XPath）
        /// </summary>
        private const string AdvancedParameterPattern = @"\{\{\{(?<key>[a-zA-Z0-9@$#&_]{1,20})\>(?<path>.*?)\}\}\}";

        public Dictionary<string,string> ProcessingApiRequestParameters(List<ApiRequestParameter> parameters)
        {
            Result = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                string value = parameter.Value;
                var matches = Regex.Matches(parameter.Value, ParameterPattern);
                if (matches.Count > 0)
                {
                    foreach (Match item in matches)
                    {
                        var tempValue = GetVariableValue(item.Groups["key"].Value, parameter.Format);
                        value = value.Replace(item.Value, tempValue);
                    }
                }

                //判断必填项
                if (parameter.Required && string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException($"参数{parameter.Key}的值为空。");
                }
                //如果是加密参数
                if (parameter.Encryption.HasValue)
                {
                    value = EncryptionHelper.GetEncryption(parameter.Encryption.Value, value, thirdPartyAccount);
                }
                if (parameter.Format.Equals("ToLower", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToLower();
                }
                else if (parameter.Format.Equals("ToUpper", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToUpper();
                }
                Result.Add(parameter.Key, value);
            }
            return Result;
        }

        private string GetVariableValue(string variableName, string format)
        {
            //检查全局变量
            if (IsGlobalVariable(variableName))
            {
                return GetGlobalVariableValue(variableName, format);
            }
            else
            //检查支付配置变量
            if (IsMerchantConfigVariable(variableName))
            {
                return GetMerchantConfigVariableValue(variableName, format);
            }
            else
            //检查订单变量
            if (IsOrderVariable(variableName))
            {
                return GetOrderVariableValue(variableName, format);
            }
            else if (IsParameter(variableName))
            {
                return GetParameterValue(variableName, format);
            }
            else
            {
                //常量
                return variableName;
            }
        }
    }
}
