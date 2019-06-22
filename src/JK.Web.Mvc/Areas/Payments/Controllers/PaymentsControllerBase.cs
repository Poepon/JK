using Abp.AspNetCore.Mvc.Authorization;
using JK.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    [Area(("Payments"))]
    public abstract class PaymentsControllerBase: JKControllerBase
    {

    }
}