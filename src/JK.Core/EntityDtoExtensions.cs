using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Application.Services.Dto;

namespace JK
{
    public static class EntityDtoExtensions
    {
        public static bool Exists(this EntityDto entity, int[] array)
        {
            return array != null && array.Any(r => r == entity.Id);
        }
    }
}
