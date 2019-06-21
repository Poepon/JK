﻿using Abp.Timing;
using JK.Cryptography;
using JK.Payments.Enumerates;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Payments.TenantConfigs.Dto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

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

        public string DataTypeName { get; set; }

        public string Desc { get; set; }
    }
    /// <summary>
    /// 支付内置变量
    /// </summary>
    public class PaymentVariable
    {
        public const string GlobalFlag = "@";
        public const string CompanyAccountFlag = "#";
        public const string OrderFlag = "$";
        public const string ParameterFlag = "&";
        private readonly PaymentOrderDto paymentOrder;
        private readonly CompanyAccountDto companyAccount;
        private readonly ApiConfigurationDto apiConfiguration;
        private readonly TenantPaymentApp tenantPaymentApp;
        private readonly CompanyDto company;

        public Dictionary<string, string> Result { get; private set; }

        public PaymentVariable(
            TenantPaymentApp tenantPaymentApp,
            CompanyDto company,
            ApiConfigurationDto apiConfiguration,
            CompanyAccountDto companyAccount) :
            this(tenantPaymentApp, company, apiConfiguration, companyAccount, null)
        {
            Result = new Dictionary<string, string>();
        }

        public void InitValues(Dictionary<string, string> values)
        {
            Result = values ?? throw new NullReferenceException(nameof(values));
        }

        public PaymentVariable(
            TenantPaymentApp tenantPaymentApp,
            CompanyDto company,
            ApiConfigurationDto apiConfiguration,
            CompanyAccountDto companyAccount,
            PaymentOrderDto paymentOrder)
        {
            this.paymentOrder = paymentOrder;
            this.companyAccount = companyAccount;
            this.apiConfiguration = apiConfiguration;
            this.tenantPaymentApp = tenantPaymentApp;
            this.company = company;
        }

        private bool IsGlobalVariable(string key)
        {
            return key.Contains(GlobalFlag);
        }
        private bool IsCompanyAccountVariable(string key)
        {
            return key.Contains(CompanyAccountFlag);
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
                case "@CallbackDomain":
                    if (tenantPaymentApp.UseSSL)
                    {
                        result = $"https://{tenantPaymentApp.CallbackDomain}";
                    }
                    else
                    {
                        result = $"http://{tenantPaymentApp.CallbackDomain}";
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private string GetCompanyAccountVariableValue(string key, string format)
        {
            string result = "";
            switch (key)
            {
                case "#MerchantId":
                    result = companyAccount.MerchantId;
                    break;
                case "#MerchantKey":
                    result = companyAccount.MerchantKey;
                    break;
                default:
                    if (companyAccount.Attributes.ContainsKey(key))
                    {
                        result = companyAccount.Attributes[key];
                    }
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
        private const string ParameterPattern = @"\{\{(?<key>[a-zA-Z0-9@$#&_]{1,20})\>{0,1}(?<path>.*?)\}\}";

        public Dictionary<SetValueLocation, Dictionary<string, string>> ProcessingApiRequestParameters(IEnumerable<ApiParameter> parameters)
        {
            var values = new Dictionary<SetValueLocation, Dictionary<string, string>>();
            var queryItem = new Dictionary<string, string>();
            var headersItem = new Dictionary<string, string>();
            var contentItem = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                string value = NewMethod(parameter.Key, parameter.ValueOrExpression, parameter.Format, parameter.Required, parameter.Encryption, parameter.EncryptionParameters);
                Result.Add(parameter.Key, value);
                switch (parameter.SetLocation)
                {
                    case SetValueLocation.Content:
                        contentItem.Add(parameter.Key, value);
                        break;
                    case SetValueLocation.Query:
                        queryItem.Add(parameter.Key, value);
                        break;
                    case SetValueLocation.Headers:
                        headersItem.Add(parameter.Key, value);
                        break;
                    default:
                        break;
                }
            }
            values.Add(SetValueLocation.Query, queryItem);
            values.Add(SetValueLocation.Query, headersItem);
            values.Add(SetValueLocation.Query, contentItem);
            return values;
        }

        private string NewMethod(string key, string exp, string format, bool required, EncryptionMethod? encryption, string encryptionParameters)
        {
            string value = exp;
            var matches = Regex.Matches(exp, ParameterPattern);
            if (matches.Count > 0)
            {
                foreach (Match item in matches)
                {
                    var tempValue = GetVariableValue(item.Groups["key"].Value, format);
                    if (item.Groups["path"].Success)
                    {
                        var path = item.Groups["path"].Value;
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (path.StartsWith("J"))
                            {
                                tempValue = GetJPathValue(tempValue, path.Substring(1));
                            }
                            else if (path.StartsWith("X"))
                            {
                                tempValue = GetXPathValue(tempValue, path.Substring(1));
                            }
                            else if (path.StartsWith("R"))
                            {
                                tempValue = GetRegexValue(tempValue, path.Substring(1));
                            }
                        }
                    }
                    value = value.Replace(item.Value, tempValue);
                }
            }

            //判断必填项
            if (required && string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"参数{key}的值为空。");
            }
            //如果是加密参数
            if (encryption.HasValue)
            {
                // encryptionParameters
                var parameters = new List<string> { value };
                var epMatches = Regex.Matches(encryptionParameters, ParameterPattern);
                if (epMatches.Count > 0)
                {
                    var tempParameterValue = "";
                    foreach (Match item in epMatches)
                    {
                        var tempValue = GetVariableValue(item.Groups["key"].Value, null);
                        tempParameterValue = encryptionParameters.Replace(item.Value, tempValue);
                    }
                    var ParameterValues = tempParameterValue.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (ParameterValues != null && ParameterValues.Length > 0)
                    {
                        parameters.AddRange(ParameterValues);
                    }
                }
                var ciphertext = (string)typeof(SecurityHelper).InvokeMember(encryption.Value.ToString(), BindingFlags.InvokeMethod, null, null, parameters.ToArray());
                value = ciphertext;
            }
            if (!string.IsNullOrEmpty(format))
            {
                if (format.Equals("ToLower", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToLower();
                }
                else if (format.Equals("ToUpper", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToUpper();
                }
            }
            return value;
        }

        public Dictionary<string, string> ProcessingApiCallbackParameters(IEnumerable<ApiParameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                string value = NewMethod(parameter.Key, parameter.ValueOrExpression, parameter.Format, parameter.Required, parameter.Encryption, parameter.EncryptionParameters);
                Result.Add(parameter.Key, value);
            }
            return Result;
        }

        private string GetJPathValue(string content, string jpath)
        {
            var obj = JObject.Parse(content);
            return obj.SelectToken(jpath)?.ToString();
        }
        private string GetXPathValue(string content, string xpath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);
            return xmlDocument.DocumentElement.SelectSingleNode(xpath).InnerText;
        }
        private string GetRegexValue(string content, string pattern)
        {
            var match = Regex.Match(content, pattern);
            if (match.Success)
            {
                if (match.Groups["value"].Success)
                {
                    return match.Groups["value"].Value;
                }
            }
            return null;
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
            if (IsCompanyAccountVariable(variableName))
            {
                return GetCompanyAccountVariableValue(variableName, format);
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
