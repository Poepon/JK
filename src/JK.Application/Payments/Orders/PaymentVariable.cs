using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Timing;
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


        public static bool IsGlobalVariable(string key)
        {
            return key.Contains(GlobalFlag);
        }
        public static bool IsMerchantConfigVariable(string key)
        {
            return key.Contains(MerchantConfigFlag);
        }
        public static bool IsOrderVariable(string key)
        {
            return key.Contains(OrderFlag);
        }

        public static bool IsParameter(string key)
        {
            return key.Contains(ParameterFlag);
        }



        public string GetGlobalVariableValue(string key, string format)
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

        public string GetMerchantConfigVariableValue(string key, string format)
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

        public string GetOrderVariableValue(string key, string format)
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

        public string GetParameterValue(Dictionary<string, string> nameValue, string key, string format)
        {
            var fkey = key.Replace(ParameterFlag, "");
            return nameValue[fkey];
        }
    }
}
