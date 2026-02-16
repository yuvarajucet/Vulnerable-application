using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnSync.Helper;
using VulnSync.Models;
using VulnSync.Utility;

namespace VulnSync.Controllers;

public class HomeController : Controller
{
    private readonly HomeHelper _homeHelper;

    public HomeController()
    {
        _homeHelper = new HomeHelper();
    }
    
    [HttpGet("/")]
    public IActionResult Index()
    {
        BugDetailResponseModel bugDetails = _homeHelper.GetBugDetails("0", string.Empty);
        return View(bugDetails);
    }

    [HttpGet("/get-bugs")]
    public IActionResult GetBugsList()
    {
        var bugList = _homeHelper.GetBugList();
        return Json(bugList);
    }
    
    [HttpPost("/load-bug")]
    public async Task<IActionResult> LoadBug()
    {
        var loadInfo = await HttpContext.Request.ReadFormAsync();
        string bugType = loadInfo["bugType"];
        string status = loadInfo["status"];
        if (string.IsNullOrEmpty(bugType) || string.IsNullOrEmpty(status))
        {
            return RedirectToAction("Index");
        }

        BugDetailResponseModel bugDetails = _homeHelper.GetBugDetails(bugType, status);
        return View("Index", bugDetails);
    }
}