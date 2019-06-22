﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration.Dto
{
    public class ApiParameterListDto : EntityDto
    {
        public string CompanyName { get; set; }

        public string ApiMethod { get; set; }

        public string Key { get; set; }

        public string ParameterType { get; set; }

        public string Desc { get; set; }
    }

    [AutoMap(typeof(ApiParameter))]
    public class ApiParameterEditDto
    {
        public int? Id { get; set; }

        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        [StringLength(32)]
        [Required]
        public string Key { get; set; }

        [StringLength(500)]
        [Required]
        public string ValueOrExpression { get; set; }

        public bool Required { get; set; }

        public ParameterType ParameterType { get; set; }

        public DataTag? DataTag { get; set; }

        [StringLength(32)]
        public string Format { get; set; }

        public GetValueLocation? GetLocation { get; set; }

        public SetValueLocation? SetLocation { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        [StringLength(100)]
        public string EncryptionParameters { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }

        public int[] ChannelIds { get; set; }
    }

    [AutoMap(typeof(ApiParameter))]
    public class ApiParameterDto : EntityDto
    {
        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        [StringLength(32)]
        [Required]
        public string Key { get; set; }

        [StringLength(500)]
        [Required]
        public string ValueOrExpression { get; set; }

        public bool Required { get; set; }

        public ParameterType ParameterType { get; set; }

        public DataTag? DataTag { get; set; }

        [StringLength(32)]
        public string Format { get; set; }

        public GetValueLocation? GetLocation { get; set; }

        public SetValueLocation? SetLocation { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        [StringLength(100)]
        public string EncryptionParameters { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
