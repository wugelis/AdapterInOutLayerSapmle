using Application.port.In;
using Application.Port.Out;
using Domain;
using Microsoft.AspNetCore.Mvc;
using WebAccountUI.Models;

namespace WebAccountUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISendMoneyUseCase _sendMoneyUseCase;
        private readonly ILoadAccountPort _loadAccountPort;

        public AccountController(ISendMoneyUseCase sendMoneyUseCase, ILoadAccountPort loadAccountPort)
        {
            _sendMoneyUseCase = sendMoneyUseCase;
            _loadAccountPort = loadAccountPort;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AccountViewModel accountViewModel)
        {
            // DIP 示例
            Account account = new Account();
            account.SetAccountId("F123456789");

            // Login 作業
            // ..略

            // 更新 Account 資訊
            _loadAccountPort.UpdateAccount(account);

            return View(accountViewModel);
        }

        public IActionResult ListAccount()
        {
            var listAccounts = _sendMoneyUseCase.ListAccounts();

            return View(listAccounts);
        }
    }
}
